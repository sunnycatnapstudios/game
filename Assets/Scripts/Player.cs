using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float Speed;
    public Animator anim;
    private SpriteRenderer spritestate;

    public float moveSpeed = 5, moveSprint, moveSneak, moveConstant;
    public Vector3 pointRef;
    public float movementInputDelay;
    public Transform movePoint;
    public List<Vector3> moveHist = new List<Vector3>();
    public LayerMask noPass;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        spritestate = GetComponent<SpriteRenderer>();

        movePoint.parent = null;
        moveConstant = moveSpeed;
        moveSprint = moveSpeed*1.3f;
        moveSneak = moveSpeed*0.45f;

        moveHist = new List<Vector3>{movePoint.position, movePoint.position, movePoint.position};
        
    }

    // Update is called once per frame
    void Update()
    {
        // // Movement controls V1
        // if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
        //     transform.position+= Vector3.up*Time.deltaTime*Speed;
        //     anim.Play("Walk Up");
        // }
        // if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
        //     transform.position+= Vector3.left*Time.deltaTime*Speed;
        //     anim.Play("Walk Left");
        //     spritestate.flipX = true;
        // }
        // if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
        //     transform.position+= Vector3.down*Time.deltaTime*Speed;
        //     anim.Play("Walk Down");
        // }
        // if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
        //     transform.position+= Vector3.right*Time.deltaTime*Speed;
        //     anim.Play("Walk Left");
        //     spritestate.flipX = false;
        // }
        // if(Input.GetMouseButtonDown(1)){
        //     Speed = 4;
        // }
        // else if(Input.GetMouseButtonUp(1)){
        //     Speed = 8;
        // }


        // Movement Conteols V2
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed*Time.deltaTime);

        if(Input.GetMouseButton(1)){moveSpeed = moveSneak;}
        else if(Input.GetKey(KeyCode.LeftShift)){moveSpeed = moveSprint;}
        else{moveSpeed = moveConstant;}

        if (Vector3.Distance(transform.position, movePoint.position) <= movementInputDelay){

            pointRef = movePoint.position;
            
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f){
                if(!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), .2f, noPass)){
                    movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                }
            }
            if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f){
                if(!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), .2f, noPass)){
                    movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                }
            }

            if (movePoint.position != pointRef){
                moveHist.Add(pointRef);
            }
            if (moveHist.Count > 3){
                moveHist.RemoveAt(0);
            }
        }
    }
}
