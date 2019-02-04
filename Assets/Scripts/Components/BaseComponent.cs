﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class BaseComponent : MonoBehaviour, IBeginDragHandler, IDragHandler,
    IEndDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public string PrefabPath="";
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
    public float pressure;


    //public int x, y;
    public bool locked = false;
    public bool Locked
    {
        get
        {
            return locked;
        }

        set
        {
            locked = value;
            SetLocked();
        }
    }

    public bool dir_locked = false;
    public bool mirror = false;
    public bool isFrontiers=false;

    public bool trigged=false;   // for composant that can be trigged. use with tag Triggerable

    protected GameController gc; // le moteur du jeu à invoquer parfois
    protected AudioSource[] audios;
    PlaygroundParameters parameters;

    protected Color PressureColor(float p)
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

    public virtual void Calcule_i_p(float[] p, float[] i,float dt)
    {
        for(int k = 0; k < 4; k++)
        {
            Calcule_i_p_blocked(p, i, dt, k);
        }
    }

    public virtual void Calcule_i_p_blocked(float[] p, float[] i, float dt,int index)
    {
        //float a = p[index];
       /* p[index] = i[index] * Rground;
        i[index] = a / Rground;*/

        //p[index] *= 0.99f;

        i[index] = 0;
    }


    protected virtual void Start()
    {
        success = 1;
        gc = (GameController)GameObject.Find("gameController").GetComponent(typeof(GameController)); //find the game engine

        parameters = transform.GetComponentInParent<PlaygroundParameters>();
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
        SetLocked();
    }

    void SetLocked()
    {
        Transform loc = transform.Find("Locked");
        if (loc)
            loc.gameObject.SetActive(Locked);
    }

    bool dragged;

    bool IsClickable()
    { // Determine if the component is clickable
        if (dragged) return false;

        if (name.Contains("Empty")) return false;

        bool designerMode = GameObject.Find("LevelManager").GetComponent<LevelManager>().designer;
        if (locked && !designerMode) return false;

        return true;
    }

    bool IsLongClickable()
    {
        if (dragged) return false;

        bool designerMode = GameObject.Find("LevelManager").GetComponent<LevelManager>().designer;
        return designerMode;
    }

    bool IsDraggable()
    {
        if (dragged) return false;
        if (itemBeingDragged != null) return false;

        bool designerMode = GameObject.Find("LevelManager").GetComponent<LevelManager>().designer;

        if (locked && !designerMode) return false;

        if (name.Contains("Empty"))
        {
            if (locked && designerMode)
                return true;
            else
                return false;
        }

        if (isFrontiers && !designerMode)
            return false;

        return true;
    }


    float clickStart;
    public void OnPointerDown(PointerEventData eventData)
    {
        clickStart = Time.time;
        Debug.Log(name);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.dragging) return;
         
        if ((Time.time - clickStart) > 0.5f)  // Long click
        {
            if(IsLongClickable())
                OnLongClick();
            else
                audios[1].Play();
        }
        else
        {
            if(IsClickable())
                OnClick();
            else
                audios[1].Play();
        }
    }

    public virtual void OnClick()
    {
        if (!dir_locked && !isFrontiers)
        {
            dir = (dir + 1) % 4;
            
            transform.localRotation = Quaternion.Euler(0, 0, dir * 90);

            audios[0].Play();
        }
        else
        {
            audios[1].Play();
        }
    }

    public GameObject configPanel;
    public virtual void OnLongClick()
    {
        Debug.Log("LongClick");
        if (GameObject.FindGameObjectWithTag("LevelManager").GetComponent<Designer>())
        {
            Debug.Log("Panel");
            //Launch Config Panel
            foreach (ConfigPanel cp in GameObject.FindObjectsOfType<ConfigPanel>())
                cp.Close();

            if (configPanel == null) return;



            GameObject CP = Instantiate(configPanel) ;
            CP.GetComponent<ConfigPanel>().component = this;

            CP.transform.SetParent(GameObject.Find("MainCanvas").transform);
            CP.transform.localScale = Vector3.one;
           
            RectTransform rect = CP.transform.GetComponent<RectTransform>();
            rect.sizeDelta =new Vector2(0,300) ;
            rect.anchoredPosition = new Vector2(0, 0);            

        }
    }
    

    public static Transform startParent;
    public static Transform endParent;

    Transform canvas;
    public static GameObject itemBeingDragged;

    

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsDraggable())
        {
            eventData.pointerDrag = null;
            return;
        }

        itemBeingDragged = gameObject;
        startParent = transform.parent;
        endParent = null;

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


    //public void OnEndDrag(PointerEventData eventData)
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        // Happens after OnDrop

        Drop();
    }

    public void Drop()
    {
        itemBeingDragged = null;
        transform.localScale = Vector3.one;//transform.localScale / 1.2f;

        GetComponent<CanvasGroup>().blocksRaycasts = true;

        if (endParent==null && startParent) //retour au point de départ 
        {
            DestroyImmediate(startParent.GetChild(0).gameObject); //On enlève le composant vide qui a été placé au début du drag 
                                                                  /* Destroyimmediate et pas destroy simple sinon present jusqu'à la fin du frame et populatecomposant fail et le trouve toujours  */
            transform.SetParent(startParent);
        }

        if (endParent && startParent) { // on échange
            if (endParent == startParent)
            {
                DestroyImmediate(startParent.GetChild(0).gameObject); //On enlève le composant vide qui a été placé au début du drag 
                transform.SetParent(endParent);
            }
            else
            {
                DestroyImmediate(startParent.GetChild(0).gameObject); //On enlève le composant vide qui a été placé au début du drag 
                Transform c = endParent.GetChild(0);
                c.SetParent(startParent);
                c.localPosition = Vector3.zero;
                transform.SetParent(endParent);
            }

            transform.SetParent(endParent);
        }

        if (endParent && startParent == null) { //provient du designer on écrase
            DestroyImmediate(endParent.GetChild(0).gameObject);
            transform.SetParent(endParent);
        }

        if (endParent == null && startParent == null) { //Designer + coup dans l'eau
            Destroy(this.gameObject);
            return;
        }

        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;

        dragged = false;
        startParent = endParent = null;

        gc.PopulateComposant();
        audios[1].Play();
    }
}
