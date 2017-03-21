using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Pipeline : BaseComponent { 

    GameObject water0,water2,bubble;
    public float x_bulle = 0;
    float r_bulle=0.1f;
 

    public override void calcule_i_p(float[] p, float[] i)
    {

        float a = p[0], b = p[2];

        q += (i[0] + i[2]) / C; 
        f += (p[0] - p[2]) / L;

        p[0] = (q + (i[0] - f) * R);
        p[2] = (q + (i[2] + f) * R);

        i[0] = (f + (a - q) / R);
        i[2] = (-f + (b - q) / R);

        //i[1]=i[3]=0;
        i[1] = p[1] / Rground;
        i[3] = p[3] / Rground;

        x_bulle -= 0.05f * f;

    }

    protected override void Start()
    {
        base.Start();
        water0 = this.transform.FindChild("Water0").gameObject;
        water2 = this.transform.FindChild("Water2").gameObject;

        bubble = this.transform.FindChild("Bubble").gameObject;

    }

    private void Update()
    {
        water0.GetComponent<Image>().color = pressureColor(pin[0]);
        water2.GetComponent<Image>().color = pressureColor(pin[2]);

        if (Mathf.Abs(f) > 0.01f)
        {
            //if (x_bulle < -0.5f + d_bulle * 0.5f) { x_bulle = 0.5f - d_bulle * 0.5f; }
            //if (x_bulle > 0.5f - d_bulle * 0.5f) { x_bulle = -0.5f + d_bulle * 0.5f; }

            float x_max = 0.5f - r_bulle;

            if (x_bulle > x_max) x_bulle = -x_max;
            if (x_bulle < -x_max) x_bulle = x_max;


            bubble.transform.localPosition =new Vector3(x_bulle*100,0,0);
            bubble.SetActive(true);
        }
        else
        {
            bubble.SetActive(false);
        }

    }

}

