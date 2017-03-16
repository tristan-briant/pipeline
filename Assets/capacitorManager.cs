using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class capacitorManager : BaseComponent {

    GameObject waterIn0, waterIn2, water0, water2, bubble;
    public float x_bulle = 0;
    float r_bulle = 0.1f;
    public float Cin;
    float q0, q2;

    public override void calcule_i_p(float[] p, float[] i)
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
        waterIn0 = this.transform.FindChild("Water-in0").gameObject;
        waterIn2 = this.transform.FindChild("Water-in2").gameObject;
        water0 = this.transform.FindChild("Water0").gameObject;
        water2 = this.transform.FindChild("Water2").gameObject;


    }

    private void Update()
    {
        waterIn0.GetComponent<Image>().color = pressureColor(q0/Cin+q/C);
        waterIn2.GetComponent<Image>().color = pressureColor(q2 / Cin + q / C);
        water0.GetComponent<Image>().color = pressureColor(pin[0]);
        water2.GetComponent<Image>().color = pressureColor(pin[2]);


    }
}
