// UI/Level/LevelCompleteUI.cs

using GameState;
using Player.SyncedData;
using Player.Tracking;
using UI.Level;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UI.Level {
    class DescendingComparer<T> : IComparer<T> where T : IComparable<T> {
        public int Compare (T x, T y)
        {
            return y.CompareTo(x);
        }
    }

    class LevelCompleteUI : MonoBehaviour {

        public GameObject levelComplete;
        public GameObject clientLevelComplete;
        public GameObject serverLevelComplete;
        
        public GameObject scoreboardViewport;
        public GameObject scoreboardEntryPrefab;

        public int entryPrefabHeight = 80;

        public void Start ()
        {
            State.GetInstance().Subscribe(
                new StateOption()
                    .LevelState(State.LEVEL_COMPLETE),
                ShowLevelComplete
            );
        }

        public void ShowLevelComplete ()
        {
            if (levelComplete == null || clientLevelComplete == null || serverLevelComplete == null) {
                return;
            }

            levelComplete.SetActive(true);
            if (State.GetInstance().Network() == State.NETWORK_CLIENT) {
                clientLevelComplete.SetActive(true);
            }
            else {
                serverLevelComplete.SetActive(true);
            }

            UpdateScoreboard();
        }

        public void ReturnToLobby ()
        {
            NetworkManager.singleton.ServerChangeScene("Lobby");
        }

        private void UpdateScoreboard ()
        {
            SortedDictionary<int, List<GameObject>> scores = new SortedDictionary<int, List<GameObject>>(new DescendingComparer<int>());

            if (scoreboardEntryPrefab == null) {
                return;
            }

            foreach (GameObject player in PlayerTracker.GetInstance().GetPlayers()) {
                if (player ==  null || player.GetComponent<PlayerDataForClients>() == null) {
                    continue;
                }

                int team = player.GetComponent<PlayerDataForClients>().GetTeam();
                if (team == PlayerDataForClients.TEAM_SPECTATOR) {
                    continue;
                }

                GameObject entry = Instantiate(scoreboardEntryPrefab);
                ScoreboardEntryUI ui = entry.GetComponent<ScoreboardEntryUI>();
                ui.SetName(player.GetComponent<PlayerDataForClients>().GetName());

                int score = player.GetComponent<PlayerDataForClients>().GetScore();
                ui.SetScore(score);
                if (player == PlayerTracker.GetInstance().GetLocalPlayer()) {
                    ui.FlagCurrentPlayer();
                }

                entry.transform.SetParent(scoreboardViewport.transform, false);

                if (!scores.ContainsKey(score)) {
                    scores.Add(score, new List<GameObject>());
                }
                scores[score].Add(entry);
            }

            int counter = 0;
            foreach (KeyValuePair<int, List<GameObject>> values in scores) {
                foreach (GameObject player in values.Value) {
                    Vector3 localPos = player.GetComponent<RectTransform>().localPosition;
                    player.GetComponent<RectTransform>().localPosition = new Vector3(localPos.x, -(entryPrefabHeight / 2) + (-(entryPrefabHeight + 2) * counter), localPos.z);
                    counter++;
                }
            }

            RectTransform transform = scoreboardViewport.GetComponent<RectTransform>();
            transform.sizeDelta = new Vector2(transform.sizeDelta.x, counter * (entryPrefabHeight + 2));
        }
    }
}
