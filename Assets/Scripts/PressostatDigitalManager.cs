using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressostatDigitalManager : BaseComponent
{


    GameObject water, water2, bubble, red, green, arrow, shine;
    public float x_bulle = 0;
    float r_bulle = 0.1f;
    public float setPointHigh, setPointLow, PMax, PMin;
    Vector3 arrowStartPosition;
    float t_shine = 0;
    public int mode = 0;
    float SPH, SPL, successSpeed;
    public float periode = 2, phase = 0;
    public float time;

    void calculateSetPoint()
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
                time = ((Time.time % periode)/periode + phase / 360.0f)%1; // Normalized time 0->1 for a periode
                if (0.15f < time && time < 0.35f)
                {
                    mean = 1;
                    tolerance = 0.3f;
                }
                else if (0.65f < time && time < 0.85f)
                {
                    mean = 0;
                    tolerance = 0.3f;
                }
                else
                {
                    mean = 0.5f;
                    tolerance = 1;
                }
                

                SPL = mean - tolerance;
                SPH = mean + tolerance;
                successSpeed = 0.01f / periode / 2;
                break;
            

        }
    }


    public override void calcule_i_p(float[] p, float[] i, float alpha)
    {
        float b = p[2];

        q += (i[2]) * alpha; //q*=0.99;
        f += (p[0] - p[2]) / L * 0;

        //p[0] = (1-alpha)*p[0] + alpha*( q + (i[0]-f)*R);
        p[2] = (q / C + (i[2] + f) * R);

        //i[0] = (1-alpha)*i[0] + alpha*( f + (a-q)/R);
        i[2] = (-f + (b - q / C) / R);

        //i[0]=i[1]=i[3]=0;
        i[0] = p[0] / Rground;
        i[1] = p[1] / Rground;
        i[3] = p[3] / Rground;


        calculateSetPoint();
        if (SPL < q / C && q / C < SPH && itemBeingDragged == null) 
            success = Mathf.Clamp(success + successSpeed, 0, 1);
        else
            success = Mathf.Clamp(success - 0.05f, 0, 1);

    }

    protected override void Start()
    {
        base.Start();
        water = this.transform.Find("Water").gameObject;
        water2 = this.transform.Find("Water2").gameObject;

        arrow = this.transform.Find("Arrow").gameObject;
        shine = this.transform.Find("Shine").gameObject;
        red = this.transform.Find("Red").gameObject;
        green = this.transform.Find("Green").gameObject;

        arrowStartPosition = new Vector3(7.6f, -19.14f, 0);// arrow.transform.localPosition;
        success = 0;
    }


    private void Update()
    {
        water2.GetComponent<Image>().color = pressureColor(pin[2]);

        float rate = Mathf.Clamp((q / C - PMin) / (PMax - PMin), 0, 1);

        Vector3 pos = new Vector3(0, rate * 55.3f, 0);
        arrow.transform.localPosition = arrowStartPosition + pos;
        water.GetComponent<Image>().fillAmount = (1 - 0.9f * rate);

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

       

        switch (mode)
        {
            case 0:
                if (SPL > 0.5f)
                {
                    red.SetActive(false);
                    green.SetActive(true);
                }
                if (SPH < 0.5f)
                {
                    red.SetActive(true);
                    green.SetActive(false);
                }
                break;
            case 1:
                time = ((Time.time % periode) / periode + phase / 360.0f) % 1; // Normalized time 0->1 for a periode
                if ( time < 0.5f)
                {
                    red.SetActive(false);
                    green.SetActive(true);
                }
                else
                {
                    red.SetActive(true);
                    green.SetActive(false);
                }
                break;
        }



        }

}
