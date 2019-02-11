using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class capacitorManager : BaseComponent {

    GameObject waterIn0, waterIn2, water0, water2, bubble0, bubble2;
    GameObject spring1, spring2, spring3, spring4, piston;
    //float x_bulle = 0;

    float f0, f2;
    public float cin;

    float q0, q2;
    float xp;

    public float Cin
    {
        get
        {
            return cin;
        }

        set
        {
            cin = value;
        }
    }

    public override void Reset_i_p()
    {
        base.Reset_i_p();
        q0 = q2 = 0;
    }

    public override void Calcule_i_p(float[] p, float[] i, float alpha)
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
        //i[1] = p[1] / Rground;
        //i[3] = p[3] / Rground;

        //Calcule_i_p_blocked(p,i,alpha,1);
        //Calcule_i_p_blocked(p,i,alpha,3);

        f0 = i[0];
        f2 = i[2];
        //x_bulle -= 0.05f * f;

    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        Calcule_i_p_blocked(p, i, dt, 1);
        Calcule_i_p_blocked(p, i, dt, 3);
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
        bubble0 = transform.Find("Mirror/Bubble0").gameObject;
        bubble2 = transform.Find("Bubble2").gameObject;
        f0 = f2 = 0;
        bubble0.GetComponent<Animator>().SetFloat("speed", f0 / fMinBubble);
        bubble2.GetComponent<Animator>().SetFloat("speed", f2 / fMinBubble);
    }

    private void Update()
    {
        waterIn0.GetComponent<Image>().color = PressureColor(q0/Cin+q/C);
        waterIn2.GetComponent<Image>().color = PressureColor(q2 / Cin + q / C);
        water0.GetComponent<Image>().color = PressureColor(pin[0]);
        water2.GetComponent<Image>().color = PressureColor(pin[2]);

        float xMax = 32f;
        //xp = Mathf.Clamp((q2-q0)/Cin*xMax, -xMax,xMax);
        xp = xMax * Mathf.Atan((q2 - q0) / Cin * xMax * 0.1f) / 1.5f;
        piston.transform.localPosition= new Vector3(xp,0,0);

        waterIn2.GetComponent<Image>().fillAmount = 0.6f + 0.4f*xp/xMax;
        spring1.transform.localScale = new Vector3(1 + xp / xMax, 1 - xp / xMax*0.4f, 1);
        spring2.transform.localScale = new Vector3(1 + xp / xMax, 1 - xp / xMax * 0.4f, 1);
        spring3.transform.localScale = new Vector3(1 - xp / xMax, 1 + xp / xMax * 0.4f, 1);
        spring4.transform.localScale = new Vector3(1 - xp / xMax, 1 + xp / xMax * 0.4f, 1);


        bubble0.GetComponent<Animator>().SetFloat("speed", f0 / fMinBubble);
        bubble2.GetComponent<Animator>().SetFloat("speed", f2 / fMinBubble);

    }
}
