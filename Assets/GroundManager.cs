﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroundManager : BaseFrontier
{
    public float pset;
    public float Pset
    {
        get
        {
            return pset;
        }

        set
        {
            pset = value;
        }
    }

    public float imax = 0.1f;
    public float Imax
    {
        get
        {
            return imax;
        }

        set
        {
            imax = value;
        }
    }

    
    //float ii = 0;
    GameObject water, water0, arrow;
    //public bool isSuccess = false;
    public bool jelly = false;
    Color jellyColor = new Color(0xFF / 255.0f, 0x42 / 255.0f, 0x6A / 255.0f);
    Color jellyColorBg = new Color(0x42 / 255.0f, 0x42 / 255.0f, 0x42 / 255.0f);
    public int mode = 0;
    public float periode = 2;



    public override void Calcule_i_p(float[] p, float[] i, float alpha)
    {
      
    }

    float pp = 0;
    public override void Constraint(float[] p, float[] i, float dt)
    {
        if (Mathf.Abs(i[0]) < imax)
        {
            pp = 0.01f * pset + 0.99f * pp;
            p[0] = pp;
            //ii = 0.01f * i[0] + 0.99f * ii;
        }
        else
        {
            pp= 0.01f * p[0] + 0.99f * pp;
            //ii = 0.01f * 0 + 0.99f * ii;
            //i[0] = ii;
            i[0] = Mathf.Clamp(i[0], -imax, imax);
        }
    }

    protected override void Start()
    {
        base.Start();
        water = this.transform.Find("Water").gameObject;
        water0 = this.transform.Find("Water0").gameObject;

        arrow = this.transform.Find("Arrow").gameObject;

    }


    private void Update()
    {
        water.GetComponent<Image>().color = PressureColor(pset);
        water0.GetComponent<Image>().color = PressureColor(pin[0]);
    }


}
