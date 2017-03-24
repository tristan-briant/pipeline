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
    float g;

    public override void calcule_i_p(float[] p, float[] i)
    {
        float a = p[1], b = p[2], c=p[3];


        q1 += (i[1]) / C * alpha;
        q3 += (i[3]) / C * alpha;
        q2 += (i[2]) / C * alpha;
        f += (p[3] - p[2]) / L * alpha;
        g += (p[3] - p[1]) / L * alpha;


        if (q3 < q2 - 0.1f)  // threshold necessary
        {
            q = (q3 + q2 + q1) / 3;

            q1 = q;
            q3 = q;
            q2 = q;
        }
        else
        {
            f = 0;
            g = 0;
        }
        p[3] = (q3 + (i[3] - f - g) * R);
        p[2] = (q2 + (i[2] + f) * R);
        p[1] = (q1 + (i[1] + g) * R);


        i[1] = (-g + (a - q1) / R);
        i[2] = (-f + (b - q2) / R);
        i[3] = (f + g + (c - q3) / R);


        i[0] = p[0] / Rground;
        // i[3] = i[3] = 0;
        //i[0]=0;

        x_bulle2 -= 0.05f * f;
        x_bulle1 -= 0.05f * g;

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

        xp = 0.9f * xp + 0.1f * Mathf.Clamp(-f, 0, 0.1f);


        piston.transform.rotation = Quaternion.identity;
        piston.transform.Rotate(new Vector3(0, 0, xp * 180));

        if (Mathf.Abs(f) > 0.01f)
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
        if (Mathf.Abs(g) > 0.01f)
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
