using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ToTrash : MonoBehaviour, IDropHandler
{

    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        Destroy(BaseComponent.itemBeingDragged.gameObject);

    }

}
