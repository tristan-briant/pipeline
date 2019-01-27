using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


public class Designer : MonoBehaviour {

    public GameObject Pg;

    [ContextMenu("SaveToFile")]
    public void SaveToFile()
    {
        string data = "";
        //gameController gc = GameObject.Find("gameController").GetComponent<gameController>();
        PlaygroundParameters parameters = GameObject.FindObjectOfType<PlaygroundParameters>();
        data = JsonUtility.ToJson(parameters) + "\n";

        /* for (int i = 0; i < parameters.N; i++)
             for (int j = 0; j < parameters.M; j++) */
        foreach (Transform slot in Pg.transform)
        {
            //Debug.Log(slot);
            BaseComponent component = slot.GetComponentInChildren<BaseComponent>();
            //Object parentObject = PrefabUtility.GetCorrespondingObjectFromSource(component);
            if (component != null && component.name!="")
            {
                string path = component.PrefabPath;
                //path = path.Substring("Assets/Resources/".Length, path.Length - "Assets/Resources/".Length - ".prefab".Length);
                data += path + "\n";
                data += JsonUtility.ToJson(component) + "\n";
            }
            else
            {
                data += "empty\n";
            }
            
        }


        Debug.Log(data);


        //Debug.Log(gc);
        /*Object parentObject = PrefabUtility.GetCorrespondingObjectFromSource(objectToExport);
        string path =  AssetDatabase.GetAssetPath(parentObject);
        path = path.Substring("Assets/Resources/".Length, path.Length - "Assets/Resources/".Length - ".prefab".Length); //remove absolute path and extension
        Debug.Log("prefab path:" + path);
        //Debug.Log(AssetDatabase.GetAssetPath(objectToExport));
        Debug.Log(JsonUtility.ToJson(path));
        Debug.Log(JsonUtility.ToJson(objectToExport));
        string json = JsonUtility.ToJson(objectToExport);*/

        //GameObject c = Instantiate(Resources.Load(path, typeof(GameObject))) as GameObject;

        //JsonUtility.FromJsonOverwrite(json,c.GetComponent<BaseComponent>());
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


    void CreateSlot(GameObject PlayGround, string PrefabSlotPath, string PrefabComponentPath, int dir)
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

        CreateSlot(Pg, "Field/SlotWall", "Frontiers/Corner", 1);
        for (int i = 1; i < N - 1; i++) CreateSlot(Pg, "Field/SlotWall", "Frontiers/Wall", 0);
        CreateSlot(Pg, "Field/SlotWall", "Frontiers/Corner", 0);

        for (int j = 1; j < M - 1; j++)
        {
            CreateSlot(Pg, "Field/SlotWall", "Frontiers/Wall", 1);
            for (int i = 1; i < N - 1; i++) CreateSlot(Pg, "Field/SlotComponent", "", 0); //empty component
            CreateSlot(Pg, "Field/SlotWall", "Frontiers/Wall", 3);
        }

        CreateSlot(Pg, "Field/SlotWall", "Frontiers/Corner", 2);

        for (int i = 1; i < N - 1; i++) CreateSlot(Pg, "Field/SlotWall", "Frontiers/Wall", 2);

        CreateSlot(Pg, "Field/SlotWall", "Frontiers/Corner", 3);

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


}