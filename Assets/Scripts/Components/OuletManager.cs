using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OuletManager : BaseFrontier
{

    public float pset;
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

    public float rin = 0.1f;
    public float Rin
    {
        get
        {
            return rin;
        }

        set
        {
            rin = value;
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



    IEnumerator generateRandom()
    {
        int s = 1;

        while (true)
        {
            ppset = pset * s;
            float time = Random.Range(0, periode);
            yield return new WaitForSeconds(time);

            const float N = 10;
            for (int i = 0; i < N; i++)
            {
                ppset = pset * (1 - 2 * i / N) * s;
                yield return new WaitForSeconds(0.02f);  // retournement en douceur
            }
            s = -s;
        }
    }


    public override void Calcule_i_p(float[] p, float[] i, float alpha)
    {

      

        float a = p[0];

        q += (i[0] + ii) * alpha;
        //f += (p[0] - pset) / L * alpha;
        f = 0;

        p[0] = (q / C + (i[0] - f) * rin);
        //p[2] = (q / C + (i[2] + f) * R);

        i[0] = (f + (a - q / C) / rin);
        ii = (-f + (pset - q / C) / rin);

        

        if (isSuccess)
        {
            if (Mathf.Abs(f) > 0.1)
                success = Mathf.Clamp(success + 0.005f, 0, 1);
            else
                success = Mathf.Clamp(success - 0.05f, 0, 1);
        }

    }

    protected override void Start()
    {
        base.Start();
        water = this.transform.Find("Water").gameObject;
        water0 = this.transform.Find("Water0").gameObject;

        arrow = this.transform.Find("Arrow").gameObject;

        if (jelly)
        {
            if (pset > 0)
            {
                water.GetComponent<Image>().color = jellyColor;
                water0.GetComponent<Image>().color = jellyColor;
            }
            else
            {
                water.GetComponent<Image>().color = jellyColorBg;
                water0.GetComponent<Image>().color = jellyColorBg;
            }
        }

        if (mode == 2) StartCoroutine(generateRandom());
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


        /*switch (mode)
        {
            case 0:  // Mode normal
                if (ppset <= 0)
                    arrow.transform.localScale = new Vector3(-1, 1, 1);
                else
                    arrow.transform.localScale = new Vector3(1, 1, 1);
                break;
            case 1: //mode périodique
            case 2:
                arrow.transform.localScale = new Vector3(Mathf.Clamp(ppset,-1,1), 1, 1);
                break;
        }

        if (ppset <= 0)
        {
            arrow.transform.localPosition = new Vector3(25 - (0.025f - 0.05f * Mathf.Sqrt(Mathf.Abs(Mathf.Sin(Time.time / 0.5f)))) * 100, 0, 0);

        }
        else
            arrow.transform.localPosition = new Vector3(25+ (0.025f - 0.05f * Mathf.Sqrt(Mathf.Abs(Mathf.Sin(Time.time / 0.5f)))) * 100 ,0,0);
            */
    }
}
