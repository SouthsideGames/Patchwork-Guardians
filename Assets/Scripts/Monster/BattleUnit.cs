using UnityEngine;

namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public class BattleUnit
    {
        /*
        *   NOTE:
        *       There's a couple type of abilities:
        *       - Passive: Activate each time the round started
        *       - Active: Can be triggered when receiving or dealing damage
        *
        *       Status effects is like the passive abilities, which means it gets updated
        *       on each start of the turn with exception of some status effect that's get
        *       activated instantanouesly.
        */

        private MonsterSlot _monsterSlot;
        private MonsterData _monsterData;

        private StatusEffectController _statusEffect;
        private AbilityController _ability;
        private int _unitIndex;

        // Stats
        private int _health;
        private int _currentHealth;
        private bool _immobilized;
        private float[] _attributeModifiers;

        public string Name { get => _monsterData.Name; }

        public int Health 
        { 
            get => _health;
            private set
            {
                _health = value;
                OnHealthChanged?.Invoke();
            }
        }
        
        public int CurrentHealth 
        { 
            get => _currentHealth; 
            private set
            {
                _currentHealth = Mathf.Clamp(value, 0, _health);
                OnCurrentHealthChanged?.Invoke();
            }
        }

        public float CurrentHealthPercent { get => Mathf.Clamp01((float)_currentHealth / _health); }
        public bool IsAlive { get => _currentHealth > 0; }
        public int UnitIndex { get => _unitIndex; }
        public bool Immobilized { get => _immobilized; set => _immobilized = value; }

        public MonsterData MonsterData { get => _monsterData; }
        public MonsterRig Rig { get => _monsterSlot.Rig; }
        public Transform Transform { get => _monsterSlot.Rig.transform; }

        public event System.Action<BattleUnit> OnTakeDamage;
        public event System.Func<int, int> OnBeforeTakeDamage;
        public event System.Action OnHealed;
        public event System.Func<int, int> OnBeforeHeal;

        public event System.Action OnTurnStarted;
        public event System.Action OnTurnCompleted;

        public event System.Action OnHealthChanged;
        public event System.Action OnCurrentHealthChanged;

        public event System.Action OnAttackCompleted;
        public event System.Action<BattleUnit, MonsterDamageData> OnAttackDealDamage;

        public AbilityController Ability { get => _ability; }

        public BattleUnit(MonsterSlot slot, MonsterData data, int unitIndex)
        {
            InitAttributeModifier();

            _monsterSlot = slot;
            _monsterData = data;
            _unitIndex = unitIndex;

            _health = GetAttributeValue(AttributeType.Health);
            _currentHealth = _health;

            _monsterSlot.SetMonster(_monsterData);
            _monsterSlot.SetFX(false);

            _statusEffect = new StatusEffectController(this);
            _ability = new AbilityController();

            InitAbilities();
            InitSynergies();
        }

        private int GetTotalHealth()
        {
            int total = MonsterDataUtility.GetTotalAttributeValue(_monsterData, AttributeType.Health);
            int intelligence = GetAttributeValue(AttributeType.Intelligence);

            total += total * Mathf.RoundToInt(intelligence * 0.02f);

            return total;
        }

        /*
        *   NOTE:
        *       The reason we use a separate method for getting the attribute is that
        *       later on when we implement status effect, it can be applied here before
        *       returnong the value
        */
        public int GetAttributeValue(AttributeType attributeType)
        {
            float mod = _attributeModifiers[(int)attributeType];

            switch (attributeType)
            {
                case AttributeType.Health:
                    return GetTotalHealth();
                default:
                    int attrValue = MonsterDataUtility.GetTotalAttributeValue(_monsterData, attributeType);
                    return Mathf.RoundToInt(attrValue + (attrValue * mod));
            }
        }

        public int GetBonusCritChance()
        {
            int atk = GetAttributeValue(AttributeType.Attack);
            return Mathf.RoundToInt(atk * 0.02f);
        }

        public void RoundStart()
        {
            _statusEffect.Update();
            _ability.Update();
        }

        public void TurnStart()
        {
            OnTurnStarted?.Invoke();
            Rig.SetToForeground();
        }

        public void TurnCompleted()
        {
            OnTurnCompleted?.Invoke();
            Rig.ResetSortingOrder();
        }

        public void TakeDamage(MonsterDamageData damage)
        {
            damage.Value = OnBeforeTakeDamage?.Invoke(damage.Value) ?? damage.Value;
            CurrentHealth -= damage.Value;

            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                Rig.TriggerDeathAnimation();
            }

            OnTakeDamage?.Invoke(damage.Dealer);

            // Popup
            Vector2 popupPos = CalculatePopupPos();
            PopupText.PopupConfig config = new PopupText.PopupConfig();

            if (damage.IsMiss)
            {
                config.TextType = PopupText.TextType.Default;
                PopupText.Show("Miss!", popupPos, config);
                return;
            }

            config.TextType = PopupText.TextType.Damage;
            string popup = damage.Value.ToString();

            if (damage.IsCrit)
            {
                config.TextType = PopupText.TextType.Critical;
                popup += " Crit!";
            }
            
            PopupText.Show(popup, popupPos, config);
        }

        public void PerformAttack(BattleUnit target, System.Action onComplete, System.Func<MonsterDamageData> onDealDamage)
        {
            Rig.TriggerAttackAnimation(() => 
            {
                onComplete?.Invoke();
                OnAttackCompleted?.Invoke();
            }, () => 
            {
                MonsterDamageData damageData = onDealDamage?.Invoke() ?? default;
                OnAttackDealDamage?.Invoke(target, damageData);
            });
        }

        public void Heal(int amount)
        {
            amount = OnBeforeHeal?.Invoke(amount) ?? amount;
            int before = CurrentHealth;
            CurrentHealth += amount;

            OnHealed?.Invoke();

            PopupText.PopupConfig config = new PopupText.PopupConfig
            {
                TextType = PopupText.TextType.Heal
            };
            PopupText.Show(string.Format("+{0}", amount), CalculatePopupPos(), config);
        }

        public void ApplyStatusEffect(StatusEffect statusEffect)
        {
            _statusEffect.Apply(statusEffect);
        }

        public void ApplyStatusEffectBehaviour(string id, int duration, StatusEffectBehaviour[] behaviours)
        {
            _statusEffect.Apply(id, duration, behaviours);
        }

        public void RemoveStatusEffect(StatusEffect statusEffect)
        {

        }
    
        public void AddAttributeModifier(AttributeType attributeType, float mod)
        {
            int index = (int)attributeType;
            _attributeModifiers[index] += mod;
        }

        private void InitAttributeModifier()
        {
            int count = System.Enum.GetNames(typeof(AttributeType)).Length;
            _attributeModifiers = new float[count];
        }
    
        private void InitSynergies()
        {
#if UNITY_EDITOR
            string synergies = string.Empty;
            int synCount = 0;
#endif
            foreach ((Synergy, int) synTuple in MonsterDataUtility.GetActiveSynergies(_monsterData))
            {
                Synergy synergy = synTuple.Item1;
                int tier = synTuple.Item2;

                if (synergy.SynergyCount > 0)
                {
                    MonsterAbility ability = synergy.GetAbility(tier);
                    _ability.RegisterAbility(this, ability);
                }

#if UNITY_EDITOR
                string synergyTier = tier > -1 ? " | " + synTuple.Item1.Synergies[tier].Name : string.Empty;
                synergies += synTuple.Item1.Name + synergyTier + "\n";
                synCount++;
#endif
            }
#if UNITY_EDITOR
            if (synCount > 0)
                Debug.Log(_monsterData.Name + " active synergies: " + synergies);
#endif
        }
    
        private void InitAbilities()
        {
            foreach (MonsterAbility ability in MonsterDataUtility.GetAbilities(_monsterData))
            {
                if (ability == null)
                    continue;
                
                _ability.RegisterAbility(this, ability);
            }
        }
    
        public Vector2 CalculatePopupPos()
        {
            Bounds bounds = Rig.GetBounds();
            return new Vector2(Transform.position.x, bounds.max.y);
        }
    }
}
 