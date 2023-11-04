﻿// Infinity Square/Space. The prototype of the game is open source. V1.0
// https://github.com/nvjob/Infinity-Square-Space
// #NVJOB Nicholas Veselov
// https://nvjob.pro
// MIT license (see License_NVJOB.txt)



using UnityEngine;



public static class Class_StarSystem
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    public static int seed;
    public static bool generationOn, systemSelected;



    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    public static float SeedAbsCos()
    {
        //--------------

        return Mathf.Abs(Mathf.Cos(seed));

        //--------------
    }



    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}