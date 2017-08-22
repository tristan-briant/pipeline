using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class FlushManager : BaseFrontier {

    protected Animation Animation;
    gameController gc; // le moteur du jeu à invoquer parfois

    //public float timeOut = 4.0f;



    protected void Awake()
    {
        Animation = GetComponent<Animation>();
        audios = GameObject.Find("PlaygroundHolder").GetComponents<AudioSource>();
        //Animation["WatchAnimation"].speed = 4 / timeOut;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {

        Animation.Play("FlushAnimation");
        gc.ResetComponant();

        audios[6].Play();

    }

    // Use this for initialization
    protected override void Start () {
        gc = (gameController)GameObject.Find("gameController").GetComponent(typeof(gameController)); //find the game engine

    }

    // Update is called once per frame
    void Update () {
		
	}
}
