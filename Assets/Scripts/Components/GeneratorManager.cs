using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorManager :  BaseComponent {

    GameObject water0, water2, shine, helice;
    float velocity = 0;
    public float setPointLow=0.1f;
    public float timeSuccess = 5f;
    public float TimeSuccess { get => timeSuccess; set => timeSuccess = Mathf.Clamp(value, 0.5f, 30f); }

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
        float p0 = p[0], p2 = p[2];

        q += (i[0] + i[2]) / C * dt; 
        f += (p[0] - p[2]) / L * dt;

        p[0] = (q + (i[0] - f) * R);
        p[2] = (q + (i[2] + f) * R);

        i[0] = (f + (p0 - q) / R);
        i[2] = (-f + (p2 - q) / R);

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

        angle += 3.14f * velocity*3;
        helice.transform.localEulerAngles = new Vector3(0, 0, angle);


        const float delta= 0.1f;
        velocity = (1 - delta) * velocity + delta * (f < 0 ? -f : 0);

        if (setPointLow < velocity && itemBeingDragged == null)
            success = Mathf.Clamp01(success + Time.deltaTime/timeSuccess);
        else
            success = Mathf.Clamp(success - 2 * Time.deltaTime / timeSuccess, 0, 1);

        bubbleAnimator.GetComponent<Animator>().SetFloat("speed", -SpeedAnim());

        
        float alpha;

        if (success < 1)
        {
            alpha = success;
            t_shine = 0;
        }
        else
        {
            t_shine += Time.deltaTime/timeSuccess;
            alpha = 0.8f + 0.2f * Mathf.Cos(t_shine * 5.0f);

        }

        Color col = shine.GetComponent<Image>().color;
        col.a = alpha;

        shine.GetComponent<Image>().color = col;
    }

}