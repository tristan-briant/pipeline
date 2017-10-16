using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour {

    private static LevelManager instanceRef;

    public int currantLevel;
    public int completedLevel;
    public int levelMax;
    public float scrollViewHight = 0;
    public bool FirstLaunch=true;
    public bool hacked = false;

    public float Volume = 0.5f;
    public string language = "english";
    public string Language
    {
        get
        {
            return language;
        }
        set
        {
            language = value;
        }
    }

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
        "Pg 4 pressostats",  //10
		"Pg 1P2R",
		"PG res parallele 3",
        "Pg jelly1",
        "Pg double DBM2",
        "Pg Rampe tension",
        "PG DBM+PST",
       "PG Capa1",
        "PG Capa2",  
        "Pg jelly2",  
        "PG Capa3", //20
        "PG Capa4",
        "PG LC1",
        "PG bonus2",          
        "PG LC2",
        "PG LC3",
        "PG LC4",
        "PG Diode1",
        "PG Diode Capa",
        "PG Diode2",
        "PG D demi pont", //30
        "PG Transistor0a",
        "PG Transistor0b",
        "PG Transistor1",
        "PG TransistorNot",
        "PG TransistorNot PNP"
    };

    List<string> playgroundNameLegacy = new List<string>()
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

    public bool LevelIsCompleted(int i) {
        if (i < 1) return true; // level 0 alway completed
        if (hacked == true) return true; //for debug purpose
        string s = PlayerPrefs.GetString("Level-" + playgroundName[i-1]);
        if (s == "completed") return true;
        return false;
    }

    public void ResetGame() {
        for (int i = 0; i < getLevelMax(); i++) {
            PlayerPrefs.SetString("Level-" + playgroundName[i],"");
        }
    }

    public void LevelCompleted(int i)
    {
        if (i < 1) return;
        PlayerPrefs.SetString("Level-" + playgroundName[i-1], "completed");
        PlayerPrefs.Save();
    }


    private void Awake()
    {
        if (instanceRef == null)
        {
            DontDestroyOnLoad(gameObject);
            instanceRef = this;
            levelMax = getLevelMax();
            // for legacy purpose
            
            int lvc=PlayerPrefs.GetInt("Level Completed");


            for(int i = 0; i < lvc; i++)
            {
                PlayerPrefs.SetString("Level-" + playgroundNameLegacy[i], "completed");
            }
            //PlayerPrefs.DeleteKey("Level Completed");

            //AudioListener.volume = Volume;

        }
        else {
            DestroyImmediate(gameObject);
        }
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
