using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InletManager : BaseFrontier {

    public float pset;
    public float imax=1;
    float pp = 0;
    float ii=0;
    GameObject water,water0, arrow;
    public bool isSuccess=false;


    public override void calcule_i_p(float[] p, float[] i)
    {
        float a = p[0];


        if (ii < imax && ii > -imax)
        {
           pp=pset;
            //pp = 0.5f * pp + 0.5f * pset; // tending to pin
            
        }
        else
        {
            pp = 0.9f * pp;
            //p[0] = pp;*/
            //i[0] = Mathf.Clamp(i[0], -imax, imax);

        }

 
        q += (i[0] + ii) / C;
        f += (p[0] - pp) / L;

        p[0] = (q + (i[0] - f) * R);
        //p[2] = (q + (i[2] + f) * R);

        i[0] = (f + (a - q) / R);
        ii = (-f + (pp - q) / R);


        if(isSuccess)
        {
            if ( Mathf.Abs(f) > 0.1)
                success = Mathf.Clamp(success + 0.005f, 0, 1);
            else
                success = Mathf.Clamp(success - 0.05f, 0, 1);
        }
        
    }

    protected override void Start()
    {
        base.Start();
        water = this.transform.FindChild("Water").gameObject;
        water0 = this.transform.FindChild("Water0").gameObject;

        arrow = this.transform.FindChild("Arrow").gameObject;
    }
        

    private void Update()
    {
        water.GetComponent<Image>().color = pressureColor(pset);
        water0.GetComponent<Image>().color = pressureColor(pin[0]);

        if (pset <= 0)
        {
            arrow.transform.localScale = new Vector3(-1,1,1);
            arrow.transform.localPosition = new Vector3(25 - (0.025f - 0.05f * Mathf.Sqrt(Mathf.Abs(Mathf.Sin(Time.time / 0.5f)))) * 100, 0, 0);

        }
        else
            arrow.transform.localPosition = new Vector3(25+ (0.025f - 0.05f * Mathf.Sqrt(Mathf.Abs(Mathf.Sin(Time.time / 0.5f)))) * 100 ,0,0);
    }


}
