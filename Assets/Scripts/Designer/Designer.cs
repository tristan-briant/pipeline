using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class Designer : MonoBehaviour
{

    const int MaxSize = 8;
    const int MinSize = 3;
    public int N, M;

    public GameObject Pg;

    static public string PGdata;

    private void Awake()
    {
        Pg = GameObject.Find("Playground");
    }

    private void Start()
    {
        N = Pg.GetComponent<PlaygroundParameters>().N;
        M = Pg.GetComponent<PlaygroundParameters>().M;
    }


    public void MakeThumb(string filename)
    {
        GameObject canvas = GameObject.Find("CanvasThumbnail");
        Pg.transform.SetParent(canvas.transform);
        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        gc.ResizePlayGround();

        Texture2D texture = new Texture2D(256, 256, TextureFormat.RGB24, false);
        Camera cam = GameObject.Find("CameraThumbnail").GetComponent<Camera>();
        cam.Render();
        RenderTexture.active = cam.targetTexture;
        texture.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);

        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();

        // save the image

        File.WriteAllBytes(filename, bytes);

        canvas = GameObject.Find("PlaygroundHolder");
        Pg.transform.SetParent(canvas.transform);
        gc.ResizePlayGround();

    }

    IEnumerator RenderThumbnail()
    {

        Texture2D texture = new Texture2D(256, 256, TextureFormat.RGB24, false);

        Camera cam = GameObject.Find("CameraThumbnail").GetComponent<Camera>();
        // Initialize and render

        cam.Render();
        RenderTexture.active = cam.targetTexture;

        // put buffer into texture
        texture.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);

        yield return 0;

        texture.Apply();

        yield return 0;

        byte[] bytes = texture.EncodeToPNG();

        // save the image
        string imagePath = "amazingPath.png";
        File.WriteAllBytes(Application.persistentDataPath + Path.DirectorySeparatorChar + imagePath, bytes);

        Debug.Log("done");

        //Tell unity to delete the texture, by default it seems to keep hold of it and memory crashes will occur after too many screenshots.
        //DestroyObject(texture);


    }

    //[ContextMenu("SaveToFile")]
    public void SaveToFile()
    {
        string filename = "PlayGround" + System.DateTime.Now.ToShortDateString().Replace("/", "-") + "-"
            + System.DateTime.Now.ToLongTimeString().Replace(":", "-");

        string file = Path.Combine(Application.persistentDataPath,filename + ".txt");

        SaveToString();
       
        File.WriteAllText(file, PGdata);

        MakeThumb(Path.Combine(Application.persistentDataPath,  filename + ".png"));
    }


    public void SaveToResources(string path)
    {
        path.Replace(".txt", "");

        SaveToString();
        Debug.Log("Saving " + path);
        StreamWriter sr = File.CreateText("Assets/Resources/Levels/" + path + ".txt");
        sr.Write(PGdata);
        sr.Close();
        sr.Dispose();

        MakeThumb("Assets/Resources/Levels/" + path + ".png");
    }

    public void SaveToString(bool reset=false)
    {
        PGdata = "";
        PlaygroundParameters parameters = FindObjectOfType<PlaygroundParameters>();
        PGdata = JsonUtility.ToJson(parameters) + "\n";

        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        Transform deck = gc.GetComponent<GameController>().DeckHolder.transform.GetChild(0);
        foreach (Transform slot in deck.transform)
        {
            BaseComponent component = slot.GetComponentInChildren<BaseComponent>();
            if (component != null && component.name != "" && !reset)
            {
                string path = component.PrefabPath;
                PGdata += path + "\n";
                PGdata += JsonUtility.ToJson(component) + "\n";
            }
            else
            {
                PGdata += "empty\n";
            }
        }

        foreach (Transform slot in Pg.transform)
        {
            BaseComponent component = slot.GetComponentInChildren<BaseComponent>();
            if (component != null && component.name != "")
            {
                string path = component.PrefabPath;
                PGdata += path + "\n";
                PGdata += JsonUtility.ToJson(component) + "\n";
            }
            else
            {
                PGdata += "empty\n";
                PGdata += "\n";

            }

        }

        Debug.Log(PGdata);
    }

    void ClearPlayground()
    {
        Pg = GameObject.Find("Playground");
        int count = Pg.transform.childCount;
        for (int i = 0; i < count; i++)        // On retire tout
        {
            Transform child = Pg.transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

        GameObject edge = GameObject.Find("LeftEdge");
        count = edge.transform.childCount;
        for (int i = 0; i < count; i++)        // On retire tout
        {
            Transform child = edge.transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

        edge = GameObject.Find("RightEdge");
        count = edge.transform.childCount;
        for (int i = 0; i < count; i++)        // On retire tout
        {
            Transform child = edge.transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

    }

    [ContextMenu("LoadFromString")]
    public void LoadFromString()
    {
        if (PGdata == null) return;

        PGdata = PGdata.Replace("\r", ""); //clean up string

        Pg = GameObject.Find("Playground");
        int count = Pg.transform.childCount;
        for (int i = 0; i < count; i++)        // On retire tout
        {
            Transform child = Pg.transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

        GameObject edge = GameObject.Find("LeftEdge");
        count = edge.transform.childCount;
        for (int i = 0; i < count; i++)        // On retire tout
        {
            Transform child = edge.transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

        edge = GameObject.Find("RightEdge");
        count = edge.transform.childCount;
        for (int i = 0; i < count; i++)        // On retire tout
        {
            Transform child = edge.transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }


        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        Transform deck = gc.GetComponent<GameController>().DeckHolder.transform.GetChild(0);

        foreach (Transform slotDeck in deck.transform)
        {
            if (slotDeck.transform.childCount > 0)
            {
                Transform child = slotDeck.transform.GetChild(0);
                DestroyImmediate(child.gameObject);
            }
        }


        string[] tokens = PGdata.Split('\n');

        int k = 0;
        JsonUtility.FromJsonOverwrite(tokens[k++], Pg.GetComponent<PlaygroundParameters>()); //k=0 puis 1  

        foreach (Transform slotDeck in deck.transform)
        {
            string prefab = tokens[k++];
            
            if (prefab != "empty")
            {
                GameObject component = Instantiate(Resources.Load(prefab)) as GameObject; //Instantiate(Resources.Load(prefab, typeof(GameObject))) as GameObject;
                //component.transform.SetParent(slotDeck.transform);
                component.GetComponent<BaseComponent>().ChangeParent(slotDeck.transform);
 
                JsonUtility.FromJsonOverwrite(tokens[k++], slotDeck.transform.GetComponentInChildren<BaseComponent>());
                slotDeck.GetComponent<CreateComponent>().Start();

            }
        }


        N = Pg.GetComponent<PlaygroundParameters>().N;
        M = Pg.GetComponent<PlaygroundParameters>().M;

        Pg.GetComponent<GridLayoutGroup>().constraintCount = N;

        GameObject slot = CreateSlot(Pg, "Field/SlotCorner", tokens[k++], 1, tokens[k++]);
        CreateSlotEdge(slot.GetComponentInChildren<BaseFrontier>().type, true);

        for (int i = 1; i < N - 1; i++)
        {
            CreateSlot(Pg, "Field/SlotWall", tokens[k++], 0, tokens[k++]);
        }

        slot = CreateSlot(Pg, "Field/SlotCorner", tokens[k++], 0, tokens[k++]);
        CreateSlotEdge(slot.GetComponentInChildren<BaseFrontier>().type, false);

        for (int j = 1; j < M - 1; j++)
        {
            slot = CreateSlot(Pg, "Field/SlotWall", tokens[k++], 1, tokens[k++]);
            CreateSlotEdge(slot.GetComponentInChildren<BaseFrontier>().type, true);

            for (int i = 1; i < N - 1; i++) CreateSlot(Pg, "Field/SlotComponent", tokens[k++], 0, tokens[k++]); //empty component

            slot = CreateSlot(Pg, "Field/SlotWall", tokens[k++], 3, tokens[k++]);
            CreateSlotEdge(slot.GetComponentInChildren<BaseFrontier>().type, false);

        }

        slot = CreateSlot(Pg, "Field/SlotCorner", tokens[k++], 2, tokens[k++]);
        CreateSlotEdge(slot.GetComponentInChildren<BaseFrontier>().type, true);

        for (int i = 1; i < N - 1; i++) CreateSlot(Pg, "Field/SlotWall", tokens[k++], 2, tokens[k++]);

        slot = CreateSlot(Pg, "Field/SlotCorner", tokens[k++], 3, tokens[k++]);
        CreateSlotEdge(slot.GetComponentInChildren<BaseFrontier>().type, false);


        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().InitializePlayground();

    }

    public void LoadFromFile(string filename)
    {
        PGdata = File.ReadAllText(filename);
        LoadFromString();
    }

    public void LoadFromRessources(string filename)
    {
        Debug.Log(filename);
        PGdata = Resources.Load<TextAsset>("Levels/" + filename.Replace(".txt", "")).ToString();
        LoadFromString();
    }

    [ContextMenu("LoadFromLastFile")]
    public void LoadFromLastFile()
    {
        string filename;
        string[] fileNames = Directory.GetFiles(Application.persistentDataPath, "*.txt");
        if (fileNames.Length > 0)
        {
            filename = fileNames[fileNames.Length - 1];
            PGdata = File.ReadAllText(filename);
            LoadFromString();
        }
    }
    

    GameObject CreateSlotFrontier(GameObject PlayGround, string PrefabSlotPath, string PrefabComponentPath, int dir, int type)
    {
        GameObject slot = Instantiate(Resources.Load(PrefabSlotPath, typeof(GameObject))) as GameObject;
        slot.transform.SetParent(PlayGround.transform);
        slot.transform.localPosition = Vector3.zero;
        slot.transform.localScale = Vector3.one;
        if (dir == 3)
        {
            slot.transform.localScale = new Vector3(1, -1, 1);
            slot.transform.localRotation = Quaternion.Euler(0, 0, -90f);
        }
        else if (dir == 5)
        {
            slot.transform.localScale = new Vector3(-1, 1, 1);
            slot.transform.localRotation = Quaternion.Euler(0, 0, 180f);
        }
        else
            slot.transform.localRotation = Quaternion.Euler(0, 0, 90f * dir); // C'est le slot qui tourne

        GameObject component = Instantiate(Resources.Load(PrefabComponentPath, typeof(GameObject))) as GameObject;
        component.transform.SetParent(slot.transform);
        component.transform.localPosition = Vector3.zero;
        component.transform.localScale = Vector3.one;
        component.GetComponent<BaseComponent>().dir = dir;
        component.GetComponent<BaseFrontier>().type = type;

        component.GetComponent<BaseFrontier>().InitializeSlot();

        return slot;
    }

    GameObject CreateSlotComponent(GameObject PlayGround, string PrefabSlotPath, string PrefabComponentPath, int dir)
    {
        GameObject slot = Instantiate(Resources.Load(PrefabSlotPath, typeof(GameObject))) as GameObject;
        slot.transform.SetParent(PlayGround.transform);
        slot.transform.localPosition = Vector3.zero;
        slot.transform.localScale = Vector3.one;
        slot.transform.localRotation = Quaternion.identity;

        if (PrefabComponentPath != "")
        {
            GameObject component = Instantiate(Resources.Load(PrefabComponentPath, typeof(GameObject))) as GameObject;
            component.transform.SetParent(slot.transform);
            component.transform.localPosition = Vector3.zero;
            component.transform.localScale = Vector3.one;
            component.GetComponent<BaseComponent>().dir = dir;
        }

        return slot;
    }
    

    GameObject CreateSlot(GameObject PlayGround, string PrefabSlotPath, string PrefabComponentPath, int dir, string data)
    {
        GameObject slot = Instantiate(Resources.Load(PrefabSlotPath, typeof(GameObject))) as GameObject;
        slot.transform.SetParent(PlayGround.transform);
        slot.transform.localPosition = Vector3.zero;
        slot.transform.localScale = Vector3.one;
        slot.transform.localRotation = Quaternion.identity;

        if (PrefabComponentPath != "empty")
        {
            GameObject component = Instantiate(Resources.Load(PrefabComponentPath, typeof(GameObject))) as GameObject;
            //component.transform.SetParent(slot.transform);
            component.GetComponent<BaseComponent>().ChangeParent(slot.transform);
            component.transform.localPosition = Vector3.zero;
            component.transform.localScale = Vector3.one;


            JsonUtility.FromJsonOverwrite(data, slot.transform.GetComponentInChildren<BaseComponent>());
            slot.transform.GetComponentInChildren<BaseComponent>().Awake();

            if (slot.GetComponentInChildren<BaseFrontier>() != null)
            { //if corner or frontier
                component.GetComponent<BaseFrontier>().InitializeSlot();
                component.GetComponent<BaseComponent>().dir = dir;  //override frontier direction in case it is in the wrong way
            }

            bool designerMode = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().designerMode;

            component.GetComponent<BaseComponent>().destroyable = designerMode;
        }
        return slot;

    }

    void CreateSlotEdge(int type,bool onTheLeft)
    {
        Transform edge;
        GameObject slot = Instantiate(Resources.Load("Field/SlotEdge", typeof(GameObject))) as GameObject;
        if (onTheLeft)
            edge = GameObject.Find("LeftEdge").transform;
        else
            edge = GameObject.Find("RightEdge").transform;

        slot.transform.SetParent(edge);
        slot.transform.localPosition = Vector3.zero;
        slot.transform.localScale = Vector3.one;

        ChangeSlotEdgeImage(slot, type);
    }

    public void ChangeSlotEdgeImage(GameObject slot, int type)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Field/AtlasEdge");

        Image im = slot.GetComponentInChildren<Image>();

        switch (type)
        {
            case 0: 
            case 1: im.color=new Color(0,0,0,0); break;
            case 2: im.color = Color.white; im.sprite = sprites[(int)Random.Range(0, 1.999f)]; break;
            case 3: 
            case 4: 
            case 5: im.color = Color.white; im.sprite = sprites[2]; break;
        }
    }

    void DrawEdges()
    {
        foreach (Transform child in GameObject.Find("RightEdge").transform)
            Destroy(child.gameObject);

        foreach (Transform child in GameObject.Find("LeftEdge").transform)
            Destroy(child.gameObject);

        // corner is special
        if (Pg.transform.GetChild(0).GetComponentInChildren<BaseFrontier>().type == 1)
            CreateSlotEdge(2, true);
        else
            CreateSlotEdge(0, true);

        if (Pg.transform.GetChild(N-1).GetComponentInChildren<BaseFrontier>().type == 1)
            CreateSlotEdge(2, false);
        else
            CreateSlotEdge(0, false);


        for (int j = 1; j < M; j++) 
        {
            int type = Pg.transform.GetChild(j * N).GetComponentInChildren<BaseFrontier>().type;
            CreateSlotEdge(type, true);

            type = Pg.transform.GetChild(j * N + N - 1).GetComponentInChildren<BaseFrontier>().type;
            CreateSlotEdge(type, false);
        }

    }

    public void ResetField()
    {
        Pg = GameObject.Find("Playground");
        PlaygroundParameters param = Pg.GetComponent<PlaygroundParameters>();
        param.N = N;
        param.M = M;

        // On retire tout
        ClearPlayground();

        Pg.GetComponent<GridLayoutGroup>().constraintCount = N;

        CreateSlotFrontier(Pg, "Field/SlotCorner", "Frontiers/Corner", 6, 0);
        CreateSlotEdge(0,true);

        for (int i = 1; i < N - 1; i++) CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 0, 1);
        CreateSlotFrontier(Pg, "Field/SlotCorner", "Frontiers/Corner", 0, 0);
        CreateSlotEdge(0, false);

        for (int j = 1; j < M - 1; j++)
        {
            int type = Mathf.Min(j+1, 4);
            CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 1, type);
            CreateSlotEdge(type, true);
            for (int i = 1; i < N - 1; i++) CreateSlotComponent(Pg, "Field/SlotComponent", "", 0); //empty component
            CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 3, type);
            CreateSlotEdge(type, false);

        }

        CreateSlotFrontier(Pg, "Field/SlotCorner", "Frontiers/Corner", 5, 5);
        CreateSlotEdge(5, true);

        for (int i = 1; i < N - 1; i++) CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 2, 4);

        CreateSlotFrontier(Pg, "Field/SlotCorner", "Frontiers/Corner", 2, 5);
        CreateSlotEdge(5, false);

        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().InitializePlayground();

    }

    public void ReduceWidth()
    {
        if (N > MinSize)
        {
            int i = N - 2;
            for (int j = M - 1; j >= 0; j--)
            {
                DestroyImmediate(Pg.transform.GetChild(i + j * N).gameObject);
            }
            Pg.GetComponent<GridLayoutGroup>().constraintCount = N - 1;
            Pg.GetComponent<PlaygroundParameters>().N = N - 1;
            N = N - 1;

            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().InitializePlayground();

            DrawEdges();
        }
        else
        {
            Debug.Log("Impossible de réduire");
        }

    }

    public void IncreaseWidth()
    {
        if (N < MaxSize)
        {
            int i = N - 1;

            CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 0, 1);
            Pg.transform.GetChild(Pg.transform.childCount - 1).SetSiblingIndex(i); // remet le nouveau child au bon endroit

            for (int j = 1; j < M - 1; j++)
            {
                CreateSlotComponent(Pg, "Field/SlotComponent", "", 0);
                Pg.transform.GetChild(Pg.transform.childCount - 1).SetSiblingIndex(i + j * (N + 1));
            }

            CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 2, 4);
            Pg.transform.GetChild(Pg.transform.childCount - 1).SetSiblingIndex(i + (M - 1) * (N + 1));

            Pg.GetComponent<GridLayoutGroup>().constraintCount = N + 1;
            Pg.GetComponent<PlaygroundParameters>().N = N + 1;
            N = N + 1;

            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().InitializePlayground();

            DrawEdges();
        }
        else
        {
            Debug.Log("Impossible d'agrandir");
        }
    }

    public void ReduceHeight()
    {
        if (M > MinSize)
        {
            int j = M - 2;
            for (int i = N - 1; i >= 0; i--)
            {
                DestroyImmediate(Pg.transform.GetChild(i + j * N).gameObject);
            }
            GameObject edge = GameObject.Find("LeftEdge");
            DestroyImmediate(edge.transform.GetChild(M - 2).gameObject);
            edge = GameObject.Find("RightEdge");
            DestroyImmediate(edge.transform.GetChild(M - 2).gameObject);

            Pg.GetComponent<PlaygroundParameters>().M = M - 1;
            M = M - 1;

 
            int type = Pg.transform.GetChild((j - 1) * N).GetComponentInChildren<BaseFrontier>().type;

            if (type <= 2) //Beach
            {
                Pg.transform.GetChild((j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 2;
               // Pg.transform.GetChild(j * N).GetComponentInChildren<BaseFrontier>().type = 4;
            }
            //else
                Pg.transform.GetChild(j * N).GetComponentInChildren<BaseFrontier>().type = 5;

            type = Pg.transform.GetChild(N - 1 + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type;

            if (type <= 2) //Beach
            {
                Pg.transform.GetChild(N - 1 + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 2;
               // Pg.transform.GetChild(N - 1 + j * N).GetComponentInChildren<BaseFrontier>().type = 4;
            }
            //else
                Pg.transform.GetChild(N - 1 + j * N).GetComponentInChildren<BaseFrontier>().type = 5;


            Pg.transform.GetChild((j - 1) * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();
            Pg.transform.GetChild(j * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();
            Pg.transform.GetChild(N - 1 + (j - 1) * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();
            Pg.transform.GetChild(N - 1 + j * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();

            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().InitializePlayground();

            DrawEdges();
        }
        else
        {
            Debug.Log("Impossible de réduire");
        }
    }

    public void IncreaseHeight()
    {
        if (M < MaxSize)
        {
            int j = M - 1;

            CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 1, 0);
            Pg.transform.GetChild(Pg.transform.childCount - 1).SetSiblingIndex(j * N); // remet le nouveau child au bon endroit

            for (int i = 1; i < N - 1; i++)
            {
                CreateSlotComponent(Pg, "Field/SlotComponent", "", 0);
                Pg.transform.GetChild(Pg.transform.childCount - 1).SetSiblingIndex(i + j * N);
            }

            CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 3, 0);
            Pg.transform.GetChild(Pg.transform.childCount - 1).SetSiblingIndex(N - 1 + j * N);

            Pg.GetComponent<PlaygroundParameters>().M = M + 1;
            M = M + 1;

 
            j = M - 1;
            int type = Pg.transform.GetChild((j - 2) * N).GetComponentInChildren<BaseFrontier>().type;

            if (type <= 2) //Beach
            {
                Pg.transform.GetChild((j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 3;
                Pg.transform.GetChild(j * N).GetComponentInChildren<BaseFrontier>().type = 5;
            }
            else
                Pg.transform.GetChild((j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 4;

            type = Pg.transform.GetChild(N - 1 + (j - 2) * N).GetComponentInChildren<BaseFrontier>().type;

            if (type <= 2) //Beach
            {
                Pg.transform.GetChild(N - 1 + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 3;
                Pg.transform.GetChild(N - 1 + j * N).GetComponentInChildren<BaseFrontier>().type = 5;
            }
            else
                Pg.transform.GetChild(N - 1 + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 4;


            Pg.transform.GetChild((j - 1) * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();
            Pg.transform.GetChild(j * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();
            Pg.transform.GetChild(N - 1 + (j - 1) * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();
            Pg.transform.GetChild(N - 1 + j * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();

            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().InitializePlayground();

            DrawEdges();
        }
        else
        {
            Debug.Log("Impossible d'agrandir");
        }
    }

    public void IncreaseBeach(bool left)
    {
        int j = 0;
        int i = 0;
        if (!left) i = N - 1;

        for (j = 1; j < M - 1; j++)
        {
            int type = Pg.transform.GetChild(i + j * N).GetComponentInChildren<BaseFrontier>().type;
            if (type >= 3) break; //ground detected
        }

        if (j == M - 1) return; //already full beach

        if (j == 1)
            Pg.transform.GetChild(i + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 0;
        else
            Pg.transform.GetChild(i + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 1;

        Pg.transform.GetChild(i + j * N).GetComponentInChildren<BaseFrontier>().type = 2;

        Pg.transform.GetChild(i + (j - 1) * N).GetComponentInChildren<BaseFrontier>().ChangeSlotImage();
        Pg.transform.GetChild(i + j * N).GetComponentInChildren<BaseFrontier>().ChangeSlotImage();
        Pg.transform.GetChild(i + (j + 1) * N).GetComponentInChildren<BaseFrontier>().ChangeSlotImage();

        DrawEdges();
    }

    public void DecreaseBeach(bool left)
    {
        int j = 0;
        int i = 0;
        if (!left) i = N - 1;

        for (j = M - 1; j > 1; j--)
        {
            int type = Pg.transform.GetChild(i + j * N).GetComponentInChildren<BaseFrontier>().type;
            if (type <= 2) break; //beach detected
        }

        if (j == 0) return; //already full ground

        if (j == 1)
            Pg.transform.GetChild(i + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 1;
        else
            Pg.transform.GetChild(i + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 2;


        Pg.transform.GetChild(i + j * N).GetComponentInChildren<BaseFrontier>().type = 3;

        if (j == M - 2)
            Pg.transform.GetChild(i + (j + 1) * N).GetComponentInChildren<BaseFrontier>().type = 5;
        else
            Pg.transform.GetChild(i + (j + 1) * N).GetComponentInChildren<BaseFrontier>().type = 4;

        Pg.transform.GetChild(i + (j - 1) * N).GetComponentInChildren<BaseFrontier>().ChangeSlotImage();
        Pg.transform.GetChild(i + j * N).GetComponentInChildren<BaseFrontier>().ChangeSlotImage();
        Pg.transform.GetChild(i + (j + 1) * N).GetComponentInChildren<BaseFrontier>().ChangeSlotImage();

        DrawEdges();

        //GameObject edge = GameObject.Find(left ? "LeftEdge" : "RightEdge");
        //ChangeSlotEdgeImage(edge.transform.GetChild(j-1).gameObject, 2);
        //ChangeSlotEdgeImage(edge.transform.GetChild(j).gameObject, 4);
        //ChangeSlotEdgeImage(edge.transform.GetChild(j+1).gameObject, 4);

    }



    static bool playMode = false;
    GameObject[] designerElements;
    public void TogglePlayMode()
    {
        playMode = !playMode;

        if(playMode)
            designerElements = GameObject.FindGameObjectsWithTag("DesignerUI");

        foreach (GameObject go in designerElements)
        {
            go.SetActive(!playMode);
        }

        GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().designerMode = !playMode;

        GameController gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        Transform deck = gc.GetComponent<GameController>().DeckHolder.transform.GetChild(0);

        deck.GetComponent<DeckManager>().DrawDeck();

        if (playMode)
        { //Go in playMode
            SaveToString();
            GetComponentInChildren<Text>().text = "Designer Mode";
            deck.parent.gameObject.SetActive(true);
            LoadFromString();
        }
        else
        { //Go in designer mode
            LoadFromString();
            GetComponentInChildren<Text>().text = "Play Mode";
            deck.parent.gameObject.SetActive(false);
        }

    }




   
}