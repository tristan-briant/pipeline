using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TeManager : BaseComponent
{

    GameObject water0, water2, water3, bubble20, bubble23, bubble03;
    float x_bulle0 = 0.5f, x_bulle2 = 0.5f, x_bulle3 = 0.5f;
    float f0,  f2, f3;
    float i0 = 0,  i2 = 0, i3 = 0;

    //public float x_bulle = 0;
    float r_bulle = 0.1f;
    //float f=0;
    //float[] pin = new float[4];

    public override void Reset_i_p()
    {
        base.Reset_i_p();
        i0 = i2 = i3 = f0 = f2 = f3 = 0;
    }

    public override void Calcule_i_p(float[] p, float[] i,float dt)
    {
        p0 = p[0];
        p2 = p[2];
        p3 = p[3];


        i0 = i[0]; x_bulle0 -= 0.05f * i0; //// pour le dessin
        //i1 = i[1]; x_bulle1 += 0.05f * i1; 
        i2 = i[2]; x_bulle2 += 0.05f * i2;
        i3 = i[3]; x_bulle3 -= 0.05f * i3;

        q += (i[0] + i[2] + i[3]) * dt; //q*=0.99;

        f0 += (p[0] - q) / L * dt;
        //f1 += (p[1] - q) / L * alpha;
        f2 += (p[2] - q) / L * dt;
        f3 += (p[3] - q) / L * dt;

        p[0] = (q / C + (i[0] - f0) * R);
        //p[1] = (q / C + (i[1] - f1) * R);
        p[2] = (q / C + (i[2] - f2) * R);
        p[3] = (q / C + (i[3] - f3) * R);

        i[0] = (f0 + (p0 - q / C) / R);
        //i[1] = (f1 + (p1 - q / C) / R);
        i[2] = (f2 + (p2 - q / C) / R);
        i[3] = (f3 + (p3 - q / C) / R);

  
        Pressure = Mathf.Clamp(0.5f * q, -1f, 1f); ;

    }


    public override void Constraint(float[] p, float[] i, float dt)
    {
        //Calcule_i_p_blocked(p, i, dt, 1);
        i[1] = 0;
    }

    protected override void Start()
    {
        base.Start();
        water0 = transform.Find("Water0").gameObject;
        water2 = transform.Find("Water2").gameObject;
        water3 = transform.Find("Water3").gameObject;

        bubble20 = transform.Find("Bubble20").gameObject;
        bubble23 = transform.Find("Bubble23").gameObject;
        bubble03 = transform.Find("Bubble03/Bubble").gameObject;

    }

    float flux(float f1, float f2) {
        if ((f1 > 0 && f2 > 0) || (f1 < 0 && f2 < 0))
            return 0;

        if (Mathf.Abs(f1) < Mathf.Abs(f2))
            return f1;
        else
            return -f2;
    }

    private void Update()
    {
        water0.GetComponent<Image>().color = PressureColor(p0);
        water2.GetComponent<Image>().color = PressureColor(p2);
        water3.GetComponent<Image>().color = PressureColor(p3);

        bubble20.GetComponent<Animator>().SetFloat("speed", SpeedAnim(f2, f0));
        bubble23.GetComponent<Animator>().SetFloat("speed", SpeedAnim(f2, f3));
        bubble03.GetComponent<Animator>().SetFloat("speed", SpeedAnim(f0, f3));

        /*
        //////////BUBBLE 0
        if (Mathf.Abs(i0) > fMinBubble)
        {
            float xmax = 0.5f - r_bulle;
            if (x_bulle0 > xmax) x_bulle0 = -xmax;
            if (x_bulle0 < -xmax) x_bulle0 = xmax;


            if (x_bulle0 > 0)
            {
                bubble0.transform.localPosition = new Vector3(x_bulle0 * 100, 0, 0);
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
        */
    }

}

