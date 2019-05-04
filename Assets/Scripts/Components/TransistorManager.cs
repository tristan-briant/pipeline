using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransistorManager : BaseComponent
{

    GameObject water, water1, water2, water3, bubble1, bubble2;

    //public float x_bulle2=0,x_bulle1 = 0;
    //float r_bulle = 0.1f;
    float q1, q2, q3, q11, q22;
    float i11, i22;
    public float xp;
    public float gain = 10;
    public float Gain { get => gain; set => gain = value; }
    float g, f1, f2 = 0, f3, f13 = 0;
    float threshold = 0.02f;
    //float fthreshold = 0.05f;
    public bool NPN = true;


    //float Rin=100;
    //float q12 = 0;//, q13 = 0;
    //public bool mirror=false;

    override public void Awake()
    {
        configPanel = Resources.Load("ConfigPanel/ConfigTransistor") as GameObject;
    }

    public  void Calcule_i_p_new(float[] p, float[] i, float dt)
    {
        /*if (mirror)
        {
            float e = p[0];
            p[0] = p[2];
            p[2] = e;
            e = i[0];
            i[0] = i[2];
            i[2] = e;
        }
        */

        //float a = p[1], b = p[2], c = p[3];
        float p1 = p[1], p2 = p[2], p3 = p[3];


        q1 += i[1] * dt;
        q2 += i[2] * dt;
        q3 += i[3] * dt;

        if (q2 - q3 > threshold)
        {
            float a = (q3 + q2) / 2;// + q1)/3;

            q3 = a;
            q2 = a;


            //f1 += (p[1] - q1) / L * dt;
            f2 += (p[2] - p[3]) / L * dt;
            //f3 += (p[3] - q3) / L * dt;

            //f1 = Mathf.Clamp(f1, 0, f2 * Gain);
            a = (q3 + q1) / 2;

            q3 = a;
            q1 = a;
            f13 += (p[1] - p[3]) / L * dt;
        }
        else
        {
            f2 = 0;
            //f13 = f1 = f3 = 0;
        }

        f13 = Mathf.Clamp(f13, 0, gain * f2);

        p[1] = (q1 / C + (i[1] - f13) * R);
        p[2] = (q2 / C + (i[2] - f2) * R);
        p[3] = (q3 / C + (i[3] + f2 + f13) * R);

        i[1] = (f13 + (p1 - q1 / C) / R);
        i[2] = (f2 + (p2 - q2 / C) / R);
        i[3] = (-f2 -f13 + (p3 - q3 / C) / R);

        /*p[1] = (q1 / C + (i[1] - f1) * R);
        p[2] = (q2 / C + (i[2] - f2) * R);
        p[3] = (q3 / C + (i[3] - f3) * R);

        i[1] = (f1 + (p1 - q1 / C) / R);
        i[2] = (f2 + (p2 - q2 / C) / R);
        i[3] = (f3 + (p3 - q3 / C) / R);*/

        /*if (NPN)
        {
            g = q2 - q3;

            if (g > 0)
            {
                q = (q2 + q3) / 2;
                q2 = q;
                q3 = q;
                f2 += (p[2] - p[3]) / L * dt;
            }
            else
            {
                f2 = 0;
            }

            if (g > 0)
            {
                float Res;
                if (f2 > 0.001f)
                    Res = Gain / f2;
                else
                    Res = 10000000;

                p[1] = (q / C + (i[1]) * Res * 0.5f);
                p[3] = (q / C + (i[3]) * Res * 0.5f);

                i[1] = (+(a - q / C) / Res * 2);
                i[3] = (+(b - q / C) / Res * 2);

                q = q1;
                q1 = 0.5f * q1 + 0.5f * q3;
                q3 = 0.5f * q3 + 0.5f * q;

                f13 += (p[1] - p[3]) / L * dt;
            }
            else
            {
                f13 = 0;
            }

            f13 = Mathf.Clamp(f13, 0, Gain * f2);

            p[1] = (q1 / C + (i[1] - f13) * R);
            p[2] = (q2 / C + (i[2] - f2) * R);
            p[3] = (q3 / C + (i[3] + f13 + f2) * R);

            i[1] = (f13 + (a - q1 / C) / R);
            i[2] = (f2 + (b - q2 / C) / R);
            i[3] = (-f2 - f13 + (c - q3 / C) / R);
        }
        else
        {
            g = q1 - q2;

            if (g > 0)
            {
                q = (q1 + q2) / 2;
                q1 = q;
                q2 = q;
                f2 += (p[1] - p[2]) / L * dt;
            }
            else
            {
                f2 = 0;
            }

            if (g > threshold)
            {
                q = q1;
                q1 = 0.9f * q1 + 0.1f * q3;
                q3 = 0.9f * q3 + 0.1f * q;

                f13 += (p[1] - p[3]) / L * dt;
            }
            else
            {
                f13 = 0;
            }

            f13 = Mathf.Clamp(f13, 0, Gain * f2);

            p[1] = (q1 / C + (i[1] - f2 - f13) * R);
            p[2] = (q2 / C + (i[2] + f2) * R);
            p[3] = (q3 / C + (i[3] + f13) * R);

            i[1] = (f13 + f2 + (a - q1 / C) / R);
            i[2] = (-f2 + (b - q2 / C) / R);
            i[3] = (-f13 + (c - q3 / C) / R);

        }*/
    }


    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        /*if (mirror)
        {
            float e = p[0];
            p[0] = p[2];
            p[2] = e;
            e = i[0];
            i[0] = i[2];
            i[2] = e;
        }*/

        p1 = p[1];
        p2 = p[2];
        p3 = p[3];

        q1 += (i[1] - f1)  * dt;
        q2 += i[2]  * dt;
        //q11 -= i11 * dt;
        q22 -= i22  * dt;

        q += (i[3] + i22 + f1) * dt;

        /*if (q1 > q11)
        {
            q1 = q11 = (q1 + q11) / 2;
            f1 += (p[1] - q) / L * dt;
        }
        else
        {
            f1 = 0;
        }*/

 
        if (q2 - q22> 0)
        {
            q2 = q22 = (q2 + q22) / 2;
            f2 += (p[2] - q) / L * dt;
        }
        else
        {
            f2 = 0;
        }

        f1 += (p[1] - q) / L * dt;
        f1 = Mathf.Clamp(f1, 0, gain * f2);

        f3 += (p[3] - q) / L * dt;


        p[1] = (q1 / C + (i[1] - f1) * R);
        p[2] = (q2 / C + (i[2] - f2) * R);
        p[3] = (q / C + (i[3] - f3) * R);

        //i[1] = (f1 + (p1 - q1 / C) / R);
        i[1] = f1;
        i[2] = (f2 + (p2 - q2 / C) / R);
        i[3] = (f3 + (p3 - q / C) / R);

        //i11= (f1 + (q11 - q / C) / R);
        i22= (f2 + (q22 - q / C) / R);

        /*if (mirror)
        {
            float e = p[0];
            p[0] = p[2];
            p[2] = e;
            e = i[0];
            i[0] = i[2];
            i[2] = e;
        }*/

        return;


        /* C = 0.3f;
         R = 0.1f;*/
        /*if (mirror) {
            float e = p[0];
            p[0] = p[2];
            p[2] = e;
            e = i[0];
            i[0] = i[2];
            i[2] = e;
        }*/

       

        q1 += (i[1]) * dt;
        q3 += (i[3]) * dt;
        q2 += (i[2]) * dt;

        if (NPN)
        {
            g = q2 - q3;

            if (g > 0)
            {
                q = (q2 + q3) / 2;
                q2 = q;
                q3 = q;
                f2 += (p[2] - p[3]) / L * dt;
            }
            else
            {
                f2 = 0;
            }

            if (g > threshold)
            {
                q = q1;
                q1 = 0.5f * q1 + 0.5f * q3;
                q3 = 0.5f * q3 + 0.5f * q;

                f13 += (p[1] - p[3]) / L * dt;
            }
            else
            {
                f13 = 0;
            }

            f13 = Mathf.Clamp(f13, 0, gain * f2);

            p[1] = (q1 / C + (i[1] - f13) * R);
            p[2] = (q2 / C + (i[2] - f2 ) * R);
            p[3] = (q3 / C + (i[3] + f13 + f2) * R);

            i[1] = (f13 + (p1 - q1 / C) / R);
            i[2] = (f2 + (p2 - q2 / C) / R);
            i[3] = (-f2 - f13 + (p3 - q3 / C) / R);
        }
        else
        {
            g = q1 - q2;

            if (g > 0) 
            {
                q = (q1 + q2) / 2;
                q1 = q;
                q2 = q;
                f2 += (p[1] - p[2] ) / L * dt;
            }
            else
            {
                f2 = 0;
            }

            if (g>threshold) {
                q = q1;
                q1 = 0.9f * q1 + 0.1f * q3;
                q3 = 0.9f * q3 + 0.1f * q;

                f13 += (p[1] - p[3] ) / L * dt;
            } 
            else
            {
                f13 = 0;
            }

            f13 = Mathf.Clamp(f13, 0 , gain * f2);

            p[1] = (q1 / C + (i[1] - f2 - f13) * R);
            p[2] = (q2 / C + (i[2] + f2 ) * R);
            p[3] = (q3 / C + (i[3] + f13 ) * R);

            i[1] = ( f13 + f2 + (p1 - q1 / C) / R);
            i[2] = ( - f2 + (p2 - q2 / C) / R);
            i[3] = (-f13  + (p3 - q3 / C) / R);

        }
           
        
        /*
        if (mirror)
        {
            float e = p[0];
            p[0] = p[2];
            p[2] = e;
            e = i[0];
            i[0] = i[2];
            i[2] = e;
        }*/


    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        if (mirror)
            i[2] = 0;
        else
            i[0] = 0;
    }

    public override void Rotate()
    {
        transform.localRotation = Quaternion.Euler(0, 0, dir * 90);

        if(!mirror)
            foreach (Transform child in transform)
                child.localScale = Vector3.one;
        else
            foreach (Transform child in transform)
                child.localScale = new Vector3(-1, 1, 1);

    }

    public override void OnClick()
    {
       
            dir = (dir + 1) % 4;

        if (dir == 2 || dir == 3)
            mirror = true;
        else
            mirror = false;


            Rotate();

            audios[0].Play();
    }


    protected override void Start()
    {
        base.Start();
        water = transform.Find("Water").gameObject;
        water1 = transform.Find("Water1").gameObject;
        water2 = transform.Find("Water2").gameObject;
        water3 = transform.Find("Water3").gameObject;
        bubble1 = transform.Find("Bubble1").gameObject;
        bubble2 = transform.Find("Bubble2").gameObject;
    }

    private void Update()
    {
        water1.GetComponent<Image>().color = PressureColor(p1);
        water2.GetComponent<Image>().color = PressureColor(p2);
        water3.GetComponent<Image>().color = PressureColor(p3);
        water.GetComponent<Image>().color = PressureColor(q);

        GetComponent<Animator>().SetFloat("open",Mathf.Clamp(f2,0,0.99f));

        bubble1.GetComponent<Animator>().SetFloat("speed",SpeedAnim(f1));
        bubble2.GetComponent<Animator>().SetFloat("speed", SpeedAnim(f2));
        
    }
}
