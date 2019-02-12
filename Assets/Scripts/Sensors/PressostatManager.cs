using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressostatManager : BaseComponent {


    GameObject water, water2, bubble, cadranMin, cadranMax, arrow, shine, tubeH,tubeV;
   
    //Vector3 arrowStartPosition;
    float t_shine = 0;
    Animator animator;

    public float setPointHigh;
    public float setPointLow;
    public float pMax=1;
    public float pMin=0;

    public float SetPointHigh { get => setPointHigh; set => setPointHigh = value; }
    public float SetPointLow { get => setPointLow; set => setPointLow = value; }
    public float PMax { get => pMax; set => pMax = value; }
    public float PMin { get => pMin; set => pMin = value; }

    public override void Calcule_i_p(float[] p, float[] i, float alpha)
    {
        float b = p[2];
        C = 0.05f;
        R = 1;
        q += (i[2])  * alpha;
        q *=0.99f;
        //f += (p[0] - p[2]) / L * 0;
        //f = 0;

        //p[0] = (1-alpha)*p[0] + alpha*( q + (i[0]-f)*R);
        p[2] = q / C + i[2]  * R;

        //i[0] = (1-alpha)*i[0] + alpha*( f + (a-q)/R);
        i[2] =  (b - q / C) / R;



        if ( setPointLow <q / C &&  q / C < setPointHigh  && itemBeingDragged == null)
            success = Mathf.Clamp(success + 0.005f, 0, 1);
        else
            success = Mathf.Clamp(success - 0.05f, 0, 1);


    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        Calcule_i_p_blocked(p, i, dt, 0);
        Calcule_i_p_blocked(p, i, dt, 1);
        Calcule_i_p_blocked(p, i, dt, 3);
    }


    protected override void Start()
    {
       
        tubeH = transform.Find("TubeH").gameObject;
        tubeV = transform.Find("TubeV").gameObject;
        water = transform.Find("Water").gameObject;
        water2 = transform.Find("TubeH/Water2").gameObject;

        cadranMin = transform.Find("Cadran Min").gameObject;
        cadranMax = transform.Find("Cadran Max").gameObject;

        shine = transform.Find("Shine").gameObject;


        animator = GetComponent<Animator>();
        //animator.Play("Pressostat-Gauge", 0, 0.5f);
        animator.SetFloat("rate", 0.5f);
        success = 0;
        
        shine.GetComponent<Image>().color = new Color(1, 1, 1, 0);

        base.Start();
    }

    public override void Rotate()
    {
        switch (dir%4)
        {
            case 0:
                tubeH.SetActive(true);
                tubeH.transform.localScale = new Vector3(1,1,1);
                tubeV.SetActive(false);
                break;
            case 1:
                tubeV.SetActive(true);
                tubeV.transform.localScale = new Vector3(1, 1, 1);
                tubeH.SetActive(false);
                break;
            case 2:
                tubeH.SetActive(true);
                tubeH.transform.localScale = new Vector3(-1, 1, 1);
                tubeV.SetActive(false);
                break;
            case 3:
                tubeV.SetActive(true);
                tubeV.transform.localScale = new Vector3(1, -1, 1);
                tubeH.SetActive(false);
                break;
        }
       
    }


    private void Update()
    {
        water2.GetComponent<Image>().color = PressureColor(pin[2]);
        water.GetComponent<Image>().color = PressureColor(pin[2]);

        float rate = Mathf.Clamp((q / C - PMin) / (PMax - PMin), 0, 0.99f);

        float rateH = Mathf.Clamp((setPointHigh - PMin) / (PMax-PMin) , 0, 1);
        float rateL = Mathf.Clamp((setPointLow - PMin) / (PMax - PMin) , 0, 1);

        Vector3 pos= new Vector3(0, rate* 55.3f, 0);

        //animator.Play("Pressostat-Gauge",0,rate);
        animator.SetFloat("rate", rate);

        cadranMax.GetComponent<Image>().fillAmount = rateH;
        cadranMin.GetComponent<Image>().fillAmount = rateL;

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
