using UnityEngine;
using MonsterBattleArena.Monster;
using DG.Tweening;

namespace MonsterBattleArena.PreviewCarousel
{
    public class PreviewCarouselController : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.25f;       // Animation duration
        [SerializeField] private float _spaces = 3.5f;          // Spaces between monsters
        [SerializeField] private float _selectedScale = 1.2f;   // The size of the entry when selected
        [SerializeField] private float _normalScale = 1.0f;     // The size of the entry when not selected
        [SerializeField] private int _maxLoadedEntries = 5;     // The maximum loaded entry
        [SerializeField] private GameObject _template;

        private CarouselEntry[] _entries;
        private int _currentIndex = 0;
        private int _availableMonsters;
        private bool _inTransition = false;

        private System.Action<int> _onEntrySelected;

        public int CurrentIndex { get => _currentIndex; }

        public void Initialize(System.Action<int> onCarouselEntrySelected, int startIndex = 0)
        {
            _onEntrySelected = onCarouselEntrySelected;
            _currentIndex = startIndex;

            _availableMonsters = PlayerProfileManager.CurrentProfile.AvailableMonsters;

            if (_maxLoadedEntries <= 0)
                _maxLoadedEntries = 1;

            _entries = new CarouselEntry[Mathf.Min(_availableMonsters, _maxLoadedEntries)];
            for (int i = 0; i < _entries.Length; i++)
            {
                int index = i - startIndex;
                GameObject entry = Instantiate(_template, _template.transform.parent);
                entry.transform.localPosition = Vector2.zero;

                _entries[i] = new CarouselEntry(entry, index);

                SetMonsterRig(_entries[i]);
                MoveEntry(_entries[i], index, false);
            }

            _template.gameObject.SetActive(false);
        }

        public void ResetPosition()
        {
            _currentIndex = 0;
            
            int index = 0;
            foreach (CarouselEntry entry in _entries)
            {
                entry.PositionIndex = index;
                
                SetMonsterRig(entry);
                MoveEntry(entry, index, false);

                index++;
            }
        }

        public bool MoveRight(bool useTransition = true)
        {
            if (_currentIndex >= _availableMonsters - 1)
                return false;

            if (_inTransition)
                return false;

            _inTransition = true;

            Sequence sequence = DOTween.Sequence();

            foreach (CarouselEntry entry in _entries)
            {
                int index = entry.PositionIndex - 1;
                bool reset = false;
                
                if (index < -2 && _currentIndex < _availableMonsters - 3)
                {
                    index = 2;
                    reset = true;
                    SetMonsterRig(entry);
                }

                sequence.Join(MoveEntry(entry, index, !useTransition || !reset));
                entry.PositionIndex = index;
            }

            sequence.OnComplete(() => {
                _currentIndex += 1;
                _inTransition = false;

                _onEntrySelected?.Invoke(_currentIndex);
            });

            return true;
        }

        public bool MoveLeft(bool useTransition = true)
        {
            if (_currentIndex <= 0)
                return false;

            if (_inTransition)
                return false;

            _inTransition = true;

            Sequence sequence = DOTween.Sequence();

            foreach (CarouselEntry entry in _entries)
            {
                int index = entry.PositionIndex + 1;
                bool reset = false;

                if (index > 2 && _currentIndex > 2)
                {
                    index = -2;
                    reset = true;
                    SetMonsterRig(entry);
                }

                sequence.Join(MoveEntry(entry, index, !useTransition || !reset));
                entry.PositionIndex = index;
            }

            sequence.OnComplete(() => {
                _currentIndex -= 1;
                _inTransition = false;

                _onEntrySelected?.Invoke(_currentIndex);
            });

            return true;
        }

        public void UpdateCurrentEntry()
        {
            UpdateEntry(_currentIndex);
        }

        public void UpdateEntry(int index)
        {
            if (index < 0)
                return;

            SetMonsterRig(_entries[index]);
        }

        private Sequence MoveEntry(CarouselEntry entry, int xPos, bool useTransition = true)
        {
            float scale = xPos == 0 ? _selectedScale : _normalScale;

            if (!useTransition)
            {
                entry.Transform.localPosition = new Vector3(_spaces * xPos, 0, 0);
                entry.Transform.localScale = Vector3.one * scale;
                return DOTween.Sequence();
            }

            Sequence sequence = DOTween.Sequence();
            sequence.Append(entry.Transform.DOLocalMoveX(_spaces * xPos, _duration));
            sequence.Join(entry.Transform.DOScale(scale, _duration));

            return sequence;
        }
    
        private void SetMonsterRig(CarouselEntry entry)
        {
            int index = _currentIndex + entry.PositionIndex;
            if (index < 0 || index >= _availableMonsters)
                return;

            // TODO: Load by id instead
            MonsterData monsterData = PlayerProfileManager.CurrentProfile.GetMonsterData(index);
            entry.MonsterRig.UpdateMonsterPart(monsterData);

            if (!PlayerProfileManager.CurrentProfile.IsInParty(monsterData))
                entry.DisableFx();
            else
                entry.EnableFx();
        }

        private void OnDrawGizmos()
        {
            float halfSelected = _selectedScale * 0.5f;
            float halfNormal = _normalScale * 0.5f;

            for (int i = 0; i < _maxLoadedEntries; i++)
            {
                Vector3 pivot = transform.position + Vector3.right * _spaces * i;
                if (i <= 0)
                    Gizmos.DrawWireCube(pivot + (Vector3.up * halfSelected), new Vector3(halfSelected, _selectedScale));
                else
                    Gizmos.DrawWireCube(pivot + (Vector3.up * halfNormal), new Vector3(halfNormal, _normalScale));

                Gizmos.DrawWireSphere(pivot, 0.25f);
            }
        }

        [System.Serializable]
        private class CarouselEntry
        {
            public MonsterRig MonsterRig { get; private set; }
            public Transform Transform { get; private set; }
            public int PositionIndex { get; set; }

            private GameObject _fx;

            public CarouselEntry(GameObject container, int index)
            {
                MonsterRig = container.GetComponentInChildren<MonsterRig>();
                PositionIndex = index;
                Transform = container.transform;

                _fx = container.transform.Find("SelectionBase").Find("FX").gameObject;
            }

            public void EnableFx() => _fx.SetActive(true);
            public void DisableFx() => _fx.SetActive(false);
        }
    }
}
