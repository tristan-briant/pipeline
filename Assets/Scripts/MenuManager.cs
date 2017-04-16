﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour{

    public int levelCompleted;
    public int levelMax;
    public GameObject levelList;
    public Sprite Locked;
    public LevelManager LVM;

    public void LoadLevel(int levelNumber)
    {
        LVM.currantLevel = levelNumber;
        SceneManager.LoadScene("level");

    }

    private void Awake()
    {
       
         LVM = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        // Load options
        string lang=PlayerPrefs.GetString("Language");
        if (lang != null)
            LVM.language = lang;
        else
            LVM.language = "english";

        if (LVM.FirstLaunch)
        {
            StartCoroutine("IntroScreen");
            LVM.FirstLaunch = false;
        }
        else {
            removeIntro();
        }
        levelMax = LVM.getLevelMax();
        generateMenu();

       /*float scale = Screen.width / 660f; // 660 = 100 + 120 + 50 + 120 + 50+ 120 +100
       levelList.transform.localScale = new Vector3( scale,scale, 1f);*/

    }

    public GameObject introScreen;
    public void removeIntro()
    {
        //introScreen.GetComponent<Canvas>().enabled=false;
        introScreen.SetActive(false);

    }

    IEnumerator IntroScreen() {
        //introScreen.GetComponent<Canvas>().enabled = true;
        introScreen.SetActive(true);

        yield return new WaitForSeconds(5f);
        if(introScreen!=null)
            removeIntro();
    }

    public void generateMenu()
    {
        //for (int i = 0; i < levelList.transform.childCount; i++)
        //    levelList.transform.GetChild(i).gameObject. ;

        foreach (Transform child in levelList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        levelCompleted = PlayerPrefs.GetInt("Level Completed");

        int heigh = (120 + 50) * (Mathf.CeilToInt(levelMax / 3)  ) + 50;

       
        levelList.GetComponent<RectTransform>().sizeDelta = new Vector2(0, heigh);
        

        //Debug.Log(" menu !");

        for (int i =0; i< LVM.getLevelMax(); i++)
        {
            //Debug.Log(" menu " + i);
            GameObject go;

            if (LVM.getPlaygroundName(i+1).Contains("jelly"))
                go = Instantiate(Resources.Load<GameObject>("ButtonJelly"));
            else
                go = Instantiate(Resources.Load<GameObject>("ButtonUnlock"));


            go.transform.SetParent(levelList.transform);//levelList.transform.GetChild(i).gameObject;
            go.transform.localScale = new Vector3(1, 1, 1);
            go.transform.localPosition = new Vector3(0, 0, 0);

            if (i > levelCompleted && !LVM.hacked)
            {
                go.GetComponentInChildren<Image>().sprite = Locked;
                go.GetComponentInChildren<Text>().text = "";
                go.transform.GetComponentInChildren<Button>().interactable = false;
            }
            else
            {
                go.GetComponentInChildren<Text>().text = (i+1).ToString();
                int level = i+1; // mandatory to pass the variable level in the delegate instead of i otherwise the last value of i is used
                go.GetComponent<Button>().onClick.AddListener(delegate { LoadLevel(level); });
            }

        }
    }


  

    void LateUpdate()
    {


        if (Input.GetKey(KeyCode.Home))
        {
            Application.Quit();
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKey(KeyCode.Menu))
        {
            Application.Quit();
        }
    }

}
