using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class diodeManager : BaseComponent {

    GameObject water0, water2, bubble;
    GameObject piston;
    public float x_bulle = 0;
    float r_bulle = 0.1f;
    float q0, q2;
    public float xp;

    public override void calcule_i_p(float[] p, float[] i, float alpha)
    {
        float a = p[0], b = p[2];


        q0 +=  (i[0]) / C * alpha;
        q2 += (i[2]) / C * alpha;
        f += (p[0] - p[2]) / L * alpha;
  

        if (q0 < q2)
        {
            q = (q0 + q2) / 2;

            q0 = q;
            q2 = q;
        }
        else
        {
            f = 0;
        }
        p[0] = (q0 + (i[0] - f) * R);
        p[2] = (q2 + (i[2] + f) * R);


        i[0] =  (f + (a - q0) / R);
        i[2] =  (-f + (b - q2) / R);

        calcule_i_p_blocked(p, i, alpha, 1);
        calcule_i_p_blocked(p, i, alpha, 3);

        x_bulle -= 0.05f * f;

    }

    protected override void Start()
    {
        base.Start();
        water0 = this.transform.Find("Water0").gameObject;
        water2 = this.transform.Find("Water2").gameObject;
        piston = transform.Find("Piston").gameObject;
        bubble = transform.Find("Bubble").gameObject;
    }

    private void Update()
    {
        water0.GetComponent<Image>().color = pressureColor(pin[0]);
        water2.GetComponent<Image>().color = pressureColor(pin[2]);

        xp = 0.9f * xp + 0.1f * Mathf.Clamp(-f, 0, 0.1f);


        piston.transform.localPosition = new Vector3(xp*100, 0, 0);

        if (Mathf.Abs(f) > 0.01f)
        {
 
            float x_max = 0.5f - r_bulle;

            if (x_bulle > x_max) x_bulle = -x_max;
            if (x_bulle < -x_max) x_bulle = x_max;


            bubble.transform.localPosition = new Vector3(x_bulle * 100, 0, 0);
            float scale = Mathf.Max(Mathf.Abs(2 * x_bulle), 0.5f);
            bubble.transform.localScale = new Vector3(scale, scale, 1);
            bubble.SetActive(true);
        }
        else
        {
            bubble.SetActive(false);
        }


    }
}
