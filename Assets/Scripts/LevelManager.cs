using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour {

    private static LevelManager instanceRef;

    public int currantLevel;
    public int levelMax;

    public bool FirstLaunch=true;


    List<string> playgroundName = new List<string>()
    {
        "playground1",
        "playground2",
        "Pg double DBM",
        "playground3",
        "Pg 4 pressostats"
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


        levelMax = playgroundName.Count;
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
