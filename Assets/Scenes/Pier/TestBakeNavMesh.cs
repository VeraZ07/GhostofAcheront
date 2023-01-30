using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class TestBakeNavMesh : MonoBehaviour
{

    NavMeshSurface[] nmsList;

    // Start is called before the first frame update
    void Start()
    {
        nmsList = FindObjectsOfType<NavMeshSurface>();

        foreach (NavMeshSurface nms in nmsList)
            nms.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
