using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Audio
{
    public class AmbientSound : MonoBehaviour
    {
        [SerializeField]
        AudioSource ambientSource;

        [SerializeField]
        AudioSource waterSource;

        float waterTimeMin = 20f;
        float waterTimeMax = 40f;
        float waterTime = 0;
        bool playing = false;

        // Start is called before the first frame update
        void Start()
        {
            //waterTime = Random.Range(waterTimeMin, waterTimeMax);
        }

        // Update is called once per frame
        void Update()
        {
            //CheckWater();
        }

        void CheckWater()
        {
            if (!waterSource.isPlaying)
            {
                if (playing)
                {
                    playing = false;
                    waterTime = Random.Range(waterTimeMin, waterTimeMax);     
                }
                else
                {
                    if (waterTime < 0)
                    {
                        playing = true;
                        waterSource.Play();
                    }
                    else
                    {
                        waterTime -= Time.deltaTime;
                    }
                }
            }
        }
    }

}
