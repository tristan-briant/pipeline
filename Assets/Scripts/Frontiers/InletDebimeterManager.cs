﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InletDebimeterManager : BaseFrontier
{

    public float pset = 1;
    public float Pset
    {
        get
        {
            return pset;
        }

        set
        {
            pset = value;
            UpdateValue();
        }
    }

    public float imax = 0.25f;
    public float Imax
    {
        get
        {
            return imax;
        }

        set
        {
            imax = value;
            UpdateValue();
        }
    }


    GameObject water =null, water0=null, arrow=null, bubble=null, value, timer;
    GameObject[] red = new GameObject[4];
    GameObject[] green = new GameObject[4];



    public override void Awake()
    {
        configPanel = Resources.Load("ConfigPanel/ConfigInlet") as GameObject;
        GetComponent<Animator>().SetFloat("rate", 0.0f);
        UpdateValue();
    }

    public override void Rotate()
    {
        Transform text = transform.Find("ValueHolder/Value");

        TextAnchor align =0;
        switch (dir)
        {
            case 0: align = TextAnchor.MiddleLeft; break;
            case 1: if (pset > 0) align = TextAnchor.MiddleLeft; else align = TextAnchor.MiddleRight; break;
            case 3: if (pset > 0) align = TextAnchor.MiddleRight; else align = TextAnchor.MiddleLeft; break;
            case 2: align = TextAnchor.MiddleRight; break;
        }
        text.GetComponent<Text>().alignment = align;

        if (dir == 1)
            text.localRotation = Quaternion.Euler(0, 0, 0);
        else
            text.localRotation = Quaternion.Euler(0, 0, 90f * dir + 90f);

        float s = Mathf.Abs(text.localScale.x);
        if (dir == 3)
            text.localScale = new Vector3(-s, s, s);
        else
            text.localScale = new Vector3(s, s, s);

    }

    float ppset;
    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {

        ppset = 0.95f * ppset + 0.05f * (pset + 2 * R * f);
        

        p0 = p[0];
        if(pset>0)
            f = Mathf.Clamp((ppset - q) / R, 0, imax); 
        else
            f = Mathf.Clamp((ppset - q) / R, -imax, 0);
        
        q += (i[0] + f) / C * dt;

        p[0] = q + i[0] * R;
        i[0] = (p0 - q) / R;

    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
    }

    public void UpdateValue()
    {
        GetComponent<Animator>().SetFloat("max", Mathf.Clamp(imax, 0, 0.99f));
        GetComponentInChildren<Text>().text = (Mathf.Round(20 * imax) / 20).ToString();

        Transform cadran = transform.Find("CadranHolder");
        if (pset > 0)
            cadran.localScale = Vector3.one;
        else
            cadran.localScale = new Vector3(-1,1,1);

        Rotate();
    }

    protected override void Start()
    {
        base.Start();
        if (transform.Find("Water"))
            water = transform.Find("Water").gameObject;
        if (transform.Find("Water0"))
            water0 = transform.Find("Water0").gameObject;

        if (transform.Find("Bubble"))
            bubble = transform.Find("Bubble").gameObject;

 
        configPanel = Resources.Load("ConfigPanel/ConfigInletDebimeter") as GameObject;
        UpdateValue();

        R = 0.5f;
    }

    
    private void Update()
    {

        water.GetComponent<Image>().color = PressureColor(ppset);
        water0.GetComponent<Image>().color = PressureColor(p0);

        bubble.GetComponent<Animator>().SetFloat("speed", SpeedAnim());

        if(pset>0)
            GetComponent<Animator>().SetFloat("rate", Mathf.Clamp(f, 0, 0.999f));
        else
            GetComponent<Animator>().SetFloat("rate", Mathf.Clamp(-f, 0, 0.999f));
        


        if (Mathf.Abs(f) > fMinBubble)
        {

            if (!audios[3].isPlaying && !audios[4].isPlaying && !audios[5].isPlaying)
            {
                int r = Random.Range(0, 3);
                audios[3 + r].Play();
            }
            audios[3].volume = audios[4].volume = audios[5].volume = Mathf.Abs(f) / fMinBubble * 0.1f;

        }
    }

}