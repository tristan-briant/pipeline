using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CrossManager : BaseComponent
{

    GameObject water, water0, water1, water2, water3, bubble0, bubble1, bubble2, bubble3;
    float x_bulle0 = 0.5f, x_bulle1 = 0.5f, x_bulle2 = 0.5f, x_bulle3 = 0.5f;
    float f0, f1, f2, f3;
    float i0 = 0, i1 = 0, i2 = 0, i3 = 0;

    /*new public float R = 2;
    new public float L = 2;
    new public float C = 2;*/

    float r_bulle = 0.1f;
 

    public override void Reset_i_p()
    {
        base.Reset_i_p();
        i0 = i1 = i2 = i3 = f0 = f1 = f2 = f3 = 0;
    }

    public override void Calcule_i_p(float[] p, float[] i, float alpha)
    {

        p0 = p[0];
        p1 = p[1];
        p2 = p[2];
        p3 = p[3];


        i0 = i[0]; x_bulle0 -= 0.05f * i0; //// pour le dessin
        i1 = i[1]; x_bulle1 += 0.05f * i1; //// pour le dessin
        i2 = i[2]; x_bulle2 += 0.05f * i2;
        i3 = i[3]; x_bulle3 -= 0.05f * i3;

        q += (i[0] + i[1] + i[2] + i[3]) * alpha; //q*=0.99;

        f0 += (p[0] - q) / L * alpha;
        f1 += (p[1] - q) / L * alpha;
        f2 += (p[2] - q) / L * alpha;
        f3 += (p[3] - q) / L * alpha;

        p[0] = (q / C + (i[0] - f0 ) * R);
        p[1] = (q / C + (i[1] - f1 ) * R);
        p[2] = (q / C + (i[2] - f2 ) * R);
        p[3] = (q / C + (i[3] - f3 ) * R);

        i[0] = (f0  + (p0 - q / C) / R);
        i[1] = (f1  + (p1 - q / C) / R);
        i[2] = (f2  + (p2 - q / C) / R);
        i[3] = (f3  + (p3 - q / C) / R);

        Pressure = Mathf.Clamp(0.125f * (p[0] + p[1] + p[2] + p[3]), -1f, 1f); ;

    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
    }

    protected override void Start()
    {
        base.Start();
        water = this.transform.Find("Water").gameObject;
        water0 = this.transform.Find("Water0").gameObject;
        water1 = this.transform.Find("Water1").gameObject;
        water2 = this.transform.Find("Water2").gameObject;
        water3 = this.transform.Find("Water3").gameObject;

        bubble0 = this.transform.Find("Bubble0").gameObject;
        bubble1 = this.transform.Find("Bubble1").gameObject;
        bubble2 = this.transform.Find("Bubble2").gameObject;
        bubble3 = this.transform.Find("Bubble3").gameObject;

    }

    private void Update()
    {
        water.GetComponent<Image>().color = PressureColor(q);
        water0.GetComponent<Image>().color = PressureColor(p0);
        water1.GetComponent<Image>().color = PressureColor(p1);
        water2.GetComponent<Image>().color = PressureColor(p2);
        water3.GetComponent<Image>().color = PressureColor(p3);

        float xmax = 0.5f - r_bulle;
        //////////BUBBLE 0
        if (Mathf.Abs(i0) > fMinBubble)
        {
           
            if (x_bulle0 > xmax) x_bulle0 = -xmax;
            if (x_bulle0 < -xmax) x_bulle0 = xmax;


            if (x_bulle0 > 0)
            {
                bubble0.transform.localPosition = new Vector3( x_bulle0 * 100, 0, 0);
                bubble0.SetActive(true);
            }
            else bubble0.SetActive(false);
        }
        else
        {
            bubble0.SetActive(false);
        }

        //////////BUBBLE 1
        if (Mathf.Abs(i1) > fMinBubble)
        {
            if (x_bulle1 > xmax) x_bulle1 = -xmax;
            if (x_bulle1 < -xmax) x_bulle1 = xmax;


            if (x_bulle1 < 0)
            {
                bubble1.transform.localPosition = new Vector3(0, -x_bulle1 * 100, 0);
                bubble1.SetActive(true);
            }
            else bubble1.SetActive(false);
        }
        else
        {
            bubble1.SetActive(false);
        }

        //////////BUBBLE 2
        if (Mathf.Abs(i2) > fMinBubble)
        {
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
            if (x_bulle3 > xmax) x_bulle3 = -xmax;
            if (x_bulle3 < -xmax) x_bulle3 = xmax;


            if (x_bulle3 > 0)
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

