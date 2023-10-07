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

        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    PlayerController pc =new List<PlayerController>(FindObjectsOfType<PlayerController>()).Find(p => p.Runner.IsServer);
            //    victim = pc;
            //    DoBite();
            //}
            //if (Input.GetKeyDown(KeyCode.Alpha2))
            //{
            //    PlayerController pc = new List<PlayerController>(FindObjectsOfType<PlayerController>()).Find(p => p.Runner.IsClient);
            //    victim = pc;
            //    DoBite();
            //}
        }
        #endregion

        #region interface implementation
        
        public void Kill(PlayerController victim, int attackId)
        {
            if (!Runner.IsServer && !Runner.IsSharedModeMasterClient)
                return;

            this.victim = victim;
            VictimId = victim.Object.InputAuthority.PlayerId;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            //victim.SetDyingState(); // Send an rpc to tell the client to set the sying state
            victim.RpcSetDyingState();
            RpcStartKilling();

            animator.SetFloat(IKiller.ParamAttackId, attackId);
            animator.SetTrigger(IKiller.ParamAttackTrigger);

        }
        #endregion

        #region private
        [Rpc(sources:RpcSources.StateAuthority, targets:RpcTargets.All, Channel = RpcChannel.Reliable, InvokeLocal = true)]
        void RpcStartKilling()
        {
            StartCoroutine(DoKilling());
        }

        IEnumerator DoKilling()
        {
            float length = 2.5f;
            yield return new WaitForSeconds(length * .34f);
            AdjustMonsterPosition();
            yield return new WaitForSeconds(length * (.41f - .34f));
            victim.LookAtYouDying();
            yield return new WaitForSeconds(length * (.51f - .41f));
            Bite();
            yield return new WaitForSeconds(length * (0.94f - .51f));
            FinalizeDeath();
        }
        #endregion

        #region animation events


       

        void FinalizeDeath()
        {
            if (Runner.IsServer || Runner.IsSharedModeMasterClient)
            {
                Debug.Log("DEAD SET");
                agent.isStopped = false;
                monster.SetIdleState();
            }

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

