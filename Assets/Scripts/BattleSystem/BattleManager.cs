using System.Collections.Generic;
using System.Collections;
using System.Linq;
using MonsterBattleArena.Monster;
using MonsterBattleArena.Util;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

namespace MonsterBattleArena.BattleSystem
{
    public class BattleManager : InstancedMonobehaviour<BattleManager>
    {
        public static string[] PlayerBattleSlots { get; private set; } = new string[3];

        // NOTE: For debugging purpose. Fills the player party with random monster if the player party is empty
        [Header("DEBUG")]
        [SerializeField] private bool _fillRandomIfEmpty = false; 
        [Space]
        [SerializeField] private int _minDamageThreshold = 5; // The min amount of damage dealt
        [SerializeField] private int _maxCritThreshold = 20; // The max chance of critical hit
        [SerializeField] private BattleUIController _ui;
        [SerializeField] private Transform _playerSlots;
        [SerializeField] private Transform _opponentSlots;
        
        /*
        *   NOTE:
        *       If encountering performance issue, this can be further optimized by combining both
        *       _playerUnits and _opponentUnits array into single array and use indexing to split
        *       the colllections
        */
        private BattleUnit[] _playerUnits;
        private BattleUnit[] _opponentUnits;
        private GameSpeed _currentGameSpeed = GameSpeed.X1;

        private DefaultDamageCalculator _damageCalculator;
        private List<QueueEntry> _turnQueue = new List<QueueEntry>();

        public IEnumerable<BattleUnit> PlayerUnits { get => _playerUnits; }
        public IEnumerable<BattleUnit> OpponentUnits { get => _opponentUnits; }
        public BattleUIController UI { get => _ui; }

        private void Start()
        {
            _damageCalculator = new DefaultDamageCalculator(_minDamageThreshold, _maxCritThreshold);

            SetGameSpeed(GameSpeed.X1);

            InitializePlayer();
            IntiializeOpponents();

            _ui.Initialize(this);

            StartRound();
        }

        private void OnDisable()
        {
            SetGameSpeed(GameSpeed.X1);
        }

        private void InitializePlayer()
        {
            _playerUnits = new BattleUnit[_playerSlots.childCount];

            for (int i = 0; i < _playerUnits.Length; i++)
            {
                int index = i;
                MonsterData data;
                if (_fillRandomIfEmpty)
                {
                    data = MonsterGenerator.GenerateRandom();
                    Debug.Log("Filled player slot " + i + " with random.");
                }
                else
                {
                    if (i >= PlayerBattleSlots.Length || string.IsNullOrEmpty(PlayerBattleSlots[i]))
                        continue;

                    data = PlayerProfileManager.CurrentProfile.GetMonsterData(PlayerBattleSlots[i]);
                }

                Transform slotTransform = _playerSlots.GetChild(i);
                _playerUnits[i] = CreateBattleUnit(slotTransform, data, index + 1);
            }

            // Resets the selected battle slots
            PlayerBattleSlots = new string[3];
        }

        private void IntiializeOpponents()
        {
            _opponentUnits = new BattleUnit[_opponentSlots.childCount];

            for (int i = 0; i < _opponentUnits.Length; i++)
            {
                int index = i;
                Transform slotObject = _opponentSlots.GetChild(i);
                MonsterData monsterData = MonsterGenerator.GenerateRandom();
                
                _opponentUnits[i] = CreateBattleUnit(slotObject, monsterData, index + 1);
            }
        }

        private void StartRound()
        {
            IEnumerator exec()
            {
                YieldInstruction delay = new WaitForSeconds(1.0f);;
                yield return delay;

                do {
                    CreateQueue();
                    _ui.SetQueue(_turnQueue);

                    // Notify all battle unit's that the round is started
                    foreach (QueueEntry entry in _turnQueue)
                    {
                        entry.BattleUnit.RoundStart();
                    }

                    int index = 0;
                    foreach (QueueEntry queueEntry in _turnQueue)
                    {
                        if (!queueEntry.BattleUnit.IsAlive)
                            continue;

                        _ui.ShowTurnIndicator(index);
                        queueEntry.BattleUnit.TurnStart();
                        yield return HandleCurrentTurn(queueEntry);
                        queueEntry.BattleUnit.TurnCompleted();

                        if (IsGameFinished())
                        {
                            break;
                        }

                        index++;
                    }

                    yield return _ui.ClearQueue();
                    yield return delay;
                }
                while(!IsGameFinished());

                // Round completed. should do another round
                OnRoundCompleted();
            }

            StartCoroutine(exec());
        }

        private void OnRoundCompleted()
        {
            // TODO: Start another round with a chance of picking new monsters
            SceneManager.LoadScene("MainMenuScene");
        }

