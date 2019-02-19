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

    private void Start()
    {
        //Pg = GameObject.Find("PlaygroundHolder").transform.GetChild(0).gameObject;
        Pg = GameObject.Find("Playground");
        N = Pg.GetComponent<PlaygroundParameters>().N;
        M = Pg.GetComponent<PlaygroundParameters>().M;
    }

    public void Close()
    {
        Destroy(gameObject);
    }


    [ContextMenu("Make Thumbnail")]
    public void MakeThumb(string filename)
    {
        GameObject canvas = GameObject.Find("CanvasThumbnail");
        Pg.transform.SetParent(canvas.transform);
        GameController gc = GameObject.FindGameObjectWithTag("gameController").GetComponent<GameController>();
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
        //GameObject.Find("Message").GetComponent<Text>().text +="saving...\n";

        string filename = "PlayGround" + System.DateTime.Now.ToShortDateString().Replace("/", "-") + "-"
            + System.DateTime.Now.ToLongTimeString().Replace(":", "-");

        string file = Path.Combine(Application.persistentDataPath,filename + ".txt");

        Debug.Log(file);
        //GameObject.Find("Message").GetComponent<Text>().text += file + "\n";

        SaveToString();
        //PlayerPrefs.SetString("SavedPlaygroud", PGdata);
        /*using (StreamWriter streamWriter = File.CreateText(file))
        {
            streamWriter.Write(PGdata);
        }*/
        File.WriteAllText(file, PGdata);

        MakeThumb(Path.Combine(Application.persistentDataPath,  filename + ".png"));
    }

    public void SaveToString()
    {
        PGdata = "";
        PlaygroundParameters parameters = FindObjectOfType<PlaygroundParameters>();
        PGdata = JsonUtility.ToJson(parameters) + "\n";

        GameController gc = GameObject.FindGameObjectWithTag("gameController").GetComponent<GameController>();
        Transform deck = gc.GetComponent<GameController>().DeckHolder.transform.GetChild(0);
        foreach (Transform slot in deck.transform)
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
            }

        }

        Debug.Log(PGdata);
    }

    public void LoadFromString()
    {
        if (PGdata == null) return;

        int count = Pg.transform.childCount;
        for (int i = 0; i < count; i++)        // On retire tout
        {
            Transform child = Pg.transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

        GameController gc = GameObject.FindGameObjectWithTag("gameController").GetComponent<GameController>();
        Transform deck = gc.GetComponent<GameController>().DeckHolder.transform.GetChild(0);

        foreach (Transform slot in deck.transform)
        {
            if (slot.transform.childCount > 0)
            {
                Transform child = slot.transform.GetChild(0);
                DestroyImmediate(child.gameObject);
            }
        }

        string[] tokens = PGdata.Split('\n');

        int k = 0;
        JsonUtility.FromJsonOverwrite(tokens[k++], Pg.GetComponent<PlaygroundParameters>()); //k=0 puis 1  

        foreach (Transform slot in deck.transform)
        {
            string prefab = tokens[k++];
            if (prefab != "empty")
            {
                GameObject component = Instantiate(Resources.Load(prefab, typeof(GameObject))) as GameObject;
                component.transform.SetParent(slot.transform);
 
                JsonUtility.FromJsonOverwrite(tokens[k++], slot.transform.GetComponentInChildren<BaseComponent>());
                slot.GetComponent<CreateComponent>().Start();

            }
        }


        N = Pg.GetComponent<PlaygroundParameters>().N;
        M = Pg.GetComponent<PlaygroundParameters>().M;

        Pg.GetComponent<GridLayoutGroup>().constraintCount = N;

        CreateSlot(Pg, "Field/SlotCorner", tokens[k++], 1, tokens[k++]);

        for (int i = 1; i < N - 1; i++)
        {
            CreateSlot(Pg, "Field/SlotWall", tokens[k++], 0, tokens[k++]);
        }

        CreateSlot(Pg, "Field/SlotCorner", tokens[k++], 0, tokens[k++]);

        for (int j = 1; j < M - 1; j++)
        {
            CreateSlot(Pg, "Field/SlotWall", tokens[k++], 1, tokens[k++]);
            for (int i = 1; i < N - 1; i++) CreateSlot(Pg, "Field/SlotComponent", tokens[k++], 0, tokens[k++]); //empty component
            CreateSlot(Pg, "Field/SlotWall", tokens[k++], 3, tokens[k++]);
        }

        CreateSlot(Pg, "Field/SlotCorner", tokens[k++], 2, tokens[k++]);

        for (int i = 1; i < N - 1; i++) CreateSlot(Pg, "Field/SlotWall", tokens[k++], 2, tokens[k++]);

        CreateSlot(Pg, "Field/SlotCorner", tokens[k++], 3, tokens[k++]);

        GameObject.FindGameObjectWithTag("gameController").GetComponent<GameController>().InitializePlayground();

        ResizePlayGround();

    }

    public void LoadFromFile(string filename)
    {
        PGdata = File.ReadAllText(filename);
        LoadFromString();
    }

    [ContextMenu("LoadFromLastFile")]
    public void LoadFromLastFile()
    {
        string filename;
        string[] fileNames = System.IO.Directory.GetFiles(Application.persistentDataPath, "*.txt");
        if (fileNames.Length > 0)
        {
            filename = fileNames[fileNames.Length - 1];
            PGdata = File.ReadAllText(filename);
            LoadFromString();
        }
    }

    [ContextMenu("ChangeBorder")]
    public void ChangeBorder()
    {
        // First, check parameter and change value of dimension 
        int N = Pg.GetComponent<GridLayoutGroup>().constraintCount;
        int M = Pg.transform.childCount / N;
        PlaygroundParameters param = Pg.GetComponent<PlaygroundParameters>();
        param.N = N;
        param.M = M;



        for (int i = 1; i < N - 1; i++)
        {
            int j = 0;
            GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot

            Transform bc = go.transform.GetChild(0);
            if (bc.name.Contains("Wall"))
            {

                GameObject c = Instantiate(Resources.Load("Frontiers/Wall", typeof(GameObject))) as GameObject;
                c.transform.SetParent(go.transform);
                c.transform.localScale = Vector3.one;
                c.transform.localPosition = Vector3.zero;

                DestroyImmediate(bc.gameObject);
            }

        }

        for (int i = 0; i < N; i++)
        {
            int j = M - 1;
            GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
            Transform bc = go.transform.GetChild(0);
            if (bc.name.Contains("Wall"))
            {

                GameObject c = Instantiate(Resources.Load("Frontiers/Wall", typeof(GameObject))) as GameObject;
                c.transform.SetParent(go.transform);
                c.transform.localScale = Vector3.one;
                c.GetComponent<BaseComponent>().dir = 2;
                c.transform.localRotation = Quaternion.Euler(0.0f, 0f, 180f);
                c.transform.localPosition = Vector3.zero;

                DestroyImmediate(bc.gameObject);
            }

        }

        for (int j = 1; j < M - 1; j++)
        {
            int i = 0;
            GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
            Transform bc = go.transform.GetChild(0);
            if (bc.name.Contains("Wall"))
            {

                GameObject c = Instantiate(Resources.Load("Frontiers/Wall", typeof(GameObject))) as GameObject;
                c.transform.SetParent(go.transform);
                c.transform.localScale = Vector3.one;
                c.GetComponent<BaseComponent>().dir = 1;
                c.transform.localRotation = Quaternion.Euler(0.0f, 0f, 90f);
                c.transform.localPosition = Vector3.zero;

                DestroyImmediate(bc.gameObject);
            }

        }
        for (int j = 1; j < M - 1; j++)
        {
            int i = N - 1;
            GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
            Transform bc = go.transform.GetChild(0);
            if (bc.name.Contains("Wall"))
            {

                GameObject c = Instantiate(Resources.Load("Frontiers/Wall", typeof(GameObject))) as GameObject;
                c.transform.SetParent(go.transform);
                c.transform.localScale = Vector3.one;
                c.GetComponent<BaseComponent>().dir = 3;
                c.transform.localRotation = Quaternion.Euler(0.0f, 0f, -90f);
                c.transform.localPosition = Vector3.zero;

                DestroyImmediate(bc.gameObject);
            }

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
            slot.transform.localScale = new Vector3(-1, 1, 1);
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
            //component.GetComponent<BaseComponent>().Rotate();
            //component.transform.localRotation = Quaternion.Euler(0, 0, 90f * dir);
        }

        return slot;
    }
    

    void CreateSlot(GameObject PlayGround, string PrefabSlotPath, string PrefabComponentPath, int dir, string data)
    {
        GameObject slot = Instantiate(Resources.Load(PrefabSlotPath, typeof(GameObject))) as GameObject;
        slot.transform.SetParent(PlayGround.transform);
        slot.transform.localPosition = Vector3.zero;
        slot.transform.localScale = Vector3.one;
        slot.transform.localRotation = Quaternion.identity;


        GameObject component = Instantiate(Resources.Load(PrefabComponentPath, typeof(GameObject))) as GameObject;
        component.transform.SetParent(slot.transform);
        component.transform.localPosition = Vector3.zero;
        component.transform.localScale = Vector3.one;


        JsonUtility.FromJsonOverwrite(data, slot.transform.GetComponentInChildren<BaseComponent>());
        slot.transform.GetComponentInChildren<BaseComponent>().Awake();

        if (slot.GetComponentInChildren<BaseFrontier>() != null)
        { //if corner or frontier
            component.GetComponent<BaseFrontier>().InitializeSlot();
            component.GetComponent<BaseComponent>().dir = dir;  //override frontier direction in case it is in the wrong way
            //component.GetComponent<BaseComponent>().Rotate();

            //component.transform.localRotation = Quaternion.Euler(0, 0, 90f * dir);
        }
        else
        {
            dir = component.GetComponent<BaseComponent>().dir;
            //component.GetComponent<BaseComponent>().Rotate();

            //component.transform.localRotation = Quaternion.Euler(0, 0, 90f * dir);
        }

        bool designerMode = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().designer;

        component.GetComponent<BaseComponent>().destroyable = designerMode;

    }


    public void ResetField()
    {
        PlaygroundParameters param = Pg.GetComponent<PlaygroundParameters>();
        param.N = N;
        param.M = M;

        // On retire tout
        int count = Pg.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform child = Pg.transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

        Pg.GetComponent<GridLayoutGroup>().constraintCount = N;

        CreateSlotFrontier(Pg, "Field/SlotCorner", "Frontiers/Corner", 6, 0);
        for (int i = 1; i < N - 1; i++) CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 0, 1);
        CreateSlotFrontier(Pg, "Field/SlotCorner", "Frontiers/Corner", 0, 0);

        for (int j = 1; j < M - 1; j++)
        {
            int type = Mathf.Min(j, 4);
            CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 1, type);
            for (int i = 1; i < N - 1; i++) CreateSlotComponent(Pg, "Field/SlotComponent", "", 0); //empty component
            CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 3, type);
        }

        CreateSlotFrontier(Pg, "Field/SlotCorner", "Frontiers/Corner", 5, 5);

        for (int i = 1; i < N - 1; i++) CreateSlotFrontier(Pg, "Field/SlotWall", "Frontiers/Wall", 2, 4);

        CreateSlotFrontier(Pg, "Field/SlotCorner", "Frontiers/Corner", 2, 5);

        GameObject.FindGameObjectWithTag("gameController").GetComponent<GameController>().InitializePlayground();

        ResizePlayGround();
    }

    void ResizePlayGround()
    {
        Transform go = Pg.transform.parent;
        RectTransform objectRectTransform = go.GetComponent<RectTransform>();
        float width = objectRectTransform.rect.width;
        float height = objectRectTransform.rect.height;

        float wx = width / (100f * (N - 1));
        float wy = height / (100f * M);

        float wc = Mathf.Min(wx, wy);

        Pg.transform.localScale = new Vector3(wc, wc, 1);
        //Pg.transform.localPosition = Vector3.zero;
    }

    public void ChangeSize()
    {
        PlaygroundParameters param = Pg.GetComponent<PlaygroundParameters>();

        GameObject Temp = new GameObject();

        Debug.Log(Pg.transform.childCount);

        int count = Pg.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform child = Pg.transform.GetChild(0);
            child.SetParent(Temp.transform);
        }


        if (N >= 4 && M >= 4)
        {
            if (N < param.N)
            {
                for (int i = N - 2; i < param.N - 2; i++)
                    for (int j = 0; j < param.M; j++)
                    {

                    }
            }



        }

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

            GameObject.FindGameObjectWithTag("gameController").GetComponent<GameController>().InitializePlayground();
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

            GameObject.FindGameObjectWithTag("gameController").GetComponent<GameController>().InitializePlayground();
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
            Pg.GetComponent<PlaygroundParameters>().M = M - 1;
            M = M - 1;

            GameObject.FindGameObjectWithTag("gameController").GetComponent<GameController>().InitializePlayground();



            /****   ***/

            int type = Pg.transform.GetChild((j - 1) * N).GetComponentInChildren<BaseFrontier>().type;

            if (type <= 2) //Beach
            {
                Pg.transform.GetChild((j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 2;
                Pg.transform.GetChild(j * N).GetComponentInChildren<BaseFrontier>().type = 4;
            }
            else
                Pg.transform.GetChild(j * N).GetComponentInChildren<BaseFrontier>().type = 5;

            type = Pg.transform.GetChild(N - 1 + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type;

            if (type <= 2) //Beach
            {
                Pg.transform.GetChild(N - 1 + (j - 1) * N).GetComponentInChildren<BaseFrontier>().type = 2;
                Pg.transform.GetChild(N - 1 + j * N).GetComponentInChildren<BaseFrontier>().type = 4;
            }
            else
                Pg.transform.GetChild(N - 1 + j * N).GetComponentInChildren<BaseFrontier>().type = 5;


            Pg.transform.GetChild((j - 1) * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();
            Pg.transform.GetChild(j * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();
            Pg.transform.GetChild(N - 1 + (j - 1) * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();
            Pg.transform.GetChild(N - 1 + j * N).GetComponentInChildren<BaseFrontier>().InitializeSlot();


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

            GameObject.FindGameObjectWithTag("gameController").GetComponent<GameController>().InitializePlayground();

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

        if (j == M - 2)
            Pg.transform.GetChild(i + (j + 1) * N).GetComponentInChildren<BaseFrontier>().type = 4;
        else
            Pg.transform.GetChild(i + (j + 1) * N).GetComponentInChildren<BaseFrontier>().type = 3;

        Pg.transform.GetChild(i + (j - 1) * N).GetComponentInChildren<BaseFrontier>().ChangeSlotImage();
        Pg.transform.GetChild(i + j * N).GetComponentInChildren<BaseFrontier>().ChangeSlotImage();
        Pg.transform.GetChild(i + (j + 1) * N).GetComponentInChildren<BaseFrontier>().ChangeSlotImage();
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

        GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>().designer = !playMode;

        GameController gc = GameObject.FindGameObjectWithTag("gameController").GetComponent<GameController>();
        Transform deck = gc.GetComponent<GameController>().DeckHolder.transform.GetChild(0);

        deck.GetComponent<DeckManager>().TogglePlayMode();

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