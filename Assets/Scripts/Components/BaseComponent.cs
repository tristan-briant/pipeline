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
        }
    }

    public bool dir_locked = false;
    public bool mirror = false;
    public bool isFrontiers=false;

    public bool trigged=false;   // for composant that can be trigged. use with tag Triggerable

    protected gameController gc; // le moteur du jeu à invoquer parfois
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
        //float a = p[index];
       /* p[index] = i[index] * Rground;
        i[index] = a / Rground;*/

        //p[index] *= 0.99f;

        i[index] = 0;
    }


    protected virtual void Start()
    {
        success = 1;
        gc = (gameController)GameObject.Find("gameController").GetComponent(typeof(gameController)); //find the game engine
        //parameters = (PlaygroundParameters)transform.parent.transform.parent.GetComponent(typeof(PlaygroundParameters)); //find the game engine

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
    }

    private void Update()
    {
        
        //c += 0.01f;
        //if (c > 1) c = 0.0f;
        //water.GetComponent<Image>().color = new Color(1, c, 1);
    }


    bool dragged;

    float clickStart;
    public void OnPointerDown(PointerEventData eventData)
    {
        clickStart = Time.time;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.dragging) return;
         
        if ((Time.time - clickStart) > 0.5f)  // Long click
        {
            OnLongClick();
        }
        else
        {
            OnClick();
        }


    }
    public virtual void OnClick()
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

        if (!dragged && (locked || dir_locked))
        {
            audios[1].Play();
        }
    }

    public GameObject ConfigPanel;
    public virtual void OnLongClick()
    {
        if (GameObject.FindGameObjectWithTag("LevelManager").GetComponent<Designer>())
        {
            //Launch Config Panel
            foreach (ConfigPanel cp in GameObject.FindObjectsOfType<ConfigPanel>())
                cp.Close();

            if (ConfigPanel == null) return;



            GameObject CP = Instantiate(ConfigPanel) ;
            CP.GetComponent<ConfigPanel>().component = this;

            CP.transform.SetParent(GameObject.Find("MainCanvas").transform);
            CP.transform.localScale = Vector3.one;
           
            RectTransform rect = CP.transform.GetComponent<RectTransform>();
            rect.sizeDelta =new Vector2(0,300) ;
            rect.anchoredPosition = new Vector2(0, 0);            

        }
    }

    //Vector3 startPos;
    //float guiDepth;
    public static Transform startParent;
    public static Transform endParent;

    Transform canvas;
    public static GameObject itemBeingDragged;

    

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemBeingDragged != null) { eventData.pointerDrag = null; return; } // If one element already draged, cancel the drag

        if (locked && !GameObject.FindGameObjectWithTag("LevelManager").GetComponent<Designer>())
        {// If locked and not in desgner mode cancel the drag
            eventData.pointerDrag = null; return;
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
