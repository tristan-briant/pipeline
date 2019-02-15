using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class FlushManager : BaseFrontier {

    protected Animation Animation;
    //gameController gc; // le moteur du jeu à invoquer parfois

    //public float timeOut = 4.0f;



    override public void Awake()
    {
        Animation = GetComponent<Animation>();
        audios = GameObject.Find("PlaygroundHolder").GetComponents<AudioSource>();
        //Animation["WatchAnimation"].speed = 4 / timeOut;
    }

    public override void OnClick()
    {

        Animation.Play("FlushAnimation");
        gc.ResetComponant();

        audios[6].Play();

    }

    // Use this for initialization
    protected override void Start () {
        gc = (GameController)GameObject.Find("gameController").GetComponent(typeof(GameController)); //find the game engine

    }

    // Update is called once per frame
    void Update () {
		
	}
}
