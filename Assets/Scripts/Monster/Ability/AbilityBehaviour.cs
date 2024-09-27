namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public abstract class AbilityBehaviour
    {
        private bool _enabled = true;

        protected MonsterAbility Ability { get; private set; }
        protected BattleUnit BattleUnit { get; private set; }
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled && !value)
                    OnDisable();
                else if (!_enabled && value)
                    OnEnable();
                    
                _enabled = value;
            }
        }

        public void Init(MonsterAbility ability, BattleUnit battleUnit)
        {
            Ability = ability;
            BattleUnit = battleUnit;
            OnInit();
        }

        /// <summary>
        /// Called on initialization
        /// </summary>
        protected virtual void OnInit()
        {
        }

        /// <summary>
        /// Called when the ability is disabled
        /// </summary>
        public virtual void OnDisable()
        {
        }

        /// <summary>
        /// Called when the ability is enabled
        /// </summary>
        public virtual void OnEnable()
        {
        }

        /// <summary>
        /// Called every round start
        /// </summary>
        public virtual void OnUpdate()
        {
        }
        
        public abstract AbilityBehaviour Copy();
    }
}
