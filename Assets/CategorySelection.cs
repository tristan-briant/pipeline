using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategorySelection : MonoBehaviour {

    public List<GameObject> buttons;
    public GameObject configPanel;

    public void Toggle()
    {
        Transform parent = buttons[0].transform.parent;
        foreach (Transform child in parent.transform)  // disable All
            child.gameObject.SetActive(false);

        foreach (GameObject but in buttons)  // Enable selected
            but.SetActive(true);
    } 

    public void LaunchConfigPanel()
    {
        GameObject CP = Instantiate(configPanel);

        //CP.transform.localPosition = Vector3.zero;
        CP.transform.SetParent(GameObject.Find("MainCanvas").transform);
        CP.transform.localScale = Vector3.one;

        RectTransform rect = CP.transform.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 300);
        rect.anchoredPosition = new Vector2(0, 0);
    }

}
