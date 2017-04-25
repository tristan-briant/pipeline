using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransistorManager : BaseComponent
{

    GameObject water, water1, water2, water3, bubble1, bubble2, valve;
    GameObject piston;
    public float x_bulle2=0,x_bulle1 = 0;
    float r_bulle = 0.1f;
    float q1, q2, q3;
    public float xp;
    public float Gain = 10;
    float g, f2 = 0, f13 = 0;
    float threshold = 0.02f;
    float fthreshold = 0.05f;
    public bool NPN = true;
    float Rin=100;
    float q12 = 0, q13 = 0;
    //public bool mirror=false;


    public override void calcule_i_p(float[] p, float[] i, float alpha)
    {
        /* C = 0.3f;
         R = 0.1f;*/
        if (mirror) {
            float e = p[0];
            p[0] = p[2];
            p[2] = e;
            e = i[0];
            i[0] = i[2];
            i[2] = e;
        }

        float a = p[1], b = p[2], c=p[3];

        q1 += (i[1]) * alpha;
        q3 += (i[3]) * alpha;
        q2 += (i[2]) * alpha;

        if (NPN)
        {
            g = q2 - q3;

            if (g > 0)
            {
                q = (q2 + q3) / 2;
                q2 = q;
                q3 = q;
                f2 += (p[2] - p[3]) / L * alpha;
            }
            else
            {
                f2 = 0;
            }

            if (g > threshold)
            {
                q = q1;
                q1 = 0.9f * q1 + 0.1f * q3;
                q3 = 0.9f * q3 + 0.1f * q;

                f13 += (p[1] - p[3]) / L * alpha;
            }
            else
            {
                f13 = 0;
            }

            f13 = Mathf.Clamp(f13, 0, Gain * f2);

            p[1] = (q1 / C + (i[1] - f13) * R);
            p[2] = (q2 / C + (i[2] - f2 ) * R);
            p[3] = (q3 / C + (i[3] + f13 + f2) * R);

            i[1] = (f13 + (a - q1 / C) / R);
            i[2] = (f2 + (b - q2 / C) / R);
            i[3] = (-f2 - f13 + (c - q3 / C) / R);
        }
        else
        {
            g = q1 - q2;

            if (g > 0) 
            {
                q = (q1 + q2) / 2;
                q1 = q;
                q2 = q;
                f2 += (p[1] - p[2] ) / L * alpha;
            }
            else
            {
                f2 = 0;
            }

            if (g>threshold) {
                q = q1;
                q1 = 0.9f * q1 + 0.1f * q3;
                q3 = 0.9f * q3 + 0.1f * q;

                f13 += (p[1] - p[3] ) / L * alpha;
            } 
            else
            {
                f13 = 0;
            }

            f13 = Mathf.Clamp(f13, 0 , Gain * f2);

            p[1] = (q1 / C + (i[1] - f2 - f13) * R);
            p[2] = (q2 / C + (i[2] + f2 ) * R);
            p[3] = (q3 / C + (i[3] + f13 ) * R);

            i[1] = ( f13 + f2 + (a - q1 / C) / R);
            i[2] = ( - f2 + (b - q2 / C) / R);
            i[3] = (-f13  + (c - q3 / C) / R);

        }
           
        calcule_i_p_blocked(p, i, alpha, 0);

        const float r = 0.1f;

        if (NPN)
        {
            x_bulle2 += 0.05f * f2;
            x_bulle1 += 0.05f * f13;
            xp = (1 - r) * xp + r * Mathf.Clamp(+f2, 0, 0.1f);
        }
        else
        {
            x_bulle2 += 0.05f * f2;
            x_bulle1 += 0.05f * f13;
            if (g > threshold)
                xp = (1 - r) * xp + r * Mathf.Clamp(+f2, 0, 0.1f);
            else
                xp = (1 - r) * xp;
        }

        if (mirror)
        {
            float e = p[0];
            p[0] = p[2];
            p[2] = e;
            e = i[0];
            i[0] = i[2];
            i[2] = e;
        }


    }

    protected override void Start()
    {
        base.Start();
        water = this.transform.FindChild("Water").gameObject;
        water1 = this.transform.FindChild("Water1").gameObject;
        water2 = this.transform.FindChild("Water2").gameObject;
        water3 = this.transform.FindChild("Water3").gameObject;
        piston = transform.FindChild("Piston").gameObject;
        bubble1 = transform.FindChild("Bubble1").gameObject;
        bubble2 = transform.FindChild("Bubble2").gameObject;
        valve = transform.FindChild("Valve").gameObject;

    }

    private void Update()
    {
        water1.GetComponent<Image>().color = pressureColor(pin[1]);
        water2.GetComponent<Image>().color = pressureColor(pin[2]);
        water3.GetComponent<Image>().color = pressureColor(pin[3]);

        /*if (mirror)
            transform.localScale = new Vector3(-1, 1, 1);*/

        /* piston.transform.rotation = Quaternion.identity;
         piston.transform.Rotate(new Vector3(0, 0, xp * 180));*/
        piston.transform.localEulerAngles = new Vector3(0, 0, xp * 180);
        valve.transform.localEulerAngles = new Vector3(0, 0, 5 * xp * 180);

        if (xp * 180 > 5) {
            water.SetActive(true);
            water.GetComponent<Image>().color = pressureColor(0.5f * (pin[1] + pin[3]));
        }
        else
        {
            water.SetActive(false);
        }

        if (NPN)
        {
            if (Mathf.Abs(f2) > fMinBubble)
            {
                 float x_max = 0.5f - r_bulle;

                if (x_bulle2 > x_max) x_bulle2 = -x_max;
                if (x_bulle2 < -x_max) x_bulle2 = x_max;

                float scale = Mathf.Max(Mathf.Abs(2 * x_bulle2), 0.5f);
                bubble2.transform.localScale = new Vector3(scale, scale, 1);
                bubble2.transform.localPosition = new Vector3((Mathf.Cos((x_bulle2 - 0.5f) * Mathf.PI / 2) * 0.5f - 0.5f) * 100, (-Mathf.Sin((x_bulle2 - 0.5f) * Mathf.PI / 2) * 0.5f - 0.5f) * 100, 0);

                bubble2.SetActive(true);
            }
            else
            {
                bubble2.SetActive(false);
            }
            if (Mathf.Abs(f13) > fMinBubble)
            {

                float x_max = 0.5f - r_bulle;

                if (x_bulle1 > x_max) x_bulle1 = -x_max;
                if (x_bulle1 < -x_max) x_bulle1 = x_max;

                bubble1.transform.localPosition = new Vector3((1 + Mathf.Cos(2 * 3.14159f * x_bulle1)) * 5, -x_bulle1 * 100, 0);
                bubble1.SetActive(true);
            }
            else
            {
                bubble1.SetActive(false);
            }
        }

        if (!NPN)
        {
            if (Mathf.Abs(f2) > fMinBubble)
            {

                float x_max = 0.5f - r_bulle;

                if (x_bulle2 > x_max) x_bulle2 = -x_max;
                if (x_bulle2 < -x_max) x_bulle2 = x_max;

                float scale = Mathf.Max(Mathf.Abs(2 * x_bulle2), 0.5f);
                bubble2.transform.localScale = new Vector3(scale, scale, 1);
                bubble2.transform.localPosition = new Vector3((Mathf.Cos((-x_bulle2 - 0.5f) * Mathf.PI / 2) * 0.5f - 0.5f) * 100, 
                    (+Mathf.Sin((-x_bulle2 - 0.5f) * Mathf.PI / 2) * 0.5f + 0.5f) * 100, 0);

                bubble2.SetActive(true);
            }
            else
            {
                bubble2.SetActive(false);
            }
            if (Mathf.Abs(f13) > fMinBubble)
            {

                float x_max = 0.5f - r_bulle;

                if (x_bulle1 > x_max) x_bulle1 = -x_max;
                if (x_bulle1 < -x_max) x_bulle1 = x_max;

                bubble1.transform.localPosition = new Vector3((1 + Mathf.Cos(2 * 3.14159f * x_bulle1)) * 5, -x_bulle1 * 100, 0);
                bubble1.SetActive(true);
            }
            else
            {
                bubble1.SetActive(false);
            }
        }

    }
}
