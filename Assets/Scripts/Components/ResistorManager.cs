using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResistorManager : BaseComponent {

    GameObject water,water0, water2, bubble;
    public float x_bulle = 0;
    float r_bulle = 0.1f;
    public float Res;


    public override void calcule_i_p(float[] p, float[] i, float alpha)
    {

        float a = p[0], b = p[2];

        q += (i[0] + i[2]) * alpha;
        //f+=alpha*(p[0]-p[2])/L;
        f = (i[0] - i[2]) / 2;

        p[0] = (q / C + (i[0]) * Res * 0.5f);
        p[2] = (q / C + (i[2]) * Res * 0.5f);

        i[0] = (+(a - q / C) / Res * 2);
        i[2] = (+(b - q / C) / Res * 2);

        //i[1]=i[3]=0;
        i[1] = p[1] / Rground;
        i[3] = p[3] / Rground;

        x_bulle -= 0.05f * f;

    }

    protected override void Start()
    {
        base.Start();
        water = this.transform.FindChild("Water").gameObject;
        water0 = this.transform.FindChild("Water0").gameObject;
        water2 = this.transform.FindChild("Water2").gameObject;

        bubble = this.transform.FindChild("Bubble").gameObject;

    }

    private void Update()
    {
        water.GetComponent<Image>().color = pressureColor(q / C);
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
            float scale = Mathf.Max(Mathf.Abs(2 * x_bulle), 0.5f);
            bubble.transform.localScale = new Vector3(scale, scale, 1);
            bubble.SetActive(true);
        }
        else
        {
            bubble.SetActive(false);
        }

    }
}
