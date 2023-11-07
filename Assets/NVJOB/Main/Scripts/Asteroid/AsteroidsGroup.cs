﻿// Infinity Square/Space. The prototype of the game is open source. V1.0
// https://github.com/nvjob/Infinity-Square-Space
// #NVJOB Nicholas Veselov
// https://nvjob.pro
// MIT license (see License_NVJOB.txt)



using UnityEngine;



public class AsteroidsGroup : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    public GameObject asteroid;
    public float asteroidSize = 45;
    public int asteroidGroupRadius = 4;
    


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    void Start ()
    {
        //--------------

        Class_AdditionalTools.RandomName(gameObject, "Asteroids Group");

        StartCoroutine(Class_Asteroid.GenAsteroidsGroup(transform, asteroid, asteroidGroupRadius, asteroidSize));

        //--------------
    }



    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}