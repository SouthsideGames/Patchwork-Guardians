using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MonsterBattleArena.UI.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button _quickPlay;
        [SerializeField] private Button _partySetup;
        [SerializeField] private Button _monsterCreator;
        [SerializeField] private Button _inventory;
        [SerializeField] private Button _quitToDesktop;
        [SerializeField] private TMP_Text _partyWarning;

        private void Start()
        {
            PlayerProfileManager.LoadDefaultProfile();

            _quickPlay.onClick.AddListener(EnterQuickplay);
            _partySetup.onClick.AddListener(() => LoadScene("PartyCreatorScene"));
            _monsterCreator.onClick.AddListener(() => LoadScene("MonsterCreatorScene"));
            _inventory.onClick.AddListener(() => LoadScene("InventoryScene"));
            _quitToDesktop.onClick.AddListener(() => Application.Quit());

            for (int i = 0; i < 5; i++)
            {
                string monster = PlayerProfileManager.CurrentProfile.GetPartySlot(i);
                if (string.IsNullOrEmpty(monster))
                {
                    _partyWarning.gameObject.SetActive(true);
                    return;
                }
            }

            _partyWarning.gameObject.SetActive(false);
        }

        private void LoadScene(string sceneName)
        {
            Backstack.RegisterBackstack(() => {
                SceneManager.LoadScene("MainMenuScene");
            });

            SceneManager.LoadScene(sceneName);
        }

        private void EnterQuickplay()
        {
            for (int i = 0; i < 5; i++)
            {
                string monster = PlayerProfileManager.CurrentProfile.GetPartySlot(i);
                if (string.IsNullOrEmpty(monster))
                {
                    Debug.Log("You need to assign monster to your party.");
                    return;
                }
            }

            SceneManager.LoadScene("BattleSetupScene");
        }

    }
}
