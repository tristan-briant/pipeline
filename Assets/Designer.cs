using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


public class Designer : MonoBehaviour {

    const int MaxSize = 8;
    const int MinSize = 5;

    public GameObject Pg;

    [ContextMenu("SaveToFile")]
    public void SaveToFile()
    {
        string data = "";
        PlaygroundParameters parameters = GameObject.FindObjectOfType<PlaygroundParameters>();
        data = JsonUtility.ToJson(parameters) + "\n";

        foreach (Transform slot in Pg.transform)
        {
            BaseComponent component = slot.GetComponentInChildren<BaseComponent>();
            if (component != null && component.name!="")
            {
                string path = component.PrefabPath;
                data += path + "\n";
                data += JsonUtility.ToJson(component) + "\n";
            }
            else
            {
                data += "empty\n";
            }
            
        }

        Debug.Log(data);
        PlayerPrefs.SetString("SavedPlaygroud", data);
    }

    [ContextMenu("LoadFromFile")]
    public void LoadFromFile()
    {
        string data =PlayerPrefs.GetString("SavedPlaygroud");
        if (data == null) return;

        int count = Pg.transform.childCount;
        for (int i = 0; i < count; i++)        // On retire tout
        {
            Transform child = Pg.transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

        string[] tokens = data.Split('\n');
  
        int k = 0;
        JsonUtility.FromJsonOverwrite(tokens[k++], Pg.GetComponent<PlaygroundParameters>()); //k=0 puis 1  

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

        GameObject.FindGameObjectWithTag("gameController").GetComponent<gameController>().InitializePlayground();

        ResizePlayGround();


       /* int i = 0;
        while(i < tokens.Length)
        {
            if(tokens[i]);
            Debug.Log(s);
            if()
        }*/

    }

    [ContextMenu("ChangeBorder")]
    public void ChangeBorder() {
        // First, check parameter and change value of dimension 
        int N = Pg.GetComponent<GridLayoutGroup>().constraintCount; 
        int M = Pg.transform.childCount / N;
        PlaygroundParameters param = Pg.GetComponent<PlaygroundParameters>();
        param.N = N;
        param.M = M;



        for (int i = 1; i < N-1; i++)
        {
            int j = 0;
            GameObject go = Pg.transform.GetChild((i) + (j) * (N)).gameObject; //the slot
            
            Transform bc = go.transform.GetChild(0);
            if (bc.name.Contains("Wall"))
            {
               
                GameObject c = PrefabUtility.InstantiatePrefab(Resources.Load("Frontiers/Wall", typeof(GameObject))) as GameObject;
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

                GameObject c = PrefabUtility.InstantiatePrefab(Resources.Load("Frontiers/Wall", typeof(GameObject))) as GameObject;
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

                GameObject c = PrefabUtility.InstantiatePrefab(Resources.Load("Frontiers/Wall", typeof(GameObject))) as GameObject;
                c.transform.SetParent(go.transform);
                c.transform.localScale = Vector3.one;
                c.GetComponent<BaseComponent>().dir = 1;
                c.transform.localRotation= Quaternion.Euler(0.0f, 0f, 90f);
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

                GameObject c = PrefabUtility.InstantiatePrefab(Resources.Load("Frontiers/Wall", typeof(GameObject))) as GameObject;
                c.transform.SetParent(go.transform);
                c.transform.localScale = Vector3.one;
                c.GetComponent<BaseComponent>().dir = 3;
                c.transform.localRotation = Quaternion.Euler(0.0f, 0f, -90f);
                c.transform.localPosition = Vector3.zero;

                DestroyImmediate(bc.gameObject);
            }

        }
        
    }


    GameObject CreateSlot(GameObject PlayGround, string PrefabSlotPath, string PrefabComponentPath, int dir)
    {
        GameObject slot= PrefabUtility.InstantiatePrefab(Resources.Load(PrefabSlotPath, typeof(GameObject))) as GameObject;
        slot.transform.SetParent(PlayGround.transform);
        slot.transform.localPosition = Vector3.zero;
        slot.transform.localScale = Vector3.one;
        slot.transform.localRotation = Quaternion.identity;

        if (PrefabComponentPath != "")
        {
            GameObject component = PrefabUtility.InstantiatePrefab(Resources.Load(PrefabComponentPath, typeof(GameObject))) as GameObject;
            component.transform.SetParent(slot.transform);
            component.transform.localPosition = Vector3.zero;
            component.transform.localScale = Vector3.one;
            component.GetComponent<BaseComponent>().dir = dir;
            component.transform.localRotation = Quaternion.Euler(0, 0, 90f * dir);
        }

        if ( !slot.GetComponent<OnDrop>() || slot.GetComponent<OnDrop>().isSlotFrontier) //if corner or frontier
            slot.transform.localRotation = Quaternion.Euler(0, 0, 90f * dir);

        return slot;
    }

    void CreateSlot(GameObject PlayGround, string PrefabSlotPath, string PrefabComponentPath, int dir, string data)
    {
        GameObject slot = PrefabUtility.InstantiatePrefab(Resources.Load(PrefabSlotPath, typeof(GameObject))) as GameObject;
        slot.transform.SetParent(PlayGround.transform);
        slot.transform.localPosition = Vector3.zero;
        slot.transform.localScale = Vector3.one;
        slot.transform.localRotation = Quaternion.identity;


        GameObject component = PrefabUtility.InstantiatePrefab(Resources.Load(PrefabComponentPath, typeof(GameObject))) as GameObject;
        component.transform.SetParent(slot.transform);
        component.transform.localPosition = Vector3.zero;
        component.transform.localScale = Vector3.one;
        component.GetComponent<BaseComponent>().dir = dir;
        component.transform.localRotation = Quaternion.Euler(0, 0, 90f * dir);


        if (!slot.GetComponent<OnDrop>() || slot.GetComponent<OnDrop>().isSlotFrontier) //if corner or frontier
            slot.transform.localRotation = Quaternion.Euler(0, 0, 90f * dir);

        JsonUtility.FromJsonOverwrite(data, slot.transform.GetComponentInChildren<BaseComponent>());
    }


    [ContextMenu("Reset field")]
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

        CreateSlot(Pg, "Field/SlotCorner", "Frontiers/Corner", 1);
        for (int i = 1; i < N - 1; i++) CreateSlot(Pg, "Field/SlotWall", "Frontiers/Wall", 0);
        CreateSlot(Pg, "Field/SlotCorner", "Frontiers/Corner", 0);

        for (int j = 1; j < M - 1; j++)
        {
            CreateSlot(Pg, "Field/SlotWall", "Frontiers/Wall", 1);
            for (int i = 1; i < N - 1; i++) CreateSlot(Pg, "Field/SlotComponent", "", 0); //empty component
            CreateSlot(Pg, "Field/SlotWall", "Frontiers/Wall", 3);
        }

        CreateSlot(Pg, "Field/SlotCorner", "Frontiers/Corner", 2);

        for (int i = 1; i < N - 1; i++) CreateSlot(Pg, "Field/SlotWall", "Frontiers/Wall", 2);

        CreateSlot(Pg, "Field/SlotCorner", "Frontiers/Corner", 3);

        GameObject.FindGameObjectWithTag("gameController").GetComponent<gameController>().InitializePlayground();

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
        Pg.transform.localPosition = Vector3.zero;
    }

