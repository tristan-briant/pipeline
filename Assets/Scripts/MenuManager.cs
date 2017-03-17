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

    public void LoadLevel(int levelNumber)
    {
        LVM.currantLevel = levelNumber;
        SceneManager.LoadScene("level");

    }

    private void Awake()
    {
       
         LVM = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

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

       float scale = Screen.width / 660f; // 660 = 100 + 120 + 50 + 120 + 50+ 120 +100
       levelList.transform.localScale = new Vector3( scale,scale, 1f);

    }

    public GameObject introScreen;
    public void removeIntro()
    {
        introScreen.GetComponent<Canvas>().enabled=false;

    }

    IEnumerator IntroScreen() {
        introScreen.GetComponent<Canvas>().enabled = true;
        yield return new WaitForSeconds(5f);
        if(introScreen!=null)
            removeIntro();
    }

    void generateMenu()
    {
        levelCompleted = PlayerPrefs.GetInt("Level Completed");
        //print(levelCompleted);

        int heigh = (120 + 50) * (Mathf.CeilToInt(levelMax / 3) + 1 ) + 50;

        levelList.GetComponent<RectTransform>().sizeDelta  = new Vector2(660,heigh);

        //levelCompleted = 5;
        Debug.Log(" menu !");

        for (int i =0; i< LVM.getLevelMax(); i++)
        {
            Debug.Log(" menu " + i);
            GameObject go = Instantiate(Resources.Load<GameObject>("ButtonUnlock"));
            go.transform.SetParent(levelList.transform);//levelList.transform.GetChild(i).gameObject;
            go.transform.localScale = new Vector3(1, 1, 1);

            if (i > levelCompleted && !LVM.hacked)
            {
                go.GetComponentInChildren<Image>().sprite = Locked;
                go.GetComponentInChildren<Text>().text = "";
                go.transform.GetComponentInChildren<Button>().interactable = false;
            }
            else
            {
                go.GetComponentInChildren<Text>().text = (i+1).ToString();
                int level = i+1; // mandotory to pass the variable level in the delegate instead ofi otherwise the last value of i is used
                go.GetComponent<Button>().onClick.AddListener(delegate { LoadLevel(level); });
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
