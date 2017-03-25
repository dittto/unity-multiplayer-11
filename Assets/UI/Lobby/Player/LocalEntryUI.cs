// UI/Lobby/Player/LocalEntryUI.cs

using Player.SyncedData;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Lobby.Player
{
    public class LocalEntryUI : MonoBehaviour, EntryInterface
    {
        public InputField nameInputField;
        public GameObject vipButton;
        public GameObject inhumerButton;
        public GameObject spectatorButton;

        public GameObject readyBackground;
        public Text nameText;
        public Text teamText; 

        private PlayerDataForClients settings;

        public void SetPlayerObject(GameObject player)
        {
            settings = player.GetComponent<PlayerDataForClients>();

            UpdateNameFromSettings(player, settings.GetName());
            UpdateTeamWithSettings(player, settings.GetTeam());
            UpdateReadyFlagFromSettings(player, settings.GetIsReadyFlag());

            settings.OnNameUpdated += UpdateNameFromSettings;
            settings.OnTeamUpdated += UpdateTeamWithSettings;
            settings.OnIsReadyFlagUpdated += UpdateReadyFlagFromSettings;
        }

        // sent from UI to change name
        public void SendNameToSettings(InputField nameText)
        {
            settings.SetName(nameText.text);
        }

        // sent from UI to change team
        public void SendTeamToSettings(int teamId)
        {
            settings.SetTeam(teamId);
        }

        public void UpdateNameFromSettings(GameObject player, string name)
        {
            nameInputField.text = name;
            nameText.text = nameInputField.text;
        }

        private void UpdateTeamWithSettings(GameObject player, int teamId)
        {
            if (teamId == PlayerDataForClients.TEAM_VIP) {
                if (vipButton) vipButton.SetActive(true);
                if (inhumerButton) inhumerButton.SetActive(false);
                if (spectatorButton) spectatorButton.SetActive(false);
                teamText.text = "VIP";
            }

            if (teamId == PlayerDataForClients.TEAM_INHUMER) {
                if (vipButton) vipButton.SetActive(false);
                if (inhumerButton) inhumerButton.SetActive(true);
                if (spectatorButton) spectatorButton.SetActive(false);
                teamText.text = "Inhumer";
            }

            if (teamId == PlayerDataForClients.TEAM_SPECTATOR) {
                if (vipButton) vipButton.SetActive(false);
                if (inhumerButton) inhumerButton.SetActive(false);
                if (spectatorButton) spectatorButton.SetActive(true);
                teamText.text = "Spectator";
            }
        }

        public void UpdateReadyFlagFromSettings(GameObject player, bool isReady)
        {
            readyBackground.SetActive(isReady);
            nameText.gameObject.SetActive(isReady);
            teamText.gameObject.SetActive(isReady);
            nameInputField.gameObject.SetActive(!isReady);

            if (isReady) {
                vipButton.SetActive(false);
                inhumerButton.SetActive(false);
                spectatorButton.SetActive(false);
            }
            else {
                UpdateTeamWithSettings(player, settings.GetTeam());
            }
        }
    }
}
