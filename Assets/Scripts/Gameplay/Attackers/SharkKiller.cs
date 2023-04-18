using DG.Tweening;
using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace GOA
{
    public class SharkKiller : MonoBehaviour, IKiller
    {
        #region fields
        [SerializeField]
        Transform[] bitePivots;

       
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
            this.victim = victim;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            victim.SetDyingState();

            animator.SetFloat(IKiller.ParamAttackId, attackId);
            animator.SetTrigger(IKiller.ParamAttackTrigger);

            //StartCoroutine(Bite(victim));
        }
        #endregion


        #region animation events
      

        public void OnBite(int id)
        {
            switch (id)
            {
                case 0: // Adjust monster position
                    
                    
                    transform.DORotateQuaternion(Quaternion.LookRotation((victim.transform.position - transform.position).normalized, Vector3.up), 0.2f); 

                    break;

                case 4: // Blind the player and move the camera outside
                    victim.LookAtYouDying();
                    break;

                case 1: // Bite the player
                    animator.speed = 0;
                    Joint joint = bitePivots[1].GetComponent<ConfigurableJoint>();
                    GameObject targetNode = victim.HeadPivot;
                    //Vector3 endV = targetNode.transform.localPosition;
                    joint.connectedBody = targetNode.transform.parent.GetComponent<Rigidbody>();
                    //if(endV.magnitude > 0)
                    //    DOTween.To(() => joint.anchor, x => joint.anchor = x, endV, 0.2f);
                    victim.GetComponent<Animator>().enabled = false;
                    
                    victim.ExplodeHead();
                    break;
                case 2:
                    // Release the player
                    
                    bitePivots[1].GetComponent<ConfigurableJoint>().connectedBody = null;
                    break;
                case 3: // Exit
                    
                    victim.SetDeadState();
                    agent.isStopped = false;
                    monster.SetIdleState();
                    break;
            }

            
        }
        #endregion
    }
}

