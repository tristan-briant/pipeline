﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragThresholdUtil : MonoBehaviour {

    void Start()
    {
        int defaultValue = EventSystem.current.pixelDragThreshold;
        EventSystem.current.pixelDragThreshold =
                Mathf.Max(
                     defaultValue,
                     (int)(defaultValue * Screen.dpi / 80f));

        //EventSystem.current.pixelDragThreshold = 10;
        Debug.Log("Screen DPI = " +  Screen.dpi);
    }
}
