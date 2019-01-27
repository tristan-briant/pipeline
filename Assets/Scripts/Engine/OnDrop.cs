using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnDrop : MonoBehaviour , IDropHandler {

    public bool isSlotFrontier = false;

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        if (BaseComponent.itemBeingDragged)
        {
            if (BaseComponent.itemBeingDragged.GetComponent<BaseComponent>().isFrontiers == isSlotFrontier)
            { // component and slot of the same type
                BaseComponent.endParent = transform;
            }
        }
    }


}

