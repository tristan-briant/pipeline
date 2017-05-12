using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour {

    public LevelManager LVM;
    public GameObject TipButton;
    //public GameObject introScreen;

    private void Awake()
    {

        LVM = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        foreach (Transform go in transform)
            GameObject.DestroyImmediate(go.gameObject);

        transform.localPosition = new Vector3(0, 0, 0);
       
        /// Prepare intro if any
        string levelName = LVM.getPlaygroundName(LVM.currantLevel);
        Object obj = Resources.Load("Intro/" + levelName + "_intro", typeof(GameObject));

        if (obj == null)
        {
            removeIntro();
            TipButton.SetActive(false);
        }
        else
        {
            TipButton.SetActive(true);
            GameObject intro = Instantiate(obj) as GameObject;
            intro.transform.SetParent(gameObject.transform);
            intro.transform.localScale = new Vector3(1, 1, 1);
            intro.transform.localPosition = new Vector3(0, 0, 0);


            if (LVM.currantLevel > LVM.completedLevel)
            {
                StartCoroutine("IntroScreen");
            }
            else
            {
                removeIntro();
            }
        }
    }

    public void removeIntro()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
    }

    public void playIntro() {
        StartCoroutine("IntroScreen");
    }

    public IEnumerator IntroScreen()
    {
        gameObject.GetComponent<Canvas>().enabled = true;
        yield return new WaitForSeconds(40f);
        removeIntro();
    }
}
