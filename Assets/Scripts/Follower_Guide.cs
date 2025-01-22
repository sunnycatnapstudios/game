using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower_Guide : MonoBehaviour
{
    public int order;
    public Player Player;
    public Vector3 newPos;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        newPos = Player.moveHist[Mathf.Abs(3-order)];
        transform.position = newPos;

    }
}
