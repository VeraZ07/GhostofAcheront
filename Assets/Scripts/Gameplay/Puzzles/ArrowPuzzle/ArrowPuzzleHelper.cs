using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class ArrowPuzzleHelper
    {

        int size = 16;

        int sizeSqrt;
        /// <summary>
        /// 0: north
        /// 1: east
        /// 2: south
        /// 3: west
        /// </summary>
        int[] values;

        public ArrowPuzzleHelper(int size)
        {
            this.size = size;
            values = new int[size];
            sizeSqrt = (int)Mathf.Sqrt(size);
        }

    
        //public int[] __Shuffle()
        //{
        //    // Reset the array
        //    for (int i = 0; i < size; i++)
        //        values[i] = -1;

        //    // Create
        //    List<int> currentIds = new List<int>();
        //    List<int> tmpIds = new List<int>();
        //    for (int i = 0; i < size; i++)
        //        tmpIds.Add(i);
            
        //    while(tmpIds.Count > 0)
        //    {
        //        int nextId = tmpIds[Random.Range(0, tmpIds.Count)];
             
        //        Check(nextId, currentIds);
        //        tmpIds.RemoveAll(id=>values[id] >= 0);
        //    }

        //    return values;
        //}

        public int[] Shuffle()
        {
            // Reset the array
            for (int i = 0; i < size; i++)
                values[i] = -1;

            for(int i=0; i<values.Length; i++)
            {
                List<int> candidates = new List<int>(new int[] { 0, 1, 2, 3 });
                do
                {
                    int candidate = candidates[Random.Range(0, candidates.Count)];
                    candidates.Remove(candidate);
                    if (CheckCandidate(i, candidate))
                        values[i] = candidate;
                }
                while (values[i] < 0);
            }

            return values;
        }       

        bool CheckCandidate(int id, int value)
        {
            switch (value)
            {
                case 0: // North
                    int other = id - sizeSqrt;
                    while(other >= 0)
                    {
                        if (values[other] == 2)
                            return false;
                        other -= sizeSqrt;
                    }
                    break;
                case 1: // East
                    break;
                case 2: // South
                    break;
                case 3: // West
                    other = id - 1;
                    while (other >= 0 && other % sizeSqrt < sizeSqrt-1)
                    {
                        if (values[other] == 1)
                            return false;
                        other--;
                    }
                    break;
            }

            // Check for round path
            if(value == 0 || value == 3)
            {
                return !IsRoundPath(id, value);
            }

            return true;
        }


        /// <summary>
        /// Avoid round path, like:
        ///     S   W
        ///     E   N      
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsRoundPath(int id, int value)
        {
            if ((value == 0 && id / sizeSqrt == 0) || (value == 3 && id % sizeSqrt == 0) || value == 1 || value == 2)
                return false;

            if (value == 0)
                return CheckPathStartingFromNorth(id);
            else // Value = 3
                return CheckPathStartingFromWest(id);
        }

        

        bool CheckPathStartingFromNorth(int id)
        {
            // Get the first id moving rom north to east
            int startingId = id;
            bool found = false;
            bool stop = false;
            id -= sizeSqrt;
            while (id >= 0 && !found && !stop)
            {
                if (values[id] == 1)
                    stop = true;
                else if (values[id] == 3)
                    found = true;
                else
                    id -= sizeSqrt;
            }

            if (found)
            {
                // Check from east to south
                found = false;
                stop = false;
                id--;
                while (id >= 0 && id % sizeSqrt < sizeSqrt - 1 && !found && !stop)
                {
                    if (values[id] == 0)
                        stop = true;
                    else if (values[id] == 2)
                        found = true;
                    else
                        id -= sizeSqrt;
                }

                if (found)
                {
                    // Check south west
                    found = false;
                    stop = false;
                    id += sizeSqrt;
                    while (id < values.Length && !found && !stop)
                    {
                        if (values[id] == 3 || values[id] < 0)
                            stop = true;
                        else if (values[id] == 1)
                            found = true;
                        else
                            id += sizeSqrt;
                    }
                    if (found)
                    {
                        // Check from east to north
                        found = false;
                        stop = false;
                        id++;
                        while (id % sizeSqrt > 0 && !found && !stop)
                        {
                            if (values[id] == 2/* || values[id] < 0*/)
                                stop = true;
                            else if (id == startingId)
                                found = true;
                            else
                                id++;
                        }
                        if (found)
                            return true;
                    }
                }
            }

            return false;
        }

        bool CheckPathStartingFromWest(int id)
        {
            Debug.Log($"PUZZLE - id:{id}");

            // Get the first id moving rom west to north
            int startingId = id;
            bool found = false;
            bool stop = false;
            id--;
            while (id >= 0 && id % sizeSqrt < sizeSqrt - 1 && !found && !stop)
            {
                if (values[id] == 2)
                    stop = true;
                else if (values[id] == 0)
                    found = true;
                else
                    id--;
            }

            if (found)
            {
                Debug.Log($"PUZZLE - found tile to north:{id}");
                // Check from north to east
                found = false;
                stop = false;
                id -= sizeSqrt;
                while (id >= 0 && !found && !stop)
                {
                    if (values[id] == 3)
                        stop = true;
                    else if (values[id] == 1)
                        found = true;
                    else
                        id -= sizeSqrt;
                }

                if (found)
                {
                    Debug.Log($"PUZZLE - found tile to east:{id}");
                    // Check from west to south
                    found = false;
                    stop = false;
                    id++;
                    while (id % sizeSqrt > 0 && !found && !stop)
                    {
                        if (values[id] == 0 || values[id] < 0)
                            stop = true;
                        else if (values[id] == 2)
                            found = true;
                        else
                            id++;
                    }
                    if (found)
                    {
                        Debug.Log($"PUZZLE - found tile to south:{id}");
                        // Check from south to west
                        found = false;
                        stop = false;
                        id += sizeSqrt;
                        while (id < values.Length && !found && !stop)
                        {
                            if (values[id] == 1/* || values[id] < 0*/)
                                stop = true;
                            else if (id == startingId)
                                found = true;
                            else
                                id++;
                        }
                        if (found)
                        {
                            Debug.Log($"PUZZLE - found closed tile:{id}");
                            return true;
                        }
                            
                    }
                }
            }

            return false;
        }


        //bool CollideWithPreviousValues(int id, int value)
        //{
        //    //return false;
           
        //    for(int i=0; i<values.Length; i++)
        //    {
        //        if (values[i] < 0)
        //            continue;
        //        if (value == 0 && values[i] == 2 && i < id && i % sizeSqrt == id % sizeSqrt)
        //            return true;
        //        if (value == 2 && values[i] == 0 && i > id && i % sizeSqrt == id % sizeSqrt)
        //            return true;
        //        if (value == 1 && values[i] == 3 && i > id && i / sizeSqrt == id / sizeSqrt)
        //            return true;
        //        if (value == 3 && values[i] == 1 && i < id && i / sizeSqrt == id / sizeSqrt)
        //            return true;
        //    }

        //    return false;

        //}

        //void Check(int id, List<int> currentIds)
        //{
        //    Debug.Log($"Checking Id:{id}");

        //    List<int> choices = new List<int>(new int[] { 0, 1, 2, 3 });

       
        //    string s = "Current ids: ";
        //    for(int i=0; i<currentIds.Count; i++)
        //    {
        //        s += currentIds[i].ToString() + " ";
        //    }
           

        //    // Filter choices
        //    if (CollideWithPreviousValues(id, 0) || currentIds.Exists(i => id % sizeSqrt == i % sizeSqrt && i < id))
        //        choices.Remove(0);
        //    if (CollideWithPreviousValues(id, 1) || currentIds.Exists(i => i > id && i / sizeSqrt == id / sizeSqrt))
        //        choices.Remove(1);
        //    if (CollideWithPreviousValues(id, 2) || currentIds.Exists(i => id % sizeSqrt == i % sizeSqrt && i > id))
        //        choices.Remove(2);
        //    if (CollideWithPreviousValues(id, 3) || currentIds.Exists(i => i < id && i / sizeSqrt == id / sizeSqrt))
        //        choices.Remove(3);
        //    //if((!IsEastEdge(id) ))

        //    int candidateValue = -1;
        //    if (choices.Count == 0)
        //    {
        //        candidateValue = Random.Range(0, 4);
                
        //    }
        //    else
        //    {
        //        candidateValue = choices[Random.Range(0, choices.Count)];
               
        //        choices.Remove(candidateValue);
        //    }
                
            
        //    values[id] = candidateValue;
        //    currentIds.Add(id);

        //    bool isFree = (IsNorthEdge(id) && candidateValue == 0) || (IsEastEdge(id) && candidateValue == 1) || (IsSouthEdge(id) && candidateValue == 2) || (IsWestEdge(id) && candidateValue == 3);
        //    if (!isFree)
        //    {
        //        // Check the next id
        //        List<int> allowedIds = new List<int>();
        //        if (id - sizeSqrt >= 0 && values[id - sizeSqrt] < 0)
        //            allowedIds.Add(id - sizeSqrt);
        //        if (id / sizeSqrt == (id + 1) / sizeSqrt && values[id + 1] < 0)
        //            allowedIds.Add(id + 1);
        //        if (id + sizeSqrt < size && values[id + sizeSqrt] < 0)
        //            allowedIds.Add(id + sizeSqrt);
        //        if (id / sizeSqrt == (id - 1) / sizeSqrt && id - 1 >= 0 && values[id - 1] < 0)
        //            allowedIds.Add(id - 1);


        //        // Check next
        //        int candidateId = -1;
        //        if(allowedIds.Count > 0)
        //            candidateId = allowedIds[Random.Range(0, allowedIds.Count)];

    
        //        if (candidateId >= 0)
        //            Check(candidateId, currentIds);

        //    }


        //}

        //bool IsNorthEdge(int id)
        //{
        //    return id - sizeSqrt < 0;
        //}

        //bool IsSouthEdge(int id)
        //{
        //    return id + sizeSqrt >= size;
        //}

        //bool IsWestEdge(int id)
        //{
        //    return id % sizeSqrt == 0;
        //}

        //bool IsEastEdge(int id)
        //{

        //    return id % sizeSqrt == sizeSqrt - 1;
        //}

        void DebugPuzzle()
        {

            string s = "[ArrowPuzzle]";
            for (int i = 0; i < size; i = i + 4)
            {
                s += $"\n{values[i]}    {values[i + 1]}   {values[i + 2]}   {values[i + 3]}";
            }
            s += "[/ArrowPuzzle]";

            Debug.Log(s);
        }
    }
}

