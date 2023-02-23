using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace GOA.Level
{
    public partial class LevelBuilder : MonoBehaviour
    {
        [SerializeField]
        GameObject globalVolumePrefab;

        [SerializeField]
        GameObject localVolumetricFogPrefab;

        [SerializeField]
        List<GameObject> floatingLightPrefabList;

        [SerializeField]
        List<GameObject> crackLightPrefabList;

        GameObject globalVolume;
        
        Transform lightingRoot;

        Dictionary<int, GameObject> localVolumetricFogDictionary = new Dictionary<int, GameObject>();

        Dictionary<int, GameObject> floatingLightDictionary = new Dictionary<int, GameObject>();
        
        Dictionary<int, GameObject> crackLightDictionary = new Dictionary<int, GameObject>();

        
        

        // Start is called before the first frame update
        //void Start()
        //{
        //    CreateLighting(FindObjectOfType<LevelBuilder>());
        //}

        //// Update is called once per frame
        //void Update()
        //{

        //}

        void CreateGlobalVolume()
        {
            globalVolume = GameObject.Instantiate(globalVolumePrefab, lightingRoot);
            globalVolume.transform.localPosition = Vector3.zero;
            globalVolume.transform.localRotation = Quaternion.identity;
        }

        void CreateRootTransform()
        {
            lightingRoot = new GameObject("Lighting").transform;
            lightingRoot.transform.position = Vector3.zero;
            lightingRoot.transform.rotation = Quaternion.identity;
        }

        void CreateLocalFogVolumes()
        {
            // Destroy old elements ( for testing purpose only )
            foreach (GameObject fog in localVolumetricFogDictionary.Values)
                DestroyImmediate(fog);

            localVolumetricFogDictionary.Clear();

            float minRatio = 0.2f;
            float maxRatio = 0.45f;
            float ratio = Random.Range(minRatio, maxRatio);
            int size = (int)Mathf.Sqrt(tiles.Length);
           
            int max = (int)(tiles.Length * ratio);
           

            // Create an array of available tiles      
            List<int> availables = new List<int>();
            for(int i=0; i<tiles.Length; i++)
            {
                if (!tiles[i].unreachable)
                    availables.Add(i);
            }

            while(localVolumetricFogDictionary.Keys.Count < max && availables.Count > 0)
            {
                // Get a random tile that has not been taken yet
                int id = availables[Random.Range(0, availables.Count)];
                availables.Remove(id);

                // Create a new fog volume
                GameObject fogObject = GameObject.Instantiate(localVolumetricFogPrefab, lightingRoot);
                

                // Put the volume in the right position
                fogObject.transform.position = tiles[id].GetPosition();
                fogObject.transform.rotation = Quaternion.identity;

                LocalVolumetricFog fogVolume = fogObject.GetComponentInChildren<LocalVolumetricFog>();

                // Set volume fade 
                float minFade = .2f;
                float maxFade = .5f;
                fogVolume.parameters.positiveFade.x = Random.Range(minFade, maxFade);
                fogVolume.parameters.positiveFade.z = Random.Range(minFade, maxFade);
                fogVolume.parameters.negativeFade.x = Random.Range(minFade, maxFade);
                fogVolume.parameters.negativeFade.z = Random.Range(minFade, maxFade);
                
                

                // Check if an adjacent tile contains another fog volume
                int top = id - size >= 0 ? id - size : -1;
                int right = id + 1 < tiles.Length && id + 1 % size != 0 ? id + 1 : -1;
                int down = id + size < tiles.Length ? id + size : -1;
                int left = id - 1 >= 0 && id - 1 % size != size - 1 ? id - 1 : -1;

                // Check for tile on top
                if (top != -1 && localVolumetricFogDictionary.ContainsKey(top))
                {
                    // Check if these two tiles are connected
                    if (!tiles[id].isUpperBorder && !tiles[id].isUpperBoundary &&
                        tiles[top].roteableWall != 3 && // Top
                        (left == -1 || tiles[top-1].roteableWall != 1)) // Left
                    {
                        // We must attach these two volumes
                        fogVolume.parameters.positiveFade.z = 0;
                        localVolumetricFogDictionary[top].GetComponentInChildren<LocalVolumetricFog>().parameters.negativeFade.z = 0f;
                    }
                }

                // Check for tile to the right
                if (right != -1 && localVolumetricFogDictionary.ContainsKey(right))
                {
                    // Check if they are connected
                    if (!tiles[id].isRightBorder && !tiles[id].isRightBoundary && tiles[id].roteableWall != 0 &&
                        (top == -1 || tiles[top].roteableWall != 2))
                    {
                        fogVolume.parameters.positiveFade.x = 0;
                        localVolumetricFogDictionary[right].GetComponentInChildren<LocalVolumetricFog>().parameters.negativeFade.x = 0f;
                    }
                }

                // Check to the bottom
                if (down != -1 && localVolumetricFogDictionary.ContainsKey(down))
                {
                    // Check for connection
                    if (!tiles[id].isBottomBorder && !tiles[id].isBottomBoundary && tiles[id].roteableWall != 3 &&
                        (left == -1 || tiles[left].roteableWall != 1))
                    {
                        fogVolume.parameters.negativeFade.z = 0;
                        localVolumetricFogDictionary[down].GetComponentInChildren<LocalVolumetricFog>().parameters.positiveFade.z = 0f;
                    }
                }

                if (left != -1 && localVolumetricFogDictionary.ContainsKey(left))
                {
                    // Check for connection
                    if (!tiles[id].isLeftBorder && !tiles[id].isLeftBoundary &&
                        tiles[left].roteableWall != 0 &&
                        (top == -1 || tiles[top - 1].roteableWall != 2))
                    {
                        fogVolume.parameters.negativeFade.x = 0;
                        localVolumetricFogDictionary[left].GetComponentInChildren<LocalVolumetricFog>().parameters.positiveFade.x = 0f;
                    }
                }

                fogVolume.parameters.textureScrollingSpeed = Vector3.up * Random.Range(0.05f, 0.09f);

                localVolumetricFogDictionary.Add(id, fogObject);
            }

            
        }

        void CreateFloatingLights()
        {
            foreach(GameObject light in floatingLightDictionary.Values)
            {
                DestroyImmediate(light);
            }
            floatingLightDictionary.Clear();

            // How many lights
            float minRatio = 0.05f;
            float maxRatio = 0.12f;
            float ratio = Random.Range(minRatio, maxRatio);
            int size = (int)Mathf.Sqrt(tiles.Length);

            int max = (int)(tiles.Length * ratio);

            // Create an array of available tiles      
            List<int> availables = new List<int>();
            for (int i = 0; i < tiles.Length; i++)
            {
                if (!tiles[i].unreachable)
                    availables.Add(i);
            }

            while (floatingLightDictionary.Keys.Count < max && availables.Count > 0)
            {
                // Get the next tile
                int id = availables[Random.Range(0, availables.Count)];

                // Remove the current tile and the adjacent ones if they are connected
                availables.Remove(id);
                
                int top = id - size >= 0 ? id - size : -1;
                int right = id + 1 < tiles.Length && id + 1 % size != 0 ? id + 1 : -1;
                int down = id + size < tiles.Length ? id + size : -1;
                int left = id - 1 >= 0 && id - 1 % size != size - 1 ? id - 1 : -1;
                
                if (top != -1)
                    availables.Remove(top);
            
                if (top != -1 && right != -1)
                    availables.Remove(top+1);

                if (right != -1)
                    availables.Remove(right);

                if (right != -1 && down != -1)
                    availables.Remove(down + 1);

                if (down != -1)
                    availables.Remove(down);

                if (down != -1 && left != -1)
                    availables.Remove(down - 1);

                if (left != -1)
                    availables.Remove(left);

                if (top != -1 && left != -1)
                    availables.Remove(top - 1);

                // Create the light
                GameObject light = Instantiate(floatingLightPrefabList[Random.Range(0, floatingLightPrefabList.Count)], lightingRoot);
                light.transform.position = tiles[id].GetPosition();
                light.transform.rotation = Quaternion.identity;
                floatingLightDictionary.Add(id, light);
            }

        }

        void CreateCrackLights()
        {
            foreach (GameObject light in crackLightDictionary.Values)
            {
                DestroyImmediate(light);
            }
            crackLightDictionary.Clear();

            // How many lights
            float minRatio = 0.02f;
            float maxRatio = 0.06f;
            float ratio = Random.Range(minRatio, maxRatio);
            int size = (int)Mathf.Sqrt(tiles.Length);

            int max = (int)(tiles.Length * ratio);

            // Create an array of available tiles       
            List<int> availables = new List<int>();
            for (int i = 0; i < tiles.Length; i++)
            {
                bool hasWall = false;
                if (tiles[i].isBottomBorder || tiles[i].isBottomBoundary || tiles[i].isUpperBorder || tiles[i].isUpperBoundary ||
                   tiles[i].isLeftBorder || tiles[i].isLeftBoundary || tiles[i].isRightBorder || tiles[i].isRightBoundary ||
                   tiles[i].roteableWall == 0 || tiles[i].roteableWall == 3)
                    hasWall = true;



                if (!hasWall)
                {
                    int top = i - size >= 0 ? i - size : -1;
                    int right = i + 1 < tiles.Length && i + 1 % size != 0 ? i + 1 : -1;
                    int down = i + size < tiles.Length ? i + size : -1;
                    int left = i - 1 >= 0 && i - 1 % size != size - 1 ? i - 1 : -1;

                    if ((top != -1 && (tiles[top].roteableWall == 2 || tiles[top].roteableWall == 3)) ||
                       (left != -1 && (tiles[left].roteableWall == 0 || tiles[left].roteableWall == 1)))
                        hasWall = true;

                }



                if (!tiles[i].unreachable && !floatingLightDictionary.ContainsKey(i))
                    availables.Add(i);
            }

            while (crackLightDictionary.Keys.Count < max && availables.Count > 0)
            {
                // Get the next tile
                int id = availables[Random.Range(0, availables.Count)];

                // Remove the current tile and the adjacent ones if they are connected
                availables.Remove(id);

                // Check adjacent tiles
                int top = id - size >= 0 ? id - size : -1;
                int right = id + 1 < tiles.Length && id + 1 % size != 0 ? id + 1 : -1;
                int down = id + size < tiles.Length ? id + size : -1;
                int left = id - 1 >= 0 && id - 1 % size != size - 1 ? id - 1 : -1;

                // Check a wall
                bool found = false;
                List<int> dirs = new List<int>(new int[] { 0, 1, 2, 3 });
                Quaternion rot = Quaternion.identity;
                while(dirs.Count > 0 && !found)
                {
                    int dir = dirs[Random.Range(0, dirs.Count)];
                    dirs.Remove(dir);
                                        
                    switch (dir)
                    {
                        case 0: // Forward
                            if (tiles[id].isUpperBorder || tiles[id].isUpperBoundary || 
                                (top != -1 && tiles[top].roteableWall == 3))
                            {
                                found = true;
                                rot = Quaternion.LookRotation(Vector3.forward, Vector3.up);
                            }
                                break;
                        //case 1: // Right
                        //    if (tiles[id].isRightBorder || tiles[id].isRightBoundary || tiles[id].roteableWall == 0 ||
                        //        (top != -1 && tiles[top].roteableWall == 2))
                        //    {
                        //        found = true;
                        //        rot = Quaternion.LookRotation(Vector3.right, Vector3.up);
                        //    }
                        //    break;
                        //case 2: // Back
                        //    if (tiles[id].isBottomBorder || tiles[id].isBottomBoundary || tiles[id].roteableWall == 3 ||
                        //        (left != -1 && tiles[left].roteableWall == 1))
                        //    {
                        //        found = true;
                        //        rot = Quaternion.LookRotation(Vector3.back, Vector3.up);
                        //    }
                        //    break;
                        //case 3: // Left
                        //    if (tiles[id].isLeftBorder || tiles[id].isLeftBoundary || 
                        //        (left != -1 && tiles[left].roteableWall == 0))
                        //    {
                        //        found = true;
                        //        rot = Quaternion.LookRotation(Vector3.left, Vector3.up);
                        //    }
                        //    break;
                    }
                }

                if (found)
                {
                    if (top != -1)
                        availables.Remove(top);

                    if (top != -1 && right != -1)
                        availables.Remove(top + 1);

                    if (right != -1)
                        availables.Remove(right);

                    if (right != -1 && down != -1)
                        availables.Remove(down + 1);

                    if (down != -1)
                        availables.Remove(down);

                    if (down != -1 && left != -1)
                        availables.Remove(down - 1);

                    if (left != -1)
                        availables.Remove(left);

                    if (top != -1 && left != -1)
                        availables.Remove(top - 1);

                    // Create the light
                    GameObject light = Instantiate(crackLightPrefabList[Random.Range(0, crackLightPrefabList.Count)], lightingRoot);
                    light.transform.position = tiles[id].GetPosition();
                    light.transform.GetChild(0).rotation = rot;
                    crackLightDictionary.Add(id, light);
                }
                

               

                
            }

        }

        void CreateLighting()
        {
            
            CreateRootTransform();

            CreateGlobalVolume();

            CreateLocalFogVolumes();

            CreateFloatingLights();

            //CreateCrackLights();
        }


    }

}
