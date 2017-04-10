using System.Collections;
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
        "Pg 0b",
        "playground1",
        "playground2",
        "Pg 3 DBM",
        "Pg jelly0",
        "Pg double DBM",
        "playground3",
        "PG bonus1",
        "PG bonus2",
        "Pg 4 pressostats",
        "Pg 1P2R",
        "Pg jelly1",
        "Pg double DBM2",
        "Pg Rampe tension",
        "PG DBM+PST",
        "PG Capa1",
        "PG Capa2",
        "Pg jelly2",
        "PG Capa3",
        "PG Capa4",
        "PG Diode Capa",
        "PG Diode1",
        "PG Diode2",
        "PG D demi pont",
        "PG Transistor0a",
        "PG Transistor0b",
        "PG Transistor1",
        "PG TransistorNot",
        "PG TransistorNot PNP"
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
            return playgroundName[level - 1];
        else
            return "";

    }



}
