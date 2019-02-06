using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TeManager : BaseComponent
{

    GameObject water0, water2, water3, bubble0, bubble2, bubble3;
    float x_bulle1 = 0.5f, x_bulle2 = 0.5f, x_bulle3 = 0.5f;
    float f1, f2, f3;
    float i1 = 0, i2 = 0, i3 = 0;

    //public float x_bulle = 0;
    float r_bulle = 0.1f;
    //float f=0;
    //float[] pin = new float[4];

    public override void Reset_i_p()
    {
        base.Reset_i_p();
        i1 = i2 = i3 = f1 = f2 = f3 = 0;
    }

    public override void Calcule_i_p(float[] p, float[] i,float alpha)
    {

        float a = p[0], b = p[2], c = p[3];

        i1 = i[0]; x_bulle1 -= 0.05f * i1; //// pour le dessin
        i2 = i[2]; x_bulle2 += 0.05f * i2;
        i3 = i[3]; x_bulle3 -= 0.05f * i3;

        q += (i[0] + i[2] + i[3]) * alpha; //q*=0.99;

        f1 += (p[0] - p[2]) / L * alpha;
        f2 += (p[2] - p[3]) / L * alpha;
        f3 += (p[3] - p[0]) / L * alpha;

        p[0] = (q / C + (i[0] - f1 + f3) * R);
        p[2] = (q / C + (i[2] - f2 + f1) * R);
        p[3] = (q / C + (i[3] - f3 + f2) * R);

        i[0] = (f1 - f3 + (a - q / C) / R);
        i[2] = (f2 - f1 + (b - q / C) / R);
        i[3] = (f3 - f2 + (c - q / C) / R);

        Calcule_i_p_blocked(p, i, alpha, 1);

        pressure = Mathf.Clamp(0.5f * q, -1f, 1f); ;

    }

    protected override void Start()
    {
        base.Start();
        water0 = this.transform.Find("Water0").gameObject;
        water2 = this.transform.Find("Water2").gameObject;
        water3 = this.transform.Find("Water3").gameObject;

        bubble0 = this.transform.Find("Bubble0").gameObject;
        bubble2 = this.transform.Find("Bubble2").gameObject;
        bubble3 = this.transform.Find("Bubble3").gameObject;

    }

    private void Update()
    {
        water0.GetComponent<Image>().color = PressureColor(pin[0]);
        water2.GetComponent<Image>().color = PressureColor(pin[2]);
        water3.GetComponent<Image>().color = PressureColor(pin[3]);


        //////////BUBBLE 0
        if (Mathf.Abs(i1) > fMinBubble)
        {
            float xmax = 0.5f - r_bulle;
            if (x_bulle1 > xmax) x_bulle1 = -xmax;
            if (x_bulle1 < -xmax) x_bulle1 = xmax;


            if (x_bulle1 > 0)
            {
                bubble0.transform.localPosition = new Vector3(x_bulle1 * 100, 0, 0);
                bubble0.SetActive(true);
            }
            else bubble0.SetActive(false);
        }
        else
        {
            bubble0.SetActive(false);
        }

        //////////BUBBLE 2
        if (Mathf.Abs(i2) > fMinBubble)
        {
            float xmax = 0.5f - r_bulle;
            if (x_bulle2 > xmax) x_bulle2 = -xmax;
            if (x_bulle2 < -xmax) x_bulle2 = xmax;


            if (x_bulle2 < 0)
            {
                bubble2.transform.localPosition = new Vector3(x_bulle2 * 100, 0, 0);
                bubble2.SetActive(true);
            }
            else bubble2.SetActive(false);
        }
        else
        {
            bubble2.SetActive(false);
        }

        //////////BUBBLE 3
        if (Mathf.Abs(i3) > fMinBubble)
        {
            float xmax = 0.5f - r_bulle;
            if (x_bulle3 > xmax) x_bulle3 = -xmax;
            if (x_bulle3 < -xmax) x_bulle3 = xmax;


            if (x_bulle3 > 0.1)
            {
                bubble3.transform.localPosition = new Vector3(0,-x_bulle3 * 100, 0);
                bubble3.SetActive(true);
            }
            else bubble3.SetActive(false);
        }
        else
        {
            bubble3.SetActive(false);
        }

    }

}

