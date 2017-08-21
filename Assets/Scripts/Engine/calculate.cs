using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class calculate : MonoBehaviour {
    
    static float q = 0, f = 0; 
    int dir;
    public float R = 1, L = 1, C = 1, Rground = 50;
    public float c=0.0f;


    GameObject water;


    public void calcule_i_p(float[] p, float[] i)
    {
        float a = p[0], b = p[2];

        q += (i[0] + i[2]) / C; //q*=0.99;
        f += (p[0] - p[2]) / L;

        p[0] = (q + (i[0] - f) * R);
        p[2] = (q + (i[2] + f) * R);

        i[0] = (f + (a - q) / R);
        i[2] = (-f + (b - q) / R);

        //i[1]=i[3]=0;
        i[1] = p[1] / Rground;
        i[3] = p[3] / Rground;
    }

   /* private void Start()
    {
        name = "pipeline";
        water = this.transform.FindChild("Water").gameObject;

    }

    private void Update()
    {
        c += 0.01f;
        if (c > 1) c = 0.0f;
        water.GetComponent<Image>().color = new Color(1,c,1);
    }*/


}

