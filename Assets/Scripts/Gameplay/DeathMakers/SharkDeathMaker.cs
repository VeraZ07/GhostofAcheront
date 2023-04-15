using DG.Tweening;
using GOA.Interfaces;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace GOA
{
    public class SharkDeathMaker : MonoBehaviour, IDeathMaker
    {

        [SerializeField]
        Transform[] bitePivots;

        private void Awake()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        public void Kill(PlayerController victim, int attackType)
        {
            switch (attackType)
            {
                case 0:
                    StartCoroutine(Bite(victim));
                    break;
            }
        }

        IEnumerator Bite(PlayerController victim)
        {
            float animationLength = 2.48f;
            float animationBiteTime = 1.36f;
            float total = 0f;
            System.DateTime start;
            Debug.Log("Start killing...");
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            victim.SetDyingState();

            // Just wait a little bit
            start = System.DateTime.Now;
            yield return new WaitForSeconds(.1f);

            // Adjust the player position and rotation
            victim.transform.DORotateQuaternion(bitePivots[0].rotation, 0.5f);
            yield return victim.transform.DOMove(bitePivots[0].position, 0.5f).WaitForCompletion();

            total = (float)(System.DateTime.Now - start).TotalSeconds;

            // Wait for the monster to open its mouth
            start = System.DateTime.Now;
            yield return new WaitForSeconds(animationBiteTime - total);
            // Bite the player



            yield return new WaitForSeconds(animationLength/* - 0.6f*/);


            victim.SetDeadState();
            agent.isStopped = false;
            

            GetComponent<MonsterController>().KillCompleted();
           
        }

    }
}

