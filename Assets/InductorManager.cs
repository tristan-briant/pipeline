using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InductorManager : BaseComponent
{

    GameObject water, water0, water2, bubble, propeller;
    //public float x_bulle = 0;
    public float Lin = 10;
    float r_bulle = 0.1f;
    float angle;


    public override void calcule_i_p(float[] p, float[] i, float alpha)
    {
        R = 80;//C = 2;
        float a = p[0], b = p[2];

        q += (i[0] + i[2]) * alpha;
        f += (p[0] - p[2]) / Lin * alpha;

        p[0] = (q / C + (i[0] - f) * R);
        p[2] = (q / C + (i[2] + f) * R);

        i[0] = (f + (a - q / C) / R);
        i[2] = (-f + (b - q / C) / R);

        //i[1]=i[3]=0;
        i[1] = p[1] / Rground;
        i[3] = p[3] / Rground;

        angle -= 0.05f * f;

    }

    protected override void Start()
    {
        base.Start();
        water0 = this.transform.FindChild("Water0").gameObject;
        water2 = this.transform.FindChild("Water2").gameObject;
        water = this.transform.FindChild("Water").gameObject;

        propeller = this.transform.FindChild("Propeller").gameObject;
        //bubble = this.transform.FindChild("Bubble").gameObject;

    }

    private void Update()
    {
        water.GetComponent<Image>().color = pressureColor(q/C);
        water0.GetComponent<Image>().color = pressureColor(pin[0]);
        water2.GetComponent<Image>().color = pressureColor(pin[2]);

        
        propeller.transform.localEulerAngles = new Vector3(0, 0, angle*300);


    }

}

