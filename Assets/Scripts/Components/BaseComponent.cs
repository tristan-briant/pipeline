using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class BaseComponent : MonoBehaviour, IBeginDragHandler, IDragHandler,
    IEndDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public string PrefabPath="";
    protected float q = 0, f = 0;
    protected float[] qq = { 0, 0, 0, 0 };
    protected float[] ff = { 0, 0, 0, 0 };
    protected GameObject configPanel;
    protected float p0, p1, p2, p3;

    public int dir=0;
    protected float R = 1f, L = 1f, C =1f, Rground = 50;
    protected float fluxMinSound =0.01f;
    protected float[] pin = new float[4];
    protected float[] iin = new float[4];
    [System.NonSerialized] public float success = 1;
    protected float fail = 0;
    protected const float fMinBubble = 0.05f;
    private float pressure;
    public float Pressure { get => pressure; set => pressure = value; }
    public bool destroyable=true;
    public bool isSuccess = false;
    public bool locked = false;
    public bool Locked { get => locked; set { locked = value; SetLocked(); } }
    public bool dir_locked = false;
    public bool mirror = false;
    public bool isFrontiers=false;

    public bool trigged=false;   // for composant that can be trigged. use with tag Triggerable

    protected GameController gc; // le moteur du jeu à invoquer parfois
    protected AudioSource[] audios;
    PlaygroundParameters parameters;

    public virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        success = 1;
        gc = (GameController)GameObject.Find("GameController").GetComponent(typeof(GameController)); //find the game engine

        parameters = transform.GetComponentInParent<PlaygroundParameters>();
        audios = GameObject.Find("PlaygroundHolder").GetComponents<AudioSource>();

        if (parameters != null)
        {
            R = parameters.R;
            C = parameters.C;
            L = parameters.L;
            Rground = parameters.Rground;
        }
        Rotate();
        SetLocked();
    }

    protected Color PressureColor(float p)
    { // donne la convention de pression
        float PMAX = 1.0f;

        Color max = new Color(0.3f, .80f, 0.80f);  // p=2
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

    public void Clamp_i_p(float[] p, float[] i)
    {
        for (int k = 0; k < 4; k++)
        {
            p[k] = Mathf.Clamp(p[k], -10, 10);
            i[k] = Mathf.Clamp(i[k], -10, 10);

        }
    }

    public virtual void Constraint(float[] p, float[] i, float dt)
    {
        // Put constraint here as i blocked or p imposed
        for (int k = 0; k < 4; k++)
        {
            Calcule_i_p_blocked(p, i, dt, k);
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

    public virtual void Calcule_i_p_blocked(float[] p, float[] i, float dt, int index)
    {
        i[index] = 0;
    }

    void ToggleLocked()
    {
        Locked = !Locked;
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

        bool designerMode = GameObject.Find("LevelManager").GetComponent<LevelManager>().designerMode;
        if (locked && !designerMode) return false;

        return true;
    }

    bool IsLongClickable()
    {
        if (dragged) return false;

        bool designerMode = GameObject.Find("LevelManager").GetComponent<LevelManager>().designerMode;
        return designerMode;
    }

    bool IsDraggable()
    {
        if (dragged) return false;
        if (itemBeingDragged != null) return false;

        bool designerMode = GameObject.Find("LevelManager").GetComponent<LevelManager>().designerMode;

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

    bool IsDestroyable()
    {
        bool designerMode = GameObject.Find("LevelManager").GetComponent<LevelManager>().designerMode;

        if (designerMode) return true;

        if (locked) return false;

        if (name.Contains("Empty")) return true;
       
        return destroyable;
    }

    bool IsEmpty()
    {
        if (name.Contains("Empty") && locked == false)
            return true;

        return false;
    }

    bool IsMovable()
    {
        bool designerMode = GameObject.Find("LevelManager").GetComponent<LevelManager>().designerMode;

        if (designerMode)
            return true;
        else
            return !locked;

    }

    public virtual void Rotate() 
    {
        transform.localRotation = Quaternion.Euler(0, 0, dir * 90);
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
         
        if ((Time.time - clickStart) > 0.25f)  // Long click
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

            Rotate();

            audios[0].Play();
        }
        else
        {
            audios[1].Play();
        }
    }

    public virtual void OnLongClick()
    {
        //Launch Config Panel
        foreach (ConfigPanel cp in FindObjectsOfType<ConfigPanel>())
            cp.Close();

        if (configPanel == null) return;

        GameObject CP = Instantiate(configPanel, GameObject.Find("CanvasConfig").transform);
        CP.GetComponent<ConfigPanel>().component = this;

        CP.transform.localScale = Vector3.one;
        CP.transform.localPosition = Vector3.zero;

        RectTransform rect = CP.transform.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, 300);
        rect.anchoredPosition = new Vector2(0, 0);

    }
    
    public static Transform startParent;
    public static Transform endParent;

    //Transform canvas;
    public static GameObject itemBeingDragged;

    public virtual void BlockCurrant()
    {
        f = 0;
    }

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

        //GetComponent<CanvasGroup >().blocksRaycasts = false;

        dragged = true;

        ChangeParent(GameObject.Find("CanvasDragged").transform);

        gc.PopulateComposant();

        transform.localScale = transform.localScale * 1.2f;

        BlockCurrant();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 vec = Input.mousePosition;
        //vec= Camera.main.ScreenToWorldPoint(vec);
        vec.z = 1.0f;
        transform.position = Camera.main.ScreenToWorldPoint(vec);
        //transform.localPosition = vec;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        // Happens after OnDrop
        Drop();
    }

    public void Drop()
    {
        itemBeingDragged = null;
        transform.localScale = Vector3.one;

        if (endParent == null && startParent) //retour au point de départ 
        {
            StartCoroutine(FlightToFinalPosition(startParent));
        }

        if (endParent && startParent)
        { // on échange
            if (endParent == startParent)
            {
                StartCoroutine(FlightToFinalPosition(endParent,0.05f));
            }
            else
            {
                Transform c = endParent.GetChild(1);
                if (c.GetComponent<BaseComponent>().IsEmpty())
                {
                    StartCoroutine(FlightToFinalPosition(endParent,0.05f));
                }
                else
                {
                    if (c.GetComponent<BaseComponent>().IsMovable())
                    {
                      
                        c.GetComponent<BaseComponent>().ChangeParent(GameObject.Find("CanvasDragged").transform);
                        gc.PopulateComposant();

                        StartCoroutine(c.GetComponent<BaseComponent>().FlightToFinalPosition(startParent));

                        StartCoroutine(FlightToFinalPosition(endParent, 0.05f));

                    }
                    else // retour à la case départ
                    {
                        StartCoroutine(FlightToFinalPosition(startParent));
                    }
                }

            }
        }

        if (endParent && startParent == null) { //provient du designer
            if (name.Contains("Rock"))
            {
                endParent.GetChild(1).GetComponent<BaseComponent>().ToggleLocked();
                Destroy(gameObject);
            }
            else
            {
                if (endParent.GetChild(1).GetComponent<BaseComponent>().IsDestroyable()) //erase if possible
                {
                    StartCoroutine(FlightToFinalPosition(endParent, 0.05f));
                }
                else
                {
                    Destroy(gameObject); // if not delete self
                }
            }
        }

        if (endParent == null && startParent == null) { //Designer + coup dans l'eau
            Destroy(gameObject);
            return;
        }

        //transform.localPosition = Vector3.zero;
        //transform.localScale = Vector3.one;

        dragged = false;
        startParent = endParent = null;

        //gc.PopulateComposant();
        //audios[1].Play();
    }

    protected float SpeedAnim()
    {
        if (!float.IsNaN(f))
            return Mathf.Atan(f) / fMinBubble;
        else
            return 0;
    }

    protected float SpeedAnim(float flux)
    {
        if (!float.IsNaN(flux))
            return Mathf.Atan(flux) / fMinBubble;
        else
            return 0;
    }

    protected float SpeedAnim(float f1, float f2)
    {
        float flux;
        if ((f1 > 0 && f2 > 0) || (f1 < 0 && f2 < 0))
            flux = 0;
        else
        {
            if (Mathf.Abs(f1) < Mathf.Abs(f2))
                flux = f1;
            else
                flux = -f2;
        }
        if (!float.IsNaN(flux))
            return Mathf.Atan(flux) / fMinBubble;
        else
            return 0;
    }

    public void ChangeParent(Transform newParent) // set new parent and change sorting layer
    {
        transform.SetParent(newParent);
        Canvas canvasParent = newParent.GetComponentInParent<Canvas>();
        if (canvasParent)
        {
            foreach (Canvas c in GetComponentsInChildren<Canvas>())
                c.sortingLayerName = canvasParent.sortingLayerName;
        }

        //transform.localPosition = Vector3.zero;
        //transform.localScale = Vector3.one;
       
    }

    public IEnumerator FlightToFinalPosition(Transform newParent,float flightTime=0.2f, bool cleanNewParent=true)
    {
        Vector3 initialPosition = transform.position;
        Vector3 finalPosition = newParent.position;

        Debug.Log(transform.position);
        Debug.Log(newParent.position);

        float t = 0;

        while (t<flightTime)
        {
            transform.position = (initialPosition * (flightTime - t) + finalPosition * t) / flightTime;
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        

        transform.SetParent(newParent);
        Canvas canvasParent = newParent.GetComponentInParent<Canvas>();
        if (canvasParent)
        {
            foreach (Canvas c in GetComponentsInChildren<Canvas>())
                c.sortingLayerName = canvasParent.sortingLayerName;
        }


        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;

        if (cleanNewParent)
            DestroyImmediate(newParent.GetChild(1).gameObject);

        transform.SetParent(newParent);

        gc.PopulateComposant();
        audios[1].Play();
    }
}
