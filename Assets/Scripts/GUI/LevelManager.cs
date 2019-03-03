using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


public class LevelManager : MonoBehaviour {

    private static LevelManager instanceRef;

    public int currentLevel;
    public int completedLevel;
    public int levelMax;
    public float scrollViewHight = 0;
    public bool FirstLaunch=true;
    public bool hacked = false;
    public bool designerMode = false;
    public bool designerScene = false;
    public string levelPath;

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

    List<string> playgroundName;

    /*List<string> playgroundName = new List<string>()
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

   */

    private void Awake()
    {
        if (instanceRef == null)
        {
            DontDestroyOnLoad(gameObject);
            instanceRef = this;
            LoadPlaygroundList();
            //levelMax = playgroundName.Count;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    public int LevelMax()
    {
        ListLevel list = GetComponent<ListLevel>();
        return list.names.Count;
    }

    public void LoadPlaygroundList()
    {
        string names = Resources.Load<TextAsset>("Levels/PlaygroundList").ToString();
        names = names.Replace("\r", ""); //clean up string 
        playgroundName = new List<string>(names.Split('\n'));
    }

    public bool LevelIsCompleted(int i) {
        if (i < 1) return true; // level 0 alway completed
        string s = PlayerPrefs.GetString("Level-" + playgroundName[i-1]);
        if (s == "completed") return true;
        return false;
    }

    public void ResetGame() {
        for (int i = 1; i <= levelMax; i++) {
            PlayerPrefs.SetString("Level-" + playgroundName[i-1],"");
        }
    }

    public void LevelCompleted(int i)
    {
        if (i < 1) return;
        PlayerPrefs.SetString("Level-" + playgroundName[i-1], "completed");
        PlayerPrefs.Save();
    }
    

    public string GetPlaygroundName(int level) {
        // Return the complete name (with path in Ressources) of the prefab playground
        // level start at 1  

        ListLevel list = GetComponent<ListLevel>();

        if( 0 < level && level <= list.names.Count )
            return list.names[level - 1];
        else
            return "";

    }



}
