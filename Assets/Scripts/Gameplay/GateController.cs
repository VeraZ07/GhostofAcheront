using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace GOA
{
    public class GateController : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> objects;

        [SerializeField]
        List<VisualEffect> vfxs;

        [SerializeField]
        Material dissolveMaterial;


        PuzzleController puzzleController;
        //bool dissolving = false;

        private void Awake()
        {
            // Randomize
            Animator[] animators = GetComponentsInChildren<Animator>();
            foreach(Animator animator in animators)
            {
                animator.SetFloat("Offset", Random.Range(0f, 1f));
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.L))
            //{
            //    Open();
            //}
#endif

        }

        void HandleOnPuzzleSolved(PuzzleController puzzleController)
        {
            if (this.puzzleController != puzzleController)
                return;

            // Open the gate
            Open();
        }

        void Open()
        {
           
            // Replace material
            foreach (GameObject obj in objects)
            {
                Renderer rend = obj.GetComponentInChildren<Renderer>();
                rend.material = dissolveMaterial;
            }

            foreach (VisualEffect vfx in vfxs)
            {
                vfx.Play();
            }

            StartCoroutine(Dissolve());
        }
        
        IEnumerator Dissolve()
        {
            yield return PlayEffects();

            yield return new WaitForSeconds(1f);
            DisableCollisionAndDestroy();
        }

        void DisableCollisionAndDestroy()
        {
            GetComponentInChildren<Collider>().enabled = false;
            int count = objects.Count;
            for (int i = 0; i < count; i++)
                Destroy(objects[i]);
        }

        IEnumerator PlayEffects()
        {
            dissolveMaterial.SetFloat("_DissolveAmount", 0f);
            float speed = .75f;
            float amount = 0;
            while (amount < 1)
            {
                amount += speed * Time.deltaTime;
                dissolveMaterial.SetFloat("_DissolveAmount", amount);
                yield return null;
            }
        }

        public void Init(PuzzleController puzzleController)
        {
            this.puzzleController = puzzleController;
            PuzzleController.OnSolvedChangedCallback += HandleOnPuzzleSolved;

            // Check if the puzzle has been solved ( it may happen on host migration ??? )
            if (puzzleController.Solved)
                DisableCollisionAndDestroy();
            
        }
    }

}
