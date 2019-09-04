using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InductorManager : BaseComponent
{

    GameObject water, water0, water2;
    public float x_bulle = 0;
    public float lin = 10;
    public float rin = 10f;
    //float r_bulle = 0.1f;
    //float angle;
    Animation anim;

    public float Lin { get => lin; set => lin = value; }
    public float Rin { get => rin; set => rin = value; }

    public override void Awake()
    {
        configPanel = Resources.Load("ConfigPanel/ConfigInductor") as GameObject;
        tubeEnd[0] = tubeEnd[2] = true;
    }


    protected override void Start()
    {
        base.Start();
        water0 = transform.Find("Water0").gameObject;
        water2 = transform.Find("Water2").gameObject;
        water = transform.Find("Water").gameObject;

        GetComponent<Animator>().SetFloat("speed", 0);
        rin = 20f;
    }

    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        //R = 16f;//C = 2;
        p0 = p[0];
        p2 = p[2];

        q += (i[0] + i[2]) / C * dt;
        f += (p[0] - p[2]) / lin * dt;

        p[0] = (q + (i[0] - f) * rin);
        p[2] = (q + (i[2] + f) * rin);

        i[0] = (f + (p0 - q) / rin);
        i[2] = (-f + (p2 - q) / rin);

    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        i[1] = 0;
        i[3] = 0;
    }

    private void Update()
    {
        water.GetComponent<Image>().color = PressureColor(q);
        water0.GetComponent<Image>().color = PressureColor(p0);
        water2.GetComponent<Image>().color = PressureColor(p2);

        GetComponent<Animator>().SetFloat("speed", -SpeedAnim());
    }

}

