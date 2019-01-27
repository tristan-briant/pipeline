using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategorySelection : MonoBehaviour {

    public List<GameObject> buttons;

    public void Toggle()
    {
        Transform parent = buttons[0].transform.parent;
        foreach (Transform child in parent.transform)  // disable All
            child.gameObject.SetActive(false);

        foreach (GameObject but in buttons)  // Enable selected
            but.SetActive(true);
    } 

}
