using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


public class LevelManager : MonoBehaviour {

    private static LevelManager instance;

    public int currentLevel;
    public int completedLevel;
    public int levelMax;
    public float scrollViewHight = 0;
    public bool FirstLaunch=true;
    public bool WithStopper=true;
    public bool hacked = false;
    static public bool designerMode = false;
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

  
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
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

    /*public void LoadPlaygroundList()
    {
        string names = Resources.Load<TextAsset>("Levels/PlaygroundList").ToString();
        names = names.Replace("\r", ""); //clean up string 
        playgroundName = new List<string>(names.Split('\n'));
    }*/

    public bool LevelIsCompleted(int i) {
        if (i < 1) return true; // level 0 alway completed
        string s = PlayerPrefs.GetString("Level-" + GetPlaygroundName(i));
        if (s == "completed") return true;
        return false;
    }

    public void ResetGame() {
        for (int i = 1; i <= levelMax; i++) {
            PlayerPrefs.SetString("Level-" + GetPlaygroundName(i), "");
        }
    }

    public void LevelCompleted(int i)
    {
        if (i < 1) return;
        PlayerPrefs.SetString("Level-" + GetPlaygroundName(i), "completed");
        PlayerPrefs.Save();
    }
    

    public string GetPlaygroundName(int level) {
        // Return the complete name (with path in Ressources) of the prefab playground
        // level start at 1  

        ListLevel list = GetComponent<ListLevel>();

        if( 1 <= level && level <= list.names.Count )
            return list.names[level - 1];
        else
            return "";

    }



}
