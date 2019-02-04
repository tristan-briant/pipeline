using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public class Pipeline : BaseComponent { 

    GameObject water0,water2,bubble;
    float x_bulle = 0;
    float r_bulle=0.1f;

    public override void Calcule_i_p(float[] p, float[] i, float alpha)
    {

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
        pressure = Mathf.Clamp(0.25f * (p[0] + p[2]), -1f, 1f); ;
    }

    protected override void Start()
    {
        base.Start();
        water0 = this.transform.Find("Water0").gameObject;
        water2 = this.transform.Find("Water2").gameObject;

        bubble = this.transform.Find("Bubble").gameObject;

    }

    private void Update()
    {
        water0.GetComponent<Image>().color = PressureColor(pin[0]);
        water2.GetComponent<Image>().color = PressureColor(pin[2]);

        if (Mathf.Abs(f) > fMinBubble)
        {
            //if (x_bulle < -0.5f + d_bulle * 0.5f) { x_bulle = 0.5f - d_bulle * 0.5f; }
            //if (x_bulle > 0.5f - d_bulle * 0.5f) { x_bulle = -0.5f + d_bulle * 0.5f; }

            float x_max = 0.5f - r_bulle;

            if (x_bulle > x_max) x_bulle = -x_max;
            if (x_bulle < -x_max) x_bulle = x_max;

            
            bubble.transform.localPosition =new Vector3(x_bulle*100,0,0);
            bubble.SetActive(true);

            
            if (!audios[3].isPlaying && !audios[4].isPlaying && !audios[5].isPlaying) {
                int r = UnityEngine.Random.Range(0, 3);
                audios[3 + r].Play();
            }
            audios[3].volume = audios[4].volume = audios[5].volume = Mathf.Abs(f) / fMinBubble * 0.1f;

        }
        else
        {
            bubble.SetActive(false);
        }

    }

}

