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
    public float pMin=0;

    public float SetPointHigh { get => setPointHigh; set { setPointHigh = value; DrawCadran(); } }
    public float SetPointLow { get => setPointLow; set { setPointLow = value; DrawCadran(); } }
    public float PMax {
        get => pMax;
        set {
            if (pMax <= pMin) { pMax = pMin; isSuccess = false; }
            else { pMax = value; isSuccess = true; }
            DrawCadran();
            InitializeSuccess();
        }
    }
    public float PMin { get => pMin; set { pMin = value; DrawCadran(); } }

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

        animator = GetComponent<Animator>();
        animator.SetFloat("rate", 0.5f);

        shine.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        configPanel = Resources.Load("ConfigPanel/ConfigPressostat") as GameObject;

        base.Start();
        InitializeSuccess();

        success = 0;
        C = 0.05f;

    }

    void InitializeSuccess()
    {
        Transform successValue = transform.Find("Gauge/ValueHolder/SuccessValue");            
       
        if (successValue)
        {
            if (IsSuccess)
            {
                successValue.gameObject.SetActive(true);
            }
            else
                successValue.gameObject.SetActive(false);
        }

        success = 0;
        Rotate();
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


    private void Update()
    {

        if (setPointLow < q && q < setPointHigh && itemBeingDragged == null)
            success = Mathf.Clamp(success + Time.deltaTime/successTime, 0, 1);
        else
            success = Mathf.Clamp(success - 10 * Time.deltaTime / successTime, 0, 1);


        water2.GetComponent<Image>().color = PressureColor(p2);
        water.GetComponent<Image>().color = PressureColor(p2);

        float rate = Mathf.Clamp((q - PMin) / (PMax - PMin), 0, 0.99f);
        animator.SetFloat("rate", rate);


        float v = Mathf.Round(20 * q) / 20;
        value.GetComponent<Text>().text = v.ToString();


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
