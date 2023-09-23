using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleBuilder : MonoBehaviour
{

    const int size = 16;

    int sizeSqrt;
    /// <summary>
    /// 0: north
    /// 1: east
    /// 2: south
    /// 3: west
    /// </summary>
    int[] values = new int[size];

    private void Awake()
    {
        sizeSqrt = (int)Mathf.Sqrt(size);
    }

    // Start is called before the first frame update
    void Start()
    {
        ComputeValues();

        DebugPuzzle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ComputeValues()
    {
        // Reset the array
        for (int i = 0; i < size; i++)
            values[i] = -1;

        // Create
        List<int> currentIds = new List<int>();
        //List<int> valuesList = new List<int>();
        for(int i=0; i<size; i++)
        {
            if (values[i] < 0)
            {
                
                Check(i, currentIds);
            }
                
        }
    }

    bool CollideWithPreviousValues(int id, int value)
    {
        List<int> ids = null;
        bool ret = false;
        switch (value)
        {
            case 0:
                ids = new List<int>(values).FindAll(i => i == 2);
                if (ids.Exists(i => i < id && i % sizeSqrt == id % sizeSqrt))
                    ret = true;
                break;
            case 1:
                ids = new List<int>(values).FindAll(i => i == 3);
                if (ids.Exists(i => i > id && i / sizeSqrt == id / sizeSqrt))
                    return true;
                break;
            case 2:
                ids = new List<int>(values).FindAll(i => i == 0);
                if (ids.Exists(i => i > id && i % sizeSqrt == id % sizeSqrt))
                    ret = true;
                break;
            case 3:
                ids = new List<int>(values).FindAll(i => i == 1);
                if (ids.Exists(i => i < id && i / sizeSqrt == id / sizeSqrt))
                    return true;
                break;

        }
        
            

        return ret;
    }

    void Check(int id, List<int> currentIds)
    {
       
        List<int> choices = new List<int>(new int[] { 0, 1, 2, 3 });
        
        // Filter choices
        if ( CollideWithPreviousValues(id, 0) || currentIds.Exists(i => id % sizeSqrt == i % sizeSqrt && i < id))
            choices.Remove(0);
        if (CollideWithPreviousValues(id, 1) || currentIds.Exists(i => i > id && i / sizeSqrt == id / sizeSqrt))
            choices.Remove(1);
        if (CollideWithPreviousValues(id, 2) || currentIds.Exists(i => id % sizeSqrt == i % sizeSqrt && i > id))
            choices.Remove(2);
        if (CollideWithPreviousValues(id, 3) || currentIds.Exists(i => i < id && i / sizeSqrt == id / sizeSqrt))
            choices.Remove(3);
        //if((!IsEastEdge(id) ))

        int candidateValue = choices[Random.Range(0, choices.Count)];
        choices.Remove(candidateValue);
        values[id] = candidateValue;
        currentIds.Add(id);

        bool isFree = (IsNorthEdge(id) && candidateValue == 0) || (IsEastEdge(id) && candidateValue == 1) || (IsSouthEdge(id) && candidateValue == 2) || (IsWestEdge(id) && candidateValue == 3);
        if (!isFree)
        {
            // Check the next id
            List<int> allowedIds = new List<int>();
            if (id - sizeSqrt >= 0 && values[id - sizeSqrt] < 0)
                allowedIds.Add(id - sizeSqrt);
            if (id / sizeSqrt == (id + 1) / sizeSqrt && values[id + 1] < 0)
                allowedIds.Add(id + 1);
            if (id + sizeSqrt < size && values[id + sizeSqrt] < 0)
                allowedIds.Add(id + sizeSqrt);
            if (id / sizeSqrt == (id - 1) / sizeSqrt && id - 1 >= 0 && values[id - 1] < 0)
                allowedIds.Add(id - 1);


            // Check next
            int candidateId = allowedIds[Random.Range(0, allowedIds.Count)];
            Check(candidateId, currentIds);

        }
       

    }

    bool IsNorthEdge(int id)
    {
        return id - sizeSqrt < 0;
    }

    bool IsSouthEdge(int id)
    {
        return id + sizeSqrt >= size;
    }

    bool IsWestEdge(int id)
    {
        return id % sizeSqrt == 0;
    }

    bool IsEastEdge(int id)
    {
        
        return id % sizeSqrt == sizeSqrt-1;
    }

    void DebugPuzzle()
    {
        
        string s = "[ArrowPuzzle]";
        for (int i=0; i<size; i = i+4)
        {
            s += $"\n{values[i]}    {values[i + 1]}   {values[i + 2]}   {values[i + 3]}";                                    
        }
        s += "[/ArrowPuzzle]";

        Debug.Log(s);
    }
}
