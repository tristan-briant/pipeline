using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebitmeterManager : BaseComponent
{

    GameObject water, water0, water2, bubble, cadranMin, cadranMax, arrow, shine, value;
    //public float x_bulle = 0;
    //float r_bulle = 0.1f;
    public float iMax;
    public float setPointHigh;
    public float setPointLow;
   
    public float SetPointHigh { get => setPointHigh; set => setPointHigh = value; }
    public float SetPointLow { get => setPointLow; set => setPointLow = value; }
    public float IMax { get => iMax; set => iMax = value; }

    float SPH, SPL, successSpeed;
    public int mode = 0;
    public float periode = 2, phase = 0;
    float t_shine = 0;

    float flux = 0;


    const float beta = 0.1f;

    protected override void Start()
    {
        base.Start();
        success = 0;
        water = transform.Find("Water").gameObject;
        water0 = transform.Find("Water0").gameObject;
        water2 = transform.Find("Water2").gameObject;

        bubble = transform.Find("Bubble").gameObject;
        cadranMin = transform.Find("Cadran Min").gameObject;
        cadranMax = transform.Find("Cadran Max").gameObject;

        arrow = transform.Find("Arrow").gameObject;
        shine = transform.Find("Shine").gameObject;
        value = transform.Find("Value").gameObject;

        f = 0;
        bubble.GetComponent<Animator>().SetFloat("speed", 0);

        configPanel = Resources.Load("ConfigPanel/ConfigDebimeter") as GameObject;
    }


    void CalculateSetPoint()
    {
        float mean, tolerance, t;
        switch (mode)
        {
            case 0:
                SPH = setPointHigh;
                SPL = setPointLow;
                successSpeed = 0.005f;
                break;
            case 1:
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


    public override void Rotate()
    {
        base.Rotate();
        foreach(Text t in GetComponentsInChildren<Text>())
        {
            t.transform.rotation = Quaternion.identity;
        }
    }

    float theta(float x)
    {
        if (x < 0) return 0;
        return x;
    }

    public override void Calcule_i_p(float[] p, float[] i, float alpha)
    {
        float a = p[0], b = p[2];

        q += (i[0] + i[2]) * alpha;
        f += (p[0] - p[2]) / L * alpha;

        p[0] = (q / C + (i[0] - f) * R);
        p[2] = (q / C + (i[2] + f) * R);

        i[0] = (f + (a - q / C) / R);
        i[2] = (-f + (b - q / C) / R);

        flux = (1 - beta) * flux + beta * f;

        CalculateSetPoint();
        if (SPL < -flux && -flux < SPH && itemBeingDragged == null)
            success = Mathf.Clamp(success + successSpeed, 0, 1);
        else
            success = Mathf.Clamp(success - 0.05f, 0, 1);

    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        Calcule_i_p_blocked(p, i, dt, 1);
        Calcule_i_p_blocked(p, i, dt, 3);
    }

   


    private void Update()
    {

        water.GetComponent<Image>().color = PressureColor(q / C);
        water0.GetComponent<Image>().color = PressureColor(pin[0]);
        water2.GetComponent<Image>().color = PressureColor(pin[2]);

        bubble.GetComponent<Animator>().SetFloat("speed", -SpeedAnim());

        const float ANGLEMAX = 180 / 4.8f;
        float angle = Mathf.Clamp(flux / iMax * ANGLEMAX, -ANGLEMAX * 1.2f, ANGLEMAX * 1.2f);

        float angleH = Mathf.Clamp((SPH) / iMax, -1, 1);
        float angleL = Mathf.Clamp((SPL) / iMax, -1, 1);

        arrow.transform.localEulerAngles = new Vector3(0, 0, angle);
        cadranMax.GetComponent<Image>().fillAmount = 0.5f - angleH * 0.32f;
        cadranMin.GetComponent<Image>().fillAmount = 0.5f - angleL * 0.32f;

        float v;
        if (Mathf.Abs(f)<1 )
            v = Mathf.Round(100 * -f)/100;
        else
            v = Mathf.Round(10 * -f) / 10;

        value.GetComponent<Text>().text = v.ToString();

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
