﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour {

    private static LevelManager instanceRef;

    public int currantLevel;
    public int levelMax;

    public bool FirstLaunch=true;
    public bool hacked = false;


    List<string> playgroundName = new List<string>()
    {
        "Pg 0",
        "playground1",
        "playground2",
        "Pg 3 DBM",
        "Pg double DBM",
        "playground3",
        "PG bonus1",
        "Pg 4 pressostats",
        "Pg 1P2R",
        "Pg double DBM2",
        "Pg Rampe tension",
        "PG DBM+PST",
        "PG Capa1"
    };

    private void Awake()
    {
        if (instanceRef == null)
        {
            DontDestroyOnLoad(gameObject);
            instanceRef = this;
        }
        else {
            DestroyImmediate(gameObject);
        }


        //levelMax = playgroundName.Count;
    }


    public int getLevelMax() {
        return playgroundName.Count;
    }

    public string getPlaygroundName(int level) { 
        // Return the complete name (with path in Ressources) of the prefab playground
        // level start at 1  

        if( 0 < level && level <= playgroundName.Count )
            return "Playgrounds/" + playgroundName[level - 1];
        else
            return "";

    }



}