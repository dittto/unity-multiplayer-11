﻿// UI/Lobby/EntriesUI.cs

using Player.Tracking;
using System.Collections.Generic;
using UI.Lobby.Player;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI.Lobby {
    public class EntriesUI : NetworkBehaviour {

        public GameObject viewportContent;
        public Text versusText;

        public GameObject localPlayerEntryPrefab;
        public GameObject otherPlayerEntryPrefab;

        public int entryPrefabHeight = 80;
        
        private Dictionary<int, GameObject> lobbyEntries = new Dictionary<int, GameObject>();

        // on start instead of awake to make sure it happens afterwards (check if better way)
        public void Start()
        {
            foreach (GameObject player in PlayerTracker.GetInstance().GetPlayers()) {
                NewLobbyPlayerAdded(player);
            }
            PlayerTracker.GetInstance().OnPlayerAdded += NewLobbyPlayerAdded;
            PlayerTracker.GetInstance().OnPlayerRemoved += OldLobbyPlayerRemoved;
            
            TeamTracker.GetInstance().OnTeamChanged += UpdateVersusText;
            TeamTracker.GetInstance().ForceRecount();
        }

        private void NewLobbyPlayerAdded(GameObject player)
        {
            if (viewportContent == null) {
                return;
            }

            bool isLocalPlayer = player == PlayerTracker.GetInstance().GetLocalPlayer();

            GameObject lobbyEntry = Instantiate(isLocalPlayer ? localPlayerEntryPrefab : otherPlayerEntryPrefab);
            lobbyEntry.transform.SetParent(viewportContent.transform, false);
            lobbyEntry.GetComponent<EntryInterface>().SetPlayerObject(player);
            
            lobbyEntries.Add(player.GetInstanceID(), lobbyEntry);

            AlignLobbyEntriesInViewport();
        }

        private void OldLobbyPlayerRemoved(GameObject player)
        {
            if (!lobbyEntries.ContainsKey(player.GetInstanceID())) {
                return;
            }

            Destroy(lobbyEntries[player.GetInstanceID()]);
            lobbyEntries.Remove(player.GetInstanceID());

            AlignLobbyEntriesInViewport();
        }
        
        private void AlignLobbyEntriesInViewport()
        {
            if (viewportContent == null) {
                return;
            }

            int counter = 0;

            foreach (GameObject player in lobbyEntries.Values) {
                Vector3 localPos = player.GetComponent<RectTransform>().localPosition;
                player.GetComponent<RectTransform>().localPosition = new Vector3(localPos.x, -(entryPrefabHeight / 2) + (-(entryPrefabHeight + 2) * counter), localPos.z);
                counter ++;
            }

            RectTransform transform = viewportContent.GetComponent<RectTransform>();
            transform.sizeDelta = new Vector2(transform.sizeDelta.x, counter * (entryPrefabHeight + 2));
        }

        private void UpdateVersusText(int vips, int inhumers)
        {
            if (versusText != null) {
                versusText.text = vips + " vs " + inhumers;
            }
        }
    }
}