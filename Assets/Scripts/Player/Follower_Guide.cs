using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower_Guide : MonoBehaviour
{
    public int order, partyIndex;
    public Player Player;
    public GameObject Follower;
    public Vector3 newPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        partyIndex = Mathf.Abs(Player.partyCount-order);

        if (partyIndex >= 0 && partyIndex < Player.moveHist.Count){

            newPos = Player.moveHist[partyIndex];
            transform.position = newPos;

        } else {newPos = transform.position;}

    }
}
