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

    
        public int[] Shuffle()
        {
            // Reset the array
            for (int i = 0; i < size; i++)
                values[i] = -1;

            // Create
            List<int> currentIds = new List<int>();
            List<int> tmpIds = new List<int>();
            for (int i = 0; i < size; i++)
                tmpIds.Add(i);
            
            while(tmpIds.Count > 0)
            {
                int nextId = tmpIds[Random.Range(0, tmpIds.Count)];
                Debug.Log($"RND - {nextId}");
                Check(nextId, currentIds);
                tmpIds.RemoveAll(id=>values[id] >= 0);
            }

            //for (int i = 0; i < size; i++)
            //{
            //    if (values[i] < 0)
            //    {
            //        Check(i, currentIds);
            //        //Check(i);
            //    }

            //}

            return values;
        }

        //void Check(int id)
        //{
        //    List<int> choices = new List<int>();

        //    if (!CollideWithPreviousValues(id, 0))
        //        choices.Add(0);
        //    if (!CollideWithPreviousValues(id, 1))
        //        choices.Add(1);
        //    if (!CollideWithPreviousValues(id, 2))
        //        choices.Add(2);
        //    if (!CollideWithPreviousValues(id, 3))
        //        choices.Add(3);

        //    int choice = choices[Random.Range(0, choices.Count)];
        //    values[id] = choice;
        //}

        bool CollideWithPreviousValues(int id, int value)
        {
            //return false;
           
            for(int i=0; i<values.Length; i++)
            {
                if (values[i] < 0)
                    continue;
                if (value == 0 && values[i] == 2 && i < id && i % sizeSqrt == id % sizeSqrt)
                    return true;
                if (value == 2 && values[i] == 0 && i > id && i % sizeSqrt == id % sizeSqrt)
                    return true;
                if (value == 1 && values[i] == 3 && i > id && i / sizeSqrt == id / sizeSqrt)
                    return true;
                if (value == 3 && values[i] == 1 && i < id && i / sizeSqrt == id / sizeSqrt)
                    return true;
            }

            return false;

        }

        void Check(int id, List<int> currentIds)
        {

            List<int> choices = new List<int>(new int[] { 0, 1, 2, 3 });

            Debug.Log($"Checking id:{id}");
            string s = "Current ids: ";
            for(int i=0; i<currentIds.Count; i++)
            {
                s += currentIds[i].ToString() + " ";
            }
            Debug.Log($"CurrentIds:{s}");

            // Filter choices
            if (CollideWithPreviousValues(id, 0) || currentIds.Exists(i => id % sizeSqrt == i % sizeSqrt && i < id))
                choices.Remove(0);
            if (CollideWithPreviousValues(id, 1) || currentIds.Exists(i => i > id && i / sizeSqrt == id / sizeSqrt))
                choices.Remove(1);
            if (CollideWithPreviousValues(id, 2) || currentIds.Exists(i => id % sizeSqrt == i % sizeSqrt && i > id))
                choices.Remove(2);
            if (CollideWithPreviousValues(id, 3) || currentIds.Exists(i => i < id && i / sizeSqrt == id / sizeSqrt))
                choices.Remove(3);
            //if((!IsEastEdge(id) ))

            int candidateValue = -1;
            if (choices.Count == 0)
            {
                candidateValue = Random.Range(0, 4);
                Debug.Log($"RND A - {candidateValue}");
            }
            else
            {
                candidateValue = choices[Random.Range(0, choices.Count)];
                Debug.Log($"RND B - {candidateValue}");
                choices.Remove(candidateValue);
            }
                
            
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
                int candidateId = -1;
                if(allowedIds.Count > 0)
                    candidateId = allowedIds[Random.Range(0, allowedIds.Count)];

                Debug.Log($"RND C - {candidateId}");

                if (candidateId >= 0)
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

            return id % sizeSqrt == sizeSqrt - 1;
        }

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

