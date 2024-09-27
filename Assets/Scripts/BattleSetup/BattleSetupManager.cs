using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MonsterBattleArena.BattleSystem;
using MonsterBattleArena.Monster;

namespace MonsterBattleArena.BattleSetup
{
    public class BattleSetupManager : MonoBehaviour
    {
        /*
        *   NOTE: Currently the opponents are generated during the start of the battle
        */

        [SerializeField] private BattleSetupUIController _ui;
        [SerializeField] private int _pickTime = 5;

        [SerializeField] private MonsterSlot[] _monsterSlots;

        private string[] _draftedMonsters;

        private Camera _camera;
        private MonsterSlot _previousSelectedSlot;
        private HashSet<int> _selectedMonsterSlot = new HashSet<int>();
        private int _currentSlotIndex = 0;
        private bool _canPick = true;

        private void Start()
        {
            _camera = Camera.main;
            _draftedMonsters = new string[3];

            _ui.Initialize(this);
            InitializeMonsterSlots();

            StartCoroutine(BattleSetup());
        }

        private void Update()
        {
            if (!_canPick)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
                if (hit)
                {
                    MonsterSlot monsterSlot = hit.collider.GetComponent<MonsterSlot>();
                    if (monsterSlot != null)
                    {
                        if (IsAlreadyPicked(monsterSlot.MonsterId))
                            return;

                        if (_previousSelectedSlot != null)
                            _previousSelectedSlot.SetFX(false);

                        SetMonsterToCurrentIndex(monsterSlot);

                        _previousSelectedSlot = monsterSlot;
                    }
                }
            }
        }

        private IEnumerator BattleSetup()
        {
            for (int i = 0; i < 6; i++)
            {
                _ui.SetSelectButtonVisisble(false);
                
                bool isPlayerTurn = i % 2 == 0;
                string cueText = isPlayerTurn ? "Pick your monster!" : "Opponent's turn!";

                yield return _ui.ShowCue(cueText);
                _canPick = isPlayerTurn;
                yield return _ui.StartSlotCountdown(i, _pickTime);

                _canPick = false;

                if (isPlayerTurn)
                {
                    if (string.IsNullOrEmpty(_draftedMonsters[_currentSlotIndex]))
                        SetMonsterToCurrentIndex(_monsterSlots[GetRandomIndex()]);

                    _previousSelectedSlot = null;
                    _currentSlotIndex++;
                }
            }

            for (int i = 0; i < _draftedMonsters.Length; i++)
            {
                BattleManager.PlayerBattleSlots[i] = _draftedMonsters[i];
            }

            SceneManager.LoadScene("BattleScene");
        }
    
        private void SetMonsterToCurrentIndex(MonsterSlot monsterSlot)
        {
            _ui.SetMonsterSlotUI(_currentSlotIndex, _camera.WorldToScreenPoint(monsterSlot.transform.position));
            _ui.SetSelectButtonVisisble(true);

            monsterSlot.SetFX(true);

            _draftedMonsters[_currentSlotIndex] = monsterSlot.MonsterId;

            _selectedMonsterSlot.Add(GetSlotIndex(monsterSlot));
        }

        private bool IsAlreadyPicked(string id)
        {
            foreach (string monsterId in _draftedMonsters)
            {
                if (string.IsNullOrEmpty(monsterId))
                    continue;
                
                if (monsterId == id)
                    return true;
            }

            return false;
        }
    
        private void InitializeMonsterSlots()
        {
            for (int i = 0; i < _monsterSlots.Length; i++)
            {
                int index = i;
                MonsterSlot slot = _monsterSlots[index];
                slot.SetMonster(PlayerProfileManager.CurrentProfile.GetPartySlot(index));
                slot.SetFX(false);
            }
        }

        private int GetSlotIndex(MonsterSlot slot)
        {
            for (int i = 0; i < _monsterSlots.Length; i++)
            {
                if (_monsterSlots[i] == slot)
                    return i;
            }

            return -1;
        }

        // TODO: Add time out and count check to avoid indefinite loop
        private int GetRandomIndex()
        {
            int index;
            do {
                index = Random.Range(0, _monsterSlots.Length);
            } while (_selectedMonsterSlot.Contains(index));

            return index;
        }
    }
}
