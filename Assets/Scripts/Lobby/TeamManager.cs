using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{

    public class TeamManager: MonoBehaviour
    {
        public const byte HomeTeamId = 1;
        public const byte AwayTeamId = 2;

        static TeamManager instance { get; set; }

        private void Awake()
        {
            if(!instance)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        


        #region private methods
       
        #endregion

        #region public methods
        public static bool HomeTeamIsFull()
        {
            return TeamIsFull(HomeTeamId);
        }

        public static bool AwayTeamIsFull()
        {
            return TeamIsFull(AwayTeamId);
        }

        public static bool TeamIsFull(byte teamId)
        {
            NetworkRunner runner = FindObjectOfType<NetworkRunner>();
            
            int count = runner.SessionInfo.MaxPlayers / 2;
            List<Player> players = new List<Player>(FindObjectsOfType<Player>());
            foreach (PlayerRef pRef in runner.ActivePlayers)
            {
                Player player = players.Find(p => p.PlayerRef == pRef);
                if (player.TeamId == teamId)
                {
                    count--;
                }
            }

           
            if (count == 0)
                return true;

            return false;
        }
        #endregion 

    }

}
