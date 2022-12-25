using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class PlayerCharacter : NetworkBehaviour
    {


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Spawned()
        {
            base.Spawned();

            // Change the color depending on the team
            Player player = new List<Player>(FindObjectsOfType<Player>()).Find(p => p.PlayerRef == Object.InputAuthority);
            MaterialSetter[] mats = GetComponentsInChildren<MaterialSetter>();
            foreach(MaterialSetter mat in mats)
            {
                if (player.TeamId == TeamManager.HomeTeamId)
                    mat.SetHomeMaterial();
                else
                    mat.SetAwayMaterial();
            }
        }
    }

}
