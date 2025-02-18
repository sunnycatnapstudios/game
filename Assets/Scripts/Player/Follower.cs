﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public int order, partyIndex;
    public Player Player;
    public PartyManager partyManager;
    private _PartyManager _partyManager;
    public float followSpeed;
    public Vector3 currentPos, newPos;
    public SpriteRenderer spriteState;
    public Animator partyAnim;

    void Awake()
    {
        
    }
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (order!=0) {transform.position = Player.transform.position;}

        spriteState = GetComponent<SpriteRenderer>();
        partyAnim = GetComponent<Animator>();
        _partyManager = GameStatsManager.Instance._partyManager;
        
    }

    // Update is called once per frame
    void Update()
    {
        float refX = transform.position.x, refY = transform.position.y;

        // Handles Party Movement and Placement
        // partyIndex = Mathf.Abs(partyManager.partyCount-order);
        partyIndex = Mathf.Abs(_partyManager.partyCount-order);

        if (partyIndex >= 0 && partyIndex < Player.moveHist.Count){ newPos = Player.moveHist[partyIndex];}
        else {newPos = transform.position;}

        followSpeed = Player.moveSpeed;
        if (order!=0) {transform.position = Vector3.MoveTowards(transform.position, newPos, followSpeed*Time.deltaTime);}
        
        if (transform.position.x-refX>0) {spriteState.flipX = true;}
        else if (transform.position.x-refX<0) {spriteState.flipX = false;}

        if (Mathf.Abs(transform.position.x-refX) > Mathf.Abs(transform.position.y-refY)){
            partyAnim.Play("PartyLeft");
        } else if (transform.position.y-refY > 0){
            partyAnim.Play("PartyUp");
        } else if (transform.position.y-refY < 0){
            partyAnim.Play("PartyDown");
        }
    }
}
