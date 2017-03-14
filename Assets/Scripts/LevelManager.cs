using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelManager : MonoBehaviour {

    private static LevelManager instanceRef;

    public int currantLevel;
    public int levelMax;

    public bool FirstLaunch=true;


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

    }

}
