﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PipelineJellyManager : BaseComponent {

    GameObject jelly0, jelly2,bubble;
    public float x_bulle = 0;
    float r_bulle = 0.1f;
    public float Capa = 1.0f;
    bool full = false;
    bool entered = false;

    public override void calcule_i_p(float[] p, float[] i)
    {

       
        q += (i[0] + i[2]) / Capa;

        if (q > 0 && !entered)
        {
            locked = true; //Once the jelly enter the pipe, the pipe cannot be moved
            entered = true;
            if (p[0] > 0) f = 1;
            else f = -1; 
        }

        if (q >= 1 && !full) {             //pipe is full
            full = true;
        }

        if(f > 0 && q>=1)
        {
            p[2] = 1;
            i[2] = -1;
        }
        if (f < 0 && q >= 1)
        {
            p[0] = 1;
            i[0] = -1;
        }


        //i[1]=i[3]=0;
        i[1] = p[1] / Rground;
        i[3] = p[3] / Rground;

        x_bulle -= 0.01f * f*100/Capa;

        if (q > 1 + 2 / Capa)
            fail = 1;
    }

    protected override void Start()
    {
        base.Start();
        jelly0 = this.transform.FindChild("Jelly0").gameObject;
        jelly2 = this.transform.FindChild("Jelly2").gameObject;

        bubble = this.transform.FindChild("Bubble").gameObject;
        jelly0.GetComponent<Image>().fillAmount = 0;
        jelly2.GetComponent<Image>().fillAmount = 0;
    }

    private void Update()
    {
        if(f>0) jelly0.GetComponent<Image>().fillAmount = q;
        if (f < 0) jelly2.GetComponent<Image>().fillAmount = q;

        if (q > 1 + 2 / Capa)
        {
            jelly0.GetComponent<Image>().color = new Color(255, 255, 255);
            jelly2.GetComponent<Image>().color = new Color(255, 255, 255);
        }

        if ( full && Mathf.Abs(f) > 0.01f)
        {
            
            float x_max = 0.5f - r_bulle;

            if (x_bulle > x_max) x_bulle = -x_max;
            if (x_bulle < -x_max) x_bulle = x_max;


            bubble.transform.localPosition = new Vector3(x_bulle * 100, 0, 0);
            bubble.SetActive(true);
        }
        else
        {
            bubble.SetActive(false);
        }

    }
}
