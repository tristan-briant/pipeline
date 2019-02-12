using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public int N,M; // size of the playground

    float[][] deltai; 
    float[][] deltap;

    public BaseComponent[][] composants;
    //public BaseComponent vide;
    public BaseComponent videFrontier;
    public BaseFrontier[] borders;
    public Sprite concrete; // image for locked component
    public int currentLevel;
    public Text levelText;
    public Button nextButton;
    public Button prevButton;

    public bool composantChanged;
    protected bool paused = false;

    public LevelManager LVM;
    public GameObject PgHolder; //The "playground Holder" 
    GameObject Pg; //The "playground" from which are determined N and M 

    bool firstPopulate = true;

    public void Pause(bool pause)
    {
        if (pause)
        {
            CancelInvoke();
            paused = true;
        }
        else
        {
            //CancelInvoke(); // Not to have 2 evolution running in parallel
            if (paused) InvokeRepeating("Evolution", 0.0f, 0.01f);
            paused = false;
        }
    }

    public void InitializePlayground()
    {
        ///////////// Array Initialization /////////////
        LVM = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        currentLevel = LVM.currentLevel;



        if (currentLevel > 0) // 0 mean level design
        {
            if (currentLevel == 1)
                prevButton.gameObject.SetActive(false);
            if (currentLevel == LVM.levelMax || !LVM.LevelIsCompleted(currentLevel))
                nextButton.gameObject.SetActive(false);

            if (PgHolder.transform.childCount > 0)
            {
                DestroyImmediate(PgHolder.transform.GetChild(0).gameObject);
            }

            Pg = Instantiate(Resources.Load("Playgrounds/" + LVM.getPlaygroundName(currentLevel), typeof(GameObject))) as GameObject;

            Pg.transform.SetParent(PgHolder.transform);
            Pg.transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            Pg = PgHolder.transform.GetChild(0).gameObject;
        }
        if (LVM.language == "french")
            levelText.text = "Niveau " + currentLevel;
        else
            levelText.text = "Level " + currentLevel;

        N = Pg.GetComponent<GridLayoutGroup>().constraintCount; // -2 for the frontier
        M = Pg.transform.childCount / N; //itou

        Engine.initialize_p_i(N, M); // create the array of currant and pressure


        composants = new BaseComponent[N][];

        for (int k = 0; k < N; k++)
        {
            composants[k] = new BaseComponent[M];
        }

        firstPopulate = true;
        PopulateComposant();

        ResizePlayGround();
    }

    private void Start()
    {
        InitializePlayground();
        InvokeRepeating("Evolution", 0.0f, 0.01f);

    }

    [ContextMenu("Resize Playground")]
    public void ResizePlayGround()
    {
        Transform go = Pg.transform.parent;
        RectTransform objectRectTransform = go.GetComponent<RectTransform>();
        float width = objectRectTransform.rect.width;
        float height = objectRectTransform.rect.height;
        Debug.Log(" W :" + width + " H  " + height);
        Debug.Log(" N :" + N + " M  " + M);

        float wx = width / (100f * (N - 1));
        float wy = height / (100f * M);

        float wc = Mathf.Min(wx, wy);

        Pg.transform.localScale = new Vector3(wc, wc, 1);

        Pg.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        //Pg.transform.localPosition = Vector3.zero;
    }
    
    public void PopulateComposant()
    {
        //Debug.Log("Populate!");
        Sprite[] sprites = Resources.LoadAll<Sprite>("Field/GrassAtlas");

        for (int i = 1; i < N-1; i++)
            for (int j = 1; j < M-1; j++)
            {
                GameObject slot = Pg.transform.GetChild((i) + (j)*(N)).gameObject; //the slot
      

                if (slot.transform.childCount > 0)
                {
                    if (slot.transform.childCount > 1) Debug.Log("Populate error" + slot.transform.childCount);
                    BaseComponent bc = (BaseComponent)slot.transform.GetChild(0).GetComponent(typeof(BaseComponent));
                    Vector3 v = bc.transform.localPosition;
                    v.z = 0;
                    bc.transform.localPosition=v;
                    if(bc.mirror)
                        bc.transform.localScale = new Vector3(-1, 1, 1);
                    else
                        bc.transform.localScale = new Vector3(1, 1, 1);
                    composants[i][j] = bc;

                    if (firstPopulate)
                    {
                        slot.GetComponent<Image>().sprite = sprites[((i + j) % 2) * 2 + +(int)Random.Range(0, 1.999f)];
                    }
                }
                else
                {
                    if (firstPopulate)
                    {
                        slot.GetComponent<Image>().sprite = sprites[( (i+j) %2)*2 + (int)Random.Range(0,1.999f) ];
                        //float c = 1.0f - Random.Range(0.0f, 0.3f);
                        slot.GetComponent<Image>().color = Color.white;
                    }
                    GameObject bc = Instantiate(Resources.Load("Components/Empty", typeof(GameObject))) as GameObject;
                    bc.transform.SetParent(slot.transform);
                    bc.transform.localScale = new Vector3(1, 1, 1);
                    bc.transform.localPosition = Vector3.zero;
                    composants[i][j] = bc.GetComponent<BaseComponent>();
                }
        }


        //if (firstPopulate)
        {
            for (int i = 0; i < N ; i++)
            {
                int j = 0;
                GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
                BaseComponent bc;
                if (go.transform.childCount > 0)
                    bc = (BaseComponent)go.transform.GetChild(0).GetComponent(typeof(BaseComponent));
                else
                {
                    bc = Instantiate(videFrontier);
                    bc.transform.SetParent(go.transform);
                    bc.transform.localScale = Vector3.one;
                    bc.transform.localPosition = Vector3.zero;
                }
                bc.transform.localRotation = Quaternion.Euler(0, 0, 0);
                composants[i][j] = bc;
                (bc as BaseFrontier).GetValueFromSlot();
            }

            for (int i = 0; i < N ; i++)
            {
                int j = M-1;
                GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
                BaseComponent bc;
                if (go.transform.childCount > 0)
                    bc = (BaseComponent)go.transform.GetChild(0).GetComponent(typeof(BaseComponent));
                else
                {
                    bc = Instantiate(videFrontier);
                    bc.transform.SetParent(go.transform);
                    bc.transform.localScale = Vector3.one;
                    bc.transform.localPosition = Vector3.zero;
                }
                bc.transform.localRotation = Quaternion.Euler(0, 0, 0);
                composants[i][j] = bc;
                (bc as BaseFrontier).GetValueFromSlot();
            }

            for (int j = 1; j < M - 1; j++)
            {
                int i = 0;
                GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
                BaseComponent bc;
                if (go.transform.childCount > 0)
                    bc = (BaseComponent)go.transform.GetChild(0).GetComponent(typeof(BaseComponent));
                else
                {
                    bc = Instantiate(videFrontier);
                    bc.transform.SetParent(go.transform);
                    bc.transform.localScale = Vector3.one;
                    bc.transform.localPosition = Vector3.zero;
                }
                bc.transform.localRotation = Quaternion.Euler(0, 0, 0);
                composants[i][j] = bc;
                (bc as BaseFrontier).GetValueFromSlot();
            }

            for (int j = 1; j < M - 1; j++)
            {
                int i = N-1;
                GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
                BaseComponent bc;
                if (go.transform.childCount > 0)
                    bc = (BaseComponent)go.transform.GetChild(0).GetComponent(typeof(BaseComponent));
                else
                {
                    bc = Instantiate(videFrontier);
                    bc.transform.SetParent(go.transform);
                    bc.transform.localScale = Vector3.one;
                    bc.transform.localPosition = Vector3.zero;
                }
                bc.transform.localRotation = Quaternion.Euler(0, 0, -0);
                composants[i][j] = bc;
                (bc as BaseFrontier).GetValueFromSlot();
            }

        }
        firstPopulate = false;
    }
    

    bool gameOver=false;
    bool fail=false;
    public GameObject winText;
    public GameObject loseText;


    void Evolution()
    {

        float success=0; 

        for(int n=0;n<1;n++)
            success= Engine.OneStep1(composants);
            //success= Engine.oneStep2(composants);

        if (success >= 1 && BaseComponent.itemBeingDragged == null && !gameOver)
        {
            gameOver = true;
            StartCoroutine("WinAnimation");
        }

        if (success < 0 && !gameOver) {
            gameOver = true;
            fail = true;
            //CancelInvoke(); // Stop calculate the evolution
            StartCoroutine("LoseAnimation");
        }

    }

    IEnumerator WinAnimation()
    {
        LVM.LevelCompleted(currentLevel);

        winText.SetActive(true);                            //lance l'animation de victoire
        yield return new WaitForSeconds(2f);
        if (currentLevel < LVM.levelMax)
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
            LVM.currentLevel = currentLevel + 1;
        SceneManager.LoadScene("level");
    }

    public void LoadNextLevel()
    {
        LVM.currentLevel = currentLevel + 1;
        SceneManager.LoadScene("level");
    }

    public void LoadPrevLevel()
    {
        LVM.currentLevel = currentLevel - 1;
        SceneManager.LoadScene("level");
    }


    void LateUpdate () {

        Engine.update_composant_p_i(composants);


        if (Input.GetKey(KeyCode.Home))
        {
            
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

    public void BackToMenu() {
        SceneManager.LoadScene(0);
    }

    public void ResetComponant()
    {
        Engine.Reset_p_i(composants);
    }

}

