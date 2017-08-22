using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElbowManager : BaseComponent {
    GameObject water2, water3, bubble;
    float r_bulle = 0.2f;
    float x_bulle = 0;


    public override void calcule_i_p(float[] p, float[] i, float alpha)
    {
        float a = p[2], b = p[3];

        q += (i[2] + i[3]) * alpha; //q*=0.99;
        f += (p[2] - p[3]) / L * alpha;

        p[2] = (q/C + (i[2] - f) * R);
        p[3] = (q/C + (i[3] + f) * R);

        i[2] = (f + (a - q/C) / R);
        i[3] = (-f + (b - q/C) / R);

        calcule_i_p_blocked(p, i, alpha, 1);
        calcule_i_p_blocked(p, i, alpha, 0);


        x_bulle += 0.05f * f;


    }

    protected override void Start()
    {
        base.Start();
        water2 = this.transform.Find("Water2").gameObject;
        water3 = this.transform.Find("Water3").gameObject;
        bubble = this.transform.Find("Bubble").gameObject;



    }

    private void Update()
    {
        water2.GetComponent<Image>().color = pressureColor(pin[2]);
        water3.GetComponent<Image>().color = pressureColor(pin[3]);
        //transform.rotation = Quaternion.identity;
        //transform.Rotate(new Vector3(0, 0, dir * 90));


        if (Mathf.Abs(f) > fMinBubble)
        {
            float x_max = 0.5f - r_bulle/2f;

            if (x_bulle >x_max) x_bulle = -x_max; /// 3 for PI
            if (x_bulle < -x_max) x_bulle = x_max;

 
            bubble.transform.localPosition = new Vector3((Mathf.Cos((x_bulle - 0.5f) * Mathf.PI / 2) * 0.5f - 0.5f) * 100, (-Mathf.Sin((x_bulle - 0.5f) * Mathf.PI / 2) * 0.5f - 0.5f) * 100, 0);
            bubble.SetActive(true);

            if (!audios[3].isPlaying && !audios[4].isPlaying && !audios[5].isPlaying)
            {
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
