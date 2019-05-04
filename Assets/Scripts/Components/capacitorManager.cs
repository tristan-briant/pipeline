using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class capacitorManager : BaseComponent {

    GameObject waterIn0, waterIn2, water0, water2, bubble0, bubble2;
    GameObject spring1, spring2, spring3, spring4, piston;

    float f0, f2;
    public float cin;
    private float rin = 0.025f;

    float q0, q2;
    //float xp;

    public float Cin { get => cin / Engine.TimeFactor(); set { cin = value * Engine.TimeFactor(); UpdateValue(); } }
    public float Rin { get => rin; set => rin = value; }

    public override void Awake()
    {
        configPanel = Resources.Load("ConfigPanel/ConfigCapacity") as GameObject;
        GetComponent<Animator>().SetFloat("position", 0.5f);
        UpdateValue();
        //Debug.Log("factor = " +  Engine.TimeFactor());
    }


    protected override void Start()
    {
        base.Start();
        waterIn0 = transform.Find("Tank/Water-in0").gameObject;
        waterIn2 = transform.Find("Tank/Water-in2").gameObject;
        water0 = transform.Find("Water0").gameObject;
        water2 = transform.Find("Water2").gameObject;
        /*spring1 = transform.Find("Tank/Spring1").gameObject;
        spring2 = transform.Find("Tank/Spring2").gameObject;
        spring3 = transform.Find("Tank/Spring3").gameObject;
        spring4 = transform.Find("Tank/Spring4").gameObject;
        piston = transform.Find("Tank/Piston").gameObject;*/
        bubble0 = transform.Find("Tank/Mirror/Bubble0").gameObject;
        bubble2 = transform.Find("Tank/Bubble2").gameObject;
        bubble0.GetComponent<Animator>().SetFloat("speed", 0);
        bubble2.GetComponent<Animator>().SetFloat("speed", 0);

        UpdateValue();
        C = 0.5f;
    }


    public override void Reset_i_p()
    {
        base.Reset_i_p();
        q0 = q2 = 0;
    }

    public override void BlockCurrant()
    {
        f = f0 = f2 = 0;
    }

    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        p0 = p[0];
        p2 = p[2];

        q += (i[0] + i[2]) / C * dt;
        q0 += i[0] / cin  / 2 * dt;
        q2 += i[2] / cin /  2 * dt;


        p[0] = (q + q0) + i[0] * rin;
        p[2] = (q + q2) + i[2] * rin;

        i[0] = (p0 - q - q0) / rin;
        i[2] = (p2 - q - q2) / rin;

        f0 = i[0];
        f2 = i[2];
    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        i[1] = i[3] = 0;
    }

    float Sature(float x) {
        if (x > 0)
            return x / (1 + x);
        else
            return x / (1 - x);
    }

    public override void Rotate()
    {
        base.Rotate();
        transform.Find("Value").rotation = Quaternion.identity;
    }


    public void UpdateValue() {

        float size = Sature(0.25f * Cin);
        GetComponent<Animator>().SetFloat("size", size);

        GetComponentInChildren<Text>().text =  (Mathf.Round(10 * Cin) / 10 ).ToString();
        
    }

    private void Update()
    {
        waterIn0.GetComponent<Image>().color = PressureColor(q0 + q);
        waterIn2.GetComponent<Image>().color = PressureColor(q2 + q);
        water0.GetComponent<Image>().color = PressureColor(p0);
        water2.GetComponent<Image>().color = PressureColor(p2);

        GetComponent<Animator>().SetFloat("position", 0.5f*(1 + Sature((q2-q0)*2)));


        bubble0.GetComponent<Animator>().SetFloat("speed", SpeedAnim(f0));
        bubble2.GetComponent<Animator>().SetFloat("speed", SpeedAnim(f2));

    }
}
