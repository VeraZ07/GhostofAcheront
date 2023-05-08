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

        [SerializeField]
        AudioSource dungeonSource;

        float waterTimeMin = 20f;
        float waterTimeMax = 40f;
        float waterTime = 0;
        bool waterPlaying = false;

        float dungeonTimeMin = 20;
        float dungeonTimeMax = 40;
        float dungeonTime = 0;
        bool dungeonPlaying = false;

        // Start is called before the first frame update
        void Start()
        {
            //waterTime = Random.Range(waterTimeMin, waterTimeMax);
        }

        // Update is called once per frame
        void Update()
        {
            //CheckWater();
            CheckDungeon();
        }

        void CheckWater()
        {
            if (!waterSource.isPlaying)
            {
                if (waterPlaying)
                {
                    waterPlaying = false;
                    waterTime = Random.Range(waterTimeMin, waterTimeMax);     
                }
                else
                {
                    if (waterTime < 0)
                    {
                        waterPlaying = true;
                        waterSource.Play();
                    }
                    else
                    {
                        waterTime -= Time.deltaTime;
                    }
                }
            }
        }

        void CheckDungeon()
        {
            if (dungeonSource.isPlaying)
            {
                if (dungeonPlaying)
                {
                    dungeonPlaying = false;
                    dungeonTime = Random.Range(dungeonTimeMin, dungeonTimeMax);
                }
                else
                {
                    if(dungeonTime > 0)
                    {
                        dungeonTime -= Time.deltaTime;
                    }
                    else
                    {
                        dungeonPlaying = true;
                        dungeonSource.Play();
                    }
                }
            }
           
        }
    }

}
