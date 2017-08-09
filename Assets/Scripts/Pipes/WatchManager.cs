using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class WatchManager : BaseComponent {

    protected Animation Animation;
    public float timeOut=4.0f;

    protected void Awake()
    {
        Animation = GetComponent<Animation>();

    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        
        Animation.Play("WatchAnimation");

        broadcastTrigger();
    }


    public void broadcastTrigger() {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Triggerable")) {
            go.transform.GetComponent<BaseComponent>().trigged=true;
        }

    }
    
}
