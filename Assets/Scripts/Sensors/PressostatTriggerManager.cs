using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class PressostatTriggerManager : PressostatManager {

    public float raiseTime = 5.0f;
    public float RaiseTime { get => raiseTime; set => raiseTime = Mathf.Clamp(value, 1f, 20f); }

    float tolerance = 0.15f;
    bool rising = false;

    public override void Awake()
    {
        symmetric = false;
    }

    protected override void Start()
    {
        base.Start();
        configPanel = Resources.Load("ConfigPanel/ConfigPressostatTrigger") as GameObject;
        IsSuccess = true;
    }

    void CalculateSetPoints(float time)
    {
        setPointHigh = pMax * (time / raiseTime + tolerance);
        setPointLow = pMax * (time / raiseTime - tolerance);
        DrawCadran();
    }


    public void TriggerStart()
    {
        Debug.Log("message reçu");
        StartCoroutine(countDown());
        rising = true;
    }

    public void TriggerEnd()
    {
        StopCoroutine(countDown());
        rising = false;
    }

    IEnumerator countDown() {
        float time = 0;

        while (true)
        {
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
            CalculateSetPoints(time);
        }
        
    }

   

    override public void UpdateSuccess()
    {
        if (!rising || itemBeingDragged!=null)
            success = 0;
        else if (setPointLow < q && q < setPointHigh)
            success = Mathf.Clamp01(success + Time.deltaTime / RaiseTime * 1.1f);


    }

    /*private void Update()
    {
        if (trigged)
        {
            trigged = false;
            activated = true;
            StartCoroutine(countDown());
        }

        if (activated)
        {
            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }


        water2.GetComponent<Image>().color = PressureColor(pin[2]);

        if (activated)
            cadran2.GetComponent<Image>().color = new Color(0,255,0);
        else
            cadran2.GetComponent<Image>().color = new Color(255, 255, 255);


        float rate = Mathf.Clamp((q - PMin) / (PMax - PMin), 0, 1);

       
        Vector3 pos = new Vector3(0, rate * 100f, 0);
        arrow.transform.localPosition = arrowStartPosition + pos;

        pos = new Vector3(0, risingCurve(time) * 100f, 0);

        cadran.transform.localPosition = cadranStartPosition+ pos;
        cadran2.transform.localPosition = cadranStartPosition+  pos;

        water.GetComponent<Image>().fillAmount = (1 - 0.9f * rate);

        float alpha;

        if (success < 1)
        {
            alpha = success;
            t_shine = 0;
        }
        else
        {
            t_shine += Time.deltaTime;
            alpha = 0.8f + 0.2f * Mathf.Cos(t_shine * 5.0f);

        }

        shine.GetComponent<Image>().color = new Color(1, 1, 1, alpha);
        
    }*/

    /*float risingCurve(float t) {
        return setPoint*(1 - Mathf.Exp ( -t / Tau) );
    }*/

}
