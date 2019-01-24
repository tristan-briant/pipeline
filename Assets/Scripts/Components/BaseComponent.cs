﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class BaseComponent : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    protected float q = 0, f = 0;
    public int dir=0;
    protected float R = 2f, L = 1f, C =1f, Rground = 50;
    protected float fluxMinSound =0.01f;
    //protected string Name;
    protected float[] pin = new float[4];
    protected float[] iin = new float[4];
    public float success = 1;
    public float fail = 0;
    public const float fMinBubble = 0.1f;
    //public bool empty = true;

    public int x, y;
    public bool locked = false;
    public bool dir_locked = false;
    public bool mirror = false;


    public bool trigged=false;   // for composant that can be trigged. use with tag Triggerable

    gameController gc; // le moteur du jeu à invoquer parfois
    protected AudioSource[] audios;
    PlaygroundParameters parameters;

    protected Color pressureColor(float p)
    { // donne la convention de pression
        float PMAX = 1.0f;

        Color max = new Color(0, 1.0f, 1.0f);  // p=2
        //Color zero = new Color(0, 0.0f / 255, 00.0f / 255); //
        Color zero = new Color(0, 100.0f/255, 140.0f/255);  // p=0
        Color min = new Color(150.0f/255, 75.0f/255, 120.0f/255);  // p=-2

        p = Mathf.Clamp(p, -PMAX, PMAX);

        if (p >= 0)
            return Color.Lerp(zero, max, p / PMAX);
        else
            return Color.Lerp(min, zero, 1 + p / PMAX);

    }

    public void Set_i_p(float[] p, float[] i) {

        if (mirror)
        {
            float e = p[0];
            p[0] = p[2];
            p[2] = e;
            e = i[0];
            i[0] = i[2];
            i[2] = e;
        }

        for (int k = 0; k < 4; k++) {
            pin[k] = p[k];
            iin[k] = i[k];
        }
    }

    public virtual void Reset_i_p()
    {
        for (int k = 0; k < 4; k++)
        {
            pin[k] = 0;
            iin[k] = 0;
        }
        q = f = 0;
    }

    public virtual void calcule_i_p(float[] p, float[] i,float dt)
    {
        /*i[0] = p[0] / Rground;
        i[1] = p[1] / Rground;
        i[2] = p[2] / Rground;
        i[3] = p[3] / Rground;*/

        for(int k = 0; k < 4; k++)
        {
            calcule_i_p_blocked(p, i, dt, k);
        }
    }

    public virtual void calcule_i_p_blocked(float[] p, float[] i, float dt,int index)
    {
        float a = p[index];
       /* p[index] = i[index] * Rground;
        i[index] = a / Rground;*/

        //p[index] *= 0.99f;

        i[index] = 0;
    }


    protected virtual void Start()
    {
        success = 1;
        gc = (gameController)GameObject.Find("gameController").GetComponent(typeof(gameController)); //find the game engine
        parameters = (PlaygroundParameters)transform.parent.transform.parent.GetComponent(typeof(PlaygroundParameters)); //find the game engine
        audios = GameObject.Find("PlaygroundHolder").GetComponents<AudioSource>();

        if (parameters != null)
        {
            R = parameters.R;
            C = parameters.C;
            L = parameters.L;
            Rground = parameters.Rground;
        }

        transform.rotation = Quaternion.identity;
        transform.Rotate(new Vector3(0, 0, dir * 90));
    }

    private void Update()
    {
        
        //c += 0.01f;
        //if (c > 1) c = 0.0f;
        //water.GetComponent<Image>().color = new Color(1, c, 1);
    }


    bool dragged;

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (!dragged && !locked && !dir_locked)
        {
            dir++;
            if (dir == 4) dir = 0;
            transform.rotation = Quaternion.identity;
            transform.Rotate(new Vector3(0, 0, dir * 90));
            //Debug.Log("Object " + Name + " clicked !" + dir);
            
            audios[0].Play();
        }

        if (!dragged && (locked || dir_locked)) {
            audios[1].Play();
        }
    }


    Vector3 startPos;
    //float guiDepth;
    public static Transform startParent;
    Transform canvas;
    public static GameObject itemBeingDragged;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemBeingDragged != null) { eventData.pointerDrag = null; return; } // If one element already draged, cancel the drag

        if (locked) { eventData.pointerDrag = null; return; } // If locked cancel the drag

        startPos = transform.position;
        itemBeingDragged = gameObject;
        startParent = transform.parent;
        GetComponent<CanvasGroup >().blocksRaycasts = false;
        canvas = GameObject.FindGameObjectWithTag("Playground").transform;
        transform.SetParent(canvas);
        dragged = true;
        gc.PopulateComposant();

        transform.localScale = transform.localScale * 1.2f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
        Vector3 vec = Input.mousePosition;
        vec.z = 1.0f;
        transform.position = Camera.main.ScreenToWorldPoint(vec);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        itemBeingDragged = null;
        transform.localScale = transform.localScale / 1.2f;
        DestroyImmediate (startParent.GetChild(0).gameObject); //On enlève le composant vide qui a été placé au début du drag 
        /* Destroyimmediate et pas destroy simple sinon present jusqu'à la fin du frame et populatecomposant fail et le trouve toujours  */

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if(transform.parent == canvas)
        {
            transform.SetParent(startParent);
        }
        //transform.localPosition = new Vector3(0, 0, 1); // finalement pas besoin

        dragged = false;

        gc.PopulateComposant();
        audios[1].Play();
    }
}
