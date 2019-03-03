using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElbowManager : BaseComponent {
    GameObject water2, water3, bubble;
    


    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        p2 = p[2];
        p3 = p[3];

        q += (i[2] + i[3]) * dt;
        f += (p[2] - p[3]) / L * dt;

        p[2] = (q/C + (i[2] - f) * R);
        p[3] = (q/C + (i[3] + f) * R);

        i[2] = (f + (p2 - q/C) / R);
        i[3] = (-f + (p3 - q/C) / R);

      

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
       
        bubble.gameObject.SetActive(true);
        bubble.GetComponent<Animator>().SetFloat("speed", 0);
    }

    private void Update()
    {
        water2.GetComponent<Image>().color = PressureColor(p2);
        water3.GetComponent<Image>().color = PressureColor(p3);
        
        bubble.GetComponent<Animator>().SetFloat("speed", SpeedAnim());

      
    }

}
