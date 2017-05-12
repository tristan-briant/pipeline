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
    public Button nextButton;
    public Button prevButton;

    public bool composantChanged;

    public LevelManager LVM;
    public GameObject PgHolder; //The "playground Holder" 
    GameObject Pg; //The "playground" from which are determined N and M 

    //public float alpha=0.001f;

    private void Awake()
    {
        ///////////// Array Initialization /////////////
        LVM = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        currantLevel = LVM.currantLevel;

       

        if (currantLevel > 0) // 0 mean level design
        {
            if (currantLevel == 1) 
               prevButton.gameObject.SetActive(false);
            if (currantLevel == LVM.levelMax || !LVM.levelIsCompleted(currantLevel)) 
               nextButton.gameObject.SetActive(false);

            if (PgHolder.transform.childCount > 0)
            {
                DestroyImmediate(PgHolder.transform.GetChild(0).gameObject);
            }

            Pg = Instantiate(Resources.Load("Playgrounds/" + LVM.getPlaygroundName(currantLevel), typeof(GameObject))) as GameObject;

            Pg.transform.SetParent(PgHolder.transform);
            Pg.transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            Pg = PgHolder.transform.GetChild(0).gameObject;
        }
        if(LVM.language=="french")
            levelText.text = "Niveau " + currantLevel;
        else
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
        Debug.Log(" W :" + width + " H  " + height);
        Debug.Log(" N :" + N + " M  " + M);

        float widthRef = 600;
        float heightRef = 700;
        float ratio = height / width / heightRef * widthRef;
        float wc;
        if ( ratio > 1)
            wc = Mathf.Min(widthRef / (N - 1), ratio * heightRef / (M - 1));
        else
            wc = Mathf.Min(widthRef /ratio/ (N - 1), heightRef / (M - 1));

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
      

                if (go.transform.childCount > 0)
                {
                    if (go.transform.childCount > 1) Debug.Log("Populate error" + go.transform.childCount);
                    BaseComponent bc = (BaseComponent)go.transform.GetChild(0).GetComponent(typeof(BaseComponent));
                    Vector3 v = bc.transform.localPosition;
                    v.z = 0;
                    bc.transform.localPosition=v;
                    if(bc.mirror)
                        bc.transform.localScale = new Vector3(-1, 1, 1);
                    else
                        bc.transform.localScale = new Vector3(1, 1, 1);
                    bc.x = i;bc.y = j;
                    composants[i ][j] = bc;

                    if (firstPopulate)
                    {
                        if (bc.GetComponent<BaseComponent>().locked)
                        {
                            go.GetComponent<Image>().sprite = Resources.Load<Sprite>("concrete");
                            go.GetComponent<Image>().color = new Color(1, 1, 1);
                        }
                        else{
                            go.GetComponent<Image>().sprite = Resources.Load<Sprite>("fond-composant");
                            float c = 1.0f - Random.Range(0.0f, 0.3f);
                            go.GetComponent<Image>().color = new Color(c, c, c);
                        }
                        //bc.transform.GetComponent<Image>().enabled=true;
                    }
                }
                else
                {
                    BaseComponent bc = Instantiate(vide);
                    bc.transform.SetParent(go.transform);
                    bc.transform.localScale = new Vector3(1, 1, 1);
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




    bool gameOver=false;
    bool fail=false;
    public GameObject winText;
    public GameObject loseText;


    // Use this for initialization
    void Start () {
        InvokeRepeating("evolution", 0.0f, 0.01f);
    }

    void evolution()
    {

        float success=0; 

        for(int n=0;n<1;n++)
            success= Engine.oneStep(composants);

        if (success >= 1 && BaseComponent.itemBeingDragged == null && !gameOver)
        {
            gameOver = true;
            StartCoroutine("WinAnimation");
        }

        if (success < 0 && !gameOver) {
            gameOver = true;
            fail = true;
            CancelInvoke(); // Stop calculate the evolution
            StartCoroutine("LoseAnimation");
        }

    }

    IEnumerator WinAnimation()
    {
        //int levelCompleted = PlayerPrefs.GetInt("Level Completed");

        //if (currantLevel > levelCompleted)
       /* if (currantLevel > LVM.completedLevel)
        {
           PlayerPrefs.SetInt("Level Completed", currantLevel);

            PlayerPrefs.Save();
            
            //LVM.completedLevel = currantLevel;
        }*/
        LVM.levelCompleted(currantLevel);

        winText.SetActive(true);                            //lance l'animation de victoire
        yield return new WaitForSeconds(2f);
        if (currantLevel < LVM.levelMax)
        {
            if (LVM.language == "french")
                levelText.text = "Niveau suivant";
            else
                levelText.text = "Next level";
            levelText.GetComponent<Button>().enabled = true;
            levelText.GetComponent<Animator>().enabled = true;
        }


    }


    IEnumerator LoseAnimation()
    {
        loseText.SetActive(true);                            //lance l'animation de victoire
        yield return new WaitForSeconds(2f);
        if (LVM.language == "french")
            levelText.text = "Réessayer";
        else
            levelText.text = "Retry";
        levelText.GetComponent<Button>().enabled = true;
        levelText.GetComponent<Animator>().enabled = true;


    }

    public void LoadNextOrRetryLevel()
    {
        if (!fail)
            LVM.currantLevel = currantLevel + 1;
        SceneManager.LoadScene("level");
    }

    public void LoadNextLevel()
    {
        //if (currantLevel < LVM.levelMax)
        LVM.currantLevel = currantLevel + 1;
        SceneManager.LoadScene("level");
    }

    public void LoadPrevLevel()
    {
        //if (currantLevel > 1)
        LVM.currantLevel = currantLevel - 1;
        SceneManager.LoadScene("level");
    }


    // Update is called once per frame
    void LateUpdate () {

        Engine.update_composant_p_i(composants);


        if (Input.GetKey(KeyCode.Home))
        {

            //Home button pressed! write every thing you want to do

        }
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKey(KeyCode.Menu))
        {
            Application.Quit();
        }
    }

    public void backToMenu() {
        SceneManager.LoadScene(0);
    }

}

