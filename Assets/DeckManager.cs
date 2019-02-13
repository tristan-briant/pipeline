using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{

    private void Start()
    {
        TogglePlayMode();
    }

    public void TogglePlayMode()
    {
        bool designerMode = GameObject.Find("LevelManager").GetComponent<LevelManager>().designer;

        if (designerMode)
        {
            foreach (Transform slot in transform)
            {
                slot.gameObject.SetActive(true);
                slot.GetComponent<CreateComponent>().designermode = true;
            }
        }
        else
        {
            foreach (Transform slot in transform)
            {
                if (slot.childCount == 0 || slot.GetChild(0).name.Contains("empty"))
                    slot.gameObject.SetActive(false);
                slot.GetComponent<CreateComponent>().designermode = false;
            }
        }
    }

}
