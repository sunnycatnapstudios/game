using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool attack;
    public bool stun;
    public float enemySpeed;
    
    public float pathDist;
    public Vector3 startPos;
    Vector3 RBound;
    Vector3 TopBound;
    Vector3 LTopBound;
    public float counter_ = 0;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        RBound = startPos+ new Vector3(pathDist, 0, 0);
        TopBound = startPos+ new Vector3(pathDist, pathDist, 0);
        LTopBound = startPos+ new Vector3(0, pathDist, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!attack){
            counter_+= Time.deltaTime;
            if(counter_<=1.5){
                //transform.position += new Vector3 (1, 0, 0)*Time.deltaTime*enemySpeed;
                transform.position = Vector3.MoveTowards(transform.position, RBound, enemySpeed);
            }
            else if(counter_<=3){
                transform.position = Vector3.MoveTowards(transform.position, TopBound, enemySpeed);
            }
            else if(counter_<=4.5){
                transform.position = Vector3.MoveTowards(transform.position, LTopBound, enemySpeed);
            }
            else if(counter_<=6){
                transform.position = Vector3.MoveTowards(transform.position, startPos, enemySpeed);
            }
            else if(counter_<=7.5){
                counter_ = 0;
            }
        }
    }
}
