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

        [SerializeField]
        NetworkObject teamPrefab;

        static Team homeTeam;
        public static Team HomeTeam
        {
            get
            {
                if (!homeTeam)
                    homeTeam = new List<Team>(FindObjectsOfType<Team>()).Find(t => t.IsHome);
                return homeTeam;
            }
        }

        static Team awayTeam;
        public static Team AwayTeam
        {
            get
            {
                if (!awayTeam)
                    awayTeam = new List<Team>(FindObjectsOfType<Team>()).Find(t => t.IsAway);
                return awayTeam;
            }
        }

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
        /// <summary>
        /// Called only by the server
        /// </summary>
        /// <param name="runner"></param>
        public static void Init(NetworkRunner runner)
        {
            if (runner.IsServer)
            {
                // Create home team
                runner.Spawn(instance.teamPrefab, Vector3.zero, Quaternion.identity, runner.LocalPlayer,
                    (runner, o) =>
                    {
                        o.GetComponent<Team>().Init(HomeTeamId);
                    }
                    );

                // Create away team
                runner.Spawn(instance.teamPrefab, Vector3.zero, Quaternion.identity, runner.LocalPlayer,
                    (runner, o) =>
                    {
                        o.GetComponent<Team>().Init(AwayTeamId);
                    }
                    );

            }
        }

        /// <summary>
        /// Every one can call this ( for example on runner shutdown )
        /// </summary>
        public static void Clear()
        {
            if (homeTeam)
                Destroy(homeTeam.gameObject);
            if (awayTeam)
                Destroy(awayTeam.gameObject);
        }

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
