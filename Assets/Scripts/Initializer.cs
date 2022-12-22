using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class Initializer : MonoBehaviour
    {
        [SerializeField]
        GameObject sessionManagerPrefab;

        private void Awake()
        {
            Instantiate(sessionManagerPrefab, Vector3.zero, Quaternion.identity);
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if(!SessionManager.Instance)
            {
                Instantiate(sessionManagerPrefab, Vector3.zero, Quaternion.identity);
            }
        }
    }

}
