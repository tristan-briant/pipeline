using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameController : MonoBehaviour {

    public int N,M; // size of the playground

    float[][] deltai; 
    float[][] deltap;

    public BaseComponent[][] composants;
    public BaseComponent vide;
    public BaseFrontier[] borders;
    public int currantLevel;
    public Text levelText;

    public bool composantChanged;

    
    public GameObject Pg; //The canvas "playground" from which are determined N and M 
    public float alpha=0.001f;

    private void Awake()
    {
        ///////////// Array Initialization /////////////

        Scene scene = SceneManager.GetActiveScene();
        print("Loaded Level number = " + (scene.name.ToString().Substring(5))); // name of scene
        currantLevel = int.Parse(scene.name.ToString().Substring(5));

        levelText.text = "Level " + currantLevel;

        N = Pg.GetComponent<GridLayoutGroup>().constraintCount; // -2 for the frontier
        M = Pg.transform.childCount/N; //itou


        Engine.initialize_p_i(N, M); // create the array of currant and pressure
      

        composants = new BaseComponent[N][];

        for (int k = 0; k < N; k++) {
            composants[k] = new BaseComponent[M];
        }
        populateComposant();

        Transform go = Pg.transform.parent;
        RectTransform objectRectTransform = go.GetComponent<RectTransform>();
        float width = objectRectTransform.rect.width;
        float height = objectRectTransform.rect.height;

        float wc = Mathf.Min(width / (N-1), (height - 120) / (M - 1));
        Pg.transform.localScale=new Vector3( wc/100, wc / 100,1) ;

    }


    bool firstPopulate = true;

    public void populateComposant()
    {
        //Debug.Log("Populate!");

        for (int i = 1; i < N-1; i++)
            for (int j = 1; j < M-1; j++)
            {
                GameObject go = Pg.transform.GetChild((i) + (j)*(N)).gameObject; //the slot

        
                if (firstPopulate)
                {
                    float c = 1.0f - Random.Range(0.0f, 0.3f);
                    go.GetComponent<Image>().color = new Color(c, c, c);
                }

                if (go.transform.childCount > 0)
                {
                    if (go.transform.childCount > 1) Debug.Log("Populate error" + go.transform.childCount);
                    BaseComponent bc = (BaseComponent)go.transform.GetChild(0).GetComponent(typeof(BaseComponent));
                    bc.x = i;bc.y = j;
                    composants[i ][j] = bc;
                }
                else
                {
                    Debug.Log(" 1 created");
                    BaseComponent bc = Instantiate(vide);
                    bc.transform.SetParent(go.transform);
                    bc.x = i; bc.y = j;
                    composants[i][j] = bc;
                }
        }


        if (firstPopulate)
        {
            for (int i = 0; i < N ; i++)
            {
                int j = 0;
                GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
                BaseComponent bc = (BaseComponent)go.transform.GetChild(0).GetComponent(typeof(BaseComponent));
                composants[i][j] = bc;
            }
            for (int i = 0; i < N ; i++)
            {
                int j = M-1;
                GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
                BaseComponent bc = (BaseComponent)go.transform.GetChild(0).GetComponent(typeof(BaseComponent));
                composants[i][j] = bc;
            }

            for (int j = 1; j < M - 1; j++)
            {
                int i = 0;
                GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
                BaseComponent bc = (BaseComponent)go.transform.GetChild(0).GetComponent(typeof(BaseComponent));
                composants[i][j] = bc;
            }
            for (int j = 1; j < M - 1; j++)
            {
                int i = N-1;
                GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
                BaseComponent bc = (BaseComponent)go.transform.GetChild(0).GetComponent(typeof(BaseComponent));
                composants[i][j] = bc;
            }

        }
        firstPopulate = false;
    }

 
     

    public float success;
    public GameObject winText;


    // Use this for initialization
    void Start () {
        InvokeRepeating("evolution", 0.0f, 0.01f);
    }

    void evolution()
    {

        float success= Engine.oneStep(composants);

        if (success >= 1 && BaseComponent.itemBeingDragged == null)
        {
            //GameObject go = GameObject.Find("WinText");
            //GameObject go = GameObject.FindGameObjectWithTag("WinText").gameObject;//.GetComponent(typeof(GameObject))

            //StartCoroutine("LoadLevel","level1");
            StartCoroutine("WinAnimation");

        }
    }

    IEnumerator WinAnimation()
    {
        int levelCompleted = PlayerPrefs.GetInt("Level Completed");

        if (currantLevel > levelCompleted)
        {
            PlayerPrefs.SetInt("Level Completed", currantLevel);
            PlayerPrefs.Save();
        }
        winText.SetActive(true);                            //lance l'animation de victoire
        yield return new WaitForSeconds(2f);
        levelText.text = "Next level";
        levelText.GetComponent<Button>().enabled = true;
        levelText.GetComponent<Animator>().enabled = true;


    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene("level" + (currantLevel+1));
    }


    // Update is called once per frame
    void LateUpdate () {

        Engine.update_composant_p_i(composants);

        /*if (composantChanged)
        {
            populateComposant(); // reorganize the componants
            composantChanged = false;
        }*/

        if (Input.GetKey(KeyCode.Home))
        {

            //Home button pressed! write every thing you want to do

        }
        if (Input.GetKey(KeyCode.Escape))
        {
            //Application.Quit();
            SceneManager.LoadScene(0);
        }
        if (Input.GetKey(KeyCode.Menu))
        {
            Application.Quit();
        }
    }

    public void backToMenu() {
        Debug.Log("Back to menu");
        SceneManager.LoadScene(0);
    }

}

