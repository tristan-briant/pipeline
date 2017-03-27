using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransistorManager : BaseComponent
{

    GameObject water1, water2, water3, bubble1, bubble2;
    GameObject piston;
    public float x_bulle2=0,x_bulle1 = 0;
    float r_bulle = 0.1f;
    float q1, q2, q3;
    public float xp;
    float g,f1,f2,f3;
    float threshold = 0.2f;

    public override void calcule_i_p(float[] p, float[] i, float alpha)
    {
        float a = p[1], b = p[2], c=p[3];


        q1 += (i[1]) * alpha;
        q3 += (i[3]) * alpha;
        q2 += (i[2]) * alpha;

        f2 += (p[2] - p[3]-threshold) / L * alpha;
        f3 += (p[3] - p[1]) / L * alpha;
        f1 = 0;

        //if (p[2] - p[3] > threshold)//
        if (f2 > 0)
        {
            q = (q3 + q2 + q1) / 3;
            q1 = q2 = q3 = q;
        }
        else
        {
            f2 = f3 = 0;
        }

        p[1] = (q1 / C + (i[1] - f1 + f3) * R);
        p[2] = (q2 / C + (i[2] - f2 + f1) * R);
        p[3] = (q3 / C + (i[3] - f3 + f2) * R);

        i[1] = (f1 - f3 + (a - q1 / C) / R);
        i[2] = (f2 - f1 + (b - q2 / C) / R);
        i[3] = (f3 - f2 + (c - q3 / C) / R);

 

        i[0] = p[0] / Rground;
   
        x_bulle2 += 0.05f * f2;
        x_bulle1 -= 0.05f * f3;

    }

    protected override void Start()
    {
        base.Start();
        water1 = this.transform.FindChild("Water1").gameObject;
        water2 = this.transform.FindChild("Water2").gameObject;
        water3 = this.transform.FindChild("Water3").gameObject;
        piston = transform.FindChild("Piston").gameObject;
        bubble1 = transform.FindChild("Bubble1").gameObject;
        bubble2 = transform.FindChild("Bubble2").gameObject;
    }

    private void Update()
    {
        water1.GetComponent<Image>().color = pressureColor(pin[1]);
        water2.GetComponent<Image>().color = pressureColor(pin[2]);
        water3.GetComponent<Image>().color = pressureColor(pin[3]);

        const float r = 0.1f;
        xp = (1 - r) * xp + r * Mathf.Clamp(+f2, 0, 0.1f);


       /* piston.transform.rotation = Quaternion.identity;
        piston.transform.Rotate(new Vector3(0, 0, xp * 180));*/
        piston.transform.localEulerAngles = new Vector3(0, 0, xp*180);

        if (Mathf.Abs(f2) > fMinBubble)
        {

            float x_max = 0.5f - r_bulle;

            if (x_bulle2 > x_max) x_bulle2 = -x_max;
            if (x_bulle2 < -x_max) x_bulle2 = x_max;


            /*bubble2.transform.rotation = Quaternion.identity;
            bubble2.transform.Rotate(new Vector3(0, 0, (x_bulle2-0.5f) * 90));*/

            // bubble2.transform.localPosition = new Vector3(x_bulle2 * 100, 0, 0);
            float scale = Mathf.Max(Mathf.Abs(2 * x_bulle2), 0.5f);
            bubble2.transform.localScale = new Vector3(scale, scale, 1);
            bubble2.transform.localPosition = new Vector3((Mathf.Cos((x_bulle2 - 0.5f) * Mathf.PI / 2) * 0.5f - 0.5f) * 100, (-Mathf.Sin((x_bulle2 - 0.5f) * Mathf.PI / 2) * 0.5f - 0.5f) * 100, 0);

            bubble2.SetActive(true);
        }
        else
        {
            bubble2.SetActive(false);
        }
        if (Mathf.Abs(f3) > fMinBubble)
        {

            float x_max = 0.5f - r_bulle;

            if (x_bulle1 > x_max) x_bulle1 = -x_max;
            if (x_bulle1 < -x_max) x_bulle1 = x_max;


            bubble1.transform.localPosition = new Vector3( (1+Mathf.Cos(2*3.14159f* x_bulle1))*5, - x_bulle1 * 100, 0);
            //float scale = Mathf.Max(Mathf.Abs(2 * x_bulle1), 0.5f);
            //bubble1.transform.localScale = new Vector3(scale, scale, 1);
            bubble1.SetActive(true);
        }
        else
        {
            bubble1.SetActive(false);
        }

    }
}
