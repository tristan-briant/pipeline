using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour , IPointerDownHandler, IPointerUpHandler
{
    public float incrementMultiplier;
    float longPressDuration = 0.5f;
    bool pressing = false;

    public FloatParameterModifier fpm;
   

    float startPressTime;

    public void OnPointerDown(PointerEventData eventData)
    {
        pressing = true;
        startPressTime = Time.time;

        fpm.IncrementValue(incrementMultiplier);
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        pressing = false;
    }

    void Update()
    {
        if (pressing && (Time.time > startPressTime + longPressDuration)) //continuous press
        {
            fpm.IncrementValue(incrementMultiplier);
        }
    }
}
