using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class optionManager : MonoBehaviour {

    LevelManager LVM;

	void Awake () {
        gameObject.SetActive(false);
        LVM = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
    }

    public void setLanguage(string language)
    {
        LVM.Language=language;
        PlayerPrefs.SetString("Language",language);
    }

    public void resetGame()
    {
        PlayerPrefs.SetInt("Level Completed",0);
        LVM.completedLevel = 0;
    }

}
