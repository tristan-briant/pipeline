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
    public LevelManager LVM;

  

    private void Start()
    {
       
         LVM = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        // Load options
        string lang=PlayerPrefs.GetString("Language");
        if (lang != null)
            LVM.language = lang;
        else
            LVM.language = "english";

        float volume= PlayerPrefs.GetFloat("Volume",0.5f);
        LVM.Volume = volume;
        AudioListener.volume = LVM.Volume;

        if (!LVM.FirstLaunch)
            GameObject.Find("MainCanvas").GetComponent<Animator>().Play("Level");
        
        levelMax = LVM.levelMax;
        LVM.completedLevel = levelCompleted = PlayerPrefs.GetInt("Level Completed");

        GenerateMenu();

        levelList.transform.localPosition =new Vector3(levelList.transform.localPosition.x, LVM.scrollViewHight,0);

        LVM.FirstLaunch = false;
    }

    public void GenerateMenu()
    {

        foreach (Transform child in levelList.transform) // Clean up levelList
            Destroy(child.gameObject);


        for (int i = 1; i <= LVM.levelMax; i++)
        {
            GameObject go;

            if (!LVM.LevelIsCompleted(i) && !LVM.LevelIsCompleted(i - 1) && !LVM.hacked)
            {
                go = Instantiate(Resources.Load<GameObject>("MenuButtons/ButtonLocked"), levelList.transform);
            }
            else
            {

                if (LVM.LevelIsCompleted(i))
                    go = Instantiate(Resources.Load<GameObject>("MenuButtons/ButtonCompleted"), levelList.transform);
                else
                    go = Instantiate(Resources.Load<GameObject>("MenuButtons/ButtonUnlocked"), levelList.transform);

                go.GetComponentInChildren<Text>().text = i.ToString();
                int level = i; // mandatory to pass the variable level in the delegate instead of i otherwise the last value of i is used
                go.GetComponent<Button>().onClick.AddListener(delegate { LoadLevel(level); });

            }


        }
    }


    public void LoadLevel(int levelNumber)
    {
        LVM.currentLevel = levelNumber;
        LVM.scrollViewHight = levelList.transform.localPosition.y;
        SceneManager.LoadScene("LevelScene");
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
