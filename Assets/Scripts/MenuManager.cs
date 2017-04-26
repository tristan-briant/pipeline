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
        LVM.scrollViewHight = levelList.transform.localPosition.y;
        SceneManager.LoadScene("level");

    }

    private void Start()
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
        LVM.completedLevel = levelCompleted = PlayerPrefs.GetInt("Level Completed");

        generateMenu();

        levelList.transform.localPosition =new Vector3(levelList.transform.localPosition.x, LVM.scrollViewHight,0);

    }

    public GameObject introScreen;
    public void removeIntro()
    {
        introScreen.SetActive(false);

    }

    IEnumerator IntroScreen() {

        introScreen.SetActive(true);

        yield return new WaitForSeconds(5f);
        if(introScreen!=null)
            removeIntro();
    }

    public void generateMenu()
    {

        foreach (Transform child in levelList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }


        int heigh = (120 + 50) * (Mathf.CeilToInt(levelMax / 3)  ) + 50;
      
        levelList.GetComponent<RectTransform>().sizeDelta = new Vector2(0, heigh);
 
        for (int i =0; i< LVM.levelMax; i++)
        {
            //Debug.Log(" menu " + i);
            GameObject go;

            if (LVM.getPlaygroundName(i+1).Contains("jelly"))
                go = Instantiate(Resources.Load<GameObject>("ButtonJelly"));
            else
                go = Instantiate(Resources.Load<GameObject>("ButtonUnlock"));


            go.transform.SetParent(levelList.transform);
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
                if(i==LVM.currantLevel-1)
                    go.GetComponentInChildren<Text>().color=new Color(0xFF/255.0f,0xEF/255.0f,0x31/255.0f,1);

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