        private YieldInstruction HandleCurrentTurn(QueueEntry entry)
        {
            // Get the target unit
            BattleUnit targetUnit = GetOpposingUnit(entry);

            // TODO: Create solver class to handle turns

            IEnumerator exec()
            {
                Vector3 defaultPos = entry.BattleUnit.Transform.position;
                Vector2 targetPos = targetUnit.Transform.position;
                targetPos.x += entry.IsPlayer ? -2.5f : 2.5f;

                entry.BattleUnit.Rig.SetToForeground();
                yield return MoveBattleUnit(entry.BattleUnit, targetPos);

                bool animationFinished = false;
                MonsterDamageData damage = _damageCalculator.CalculateDamageAmount(entry.BattleUnit, targetUnit);
                entry.BattleUnit.PerformAttack(targetUnit, 
                () => 
                {
                    animationFinished = true;
                }, 
                () => 
                {
                    targetUnit.TakeDamage(damage);
                    return damage;
                });

                while (!animationFinished)
                    yield return null;

                yield return MoveBattleUnit(entry.BattleUnit, defaultPos);
            }

            if (entry.BattleUnit.Immobilized)
            {
                Debug.Log(entry.BattleUnit.Name + " is immobilized!");
                return new YieldInstruction();
            }

            return StartCoroutine(exec());
        }

        private YieldInstruction MoveBattleUnit(BattleUnit unit, Vector3 position)
        {
            Transform t = unit.Transform;

            return DOTween.Sequence()
                .Append(t.DOJump(position, 1, 1, 0.8f))
                .SetEase(Ease.InBack)
                .WaitForCompletion();
        }

        private BattleUnit GetOpposingUnit(QueueEntry entry)
        {
            int lane = GetMonsterLane(entry);
            BattleUnit[] battleUnits = entry.IsPlayer ? _opponentUnits : _playerUnits;

            if (battleUnits[lane].IsAlive)
                return battleUnits[lane];

            /*
            *   NOTE:
            *       If the monster at the opposing lane is not alive, check the
            *       all opponents starting from the front most
            */
            for (int i = 0; i < battleUnits.Length; i++)
            {
                if (battleUnits[i].IsAlive)
                    return battleUnits[i];
            }

            return null;
        }

        private BattleUnit CreateBattleUnit(Transform slotTransform, MonsterData monsterData, int index)
        {
            MonsterSlot slot = slotTransform.GetComponent<MonsterSlot>();

            if (slot == null)
            {
                Debug.LogError("Unable to get MonsterSlot component!", slotTransform);
                return null;
            }

            BattleUnit unit = new BattleUnit(slot, monsterData, index);
            _ui.InitializeUnitStatus(unit, slot);

            return unit;
        }

        private void CreateQueue()
        {
            _turnQueue.Clear();
            _turnQueue.AddRange(_playerUnits.Where(x => x.IsAlive).Select(x => new QueueEntry(x, true)));
            _turnQueue.AddRange(_opponentUnits.Where(x => x.IsAlive).Select(x => new QueueEntry(x, false)));

            // Sort by the most speed
            _turnQueue.Sort((a, b) => {
                int attrA = a.BattleUnit.GetAttributeValue(AttributeType.Speed);
                int attrB = b.BattleUnit.GetAttributeValue(AttributeType.Speed);
                return attrB.CompareTo(attrA);
            });
        }

        private int GetMonsterLane(QueueEntry entry)
        {
            BattleUnit[] battleUnits = entry.IsPlayer ? _playerUnits : _opponentUnits;
            return System.Array.IndexOf(battleUnits, entry.BattleUnit);
        }

        private bool IsGameFinished()
        {
           if (_opponentUnits.Where(x => x.IsAlive).Count() <= 0)
           {
               return true;
           }

           if (_playerUnits.Where(x => x.IsAlive).Count() <= 0)
           {
                return true;
           }

           return false;
        }

        public void SetGameSpeed(GameSpeed gameSpeed)
        {
            _currentGameSpeed = gameSpeed;

            switch (_currentGameSpeed)
            {
                case GameSpeed.X1:
                    Time.timeScale = 1.0f;
                    break;
                case GameSpeed.X2:
                    Time.timeScale = 1.5f;
                    break;
                case GameSpeed.X3:
                    Time.timeScale = 2.0f;
                    break;
            }
        }

        [System.Serializable]
        public class QueueEntry
        {
            public BattleUnit BattleUnit { get; private set; }
            public bool IsPlayer { get; private set; }

            public QueueEntry(BattleUnit unit, bool isPlayer)
            {
                BattleUnit = unit;
                IsPlayer = isPlayer;
            }
        }
    }
}
