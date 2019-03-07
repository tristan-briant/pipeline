using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ValveManager : BaseComponent {

    GameObject water0, water2, bubble,tubeOpen,tubeClosed;
    //public float x_bulle = 0;
    //float r_bulle = 0.1f;
    public bool open=false;
    float q0=0, q2=0;
    public float openTime=4.0f;

    public override void Reset_i_p()
    {
        base.Reset_i_p();
        q0 = q2 = 0;
    }

    public override void Calcule_i_p(float[] p, float[] i, float dt)
    {
        p0 = p[0];
        p2 = p[2];
        
        //q0 += i[0] * dt;
        //q2 += i[2] * dt;

        q0 += (i[0] - f) / C * dt;
        q2 += (i[2] + f) / C * dt;
        f += (p[0] - p[2]) / L * dt;

        if (open)
            q0 = q2 = (q0 + q2) / 2;
        else
            f = 0;

        p[0] = (q0 / C + (i[0] - f) * R);
        p[2] = (q2 / C + (i[2] + f) * R);

        i[0] = (f + (p0 - q0) / R);
        i[2] = (-f + (p2 - q2) / R);

    }

    public override void Constraint(float[] p, float[] i, float dt)
    {
        i[1] = i[3] = 0;
    }

    protected override void Start()
    {
        base.Start();
        water0 = transform.Find("Water0").gameObject;
        water2 = transform.Find("Water2").gameObject;

        bubble = transform.Find("Bubble").gameObject;
        tubeOpen = transform.Find("TubeOpen").gameObject;
        tubeClosed = transform.Find("TubeClosed").gameObject;
    }


    public void TriggerStart()
    {
        Debug.Log("message reçu");
        open = true;
        tubeOpen.SetActive(true);
        tubeClosed.SetActive(false);
    }

    public void TriggerEnd()
    {
        open = false;
        tubeOpen.SetActive(false);
        tubeClosed.SetActive(true);
    }

    IEnumerator countDown() {
        yield return new WaitForSeconds(openTime);

        open = false;
    }

    private void Update()
    {
        /*if (trigged) {
            trigged = false;
            open = true;
            StartCoroutine(countDown());
        }*/

        water0.GetComponent<Image>().color = PressureColor(p0);
        water2.GetComponent<Image>().color = PressureColor(p2);
        
        bubble.GetComponent<Animator>().SetFloat("speed", -SpeedAnim());

    }
}
