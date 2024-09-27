using System.Collections;
using MonsterBattleArena.Player;
using MonsterBattleArena.Monster;
using MonsterBattleArena.MonsterCreator;
using MonsterBattleArena.PartyCreator.UI;
using MonsterBattleArena.PreviewCarousel;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace MonsterBattleArena.PartyCreator
{
    /*
    *   MOTE: Use id refence instead of MonsterData reference
    */

    public class PartyCreatorManager : MonoBehaviour
    {
        private const string KEY_LOAD_MODE = "party_creator_load_mode"; 
        private const string KEY_SELECTION_INDEX = "monster_selection_index";
        private const string KEY_SELECTED_PARTY_INDEX = "selected_party_index";
        private const string VAL_MONSTER_SELECTION = "monster_selection";

        [SerializeField] private PartyCreatorUI _ui;
        [SerializeField] private Camera _camera;
        [SerializeField] private PreviewCarouselController _previewCarousel;
        [SerializeField] private Transform _partySlotsContainer;
        [SerializeField] private RectTransform _carouselScreenPosition;

        private MonsterSlot[] _partySlots;
        private Vector2 _carouselViewportOffset;
        private int _selectedPartyIndex;

        public Camera Camera { get => _camera; }
        public int SelectedPartyIndex { get => _selectedPartyIndex; set => _selectedPartyIndex = value; }

        public event System.Action<int> OnCarouselEntrySelected;
        public event System.Action<int> OnPartySlotChanged;

        private void Start()
        {
            string loadMode = PlayerPrefs.GetString(KEY_LOAD_MODE, string.Empty);
            int monsterSelectionIndex = PlayerPrefs.GetInt(KEY_SELECTION_INDEX, 0);
            
            _selectedPartyIndex = PlayerPrefs.GetInt(KEY_SELECTED_PARTY_INDEX, -1);

            _ui.Initialize(this);
            _previewCarousel.Initialize(OnCarouselEntrySelected, monsterSelectionIndex);

            InitializePartySlots();

            CalculateCarouselViewportOffset(() => {
                if (loadMode == VAL_MONSTER_SELECTION)
                    LoadPreviewCarouselScreen();
            });

            PlayerPrefs.DeleteKey(KEY_LOAD_MODE);
            PlayerPrefs.DeleteKey(KEY_SELECTION_INDEX);
        }

        public void BackToMainMenu()
        {
            SceneManager.LoadScene("MainMenuScene");
        }

        /// <summary>
        /// Move camera to the preview carousel position
        /// </summary>
        /// <param name="viewportOffset"></param>
        public void LoadPreviewCarouselScreen()
        {
            Vector3 target = _previewCarousel.transform.position;
            target.z = -10;

            Vector2 viewportPoint = _camera.WorldToViewportPoint(_previewCarousel.transform.position);

            // Invoke the on selected callback when showing the monster selection screen
            OnCarouselEntrySelected?.Invoke(_previewCarousel.CurrentIndex);

            _ui.ShowMonsterSelectionUI(() => {
                _camera.transform.DOMove(_camera.ViewportToWorldPoint(viewportPoint + _carouselViewportOffset), 0.25f); 
            });

            Backstack.RegisterBackstack(() => {
                LoadPartyCreatorScreen();
            });
        }

        /// <summary>
        /// Move camera back to the party creator position
        /// </summary>
        public void LoadPartyCreatorScreen()
        {
            _ui.ShowPartySetupUI();
            _camera.transform.DOMove(new Vector3(0, 0, -10), 0.25f)
                .OnComplete(() => _previewCarousel.ResetPosition());

            Backstack.RegisterBackstack(() => {
                BackToMainMenu();
            });
        }

        /// <summary>
        /// Create new monster
        /// </summary>
        public void LoadMonsterCreatorScene(bool backToMonsterSelectionScreen = true)
        {
            if (_selectedPartyIndex > -1)
            {
                PlayerPrefs.SetInt(KEY_SELECTED_PARTY_INDEX, _selectedPartyIndex);
            }

            SceneManager.LoadScene("MonsterCreatorScene");

            // Save the loaded carousel index to load after finished editing the monster
            PlayerPrefs.SetInt(KEY_SELECTION_INDEX, _previewCarousel.CurrentIndex);

            if (backToMonsterSelectionScreen)
            {
                // Set the load mode for when loading back the party creator scene
                PlayerPrefs.SetString(KEY_LOAD_MODE, VAL_MONSTER_SELECTION);
            }

            Backstack.ClearBackstack();
            Backstack.RegisterBackstack(() => {
                SceneManager.LoadScene("PartyCreatorScene");
            });
        }

        /// <summary>
        /// Edit the selected monster
        /// </summary>
        /// <param name="id">selected id</param>
        public void EditMonster(string id, bool backToMonsterSelectionScreen = true)
        {
            if (PlayerProfileManager.CurrentProfile.AvailableMonsters <= 0)
                return;

            /* 
            *   This player prefs value will be checked by MonsterCreatorManager wheter or not 
            *   we want to edit an existing monster or creating a new one.
            */ 
            PlayerPrefs.SetString(MonsterCreatorManager.KEY_MONSTER_TO_EDIT, id);

            LoadMonsterCreatorScene(backToMonsterSelectionScreen);
        }

        /// <summary>
        /// Edit the selected monster in the preview carousle
        /// </summary>
        public void EditCurrentSelectedMonster()
        {
            MonsterData data = PlayerProfileManager.CurrentProfile.GetMonsterData(_previewCarousel.CurrentIndex);
            EditMonster(data.Id);
        }

        /// <summary>
        /// Set currently selected monster to the selected party slot index
        /// </summary>
        public void SetCurrentSelectedToPartyIndex()
        {
            if (SetPartySlot(_selectedPartyIndex, _previewCarousel.CurrentIndex))
            {
                OnCarouselEntrySelected?.Invoke(_previewCarousel.CurrentIndex);
            }
        }

        /// <summary>
        /// Remove monster from party slot
        /// </summary>
        /// <param name="index"></param>
        public bool ClearPartySlot(int index)
        {
            return SetPartySlot(index, -1);
        }

        private bool SetPartySlot(int partyIndex, int monsterIndex)
        {
            PlayerProfile current = PlayerProfileManager.CurrentProfile;
            MonsterData previousData = current.GetMonsterData(current.GetPartySlot(partyIndex));
            MonsterData monsterData = current.GetMonsterData(monsterIndex);

            if (current.SetPartySlot(monsterData, partyIndex))
            {
                if (previousData != null)
                {
                    int indexInCollection = current.GetMonsterIndex(previousData);
                    _previewCarousel.UpdateEntry(indexInCollection);
                }

                _partySlots[partyIndex].SetMonster(monsterData?.Id);
                _previewCarousel.UpdateEntry(monsterIndex);

                OnPartySlotChanged?.Invoke(partyIndex);
                PlayerProfileManager.SaveCurrentProfile();

                return true;
            }

            return false;
        }

        public bool PreviousMonster()
        {
            return _previewCarousel.MoveLeft();
        }

        public bool NextMonster()
        {
            return _previewCarousel.MoveRight();
        }

        public PartySlot GetPartySlot(int index)
        {
            return _partySlotsContainer.GetChild(index).GetComponent<PartySlot>();
        }

        private void InitializePartySlots()
        {
            _partySlots = new MonsterSlot[_partySlotsContainer.childCount];
            for (int i = 0; i < _partySlots.Length; i++)
            {
                MonsterSlot slot = _partySlotsContainer.GetChild(i).GetComponent<MonsterSlot>();
                slot.SetMonster(PlayerProfileManager.CurrentProfile.GetPartySlot(i));
                _partySlots[i] = slot;

                _ui.InitializePartySlotUI(slot, i);
            }
        }

        private void CalculateCarouselViewportOffset(System.Action onLayoutCalculationCompleted)
        {
            IEnumerator exec()
            {
                // Wait for the end of frame to finish the layout rebuild
                yield return new WaitForEndOfFrame();

                Vector3 worldPos = _carouselScreenPosition.transform.position;
                Vector3 viewPortPos = _camera.ScreenToViewportPoint(worldPos);

                _carouselViewportOffset = (Vector3.one * 0.5f) - viewPortPos;

                onLayoutCalculationCompleted?.Invoke();
            }

            StartCoroutine(exec());
        }
    
    }
}
