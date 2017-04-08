using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class PressostatTriggerManager : BaseComponent {

    GameObject water, water2, bubble, cadran2, cadran, arrow, shine;
    public float x_bulle = 0;
    float r_bulle = 0.1f;
    //public float setPointHigh, setPointLow, PMax, PMin;
    public float setPoint, tolerance, PMax,PMin=0;
    Vector3 arrowStartPosition, cadranStartPosition;
    float t_shine = 0;
    public float openTime = 4.0f;
    public float Tau = 1;
    bool activated = false;
    //public new float f;


    public override void OnPointerClick(PointerEventData eventData)
    {
        //Do not rotate
    }

    public override void calcule_i_p(float[] p, float[] i, float alpha)
    {
        C = 1;
        float b = p[2];

        q += (i[2])  * alpha; //q*=0.99;
        f += (p[0] - p[2]) / L * 0;

        p[2] = (q / C + (i[2] + f) * R);

        i[2] = (-f + (b - q / C) / R);

        calcule_i_p_blocked(p, i, alpha, 0);
        calcule_i_p_blocked(p, i, alpha, 1);
        calcule_i_p_blocked(p, i, alpha, 3);
        

        /*i[0] = p[0] / Rground;
        i[1] = p[1] / Rground;
        i[3] = p[3] / Rground;*/


        if ( risingCurve(time)-tolerance/2 < q / C && q / C < risingCurve(time) + tolerance / 2 && itemBeingDragged == null && activated)
            success = Mathf.Clamp(success + 0.01f/openTime/0.9f, 0, 1); //0.01=10ms with 90% tolerance
        else
            success = Mathf.Clamp(success - 0.05f, 0, 1);


    }

    protected override void Start()
    {
        base.Start();
        water = this.transform.FindChild("Water").gameObject;
        water2 = this.transform.FindChild("Water2").gameObject;

        cadran = this.transform.FindChild("Cadran").gameObject;
        cadran2 = this.transform.FindChild("Cadran2").gameObject;

        arrow = this.transform.FindChild("Arrow").gameObject;
        shine = this.transform.FindChild("Shine").gameObject;

        arrowStartPosition = arrow.transform.localPosition;
        cadranStartPosition = cadran.transform.localPosition;
    }

    IEnumerator countDown() {
        yield return new WaitForSeconds(openTime);

        activated = false;
    }

    float time=0;

    private void Update()
    {
        if (trigged)
        {
            trigged = false;
            activated = true;
            StartCoroutine(countDown());
        }

        if (activated)
        {
            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }


        water2.GetComponent<Image>().color = pressureColor(pin[2]);

        if (activated)
            cadran2.GetComponent<Image>().color = new Color(0,255,0);
        else
            cadran2.GetComponent<Image>().color = new Color(255, 255, 255);


        float rate = Mathf.Clamp((q - PMin) / (PMax - PMin), 0, 1);

       
        Vector3 pos = new Vector3(0, rate * 100f, 0);
        arrow.transform.localPosition = arrowStartPosition + pos;

        pos = new Vector3(0, risingCurve(time) * 100f, 0);

        cadran.transform.localPosition = cadranStartPosition+ pos;
        cadran2.transform.localPosition = cadranStartPosition+  pos;

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

    }

    float risingCurve(float t) {
        return setPoint*(1 - Mathf.Exp ( -t / Tau) );
    }

}
