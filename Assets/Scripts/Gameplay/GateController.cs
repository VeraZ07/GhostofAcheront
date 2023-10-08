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

        [SerializeField]
        List<GameObject> symbols;

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

        private void OnEnable()
        {
            PuzzleController.OnSolvedChangedCallback += HandleOnPuzzleSolved;
        }

        private void OnDisable()
        {
            PuzzleController.OnSolvedChangedCallback -= HandleOnPuzzleSolved;
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
                if (obj)
                {
                    Renderer rend = obj.GetComponentInChildren<Renderer>();
                    rend.material = dissolveMaterial;
                }
                
            }

            foreach (VisualEffect vfx in vfxs)
            {
                if(vfx)
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
           
            // Check if the puzzle has been solved ( it may happen on host migration ??? )
            if (puzzleController.Solved)
                DisableCollisionAndDestroy();

            foreach(GameObject s in symbols)
            {
                s.SetActive(false);
            }

            symbols[puzzleController.PuzzleIndex].SetActive(true);
            
        }
    }

}
