using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorManager :  BaseComponent {

    GameObject water0, water2, shine, helice;
    float velocity = 0;
    float setPointLow=0.001f;
    public float chargeSuccess = 5f;
    public float ChargeSuccess { get => chargeSuccess; set => chargeSuccess = Mathf.Clamp(value, 0.5f, 30f); }

    float t_shine = 0;

    Animator bubbleAnimator;

    public override void Awake()
    {
        Color col = transform.Find("Shine").GetComponent<Image>().color;
        col.a = 0;
        transform.Find("Shine").GetComponent<Image>().color = col;
    }

    protected override void Start()
    {
        base.Start();
        water0 = transform.Find("Water0").gameObject;
        water2 = transform.Find("Water2").gameObject;

        shine = transform.Find("Shine").gameObject;
        helice = transform.Find("Helice").gameObject;

        bubbleAnimator = transform.Find("Bubble").GetComponent<Animator>();

        configPanel = Resources.Load<GameObject>("ConfigPanel/ConfigGenerator");
    }


    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        const float Res = 0.5f;

        p0 = p[0];
        p2 = p[2];

        q += (i[0] + i[2]) / C * dt;
        f = (i[0] - i[2]) / 2;

        p[0] = (q + i[0] * Res * 0.5f);
        p[2] = (q + i[2] * Res * 0.5f);

        i[0] = (p0 - q) / Res * 2;
        i[2] = (p2 - q) / Res * 2;
    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        i[1] = i[3] = 0;
    }

    

    float angle=0;


    private void Update()
    {

        water0.GetComponent<Image>().color = PressureColor(pin[0]);
        water2.GetComponent<Image>().color = PressureColor(pin[2]);

        angle += 3.14f * velocity * 3;
        helice.transform.localEulerAngles = new Vector3(0, 0, angle);


        const float delta= 0.1f;
        velocity = (1 - delta) * velocity + delta * (f < 0 ? -f : 0);

        if (setPointLow < velocity && itemBeingDragged == null)
            success = Mathf.Clamp01(success - f * Time.deltaTime / chargeSuccess);
        else
            success = Mathf.Clamp01(success + (-1 + f) * Time.deltaTime);
            
        //success -= f * Time.deltaTime / chargeSuccess;

        bubbleAnimator.GetComponent<Animator>().SetFloat("speed", -SpeedAnim());

        
        float alpha;

        if (success < 1)
        {
            alpha = success;
            t_shine = 0;
        }
        else
        {
            t_shine += Time.deltaTime/chargeSuccess;
            alpha = 0.8f + 0.2f * Mathf.Cos(t_shine * 5.0f);

        }

        Color col = shine.GetComponent<Image>().color;
        col.a = alpha;

        shine.GetComponent<Image>().color = col;
    }

}