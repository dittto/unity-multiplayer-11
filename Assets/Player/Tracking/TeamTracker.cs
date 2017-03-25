// Player/Tracking/TeamTracker.cs

using Player.SyncedData;
using UnityEngine;

namespace Player.Tracking {
    public class TeamTracker {

        private static TeamTracker instance;
        
        public delegate void TeamChanged(int numVIPs, int numInhumers);
        public event TeamChanged OnTeamChanged;

        private int vips = 0;
        private int inhumers = 0;

        private TeamTracker()
        {
            foreach (GameObject player in PlayerTracker.GetInstance().GetPlayers()) {
                AddTeamTrackingToPlayers(player);
            }
            PlayerTracker.GetInstance().OnPlayerAdded += AddTeamTrackingToPlayers;
            PlayerTracker.GetInstance().OnPlayerRemoved += (GameObject player) => {
                CountTeams();
            };
        }
        
        public static TeamTracker GetInstance()
        {
            if (instance == null) {
                instance = new TeamTracker();
            }

            return instance;
        }

        public void ForceRecount()
        {
            CountTeams();
        }

        private void AddTeamTrackingToPlayers(GameObject player)
        {
            player.GetComponent<PlayerDataForClients>().OnTeamUpdated += (GameObject localPlayer, int teamId) => {
                CountTeams();
            };
        }

        private void CountTeams()
        {
            vips = inhumers = 0;
            foreach (GameObject player in PlayerTracker.GetInstance().GetPlayers()) {
                if (player.GetComponent<PlayerDataForClients>().GetTeam() == PlayerDataForClients.TEAM_VIP) {
                    vips++;
                }
                else if (player.GetComponent<PlayerDataForClients>().GetTeam() == PlayerDataForClients.TEAM_INHUMER) {
                    inhumers++;
                }
            }

            if (OnTeamChanged != null) {
                OnTeamChanged(vips, inhumers);
            }
        }

        public int[] GetTeams()
        {
            return new int[] { vips, inhumers };
        }
    }
}