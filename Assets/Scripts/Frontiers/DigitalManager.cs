using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DigitalManager : BaseFrontier
{

    public float pset = 1;
    public float Pset
    {
        get
        {
            return pset;
        }

        set
        {
            pset = value;
        }
    }

    public float imax = 0.1f;
    public float Imax
    {
        get
        {
            return imax;
        }

        set
        {
            imax = value;
        }
    }

 

    GameObject water=null, water0=null, arrow=null, bubble=null, red, green ,value;
    public float periode = 4;
    public float Periode { get => periode; set => periode = value; }

 
    float ppset;
    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        if (Time.time / periode % 1 > 0.5f)
            ppset = pset;
        else
            ppset = 0f;

        p0 = p[0];

        f = Mathf.Clamp((ppset - q) / R, -Imax, Imax);  ;
        q += (i[0] + f) / C * dt;

        p[0] = (q + i[0] * R);
        i[0] =  (p0 - q) / R;

    }

    float pp = 0;
    public override void Constraint(float[] p, float[] i, float dt)
    {
        /*
            if (Mathf.Abs(i[0]) < Imax)
            {
                pp = 0.01f * ppset + 0.99f * pp;
                p[0] = pp;
            }
            else
            {
                pp = 0.01f * p[0] + 0.99f * pp;
                i[0] = Mathf.Clamp(i[0], -Imax, Imax);
            }
      */
    }

    protected override void Start()
    {
        base.Start();
        if (transform.Find("Water"))
            water = transform.Find("Water").gameObject;
        if (transform.Find("Water0"))
            water0 = transform.Find("Water0").gameObject;

        if (transform.Find("Arrow"))
            arrow = transform.Find("Arrow").gameObject;
        if (transform.Find("Bubble"))
            bubble = transform.Find("Bubble").gameObject;

        red = transform.Find("Red").gameObject;
        green = transform.Find("Green").gameObject;
        value = transform.Find("Value").gameObject;

        if (isSuccess)
            success = 0;
        configPanel = Resources.Load("ConfigPanel/ConfigInlet") as GameObject;
        R = 0.1f;
        C = 1f;
    }

    private void Update()
    {

        water.GetComponent<Image>().color = PressureColor(ppset);
        water0.GetComponent<Image>().color = PressureColor(p0);

        bubble.GetComponent<Animator>().SetFloat("speed", SpeedAnim());


        bool isGreen = Time.time / periode % 1 < 0.5;

        red.SetActive(!isGreen);
        green.SetActive(isGreen);
        value.GetComponent<Text>().text = f.ToString("F2");


        if (arrow)
        {
            if (ppset <= 0)
                arrow.GetComponent<Animator>().SetBool("Negative", true);
            else
                arrow.GetComponent<Animator>().SetBool("Negative", false);
        }

        if (isSuccess)
        {
            const float timeSuccess = 4.0f;
            if (f > 0.2f)
                success = Mathf.Clamp(success + Time.deltaTime / timeSuccess, 0, 1);
            else
                success = Mathf.Clamp(success - 10 * Time.deltaTime / timeSuccess, 0, 1);
        }

        if (Mathf.Abs(f) > fMinBubble)
        {

            if (!audios[3].isPlaying && !audios[4].isPlaying && !audios[5].isPlaying)
            {
                int r = UnityEngine.Random.Range(0, 3);
                audios[3 + r].Play();
            }
            audios[3].volume = audios[4].volume = audios[5].volume = Mathf.Abs(f) / fMinBubble * 0.1f;

        }


    }

}
