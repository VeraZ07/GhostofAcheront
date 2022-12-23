using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class Team : NetworkBehaviour
    {
        [UnitySerializeField]
        [Networked] byte TeamId { get; set; }


        public bool IsHome
        {
            get { return TeamId == TeamManager.HomeTeamId; }
        }

        public bool IsAway
        {
            get { return TeamId == TeamManager.AwayTeamId; }
        }



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        #region public methods
        public void Init(byte teamId)
        {
            TeamId = teamId;
        }

        #endregion
    }

}
