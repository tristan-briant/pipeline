using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressostatManager : BaseComponent {


    GameObject water, water2, bubble, cadranMin, cadranMax, arrow, shine;
    public float x_bulle = 0;
    float r_bulle = 0.1f;
    public float setPointHigh, setPointLow, PMax,PMin;
    Vector3 arrowStartPosition;
    float t_shine = 0;
    //public new float f;

    public override void calcule_i_p(float[] p, float[] i, float alpha)
    {
        float b = p[2];

        q += (i[2])  * alpha;
        q *=0.99f;
        f += (p[0] - p[2]) / L * 0;

        //p[0] = (1-alpha)*p[0] + alpha*( q + (i[0]-f)*R);
        p[2] = (q / C + (i[2] + f) * R);

        //i[0] = (1-alpha)*i[0] + alpha*( f + (a-q)/R);
        i[2] = (-f + (b - q / C) / R);

        //i[0]=i[1]=i[3]=0;
        i[0] = p[0] / Rground;
        i[1] = p[1] / Rground;
        i[3] = p[3] / Rground;


        if ( setPointLow <q / C &&  q / C < setPointHigh  && itemBeingDragged == null)
            success = Mathf.Clamp(success + 0.005f, 0, 1);
        else
            success = Mathf.Clamp(success - 0.05f, 0, 1);


    }

    protected override void Start()
    {
        base.Start();
        water = this.transform.Find("Water").gameObject;
        water2 = this.transform.Find("Water2").gameObject;

        //bubble = this.transform.FindChild("Bubble").gameObject;
        cadranMin = this.transform.Find("Cadran Min").gameObject;
        cadranMax = this.transform.Find("Cadran Max").gameObject;

        arrow = this.transform.Find("Arrow").gameObject;
        shine = this.transform.Find("Shine").gameObject;

        arrowStartPosition=arrow.transform.localPosition;
    }


    private void Update()
    {
        water2.GetComponent<Image>().color = pressureColor(pin[2]);

        float rate = Mathf.Clamp((q / C - PMin) / (PMax - PMin), 0, 1);

        float rateH = Mathf.Clamp((setPointHigh - PMin) / (PMax-PMin) , 0, 1);
        float rateL = Mathf.Clamp((setPointLow - PMin) / (PMax - PMin) , 0, 1);

        Vector3 pos= new Vector3(0, rate* 55.3f, 0);
        arrow.transform.localPosition = arrowStartPosition+pos; 
        cadranMax.GetComponent<Image>().fillAmount = rateH;
        cadranMin.GetComponent<Image>().fillAmount = rateL;
        water.GetComponent<Image>().fillAmount = (1-0.9f*rate);

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
