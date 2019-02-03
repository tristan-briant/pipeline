using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseFrontier : BaseComponent {

    public int type = 0; //0 = corner beach, 1 = beach,  2 = beach corner>ground, 3 = beach>corner ground, 4 = ground, 5= corner ground...

    protected override void Start() {
        gc = (GameController)GameObject.Find("gameController").GetComponent(typeof(GameController)); //find the game engine
        audios = GameObject.Find("PlaygroundHolder").GetComponents<AudioSource>();



    }

    public void InitializeSlot() { //set the Frontier slot
        Transform slot = transform.parent;
        Sprite[] sprites = Resources.LoadAll<Sprite>("Field/BorderAtlas");

        int index=0;
        switch (type)
        {
            case 0: index = 7;  break;
            case 1: index = (int)Random.Range(5, 6.999f); break;
            case 2: index = 4; break;
            case 3: index = 3; break;
            case 4: index = (int)Random.Range(1, 2.999f); break;
            case 5: index = 0; break;
        }
        slot.GetComponent<Image>().sprite = sprites[index];

        if (dir == 3)
        {
            slot.transform.localScale = new Vector3(-1, 1, 1);
            slot.transform.localRotation = Quaternion.Euler(0, 0, -90f);
        }
        else if (dir == 5)
        {
            slot.transform.localScale = new Vector3(-1, 1, 1);
            slot.transform.localRotation = Quaternion.Euler(0, 0, 180f);
        }
        else if (dir == 6)
        {
            slot.transform.localScale = new Vector3(-1, 1, 1);
            slot.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
            slot.transform.localRotation = Quaternion.Euler(0, 0, 90f * dir); // C'est le slot qui tourne

    }


    public override void calcule_i_p(float[] p, float[] i, float alpha)
    {
        calcule_i_p_blocked(p, i, alpha, 0);
    }


}
