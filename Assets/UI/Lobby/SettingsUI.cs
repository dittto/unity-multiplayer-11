// UI/Lobby/SettingsUI.cs

using GameState;
using Modal;
using Player.SyncedData;
using Player.Tracking;
using UI.Level;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI.Lobby {
    public class SettingsUI : NetworkBehaviour {

        [SyncVar]
        private string level = "Waiting for first choice";
        [SyncVar]
        private string timeLimit = "Waiting for first choice";

        public GameObject clientUI;
        public GameObject serverUI;

        public Dropdown serverLevelSelect;
        public Dropdown serverTimeSelect;
        public Text serverIPAddress;
        public Text clientLevelSelect;
        public Text clientTimeSelect;

        public GameObject readyWaitingButton;
        public GameObject readyNowButton;

        public GameObject startGameButton;
        public Text startGameButtonText;

        private bool allowServerStart = false;

        public void Awake()
        {
            State.GetInstance()
                .Level(State.LEVEL_IN_LOBBY)
                .Publish();
        }
        
        public void Start()
        {
            if (State.GetInstance().Network() == State.NETWORK_CLIENT) {
                clientUI.SetActive(true);
            }
            else {
                serverUI.SetActive(true);
                ChangeLevel(serverLevelSelect);
                ChangeTimeLimit(serverTimeSelect);
                SetIPAddress();
            }
        }

        public void OnGUI()
        {
            UpdateClientSettings();
            UpdateServerStartButton();
        }

        [Server]
        public void SetIPAddress()
        {
            serverIPAddress.text = Network.player.ipAddress;
        }

        [Server]
        public void ChangeLevel(Dropdown target)
        {
            level = target.options[target.value].text;
        }
        
        [Server]
        public void ChangeTimeLimit(Dropdown target)
        {
            timeLimit = target.options[target.value].text;
        }

        [Client]
        public void SetReadyState(bool isReady)
        {
            PlayerTracker.GetInstance().GetLocalPlayer().GetComponent<PlayerDataForClients>().SetIsReadyFlag(isReady);
            if (readyWaitingButton == null || readyNowButton == null) {
                return;
            }

            readyWaitingButton.SetActive(!isReady);
            readyNowButton.SetActive(isReady);
        }

        public void LeaveLobby()
        {
            if (State.GetInstance().Network() == State.NETWORK_CLIENT) {
                NetworkManager.singleton.StopClient();
            }
            if (State.GetInstance().Network() == State.NETWORK_SERVER) {
                NetworkManager.singleton.StopHost();
            }
            State.GetInstance().Game(State.GAME_DISCONNECTING);
        }

        [Server]
        public void StartGame()
        {
            if (!allowServerStart) {
                return;
            }

            ModalManager.GetInstance().Show(
                "Ready to start the game?",
                "Yes!",
                "Not yet...",
                () => {
                    LevelData.GetInstance().levelTime = (serverTimeSelect.value * 5) + 5;
                    RpcUpdateClientStateOnStart();
                    NetworkManager.singleton.ServerChangeScene(serverLevelSelect.value == 0 ? "Level A" : "Level B");
                },
                () => {
                    ModalManager.GetInstance().Hide();
                }
            );
        }

        [ClientRpc]
        private void RpcUpdateClientStateOnStart()
        {
            State.GetInstance()
                .Level(State.LEVEL_NOT_READY)
                .Publish();
        }

        [ClientCallback]
        private void UpdateClientSettings()
        {
            clientLevelSelect.text = level;
            clientTimeSelect.text = timeLimit;
        }

        [ServerCallback]
        private void UpdateServerStartButton()
        {
            int[] teams = TeamTracker.GetInstance().GetTeams();
            if (teams[0] == 0 || teams[1] == 0) {
                startGameButtonText.text = "Waiting for teams";
                allowServerStart = false;
                return;
            }

            bool allReady = true;
            foreach (GameObject player in PlayerTracker.GetInstance().GetPlayers()) {
                if (!player) {
                    continue;
                }
                PlayerDataForClients settings = player.GetComponent<PlayerDataForClients>();
                if (!settings.GetIsReadyFlag() && !settings.GetIsServerFlag()) {
                    allReady = false;
                }
            }
            
            startGameButtonText.text = allReady ? "Start game" : "Waiting on ready";
            allowServerStart = allReady;
        }
    }
}