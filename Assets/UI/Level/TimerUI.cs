// UI/Level/TimerUI.cs

using GameState;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI.Level {
    public class TimerUI : NetworkBehaviour {
        
        [SyncVar]
        public int timer = 10;

        public Text timerText;

        public void Awake ()
        {
            timer = LevelData.GetInstance().levelTime;
            SubscribeToServerReady();
        }

        public void Update ()
        {
            timerText.text = Mathf.FloorToInt(timer / 60).ToString().PadLeft(2, '0') + ":" + (timer % 60).ToString().PadLeft(2, '0');
        }

        public void OnDestroy()
        {
            StopAllCoroutines();
        }

        [ServerCallback]
        private void SubscribeToServerReady ()
        {
            State.GetInstance().Subscribe(
                new StateOption()
                    .LevelState(State.LEVEL_READY),
                StartTimer
            );
        }

        [Server]
        private void StartTimer ()
        {
            if (this != null) {
                StartCoroutine(this.WaitForTimerToEnd());
                RpcStartTheGame();
            }
        }

        [Server]
        private IEnumerator WaitForTimerToEnd ()
        {
            while (timer > 0) {
                yield return new WaitForSeconds(1);
                timer--;
            }
            
            RpcEndTheGame();
        }

        [ClientRpc]
        private void RpcStartTheGame()
        {
            State.GetInstance()
                .Level(State.LEVEL_PLAYING)
                .Publish();
        }

        [ClientRpc]
        private void RpcEndTheGame()
        {
            State.GetInstance()
                .Level(State.LEVEL_COMPLETE)
                .Publish();
        }
    }
}
