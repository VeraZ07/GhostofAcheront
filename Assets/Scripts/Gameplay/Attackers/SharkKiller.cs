using DG.Tweening;
using Fusion;
using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace GOA
{
    public class SharkKiller : NetworkBehaviour, IKiller
    {
        #region fields
        [SerializeField]
        Transform[] bitePivots;

        [Networked(OnChanged = nameof(OnVictimIdChanged))]
        [UnitySerializeField]
        public int VictimId { get; private set; } = -1;

        
        PlayerController victim;
        MonsterController monster;
        NavMeshAgent agent;
        Animator animator;


        #endregion

        #region native
        private void Awake()
        {
            monster = GetComponent<MonsterController>();
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }
        #endregion

        #region interface implementation
        
        public void Kill(PlayerController victim, int attackId)
        {
            if (!Runner.IsServer)
                return;

            this.victim = victim;
            VictimId = victim.Object.InputAuthority.PlayerId;
            agent.velocity = Vector3.zero;
            //agent.isStopped = true;
            victim.SetDyingState();

            animator.SetFloat(IKiller.ParamAttackId, attackId);
            animator.SetTrigger(IKiller.ParamAttackTrigger);

            StartCoroutine(SetVictimDeadForSure(5f));

            //StartCoroutine(Bite(victim));
        }
        #endregion


        #region animation events
      

        public void OnBite(int id)
        {
            Debug.Log("Bite:" + id);
            switch (id)
            {
                case 0: // Adjust monster position
                    transform.DORotateQuaternion(Quaternion.LookRotation((victim.transform.position - transform.position).normalized, Vector3.up), 0.2f); 

                    break;

                case 4: // Blind the player and move the camera outside
                    victim.LookAtYouDying();
                    break;

                case 1: // Bite the player
                    StartCoroutine(DoBite());
                    break;
                case 2:
                    // Release the player
                    
                    bitePivots[1].GetComponent<FixedJoint>().connectedBody = null;
                    break;
                case 3: // Exit

                    if (Runner.IsServer)
                    {
                        Debug.Log("DEAD SET");
                        victim.SetDeadState();
                        //agent.isStopped = false;
                        monster.SetIdleState();
                    }
                    break;
            }

            
        }

        IEnumerator DoBite()
        {
            Debug.Log("DoBite()");
            Joint joint = bitePivots[1].GetComponent<FixedJoint>();
            Vector3 oldPos = bitePivots[1].localPosition;

            // Wait for the animation to run in order to obtain the right values for the pivots
            yield return new WaitForEndOfFrame();
            Vector3 dir = bitePivots[1].transform.position - victim.HeadPivot.transform.position;
            victim.transform.position += dir;
            
            victim.GetComponent<Animator>().enabled = false;
            joint.connectedBody = victim.HeadPivot.transform.parent.GetComponent<Rigidbody>();
            
            victim.ExplodeHead();
        }
        IEnumerator SetVictimDeadForSure(float delay)
        {
            if (!Runner.IsServer)
                yield break;
        }

        #endregion

        #region fusion callbacks
        public static void OnVictimIdChanged(Changed<SharkKiller> changed)
        {
            if (changed.Behaviour.Runner.IsClient)
            {
                // Find player controller by player id
                changed.Behaviour.victim = new List<PlayerController>(FindObjectsOfType<PlayerController>()).Find(p => p.Object.InputAuthority.PlayerId == changed.Behaviour.VictimId);
            }
     
        }
        #endregion
    }
}

