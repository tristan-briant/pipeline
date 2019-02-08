using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigFile : MonoBehaviour
{
    string[] fileNames;

    void Start()
    {
        ListFile();
    }

    [ContextMenu("List File")]
    public void ListFile()
    {
        GameObject fileList = GameObject.Find("FileList");
        GameObject fileButton = fileList.transform.GetChild(0).gameObject;

        int index = 0;

        fileNames = System.IO.Directory.GetFiles(Application.persistentDataPath);
        foreach (string file in fileNames)
        {
            Debug.Log(file);
            GameObject go = Instantiate(fileButton);
            go.transform.SetParent(fileList.transform);
            go.transform.localScale = Vector3.one;
            go.GetComponent<Button>().onClick.RemoveAllListeners();
            int i = index;
            go.GetComponent<Button>().onClick.AddListener(() => LoadFile(i));
            index++;
        }

        Destroy(fileButton.gameObject); // Détruit le modèle 

    }

    public void LoadFile(int i)
    {
        Debug.Log("Load File:" + fileNames[i]);
        GetComponent<Designer>().LoadFromFile(fileNames[i]);
    }


    public void Close()
    {
        Destroy(this.gameObject);
    }
}
