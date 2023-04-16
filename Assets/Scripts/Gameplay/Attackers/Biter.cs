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

            // Adjust the player position and rotation
            //victim.transform.DORotateQuaternion(bitePivots[0].rotation, 0.25f);
            //yield return victim.transform.DOMove(bitePivots[0].position, 0.25f).WaitForCompletion();

            //total += (float)(System.DateTime.Now - start).TotalSeconds;

            // Wait for the monster to open its mouth
            //start = System.DateTime.Now;
            //yield return new WaitForSeconds(animationBiteTime - total);
            //// Bite the player
            //FixedJoint joint = bitePivots[1].GetComponent<FixedJoint>();
            //GameObject targetNode = new List<Transform>(victim.GetComponentsInChildren<Transform>()).Find(t => t.gameObject.name.Equals("BiteTarget")).gameObject;
            //// Just move the player to the right position
            //Vector3 dir = joint.transform.position - targetNode.transform.position;
            //Debug.Log("Joint.Pos:" + joint.transform.position);
            //Debug.Log("Target.Pos:" + targetNode.transform.position);
            //Debug.Log("Dir:" + dir);
            //yield return targetNode.transform.root.DOMove(targetNode.transform.root.position + dir, 0.5f, false).WaitForCompletion();

            //total += (float)(System.DateTime.Now - start).TotalSeconds;




            
           
        }
        #endregion

        #region animation events
      

        public void OnBite(int id)
        {
            switch (id)
            {
                case 0: // Adjust monster position
                    Vector3 dir = victim.transform.position - bitePivots[0].position;
                    transform.DOMove(transform.position + dir, 0.2f);
                    break;

                case 4: // Blind the player and move the camera outside
                    victim.SwitchToGhostMode();
                    break;

                case 1: // Bite the player
                    Joint joint = bitePivots[1].GetComponent<ConfigurableJoint>();
                    GameObject targetNode = victim.HeadPivot;
                    Vector3 endV = targetNode.transform.localPosition;
                    joint.connectedBody = targetNode.transform.parent.GetComponent<Rigidbody>();
                    if(endV.magnitude > 0)
                        DOTween.To(() => joint.anchor, x => joint.anchor = x, endV, 0.2f);
                    targetNode.transform.root.GetComponent<Animator>().enabled = false;
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

