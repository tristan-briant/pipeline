using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressostatManager : BaseComponent {


    GameObject water, water2, bubble, cadranMin, cadranMax, arrow, shine, tubeH, tubeV, gauge, value;

    float successTime = 2.0f; //Time to obtain a success
    float t_shine = 0;
    Animator animator;

    public float setPointHigh;
    public float setPointLow;
    public float pMax=1;
    //float pMin=0;

    public bool symmetric = true;
    public bool Symmetric { get => symmetric; set { symmetric = value; DrawCadran(); } }


    public float SetPointHigh { get => setPointHigh;
        set
        {
            if (value <= SetPointLow) { setPointHigh = setPointLow; isSuccess = false; }
            else { setPointHigh = value; isSuccess = true; }
            DrawCadran();
        }
    }
    public float SetPointLow { get => setPointLow;
        set
        {
            if (value >= setPointHigh) { setPointLow = setPointHigh; isSuccess = false; }
            else { setPointLow = value; isSuccess = true; }
            DrawCadran();
        }
    }

    public float PMax {
        get => pMax;
        set {
            pMax = Mathf.Max(value,0.1f);
            DrawCadran();
        }
    }
    public float PMin { get => symmetric ? -pMax : 0; }


    protected override void Start()
    {

        tubeH = transform.Find("TubeH").gameObject;
        tubeV = transform.Find("TubeV").gameObject;
        water2 = transform.Find("TubeH/Water2").gameObject;

        water = transform.Find("Gauge/Water").gameObject;
        cadranMin = transform.Find("Gauge/Cadran Min").gameObject;
        cadranMax = transform.Find("Gauge/Cadran Max").gameObject;

        gauge = transform.Find("Gauge").gameObject;

        DrawCadran();

        shine = transform.Find("Gauge/Shine").gameObject;
        value = transform.Find("Gauge/Arrow/Value").gameObject;

        //animator = transform.Find("Gauge").GetComponent<Animator>();
        animator = GetComponent<Animator>();
        animator.SetFloat("rate", 0.5f);

        shine.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        configPanel = Resources.Load("ConfigPanel/ConfigPressostat") as GameObject;

        base.Start();

        success = 0;
        C = 0.05f;

    }

  

    protected void DrawCadran()
    {
        float rateH = Mathf.Clamp((setPointHigh - PMin) / (PMax - PMin), 0, 1);
        float rateL = Mathf.Clamp((setPointLow - PMin) / (PMax - PMin), 0, 1);

        cadranMax.GetComponent<Image>().fillAmount = rateH;
        cadranMin.GetComponent<Image>().fillAmount = rateL;
    }

    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        p2 = p[2];
        
        q += i[2] / C * dt;
        q *= 0.99f;

        p[2] = q + i[2] * R;
        i[2] = (p2 - q) / R;
    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        i[0] = i[1] = i[3] = 0;
    }
    
    public override void Rotate()
    {
        switch (dir%4)
        {
            case 0:
                gauge.transform.localPosition = new Vector3(2, 4, 0);
                tubeH.SetActive(true);
                tubeH.transform.localScale = new Vector3(1,1,1);
                tubeV.SetActive(false);
                break;
            case 1:
                gauge.transform.localPosition = new Vector3(-2, 10, 0);
                tubeV.SetActive(true);
                tubeV.transform.localScale = new Vector3(1, 1, 1);
                tubeH.SetActive(false);
                break;
            case 2:
                gauge.transform.localPosition = new Vector3(-20, 4, 0);
                tubeH.SetActive(true);
                tubeH.transform.localScale = new Vector3(-1, 1, 1);
                tubeV.SetActive(false);
                break;
            case 3:
                gauge.transform.localPosition = new Vector3(2, -17, 0);
                tubeV.SetActive(true);
                tubeV.transform.localScale = new Vector3(1, -1, 1);
                tubeH.SetActive(false);
                break;
        }
       
    }

    public virtual void UpdateSuccess()
    {
        if (setPointLow < q && q < setPointHigh && itemBeingDragged == null)
            success = Mathf.Clamp(success + Time.deltaTime / successTime, 0, 1);
        else
            success = Mathf.Clamp(success - 10 * Time.deltaTime / successTime, 0, 1);
    }

    private void Update()
    {
        UpdateSuccess();
        animator.SetFloat("success",success);

        water2.GetComponent<Image>().color = PressureColor(p2);
        water.GetComponent<Image>().color = PressureColor(p2);

        float rate = Mathf.Clamp((q - PMin) / (PMax - PMin), 0, 0.99f);
        animator.SetFloat("rate", rate);


        float v = Mathf.Round(20 * q) / 20;
        value.GetComponent<Text>().text = v.ToString();

    }

}
