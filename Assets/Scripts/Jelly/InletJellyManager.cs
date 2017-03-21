using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InletJellyManager : BaseFrontier {

    public float pset;
    public float imax = 1;
    float pp = 0;
    float ii = 0;
    GameObject jelly0, jelly2, arrow;
    public bool isSuccess = false;
    public float Capa=100;
    bool full = false;
    bool entered = false;


    protected override void Start()
    {
        base.Start();
        jelly0 = this.transform.FindChild("Jelly0").gameObject;
        jelly2 = this.transform.FindChild("Jelly2").gameObject;

        jelly0.GetComponent<Image>().fillAmount = 0;
        jelly2.GetComponent<Image>().fillAmount = 0;

        arrow = this.transform.FindChild("Arrow").gameObject;

    }

    public override void calcule_i_p(float[] p, float[] i)
    {
        if (pset > 0)
            ii = 1;
        else
            ii = 0;

        q += (i[0] + ii) / Capa;

        if (q > 0 && !entered)
        {
            entered = true;
            if (p[0] > 0) f = 1;
            else f = -1;
        }

        if (q >= 1 && !full)
        {             //pipe is full
            full = true;
        }
        
        if (f < 0 && q >= 1)
        {
            p[0] = 1;
            i[0] = -1;
        }

        if (isSuccess)
        {
            if (full)
                success = 1;
            else
                success = 0;
        }

        if (q > 1 + 2 / Capa)
            fail = 1;

    }

    // Update is called once per frame
    private void Update()
    {
        if (f > 0) jelly0.GetComponent<Image>().fillAmount = q;
        if (f < 0) jelly2.GetComponent<Image>().fillAmount = q;

        /*if (q > 1+2/Capa)
        {
            jelly0.GetComponent<Image>().color = new Color(255, 255, 255);
            jelly2.GetComponent<Image>().color = new Color(255, 255, 255);
        }*/

        if (pset <= 0)
        {
            arrow.transform.localScale = new Vector3(-1, 1, 1);
            arrow.transform.localPosition = new Vector3(25 - (0.025f - 0.05f * Mathf.Sqrt(Mathf.Abs(Mathf.Sin(Time.time / 0.5f)))) * 100, 0, 0);

        }
        else
            arrow.transform.localPosition = new Vector3(25 + (0.025f - 0.05f * Mathf.Sqrt(Mathf.Abs(Mathf.Sin(Time.time / 0.5f)))) * 100, 0, 0);
    }
}
