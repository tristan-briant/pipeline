using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour{

    public int levelCompleted;
    public int levelMax;
    public GameObject levelList;
    public Sprite Locked; 

    public void LoadLevel(int levelNumber)
    {
        SceneManager.LoadScene("level" + levelNumber);

    }

    private void Awake()
    {
        generateMenu();
        float scale = Screen.width / 660f; // 660 = 100 + 120 + 50 + 120 + 50+ 120 +100
        levelList.transform.localScale = new Vector3( scale,scale, 1f);

    }

    void generateMenu()
    {
        levelCompleted = PlayerPrefs.GetInt("Level Completed");
        print(levelCompleted);

        //levelCompleted = 5;

        for (int i =0; i< levelList.transform.childCount; i++)
        {
            GameObject go=levelList.transform.GetChild(i).gameObject;


            if (i > levelCompleted)
            {
                go.GetComponentInChildren<Image>().sprite = Locked;
                go.GetComponentInChildren<Text>().text = "";
                go.transform.GetComponentInChildren<Button>().interactable = false;
            }
            else
            {
                go.GetComponentInChildren<Text>().text = (i+1).ToString();
            }

        }
    }


    void LateUpdate()
    {


        if (Input.GetKey(KeyCode.Home))
        {
            Application.Quit();
            //Home button pressed! write every thing you want to do

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
