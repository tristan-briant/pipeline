using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElbowManager : BaseComponent {
    GameObject water2, water3, bubble;
    //float x_bulle = 0;


    public override void Calcule_i_p(float[] p, float[] i, float alpha)
    {
        float a = p[2], b = p[3];

        q += (i[2] + i[3]) * alpha; //q*=0.99;
        f += (p[2] - p[3]) / L * alpha;

        p[2] = (q/C + (i[2] - f) * R);
        p[3] = (q/C + (i[3] + f) * R);

        i[2] = (f + (a - q/C) / R);
        i[3] = (-f + (b - q/C) / R);

        //x_bulle += 0.05f * f;

        Pressure = Mathf.Clamp(0.25f * (p[2] + p[3]), -1f, 1f);

        if (float.IsNaN(Pressure))
            Pressure = 0;
        if (float.IsNaN(f))
            f = 0;
    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        Calcule_i_p_blocked(p, i, dt, 1);
        Calcule_i_p_blocked(p, i, dt, 0);

    }

    protected override void Start()
    {
        base.Start();
        water2 = this.transform.Find("Water2").gameObject;
        water3 = this.transform.Find("Water3").gameObject;
        bubble = this.transform.Find("Bubble").gameObject;
        //f = 0;
        bubble.gameObject.SetActive(true);
        bubble.GetComponent<Animator>().SetFloat("speed", 0);
    }

    private void Update()
    {
        water2.GetComponent<Image>().color = PressureColor(pin[2]);
        water3.GetComponent<Image>().color = PressureColor(pin[3]);

        //float speed = Mathf.Atan(f) / fMinBubble;
        /*if (f >= 0)
            speed = Mathf.Sqrt(f / fMinBubble);
        else
            speed = -Mathf.Sqrt(-f / fMinBubble);*/

        bubble.GetComponent<Animator>().SetFloat("speed", SpeedAnim());

      
    }

}
