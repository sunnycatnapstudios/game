using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool attack, stun, pathReturn; // Enemy States
    public float enemySpeed, attackSpeed; // Enemy Speeds
    public float counter_ = 0f, stunTime = 3f, stunTimer; // Enemy Timers
    public float detectRange, pursueRange, playerDist; // Enemy Navigation
    public Vector3 startPos, currentPos, pathBounds; // Enemy Positions

    Vector3 RBound;
    Vector3 TopBound;
    Vector3 LTopBound;

    public Transform target;
    public LayerMask projectile;

    public Vector2 pathDist;




    void EnemyPatrol()
    {
        if (pathDist.x *pathDist.y == 0){ // Checks if it's a straight line

            pathBounds = startPos + new Vector3(pathDist.x, pathDist.y, 0f);

            if (transform.position != pathBounds && !pathReturn){

                transform.position = Vector3.MoveTowards(transform.position, pathBounds, enemySpeed*Time.deltaTime);
            }
            else {
                counter_+=Time.deltaTime;
                
                if (counter_ >= 1.5){
                    pathReturn = true;
                    transform.position = Vector3.MoveTowards(transform.position, startPos, enemySpeed*Time.deltaTime);
                    if (transform.position == startPos){
                        counter_ = 0;
                        pathReturn = false;
                    }
                }
                
            }
        } else{
            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        playerDist = Vector3.Distance(target.position, transform.position);

        Vector3 direction = target.position - transform.position;
        direction.Normalize();
        
        if (playerDist <= detectRange || attack && !stun){

            transform.position = Vector3.MoveTowards(transform.position, target.position, attackSpeed*Time.deltaTime);
            attack = true;
            if (playerDist >= pursueRange){
                attack = false;
            }
        }

        // else if(!Physics2D.OverlapCircle((transform.position), .2f, projectile) && !stun){
        //     if (!attack){
        //         counter_+= Time.deltaTime;
        //         if(counter_<=1.5){
        //             transform.position = Vector3.MoveTowards(transform.position, RBound, enemySpeed*Time.deltaTime);
        //         }
        //         else if(counter_<=3){
        //             transform.position = Vector3.MoveTowards(transform.position, TopBound, enemySpeed*Time.deltaTime);
        //         }
        //         else if(counter_<=4.5){
        //             transform.position = Vector3.MoveTowards(transform.position, LTopBound, enemySpeed*Time.deltaTime);
        //         }
        //         else if(counter_<=6){
        //             transform.position = Vector3.MoveTowards(transform.position, startPos, enemySpeed*Time.deltaTime);
        //         }
        //         else if(counter_<=7.5){
        //             counter_ = 0;
        //         }
        //     }
        // }

        else if(!Physics2D.OverlapCircle((transform.position), .2f, projectile) && !stun){
            if (!attack){
                EnemyPatrol();
            }
        }
        else if (Physics2D.OverlapCircle((transform.position), .2f, projectile)||stun){
            if (!attack){
                stunTimer += Time.deltaTime;
                if (stunTimer<=stunTime){
                    stun = true;
                    detectRange = 0;
                }
                else {
                    stun = false;
                    stunTimer = 0;
                    detectRange = 4.5f;
                }
            }
        }
    }
}
