using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour {

    public LevelManager LVM;
    //public GameObject introScreen;

    private void Awake()
    {

        LVM = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        if (LVM.FirstLaunch)
        {
            StartCoroutine("IntroScreen");
            LVM.FirstLaunch = false;
        }
        else
        {
            removeIntro();
        }

        int currantLevel = LVM.currantLevel;
        string levelName = LVM.getPlaygroundName(currantLevel);


        Object obj = Resources.Load("Intro/" + levelName + "_intro", typeof(GameObject));

        if (obj == null)
            removeIntro();
        else
        {
            GameObject intro = Instantiate(obj) as GameObject;
            intro.transform.SetParent(gameObject.transform);
            intro.transform.localScale = new Vector3(1,1,1);
            StartCoroutine("IntroScreen");
        }
    }

    public void removeIntro()
    {
        gameObject.GetComponent<Canvas>().enabled = false;

    }

    IEnumerator IntroScreen()
    {
        gameObject.GetComponent<Canvas>().enabled = true;
        yield return new WaitForSeconds(20f);
        removeIntro();
    }
}
