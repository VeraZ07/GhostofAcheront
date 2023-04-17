using DG.Tweening;
using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace GOA
{
    public class Biter : MonoBehaviour, IAttacker
    {
        #region fields
        [SerializeField]
        Transform[] bitePivots;

        [SerializeField]
        float alignmentSpeed = 0.05f; 

        [SerializeField]
        bool playerAlignment = false; // True if you want to align the player, otherwise the monster will be aligned

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
        public int GetAttackId()
        {
            return 0;
        }

        public void Kill(PlayerController victim)
        {
            this.victim = victim;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            victim.SetDyingState();

            animator.SetFloat(IAttacker.ParamAttackId, GetAttackId());
            animator.SetTrigger(IAttacker.ParamAttackTrigger);

            //StartCoroutine(Bite(victim));
        }
        #endregion

        #region private methods

        IEnumerator Bite(PlayerController victim)
        {
            this.victim = victim;

            animator.SetFloat(IAttacker.ParamAttackId, GetAttackId());
            animator.SetTrigger(IAttacker.ParamAttackTrigger);

            //float animationLength = 2.48f;
            float animationBiteTime = 1.36f;
            float total = 0f;
            System.DateTime start;
            Debug.Log("Start killing...");

            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            victim.SetDyingState();

            // Just wait a little bit
            start = System.DateTime.Now;
            yield return new WaitForSeconds(.1f);
        }
        #endregion

        #region animation events
      

        public void OnBite(int id)
        {
            switch (id)
            {
                case 0: // Adjust monster position
                    if(alignmentSpeed > 0)
                    {
                        Vector3 dir = victim.transform.position - bitePivots[0].position;
                        dir.y = 0;
                      
                        if(!playerAlignment)
                            transform.DOMove(transform.position + dir, alignmentSpeed, true);
                        else
                            victim.transform.DOMove(transform.position - dir, alignmentSpeed, true);
                    }
                    
                    break;

                case 4: // Blind the player and move the camera outside
                    victim.LookAtYouDying();
                    break;

                case 1: // Bite the player
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