    public int N,M;
    [ContextMenu("ChangeSize")]
    public void ChangeSize() {
        PlaygroundParameters param = Pg.GetComponent<PlaygroundParameters>();

        GameObject Temp=new GameObject();

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
                for (int i=N-2;i<param.N-2;i++)
                    for(int j = 0; j < param.M; j++)
                    {
                   
                    }
            }



        }

    }
    [ContextMenu("Reduce Width")]
    public void ReduceWidth()
    {
        if (N > MinSize)
        {
            int i = N - 2;
            for (int j = M-1; j >= 0; j--)
            {
                DestroyImmediate(Pg.transform.GetChild(i + j * N).gameObject);
            }
            Pg.GetComponent<GridLayoutGroup>().constraintCount = N - 1;
            Pg.GetComponent<PlaygroundParameters>().N = N - 1;
            N = N - 1;

            GameObject.FindGameObjectWithTag("gameController").GetComponent<gameController>().InitializePlayground();
        }
        else
        {
            Debug.Log("Impossible de réduire");
        }
    }

    [ContextMenu("Increase Width")]
    public void IncreaseWidth()
    {
        if (N < MaxSize)
        {
            int i = N - 1;

            CreateSlot(Pg, "Field/SlotWall", "Frontiers/Wall", 0);
            Pg.transform.GetChild(Pg.transform.childCount-1).SetSiblingIndex(i); // remet le nouveau child au bon endroit

            for (int j = 1; j < M-1; j++)
            {
                CreateSlot(Pg, "Field/SlotComponent", "", 0);
                Pg.transform.GetChild(Pg.transform.childCount-1).SetSiblingIndex(i + j * (N + 1));
            }

            CreateSlot(Pg, "Field/SlotWall", "Frontiers/Wall", 2);
            Pg.transform.GetChild(Pg.transform.childCount-1).SetSiblingIndex(i + (M - 1) * (N + 1));

            Pg.GetComponent<GridLayoutGroup>().constraintCount = N + 1;
            Pg.GetComponent<PlaygroundParameters>().N = N + 1;
            N = N + 1;

            GameObject.FindGameObjectWithTag("gameController").GetComponent<gameController>().InitializePlayground();
        }
        else
        {
            Debug.Log("Impossible d'agrandir");
        }
    }

    [ContextMenu("Reduce Height")]
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

            GameObject.FindGameObjectWithTag("gameController").GetComponent<gameController>().InitializePlayground();
        }
        else
        {
            Debug.Log("Impossible de réduire");
        }
    }

    [ContextMenu("Increase Height")]
    public void IncreaseHeight()
    {
        if (M < MaxSize)
        {
            int j = M - 1;

            CreateSlot(Pg, "Field/SlotWall", "Frontiers/Wall", 1);
            Pg.transform.GetChild(Pg.transform.childCount - 1).SetSiblingIndex(j * N); // remet le nouveau child au bon endroit

            for (int i = 1; i < N - 1; i++)
            {
                CreateSlot(Pg, "Field/SlotComponent", "", 0);
                Pg.transform.GetChild(Pg.transform.childCount - 1).SetSiblingIndex(i + j * N);
            }

            CreateSlot(Pg, "Field/SlotWall", "Frontiers/Wall", 3);
            Pg.transform.GetChild(Pg.transform.childCount - 1).SetSiblingIndex(N-1 + j * N);

            Pg.GetComponent<PlaygroundParameters>().M = M + 1;
            M = M + 1;

            GameObject.FindGameObjectWithTag("gameController").GetComponent<gameController>().InitializePlayground();
        }
        else
        {
            Debug.Log("Impossible d'agrandir");
        }
    }

}