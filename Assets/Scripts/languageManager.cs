using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class languageManager : MonoBehaviour {

    public string text_fr = "";
    public string text_en = "";
    GameObject LVM;

    // Use this for initialization
    void Awake() {
        LVM = GameObject.FindGameObjectWithTag("LevelManager");
    }

    private void Update()
    {
         switch (LVM.GetComponent<LevelManager>().language) {
            case "english":
            case "English":
            default:
                GetComponent<Text>().text = text_en;
                break;
            case "french":
            case "French":
                GetComponent<Text>().text = text_fr;
                break;
        }
    }
	
}
