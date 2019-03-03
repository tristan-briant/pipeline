using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;


public class Pipeline : BaseComponent { 

    GameObject water0,water2,bubble;
 
    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        p0 = p[0];
        p2 = p[2];

        q += (i[0] + i[2]) * dt; 
        f += (p[0] - p[2]) / L * dt;

        p[0] = (q / C + (i[0] - f) * R);
        p[2] = (q / C + (i[2] + f) * R);

        i[0] = (f + (p0 - q / C) / R);
        i[2] = (-f + (p2 - q / C) / R);

        Pressure = Mathf.Clamp(0.25f * (p[0] + p[2]), -1f, 1f); ;
    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        i[1] = 0;
        i[3] = 0;
    }

    protected override void Start()
    {
        base.Start();
        water0 = transform.Find("Water0").gameObject;
        water2 = transform.Find("Water2").gameObject;

        bubble = transform.Find("Bubble").gameObject;

    }

    private void Update()
    {
        water0.GetComponent<Image>().color = PressureColor(p0);
        water2.GetComponent<Image>().color = PressureColor(p2);


        bubble.GetComponent<Animator>().SetFloat("speed", -SpeedAnim());



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

