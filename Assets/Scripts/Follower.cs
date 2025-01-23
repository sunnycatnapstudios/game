using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{

    public Transform target;
    private float followSpeed;
    public Player Player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        followSpeed = Player.moveSpeed;
        transform.position = Vector3.MoveTowards(transform.position, target.position, followSpeed*Time.deltaTime);
    }
}
