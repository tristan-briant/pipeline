using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class capacitorManager : BaseComponent {

    GameObject waterIn0, waterIn2, water0, water2, bubble;
    GameObject spring1, spring2, spring3, spring4, piston;
    public float x_bulle = 0;
    float r_bulle = 0.1f;
    public float Cin;
    float q0, q2;
    public float xp;

    public override void Reset_i_p()
    {
        base.Reset_i_p();
        q0 = q2 = 0;
    }

    public override void calcule_i_p(float[] p, float[] i, float alpha)
    {
        float a = p[0], b = p[2];

        q +=  (i[0] + i[2]);
        //f+=alpha*(p[0]-p[2])/Lin;
        q0 += i[0];
        q2 +=  i[2];

        //f=(i[0]-i[2])/2;
        //pin = a; pout = b;

        p[0] =  (q/C + q0/Cin);
        p[2] =  (q/C + q2/Cin);

        i[0] =  (+(a - q/C - q0/Cin) / R);
        i[2] = (+(b - q/C - q2/Cin) / R);

        //i[1]=i[3]=0;
        i[1] = p[1] / Rground;
        i[3] = p[3] / Rground;

        x_bulle -= 0.05f * f;

    }

    protected override void Start()
    {
        base.Start();
        waterIn0 = this.transform.Find("Water-in0").gameObject;
        waterIn2 = this.transform.Find("Water-in2").gameObject;
        water0 = this.transform.Find("Water0").gameObject;
        water2 = this.transform.Find("Water2").gameObject;
        spring1 = transform.Find("Spring1").gameObject;
        spring2 = transform.Find("Spring2").gameObject;
        spring3 = transform.Find("Spring3").gameObject;
        spring4 = transform.Find("Spring4").gameObject;
        piston = transform.Find("Piston").gameObject;
    }

    private void Update()
    {
        waterIn0.GetComponent<Image>().color = pressureColor(q0/Cin+q/C);
        waterIn2.GetComponent<Image>().color = pressureColor(q2 / Cin + q / C);
        water0.GetComponent<Image>().color = pressureColor(pin[0]);
        water2.GetComponent<Image>().color = pressureColor(pin[2]);

        float xMax = 32f;
        //xp = Mathf.Clamp((q2-q0)/Cin*xMax, -xMax,xMax);
        xp = xMax * Mathf.Atan((q2 - q0) / Cin * xMax * 0.1f) / 1.5f;
        piston.transform.localPosition= new Vector3(xp,0,0);

        waterIn2.GetComponent<Image>().fillAmount = 0.6f + 0.4f*xp/xMax;
        spring1.transform.localScale = new Vector3(1 + xp / xMax, 1 - xp / xMax*0.4f, 1);
        spring2.transform.localScale = new Vector3(1 + xp / xMax, 1 - xp / xMax * 0.4f, 1);
        spring3.transform.localScale = new Vector3(1 - xp / xMax, 1 + xp / xMax * 0.4f, 1);
        spring4.transform.localScale = new Vector3(1 - xp / xMax, 1 + xp / xMax * 0.4f, 1);

    }
}
