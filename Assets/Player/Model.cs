// Player/Model.cs

using UnityEngine;
using Player.SyncedData;

namespace Player {
    public class Model : MonoBehaviour {

        public GameObject inhumerPrefab;
        public GameObject vipPrefab;
        public GameObject spectatorPrefab;

        private GameObject currentModel;
        private bool isPlayerCharacter = false;

        public void Awake ()
        {
            PlayerDataForClients playerData = GetComponent<PlayerDataForClients>();
            ChooseModel(gameObject, playerData.GetTeam());
            playerData.OnTeamUpdated += ChooseModel;

        }

        public bool IsPlayerCharacter ()
        {
            return isPlayerCharacter;
        }

        private void ChooseModel (GameObject current, int team)
        {
            if (team == PlayerDataForClients.TEAM_INHUMER) {
                AddModelToPlayer(inhumerPrefab);
                UpdatePlayerColour(team);

                return;
            }
            if (team == PlayerDataForClients.TEAM_VIP) {
                AddModelToPlayer(vipPrefab);
                UpdatePlayerColour(team);

                return;
            }
            if (team == PlayerDataForClients.TEAM_SPECTATOR) {
                AddModelToPlayer(spectatorPrefab);
                isPlayerCharacter = true;

                return;
            }

            isPlayerCharacter = false;
        }

        private void AddModelToPlayer (GameObject prefab)
        {
            Destroy(currentModel);
            currentModel = Instantiate<GameObject>(prefab);
            currentModel.transform.SetParent(transform);
        }

        private void UpdatePlayerColour (int team)
        {
            isPlayerCharacter = true;
            foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>()) {
                renderer.material.color = team == PlayerDataForClients.TEAM_VIP ? Color.red : Color.blue;
            }
        }
    }
}