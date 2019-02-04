using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InductorManager : BaseComponent
{

    GameObject water, water0, water2, bubble, propeller;
    public float x_bulle = 0;
    public float Lin = 10;
    //float r_bulle = 0.1f;
    float angle;
    Animation anim;

    public override void Calcule_i_p(float[] p, float[] i, float alpha)
    {
        R = 16f;//C = 2;
        float a = p[0], b = p[2];

        q += (i[0] + i[2]) * alpha;
        f += (p[0] - p[2]) / Lin * alpha;

        p[0] = (q / C + (i[0] - f) * R);
        p[2] = (q / C + (i[2] + f) * R);

        i[0] = (f + (a - q / C) / R);
        i[2] = (-f + (b - q / C) / R);

        //i[1]=i[3]=0;
        i[1] = p[1] / Rground;
        i[3] = p[3] / Rground;

        angle -= 0.05f * f;

    }

    protected override void Start()
    {
        base.Start();
        water0 = this.transform.Find("Water0").gameObject;
        water2 = this.transform.Find("Water2").gameObject;
        water = this.transform.Find("Water").gameObject;

        propeller = this.transform.Find("Propeller").gameObject;
        bubble = this.transform.Find("Bubble").gameObject;
        anim=bubble.GetComponent<Animation>();


    }

    private void Update()
    {
        water.GetComponent<Image>().color = PressureColor(q/C);
        water0.GetComponent<Image>().color = PressureColor(pin[0]);
        water2.GetComponent<Image>().color = PressureColor(pin[2]);

        
        propeller.transform.localEulerAngles = new Vector3(0, 0, angle*300);


        if (Mathf.Abs(f) > fMinBubble)
        {

            anim["bubble-inductor"].speed = -3*f;
            
            bubble.SetActive(true);
            /*if (!audios[3].isPlaying && !audios[4].isPlaying && !audios[5].isPlaying)
            {
                int r = UnityEngine.Random.Range(0, 3);
                audios[3 + r].Play();
            }
            audios[3].volume = audios[4].volume = audios[5].volume = Mathf.Abs(f) / fMinBubble * 0.1f;*/

        }
        else
        {
            bubble.SetActive(false);
        }


    }

}

