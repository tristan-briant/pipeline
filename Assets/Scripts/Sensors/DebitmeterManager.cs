using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebitmeterManager : BaseComponent {

    GameObject water0, water2, bubble,cadranMin,cadranMax,arrow,shine;
    public float x_bulle = 0;
    float r_bulle = 0.1f;
    public float setPointHigh, setPointLow, iMax;
    float SPH, SPL, successSpeed;
    public int mode = 0;
    public float periode = 2,phase = 0;
    float t_shine = 0;
    //public new float f;
    new float ff=0;
    const float beta=0.1f;

    void calculateSetPoint()
    {
        float mean, tolerance, t;
        switch (mode)
        {
            case 0 :
                SPH = setPointHigh;
                SPL = setPointLow;
                successSpeed = 0.005f;
                break;
            case 1 :
                 mean = 0.5f * (setPointHigh + setPointLow);
                 tolerance = (setPointHigh - setPointLow) * 0.5f;
                 t = 2 * Mathf.PI * Time.time;

                SPL = mean * Mathf.Sin(t / periode + Mathf.PI * phase / 180.0f) - tolerance;
                SPH = SPL + 2 * tolerance;
                successSpeed = 0.01f / periode / 2;
                break;
            case 2:
                mean = 0.5f * (setPointHigh + setPointLow);
                tolerance = (setPointHigh - setPointLow) * 0.5f;
                t = 2 * Mathf.PI * Time.time;

                SPL = mean * theta(Mathf.Sin(t / periode + Mathf.PI * phase / 180.0f)) - tolerance;
                SPH = SPL + 2 * tolerance;
                successSpeed = 0.01f / periode / 2;
                break;

        }
    }


    float theta(float x) {
        if( x < 0) return 0;
        return x;
    }

    public override void Calcule_i_p(float[] p, float[] i, float alpha)
    {
        //C = 5.0f;
        //L = 3f;
        float a = p[0], b = p[2];

        q += (i[0] + i[2]) * alpha; 
        f += (p[0] - p[2]) / L * alpha;

        p[0] = (q / C + (i[0] - f) * R);
        p[2] = (q / C + (i[2] + f) * R);

        i[0] = (f + (a - q / C) / R);
        i[2] = (-f + (b - q / C) / R);

        Calcule_i_p_blocked(p, i, alpha, 1);
        Calcule_i_p_blocked(p, i, alpha, 3);

        x_bulle -= 0.05f * f;

        ff = (1 - beta) * ff + beta * f;

        calculateSetPoint();
        if ( SPL < -ff  && -ff <SPH && itemBeingDragged == null)
            success = Mathf.Clamp(success + successSpeed, 0, 1);
        else
            success = Mathf.Clamp(success - 0.05f, 0, 1);

    }

    protected override void Start()
    {
        base.Start();
        success = 0;
        water0 = this.transform.Find("Water0").gameObject;
        water2 = this.transform.Find("Water2").gameObject;

        bubble = this.transform.Find("Bubble").gameObject;
        cadranMin = this.transform.Find("Cadran Min").gameObject;
        cadranMax = this.transform.Find("Cadran Max").gameObject;

        arrow = this.transform.Find("Arrow").gameObject;
        shine = this.transform.Find("Shine").gameObject;

    }

   
    private void Update()
    {

        water0.GetComponent<Image>().color = PressureColor(pin[0]);
        water2.GetComponent<Image>().color = PressureColor(pin[2]);

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
        float angle = Mathf.Clamp(ff / iMax * ANGLEMAX, -ANGLEMAX, ANGLEMAX);

        //float angleH, angleL;
        float angleH = Mathf.Clamp((SPH) / iMax, -1, 1);
        float angleL = Mathf.Clamp((SPL) / iMax, -1, 1);


        /*if (mode == 0)
        {
            angleH = Mathf.Clamp((setPointHigh) / iMax, -1, 1);
            angleL = Mathf.Clamp((setPointLow) / iMax, -1, 1);
        }
        else if(mode==1)
        {
            float mean = 0.5f * (setPointHigh + setPointLow);
            float tol = (setPointHigh - setPointLow) * 0.5f;
            float t = 2 * Mathf.PI * Time.time;

            angleH = Mathf.Clamp((mean * Mathf.Sin(t / periode) + tol) / iMax, -1, 1);
            angleL = Mathf.Clamp((mean * Mathf.Sin(t / periode) - tol) / iMax, -1, 1);

        }
        else
        {
            float mean = 0.5f * (setPointHigh + setPointLow);
            float tol = (setPointHigh - setPointLow) * 0.5f;
            float t = 2 * Mathf.PI * Time.time;

            angleH = Mathf.Clamp((mean * theta(Mathf.Sin(t / periode)) + tol) / iMax, -1, 1);
            angleL = Mathf.Clamp((mean * theta(Mathf.Sin(t / periode)) - tol) / iMax, -1, 1);
        }*/


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
