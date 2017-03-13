using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InletManager : BaseFrontier {

    public float pset;
    public float imax=1;
    float pp = 0;
    
    GameObject water,water0, arrow;


    public override void calcule_i_p(float[] p, float[] i)
    {


        if (i[0] < imax && i[0] > -imax)
        {
            p[0] = pp;
            pp = 0.9f * pp + 0.1f * pset; // tending to pin
        }
        else
        {
            pp = 0.9f * pp;
            p[0] = pp;
        }
        //i[0]=constrain(i[0],-imax,imax);

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
