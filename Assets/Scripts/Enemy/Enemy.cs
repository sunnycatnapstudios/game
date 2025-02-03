using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool attack, stun, searching, pathReturn, caught, demotestFreeze; // Enemy States
    public float enemySpeed = 3, attackSpeed = 5; // Enemy Speeds
    public float counter_ = 0f, stunTime = 3f, stunTimer, searchTimer, intervalCheck = .4f; // Enemy Timers
    public float detectRange, caughtRange = 1f, baseRange = 3f, pursueRange = 5.5f, wakeRange = 4.5f, playerDist, refX, refY; // Enemy Navigation
    [HideInInspector] public Vector3 startPos, pathBounds; // Enemy Positions

    public Transform target;
    public LayerMask projectile, player;

    public Vector2 pathDist;

    public Animator enemyAnim;
    public SpriteRenderer spriteState;


    void OnDrawGizmos() { // Draws a Debug for NPC interact radius
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }


    void EnemyPatrol()
    {
        if (pathDist.x *pathDist.y == 0){ // Checks if it's a straight line

            pathBounds = startPos + new Vector3(pathDist.x, pathDist.y, 0f);

            if (!pathReturn){
                counter_+=Time.deltaTime;
                if (counter_>=1.5f){
                    transform.position = Vector3.MoveTowards(transform.position, pathBounds, enemySpeed*Time.deltaTime);
                    if (transform.position == pathBounds){
                        counter_ = 0f;
                        pathReturn = true;}
                }
            } else {
                counter_+=Time.deltaTime;
                if (counter_ >= 1.5f){
                    transform.position = Vector3.MoveTowards(transform.position, startPos, enemySpeed*Time.deltaTime);
                    if (transform.position == startPos){
                        counter_ = 0f;
                        pathReturn = false;
                    }
                }
            }
        } else { // Handles Square Paths
            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        enemyAnim = GetComponent<Animator>();
        spriteState = GetComponent<SpriteRenderer>();
        detectRange = baseRange;

    }

    // Update is called once per frame
    void Update()
    {
        playerDist = Vector3.Distance(target.position, transform.position);
        refX = transform.position.x;
        refY = transform.position.y;

        Vector3 direction = target.position - transform.position;
        direction.Normalize();

        if (playerDist <= detectRange || attack &&! stun){ // Attack Player // Will be changed later to account for pathfinding
            
            if (!demotestFreeze){transform.position = Vector3.MoveTowards(transform.position, target.position, attackSpeed*Time.deltaTime);}
            attack = true;
            caught = Physics2D.OverlapCircle((transform.position), caughtRange, player);
            searchTimer = 0f; detectRange = baseRange; intervalCheck = .4f; searching = false;
            if (playerDist >= pursueRange){
                attack = false;
            }
        } else if (Physics2D.OverlapCircle((transform.position), .2f, projectile)||stun) // Stun Enemy
        {
            searchTimer = 0f;
            if (stunTimer<=stunTime) {stunTimer+=Time.deltaTime; stun = true;} else {stun = false;}

            if (stunTimer<=stunTime){
                stun = true;
                stunTimer+=Time.deltaTime;
                detectRange = 0f;
                enemyAnim.Play("Stun Down");
            } else {
                detectRange = wakeRange;
                stunTimer = 0f;
                searching = true;
                stun = false;
            }
        } else if (searching) // Search for Player w Temp Increased Radius
        {
            enemyAnim.Play("Enem Left");
            if (searchTimer<=2.1f) {
               searchTimer+=Time.deltaTime;
                if (searchTimer>=intervalCheck) {spriteState.flipX = !spriteState.flipX; intervalCheck+=.4f;}
            } else {
                enemyAnim.Play("Enem Down");
                searchTimer = 0f; detectRange = baseRange; intervalCheck = .4f; searching = false;
            }
        } else if (!demotestFreeze){EnemyPatrol();} // Enemy Idle Movement Path

        if (Mathf.Abs(transform.position.x-refX) > Mathf.Abs(transform.position.y-refY) && !stun && !searching){
            if (transform.position.x-refX > 0){
                spriteState.flipX = true;
            } else {spriteState.flipX = false;}
            enemyAnim.Play("Enem Left");
        } else if (Mathf.Abs(transform.position.x-refX) <= Mathf.Abs(transform.position.y-refY) && !stun && !searching){
            if (transform.position.y-refY > 0){
                enemyAnim.Play("Enem Up");
            } else {enemyAnim.Play("Enem Down");}
        }
    }
}
