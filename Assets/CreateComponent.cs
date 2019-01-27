using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class CreateComponent : MonoBehaviour, IBeginDragHandler , IDragHandler , IEndDragHandler
{

    public string PrefabComponentPath = "";

    GameObject NewComponent;

    public void Start()
    {
        PrefabComponentPath = transform.GetComponentInChildren<BaseComponent>().PrefabPath;
        Destroy(transform.GetChild(0).GetComponent<BaseComponent>());
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        NewComponent = PrefabUtility.InstantiatePrefab(Resources.Load(PrefabComponentPath, typeof(GameObject))) as GameObject;
      
        Transform canvas = GameObject.FindGameObjectWithTag("Playground").transform;
        NewComponent.transform.SetParent(canvas);

        NewComponent.transform.localPosition = Vector3.zero;
        NewComponent.transform.localScale = Vector3.one;
        NewComponent.GetComponent<BaseComponent>().dir = 0;
        NewComponent.transform.localRotation = Quaternion.Euler(0, 0, 0);

        BaseComponent.itemBeingDragged = NewComponent;
        BaseComponent.startParent = null;
        BaseComponent.endParent = null;
    }


    public void OnDrag(PointerEventData eventData)
    {

        Vector3 vec = Input.mousePosition;
        vec.z = 1.0f;
        NewComponent.transform.position = Camera.main.ScreenToWorldPoint(vec);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        NewComponent.GetComponent<BaseComponent>().Drop();
    }
}
