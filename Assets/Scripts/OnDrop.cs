using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnDrop : MonoBehaviour , IDropHandler {

    void IDropHandler.OnDrop(PointerEventData eventData)
    {

        if (transform.childCount == 0)
        {  // No child => empty // normally never happen
            BaseComponent.itemBeingDragged.transform.SetParent(transform);
        }
        else
        {
            GameObject item = transform.GetChild(0).gameObject;
            if (item.GetComponent<BaseComponent>().locked == false) { 
                item.transform.SetParent(BaseComponent.startParent);
                BaseComponent.itemBeingDragged.transform.SetParent(transform);
            }
        }

    }

}

