using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{

    public int order, partyIndex;
    public Transform target;
    public Player Player;
    private float followSpeed;
    public Vector3 newPos, currentPos;
    public SpriteRenderer spriteState;
    public Animator partyAnim;

    // Start is called before the first frame update
    void Start()
    {
        if (order!=0){transform.position = Player.transform.position;}
        spriteState = GetComponent<SpriteRenderer>();
        partyAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (order > Player.partyCount){
            Destroy(gameObject);
        }

        float refX = transform.position.x, refY = transform.position.y;
        // // Movement V1
        // followSpeed = Player.moveSpeed;
        // transform.position = Vector3.MoveTowards(transform.position, target.position, followSpeed*Time.deltaTime);

        // Movement V2
        followSpeed = Player.moveSpeed;
        transform.position = Vector3.MoveTowards(transform.position, newPos, followSpeed*Time.deltaTime);

        // Handles Party Movement and Placement
        partyIndex = Mathf.Abs(Player.partyCount-order);
        
        if (partyIndex >= 0 && partyIndex < Player.moveHist.Count){

            newPos = Player.moveHist[partyIndex];

        } else {newPos = transform.position;}

        if (Mathf.Abs(transform.position.x-refX) > Mathf.Abs(transform.position.y-refY)){
            if (transform.position.x-refX > 0){
                spriteState.flipX = true;
            } else {spriteState.flipX = false;}
            partyAnim.Play("PartyLeft");
        } else if (Mathf.Abs(transform.position.x-refX) <= Mathf.Abs(transform.position.y-refY)){
            if (transform.position.y-refY > 0){
                partyAnim.Play("PartyUp");
            } else if (transform.position.y-refY < 0){
                partyAnim.Play("PartyDown");
            }
        }
    }
}
