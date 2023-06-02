using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class AccountManager : MonoBehaviour
    {
        public static AccountManager Instance { get; private set; }

        string userName;
        public string UserName
        {
            get { return userName; }
        }

        string userId;
        

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                // Test
                userName = System.Guid.NewGuid().ToString();
                
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        }
    }

}
