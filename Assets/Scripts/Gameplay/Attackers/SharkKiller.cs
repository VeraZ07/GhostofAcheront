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

        //[Networked(OnChanged = nameof(OnVictimIdChanged))]
        //[UnitySerializeField]
        //public int VictimId { get; private set; } = -1;

        
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
        
        /// <summary>
        /// Server only
        /// </summary>
        /// <param name="victim"></param>
        /// <param name="attackId"></param>
        public void Kill(PlayerController victim, int attackId)
        {
            if (!Runner.IsServer && !Runner.IsSharedModeMasterClient)
                return;

            
            this.victim = victim;
            //VictimId = victim.Object.InputAuthority.PlayerId;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            //victim.SetDyingState(); // Send an rpc to tell the client to set the sying state
            victim.RpcSetDyingState();
            RpcStartKilling(victim.Id);

            animator.SetFloat(IKiller.ParamAttackId, attackId);
            animator.SetTrigger(IKiller.ParamAttackTrigger);

        }
        #endregion

        #region private
        [Rpc(sources:RpcSources.StateAuthority, targets:RpcTargets.All, Channel = RpcChannel.Reliable, InvokeLocal = true)]
        void RpcStartKilling(NetworkBehaviourId victimId)
        {
            StartCoroutine(DoKilling(victimId));
        }

        IEnumerator DoKilling(NetworkBehaviourId victimId)
        {
            //if (!Runner.IsServer && !Runner.IsSharedModeMasterClient)
            //    victim = new List<PlayerController>(FindObjectsOfType<PlayerController>()).Find(p => p.Object.InputAuthority.PlayerId == victimId);
            if (!Runner.IsServer && !Runner.IsSharedModeMasterClient)
                Runner.TryFindBehaviour(victimId, out victim);
            
            AdjustMonsterPosition();
            victim.LookAtYouDying();
            yield return new WaitForSeconds(.2f);
            Bite();
            yield return new WaitForSeconds(.4f);
            FinalizeDeath();
            yield return new WaitForSeconds(1f);
            ResetMonsterState();
        }
        #endregion

        #region animation events

        void ResetMonsterState()
        {
            if (Runner.IsServer || Runner.IsSharedModeMasterClient)
            {

                agent.isStopped = false;
                monster.SetIdleState();
            }
        }
       

        void FinalizeDeath()
        {
            if (victim.HasStateAuthority)
                victim.SetDeadState();
        }

        void AdjustMonsterPosition()
        {
            transform.DORotateQuaternion(Quaternion.LookRotation((victim.transform.position - transform.position).normalized, Vector3.up), 0.2f);
        }

        void Bite()
        {
           
            victim.CharacterObject.transform.position += Vector3.up * .1f;
            victim.GetComponent<Animator>().enabled = false;
            
            //Rigidbody[] bones = victim.GetComponentsInChildren<Rigidbody>();

            //foreach (Rigidbody bone in bones)
            //    bone.AddForce(victim.transform.up * 10f, ForceMode.VelocityChange);

            victim.ExplodeHead();
            
        }

   


        #endregion

        #region fusion callbacks
        //public static void OnVictimIdChanged(Changed<SharkKiller> changed)
        //{
        //    if (changed.Behaviour.Runner.IsClient && !changed.Behaviour.Runner.IsSharedModeMasterClient)
        //    {
        //        Debug.Log("OnVictimChange");
        //        // Find player controller by player id
        //        changed.Behaviour.victim = new List<PlayerController>(FindObjectsOfType<PlayerController>()).Find(p => p.Object.InputAuthority.PlayerId == changed.Behaviour.VictimId);
        //    }
     
        //}
        #endregion
    }
}

