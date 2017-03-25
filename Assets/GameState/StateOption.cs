// GameState/StateOption.cs

using UnityEngine;

namespace GameState
{
    public struct StateOption
    {
        public string oldNetwork;
        public string newNetwork;
        public string oldGame;
        public string newGame;
        public string oldLevel;
        public string newLevel;

        public StateOption (
            string oldNetworkState = "", 
            string oldGameState = "", 
            string oldLevelState = "", 
            string newNetworkState = "", 
            string newGameState = "", 
            string newLevelState = ""
        ) {
            oldNetwork = oldNetworkState;
            newNetwork = newNetworkState;
            oldGame = oldGameState;
            newGame = newGameState;
            oldLevel = oldLevelState;
            newLevel = newLevelState;
        }

        public StateOption PreviousNetworkState (string oldNetworkState)
        {
            oldNetwork = oldNetworkState;

            return this;
        }

        public StateOption NetworkState(string newNetworkState)
        {
            newNetwork = newNetworkState;

            return this;
        }

        public StateOption PreviousGameState(string oldGameState)
        {
            oldGame = oldGameState;

            return this;
        }

        public StateOption GameState(string newGameState)
        {
            newGame = newGameState;

            return this;
        }

        public StateOption PreviousLevelState(string oldLevelState)
        {
            oldLevel = oldLevelState;

            return this;
        }

        public StateOption LevelState(string newLevelState)
        {
            newLevel = newLevelState;

            return this;
        }

        public bool Matches(
            string oldNetworkState,
            string oldGameState,
            string oldLevelState,
            string newNetworkState,
            string newGameState,
            string newLevelState,
            bool isNetworkDirty,
            bool isGameDirty,
            bool isLevelDirty
            )
        {
            if (oldNetwork != null && oldNetwork != "" && oldNetworkState != "" && oldNetwork != oldNetworkState) {
                return false;
            }

            if (newNetwork != null && newNetwork != "" && newNetworkState != "" && newNetwork != newNetworkState) {
                return false;
            }

            if (oldGame != null && oldGame != "" && oldGameState != "" && oldGame != oldGameState) {
                return false;
            }

            if (newGame != null && newGame != "" && newGameState != "" && newGame != newGameState) {
                return false;
            }
            
            if (oldLevel != null && oldLevel != "" && oldLevelState != "" && oldLevel != oldLevelState) {
                return false;
            }

            if (newLevel != null && newLevel != "" && newLevelState != "" && newLevel != newLevelState) {
                return false;
            }

            bool anyDirty = false;
            if (oldNetwork != null && newNetwork != null && oldNetwork != "" && newNetwork != "" && isNetworkDirty) {
                anyDirty = true;
            }
            if (oldGame != null && newGame != null && oldGame != "" && newGame != "" && isGameDirty) {
                anyDirty = true;
            }
            if (((oldLevel != null && oldLevel != "") || (newLevel != null && newLevel != "")) && isLevelDirty) {
                anyDirty = true;
            }
            if (!anyDirty) {
                return false;
            }
            
            return true;
        }
    }
}
