using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{

    public int order, partyIndex;
    public Player Player;
    public float followSpeed;
    public Vector3 currentPos, newPos;
    public SpriteRenderer spriteState;
    public Animator partyAnim;

    // Start is called before the first frame update
    void Start()
    {
        if (order!=0) {transform.position = Player.transform.position;}

        spriteState = GetComponent<SpriteRenderer>();
        partyAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float refX = transform.position.x, refY = transform.position.y;

        // Handles Party Movement and Placement
        partyIndex = Mathf.Abs(Player.partyCount-order);

        if (partyIndex >= 0 && partyIndex < Player.moveHist.Count){ newPos = Player.moveHist[partyIndex];}
        else {newPos = transform.position;}

        followSpeed = Player.moveSpeed;
        transform.position = Vector3.MoveTowards(transform.position, newPos, followSpeed*Time.deltaTime);
        
        spriteState.flipX = transform.position.x-refX > 0;

        if (Mathf.Abs(transform.position.x-refX) > Mathf.Abs(transform.position.y-refY)){
            partyAnim.Play("PartyLeft");
        } else if (transform.position.y-refY > 0){
            partyAnim.Play("PartyUp");
        } else if (transform.position.y-refY < 0){
            partyAnim.Play("PartyDown");
        }
    }
}
