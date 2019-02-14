using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

public class CreateComponent : MonoBehaviour, IBeginDragHandler , IDragHandler , IEndDragHandler, IDropHandler
{
    public bool designermode=false;

    public string PrefabComponentPath = "";

    GameObject NewComponent;

    public void Start()
    {
        if (transform.childCount > 0)
        {
            transform.GetChild(0).GetComponent<BaseComponent>().Awake();
            transform.GetChild(0).GetComponent<BaseComponent>().enabled = false;
            transform.GetChild(0).localPosition = Vector3.zero;
            transform.GetChild(0).localScale = 0.8f * Vector3.one;
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {

        if (transform.childCount == 0)
        {
            eventData.pointerDrag = null;
            return;
        }


        NewComponent = Instantiate(transform.GetChild(0).gameObject);

        if (designermode) // To allow reodering or removing
            Destroy(transform.GetChild(0).gameObject);


        NewComponent.GetComponent<BaseComponent>().enabled = true;
        Transform canvas = GameObject.FindGameObjectWithTag("Playground").transform;
        NewComponent.transform.SetParent(canvas);

        NewComponent.transform.localPosition = Vector3.zero;
        NewComponent.transform.localScale = Vector3.one * 1.2f;
        NewComponent.GetComponent<BaseComponent>().dir = 0;
        NewComponent.GetComponent<BaseComponent>().destroyable = true;
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

    public void OnDrop(PointerEventData eventData)
    {
        if(designermode && transform.childCount == 0)
        {
            if (BaseComponent.itemBeingDragged.GetComponent<BaseComponent>().isFrontiers) return;
            if (BaseComponent.itemBeingDragged.name.Contains("Empty")) return;

            //GameObject c = Instantiate(BaseComponent.itemBeingDragged);
            PrefabComponentPath = BaseComponent.itemBeingDragged.GetComponent<BaseComponent>().PrefabPath;

            GameObject c = Instantiate(Resources.Load(PrefabComponentPath, typeof(GameObject))) as GameObject;

            string data = JsonUtility.ToJson(BaseComponent.itemBeingDragged.GetComponent<BaseComponent>());
            JsonUtility.FromJsonOverwrite(data, c.GetComponent<BaseComponent>());
            //EditorUtility.CopySerialized(BaseComponent.itemBeingDragged.GetComponent<BaseComponent>(), c.GetComponent<BaseComponent>());

            c.transform.SetParent(transform);
            c.transform.localPosition = Vector3.zero;
            c.transform.localRotation = Quaternion.identity;
            c.transform.localScale = 0.8f * Vector3.one;
            c.GetComponent<BaseComponent>().locked = false;
            c.GetComponent<BaseComponent>().enabled = false;
            c.GetComponent<BaseComponent>().StartCoroutine("Awake"); //usefull for elements that display value


            Destroy(BaseComponent.itemBeingDragged.gameObject);
        }

    }
}
