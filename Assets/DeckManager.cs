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
        bool designerMode =LevelManager.designerMode;

        if (designerMode)
        {
            gameObject.SetActive(true);
            foreach (Transform slot in transform)
            {
                slot.gameObject.SetActive(true);
                slot.GetComponent<CreateComponent>().designermode = true;
            }
            //text.gameObject.SetActive(true);
            transform.parent.Find("Toggle").gameObject.SetActive(true);
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
                ShowDeck(false);
            else
                ShowDeck(true);

            transform.parent.Find("Toggle").gameObject.SetActive(false);

            //text.gameObject.SetActive(false);

        }
    }

    public void ShowDeck(bool visible=true)
    {
        transform.parent.parent.parent.GetComponent<Animator>().SetBool("visible", visible);
    }


    public void ToggleShowDeck()
    {
        bool visible = transform.parent.parent.parent.GetComponent<Animator>().GetBool("visible");
        transform.parent.parent.parent.GetComponent<Animator>().SetBool("visible", !visible);
    }
}
