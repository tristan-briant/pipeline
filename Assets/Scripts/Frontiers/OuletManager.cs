using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OuletManager : BaseFrontier
{

    public float pset = 1;
    public float Pset
    {
        get
        {
            return pset;
        }

        set
        {
            pset = value;
        }
    }
    float ppset;

    public float imax = 1f;
    public float Imax
    {
        get
        {
            return imax;
        }

        set
        {
            imax = value;
        }
    }



    //float pp = 0;
    float ii = 0;
    GameObject water, water0, arrow;
    public bool isSuccess = false;
    public bool jelly = false;
    Color jellyColor = new Color(0xFF / 255.0f, 0x42 / 255.0f, 0x6A / 255.0f);
    Color jellyColorBg = new Color(0x42 / 255.0f, 0x42 / 255.0f, 0x42 / 255.0f);
    public int mode = 0;
    public float periode = 2;

    

    public override void Calcule_i_p(float[] p, float[] i, float alpha)
    {
       /* float a = p[0];

        q += (i[0] + ii) * alpha;
        f = 0;

        p[0] = (q / C + (i[0] - f) * rin * 0.5f);
        //p[2] = (q / C + (i[2] + f) * R);

        i[0] = (f + (a - q / C) / (rin * 0.5f));
        ii = (-f + (pset - q / C) / (rin * 0.5f));
        */

        if (isSuccess)
        {
            if (Mathf.Abs(f) > 0.1)
                success = Mathf.Clamp(success + 0.005f, 0, 1);
            else
                success = Mathf.Clamp(success - 0.05f, 0, 1);
        }

    }

    float pp = 0;
    public override void Constraint(float[] p, float[] i, float dt)
    {
        if (Mathf.Abs(i[0]) < Imax)
        {
            pp = 0.01f * pset + 0.99f * pp;
            p[0] = pp;
            //ii = 0.01f * i[0] + 0.99f * ii;
        }
        else
        {
            pp = 0.01f * p[0] + 0.99f * pp;
            //ii = 0.01f * 0 + 0.99f * ii;
            //i[0] = ii;
            i[0] = Mathf.Clamp(i[0], -Imax, Imax);
        }
    }

    protected override void Start()
    {
        base.Start();
        water = this.transform.Find("Water").gameObject;
        water0 = this.transform.Find("Water0").gameObject;

        arrow = this.transform.Find("Arrow").gameObject;

    }


    private void Update()
    {

        if (!jelly)
        {
            water.GetComponent<Image>().color = PressureColor(pset);
            water0.GetComponent<Image>().color = PressureColor(pin[0]);
        }

        if (ppset <= 0)
            arrow.GetComponent<Animator>().SetBool("Negative", true);
        else
            arrow.GetComponent<Animator>().SetBool("Negative", false);


        
    }
}
