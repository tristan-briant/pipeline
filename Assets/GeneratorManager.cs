using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorManager :  BaseComponent {

    GameObject water0, water2, bubble, shine, helice;
    public float x_bulle = 0;
    float r_bulle = 0.1f;
    float velocity = 0;
    public float damping = 0.025f;
    public float setPointHigh, setPointLow, iMax;

    float t_shine = 0;
    //public new float f;

    public override void calcule_i_p(float[] p, float[] i)
    {
        //C = 5.0f; L = 10f;
        float a = p[0], b = p[2];

        q += (i[0] + i[2]) * alpha; //q*=0.99;
        f += (p[0] - p[2]) / L * alpha;

        p[0] = (q / C + (i[0] - f) * R);
        p[2] = (q / C + (i[2] + f) * R);

        i[0] = (f + (a - q / C) / R);
        i[2] = (-f + (b - q / C) / R);

        //i[1]=i[3]=0;
        i[1] = p[1] / Rground;
        i[3] = p[3] / Rground;

        x_bulle -= 0.05f * f;


        if (f<0)
            velocity = Mathf.Clamp(velocity - 0.005f*f, 0, 3);
        else
            velocity = Mathf.Clamp(velocity - 0.05f*f, 0, 3);

        velocity -= damping * velocity * alpha;

        if (setPointLow < velocity && velocity < setPointHigh && itemBeingDragged == null)
            success = Mathf.Clamp(success + 0.002f, 0, 1);//5 seconds to win
        else
            success = Mathf.Clamp(success - 0.01f, 0, 1);


    }

    protected override void Start()
    {
        base.Start();
        water0 = this.transform.FindChild("Water0").gameObject;
        water2 = this.transform.FindChild("Water2").gameObject;

        bubble = this.transform.FindChild("Bubble").gameObject;
        shine = this.transform.FindChild("Shine").gameObject;
        helice = this.transform.FindChild("Helice").gameObject;
    }


    float angle=0;

    private void Update()
    {

        water0.GetComponent<Image>().color = pressureColor(pin[0]);
        water2.GetComponent<Image>().color = pressureColor(pin[2]);

        angle += 3.14f * velocity*3;
        helice.transform.localEulerAngles = new Vector3(0, 0, angle);

        if (Mathf.Abs(f) > fMinBubble)
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

        const float ANGLEMAX = 180 / 4.8f;
       // float angle = Mathf.Clamp(f / iMax * ANGLEMAX, -ANGLEMAX, ANGLEMAX);

        float angleH = Mathf.Clamp((setPointHigh) / iMax, -1, 1);
        float angleL = Mathf.Clamp((setPointLow) / iMax, -1, 1);


        float alpha;

        if (success < 1)
        {
            alpha = success;
            t_shine = 0;
        }
        else
        {
            t_shine += Time.deltaTime;
            alpha = 0.8f + 0.2f * Mathf.Cos(t_shine * 5.0f);

        }

        Color col = shine.GetComponent<Image>().color;
        col.a = alpha;

        shine.GetComponent<Image>().color = col;
    }

}