using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ValveManager : BaseComponent {

    GameObject water0, water2, bubble,tubeOpen,tubeClosed;
    public float x_bulle = 0;
    float r_bulle = 0.1f;
    public bool open=false;
    float q1=0, q2=0;
    public float openTime=4.0f;
 


    public override void calcule_i_p(float[] p, float[] i, float alpha)
    {

        float a = p[0], b = p[2];
        /*if (countDown > 0)
            open = true;
        else
            open = false;*/

        q1 += (i[0])* alpha;
        q2 += (i[2]) * alpha;
        f += (p[0] - p[2]) / L * alpha;


        if (open)
        {
            q = (q1 + q2) / 2;

            q1 = q;
            q2 = q;
        }
        else
        {
            f = 0;
        }
        p[0] = (q1 / C + (i[0] - f) * R);
        p[2] = (q2 / C + (i[2] + f) * R);

        i[0] = (f + (a - q1 / C) / R);
        i[2] = (-f + (b - q2 / C) / R);

        calcule_i_p_blocked(p, i, alpha, 1);
        calcule_i_p_blocked(p, i, alpha, 3);


        x_bulle -= 0.05f * f;

    }

    protected override void Start()
    {
        base.Start();
        water0 = this.transform.FindChild("Water0").gameObject;
        water2 = this.transform.FindChild("Water2").gameObject;

        bubble = this.transform.FindChild("Bubble").gameObject;
        tubeOpen = this.transform.FindChild("TubeOpen").gameObject;
        tubeClosed = this.transform.FindChild("TubeClosed").gameObject;
    }

    IEnumerator countDown() {
        yield return new WaitForSeconds(openTime);

        open = false;
    }

    private void Update()
    {
        if (trigged) {
            trigged = false;
            open = true;
            StartCoroutine(countDown());
        }

        water0.GetComponent<Image>().color = pressureColor(pin[0]);
        water2.GetComponent<Image>().color = pressureColor(pin[2]);

        if (!open) {
            tubeOpen.SetActive(false);
            tubeClosed.SetActive(true);
        }
        else
        {
            tubeOpen.SetActive(true);
            tubeClosed.SetActive(false);
        }

        if (Mathf.Abs(f) > 0.01f)
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

    }
}
