﻿// Infinity Square/Space. The prototype of the game is open source. V1.0
// https://github.com/nvjob/Infinity-Square-Space
// #NVJOB Nicholas Veselov
// https://nvjob.pro
// MIT license (see License_NVJOB.txt)



using UnityEngine;
using System.Collections.Generic;



public class DebrisesPool : MonoBehaviour {
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    public List<Debrises> debrisList = new List<Debrises>();

    //--------------

    static Transform stThisTransform;
    static int[] stNumberDebrises;
    static GameObject[][] stDebrises;
    

    
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        
    private void Awake()
    {
        //--------------

        stThisTransform = transform;
        AddObjectsToPool();

        //--------------
    }
    


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    


    private void AddObjectsToPool()
    {
        //--------------

        stNumberDebrises = new int[debrisList.Count];
        stDebrises = new GameObject[debrisList.Count][];

        //--------------

        for (int num = 0; num < debrisList.Count; num++)
        {
            stNumberDebrises[num] = debrisList[num].numberDebrises;
            stDebrises[num] = new GameObject[stNumberDebrises[num]];
            InstanInPool(debrisList[num].debris, stDebrises[num]);
        }

        //--------------
    }
    


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    static private void InstanInPool(GameObject obj, GameObject[] objs)
    {
        //--------------

        for (int i = 0; i < objs.Length; i++)
        {
            objs[i] = Instantiate(obj);
            objs[i].SetActive(false);
            objs[i].transform.parent = stThisTransform;
        }
        
        //--------------
    }



    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    static public GameObject GiveDebris(int num)
    {
        //--------------
        
        for (int i = 0; i < stNumberDebrises[num]; i++) if (!stDebrises[num][i].activeSelf) return stDebrises[num][i];
        return null;
        
        //--------------
    }



    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    static public void TakeDebris(GameObject obj)
    {
        //--------------

        obj.SetActive(false);
        if (obj.transform.parent != stThisTransform) obj.transform.parent = stThisTransform;

        //--------------
    }



    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}



[System.Serializable]

public class Debrises
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    public GameObject debris;
    public int numberDebrises = 100;



    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}
