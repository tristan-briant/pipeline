using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnDrop : MonoBehaviour , IDropHandler {

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        BaseComponent.endParent = transform;

        //if (BaseComponent.itemBeingDragged != null) //Le Drop vient d'un composant
        {

           /* if (transform.childCount == 0)
            {  // No child => empty // normally never happen
                BaseComponent.itemBeingDragged.transform.SetParent(transform);
            }
            else
            {*/

            /*GameObject item = transform.GetChild(0).gameObject;
            if (item.GetComponent<BaseComponent>().locked == false)
            {
                if (BaseComponent.startParent)
                {
                    item.transform.SetParent(BaseComponent.startParent);
                    item.transform.localPosition = Vector3.zero;
                }
                else
                    Destroy(item); // Nulpart où aller (vient du designer) on le vire

                BaseComponent.itemBeingDragged.transform.SetParent(transform);
            }*/
            //}
            //BaseComponent.itemBeingDragged.transform.localPosition = Vector3.zero;
        }
    }


}

