using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebitmeterManager : BaseComponent {

    GameObject water0, water2, bubble,cadranMin,cadranMax,arrow,shine;
    public float x_bulle = 0;
    float r_bulle = 0.1f;
    public float setPointHigh, setPointLow, iMax;

    float t_shine = 0;
    //public new float f;

    public override void calcule_i_p(float[] p, float[] i)
    {
        //C = 5.0f;
        L = 5f;
        float a = p[0], b = p[2];

        q += (i[0] + i[2]) / C; //q*=0.99;
        f += (p[0] - p[2]) / L;

        p[0] = (q + (i[0] - f) * R);
        p[2] = (q + (i[2] + f) * R);

        i[0] = (f + (a - q) / R);
        i[2] = (-f + (b - q) / R);

        //i[1]=i[3]=0;
        i[1] = p[1] / Rground;
        i[3] = p[3] / Rground;

        x_bulle -= 0.05f * f;

        if (-f < setPointHigh && -f > setPointLow && itemBeingDragged ==null)
            success = Mathf.Clamp(success + 0.005f, 0, 1);
        else
            success = Mathf.Clamp(success - 0.05f, 0, 1);


    }

    protected override void Start()
    {
        base.Start();
        water0 = this.transform.FindChild("Water0").gameObject;
        water2 = this.transform.FindChild("Water2").gameObject;

        bubble = this.transform.FindChild("Bubble").gameObject;
        cadranMin = this.transform.FindChild("Cadran Min").gameObject;
        cadranMax = this.transform.FindChild("Cadran Max").gameObject;

        arrow = this.transform.FindChild("Arrow").gameObject;
        shine = this.transform.FindChild("Shine").gameObject;

    }

   
    private void Update()
    {

        water0.GetComponent<Image>().color = pressureColor(pin[0]);
        water2.GetComponent<Image>().color = pressureColor(pin[2]);

        if (Mathf.Abs(f) > 0.01f)
        {
            //if (x_bulle < -0.5f + d_bulle * 0.5f) { x_bulle = 0.5f - d_bulle * 0.5f; }
            //if (x_bulle > 0.5f - d_bulle * 0.5f) { x_bulle = -0.5f + d_bulle * 0.5f; }

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
        float angle = Mathf.Clamp(f / iMax * ANGLEMAX, -ANGLEMAX, ANGLEMAX);

        float angleH = Mathf.Clamp( (setPointHigh) / iMax, -1, 1);
        float angleL = Mathf.Clamp( (setPointLow) / iMax, -1, 1);



        arrow.transform.localEulerAngles= new Vector3(0, 0, angle);
        cadranMax.GetComponent<Image>().fillAmount = 0.5f-angleH*0.33f  ;
        cadranMin.GetComponent<Image>().fillAmount = 0.5f-angleL * 0.33f;

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

        shine.GetComponent<Image>().color = new Color(1, 1, 1, alpha);

    }

}
