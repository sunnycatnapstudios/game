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

    // Start is called before the first frame update
    void Start()
    {
        if (order!=0){transform.position = Player.transform.position;}
    }

    // Update is called once per frame
    void Update()
    {
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

        // transform.position = Vector3.MoveTowards(transform.position, newPos, followSpeed*Time.deltaTime);

    }
}
