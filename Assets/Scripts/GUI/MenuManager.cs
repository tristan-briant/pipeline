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
        LVM.currentLevel = levelNumber;
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

        float volume= PlayerPrefs.GetFloat("Volume",0.5f);
        LVM.Volume = volume;
        AudioListener.volume = LVM.Volume;

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


        int heigh = (120 + 50) * (Mathf.CeilToInt( (levelMax+2) / 3)  ) + 50;
      
        levelList.GetComponent<RectTransform>().sizeDelta = new Vector2(0, heigh);
 
        for (int i =1; i<= LVM.levelMax; i++)
        {
            //Debug.Log(" menu " + i);
            GameObject go;

            if (LVM.getPlaygroundName(i).Contains("jelly"))
                go = Instantiate(Resources.Load<GameObject>("MenuButtons/ButtonJelly"));
            else
                go = Instantiate(Resources.Load<GameObject>("MenuButtons/ButtonUnlock"));

           

            go.transform.SetParent(levelList.transform);
            go.transform.localScale = new Vector3(1, 1, 1);
            go.transform.localPosition = new Vector3(0, 0, 0);

            //if (i > levelCompleted && !LVM.hacked)
            if (!LVM.LevelIsCompleted(i-1) && !LVM.LevelIsCompleted(i))
            {
                go.GetComponentInChildren<Image>().sprite = Locked;
                go.GetComponentInChildren<Text>().text = "";
                go.GetComponentInChildren<languageManager>().enabled = false;
                go.GetComponent<Animator>().enabled = false;
                go.transform.GetComponentInChildren<Button>().interactable = false;
            }
            else
            {
                if (!LVM.LevelIsCompleted(i))
                {
                    go.GetComponentInChildren<languageManager>().enabled = true;
                    go.GetComponent<Animator>().enabled = true;
                }
                else
                {
                    go.GetComponentInChildren<languageManager>().enabled = false;
                    go.GetComponent<Animator>().enabled = false;
                    go.GetComponentInChildren<Text>().text = i.ToString();

                    // Show the icon if it exists
                    Object obj = Resources.Load("MenuButtons/Icons/icon-" + LVM.getPlaygroundName(i), typeof(Sprite));
                    Debug.Log("MenuButtons/Icons/icon-" + LVM.getPlaygroundName(i));
                    if (obj != null)
                    {
                        GameObject icon = go.transform.Find("Icon").gameObject;
                        icon.SetActive(true);
                        icon.GetComponent<Image>().sprite=(Sprite)obj;
                    }
                }
                if(i==LVM.currentLevel)
                    go.GetComponentInChildren<Text>().color=new Color(0xFF/255.0f,0xEF/255.0f,0x31/255.0f,1);

                int level = i; // mandatory to pass the variable level in the delegate instead of i otherwise the last value of i is used
                go.GetComponent<Button>().onClick.AddListener(delegate { LoadLevel(level); });
            }

        }
    }

    public void LoadDesigner()
    {
        SceneManager.LoadScene("LevelDesigner");
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
