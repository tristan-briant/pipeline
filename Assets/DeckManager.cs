using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    public Text text;

    private void Start()
    {
        DrawDeck();
    }

    public void DrawDeck()
    {
        bool designerMode = GameObject.Find("LevelManager").GetComponent<LevelManager>().designerMode;

        if (designerMode)
        {
            gameObject.SetActive(true);
            foreach (Transform slot in transform)
            {
                slot.gameObject.SetActive(true);
                slot.GetComponent<CreateComponent>().designermode = true;
            }
            text.gameObject.SetActive(true);
        }
        else
        {
            int count = 0;
            foreach (Transform slot in transform)
            {
                if (slot.childCount == 0 || slot.GetChild(0).name.Contains("empty"))
                {
                    slot.gameObject.SetActive(false);
                }
                else
                {
                    slot.gameObject.SetActive(true);
                    slot.GetComponent<CreateComponent>().designermode = false;
                    count++;
                }
            }

            if (count == 0)
            {
                gameObject.SetActive(false);
                Debug.Log(count);
            }

            text.gameObject.SetActive(false);

        }
    }

}
