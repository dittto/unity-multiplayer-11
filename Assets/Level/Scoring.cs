// Level/Scoring.cs

using UnityEngine;
using UnityEngine.Networking;
using GameState;
using Player.SyncedData;
using Player.Tracking;
using System.Collections;

namespace Level {
    public class Scoring : NetworkBehaviour {

        private bool keepScoring = false;

        public void Awake ()
        {
            if (State.GetInstance().Network() == State.NETWORK_SERVER) {
                SubscribeToServerPlaying();

                foreach (GameObject player in PlayerTracker.GetInstance().GetPlayers()) {
                    ResetPlayerScore(player);
                }
                PlayerTracker.GetInstance().OnPlayerAdded += ResetPlayerScore;
            }
        }

        [Server]
        private void ResetPlayerScore(GameObject player)
        {
            player.GetComponent<PlayerDataForClients>().ResetScore();
        }
        
        [Server]
        private void SubscribeToServerPlaying ()
        {
            State.GetInstance().Subscribe(
                new StateOption().LevelState(State.LEVEL_PLAYING),
                StartScoringTimer
            );
            State.GetInstance().Subscribe(
                new StateOption().LevelState(State.LEVEL_COMPLETE),
                () => {
                    keepScoring = false;
                }
            );
        }

        [Server]
        private void StartScoringTimer ()
        {
            if (this != null) {
                keepScoring = true;
                StartCoroutine(this.CalculateScore());
            }
        }

        [Server]
        private IEnumerator CalculateScore ()
        {
            while (keepScoring) {

                GameObject highestXPlayer = null;
                float highestX = float.MinValue;
                foreach (GameObject player in PlayerTracker.GetInstance().GetPlayers()) {
                    int team = player.GetComponent<PlayerDataForClients>().GetTeam();
                    if (player.transform.position.x > highestX && team != PlayerDataForClients.TEAM_SPECTATOR) {
                        highestXPlayer = player;
                        highestX = player.transform.position.x;
                    }
                }

                if (highestXPlayer) {
                    highestXPlayer.GetComponent<PlayerDataForClients>().ServerIncrementScore();
                }

                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}
