// UI/Lobby/Player/RemoteEntryUI.cs

using Player.SyncedData;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Lobby.Player
{
    public class RemoteEntryUI : MonoBehaviour, EntryInterface
    {
        public GameObject isReadyBackground;
        public GameObject isServerBackground;
        public Text nameText;
        public Text teamText;

        private PlayerDataForClients settings;

        public void SetPlayerObject(GameObject player)
        {
            settings = player.GetComponent<PlayerDataForClients>();

            // force a change when setup so we have initial settings
            UpdateNameFromSettings(player, settings.GetName());
            UpdateTeamFromSettings(player, settings.GetTeam());
            UpdateReadyFlagFromSettings(player, settings.GetIsReadyFlag());
            UpdateServerFlagFromSettings(player, settings.GetIsServerFlag());

            // set up events so when client player settings change, hud updates
            settings.OnNameUpdated += UpdateNameFromSettings;
            settings.OnTeamUpdated += UpdateTeamFromSettings;
            settings.OnIsReadyFlagUpdated += UpdateReadyFlagFromSettings;
            settings.OnIsServerFlagUpdated += UpdateServerFlagFromSettings;
        }

        // used when PlayerDataForClients changes name
        public void UpdateNameFromSettings(GameObject player, string name)
        {
            nameText.text = name;
        }

        // used when PlayerDataForClients changes team
        public void UpdateTeamFromSettings(GameObject player, int teamId)
        {
            if (teamId == PlayerDataForClients.TEAM_VIP) {
                teamText.text = "VIP";
            }

            if (teamId == PlayerDataForClients.TEAM_INHUMER) {
                teamText.text = "Inhumer";
            }

            if (teamId == PlayerDataForClients.TEAM_SPECTATOR) {
                teamText.text = "Spectator";
            }
        }

        public void UpdateReadyFlagFromSettings (GameObject player, bool isReady)
        {
            isReadyBackground.SetActive(isReady);
        }

        public void UpdateServerFlagFromSettings(GameObject player, bool isServer)
        {
            isServerBackground.SetActive(isServer);
        }
    }
}
