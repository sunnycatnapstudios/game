using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{

    // public float Speed;
    public Animator anim;
    private SpriteRenderer spritestate;

    public float moveSpeed = 5, movementInputDelay = 0.05f;
    private float moveSprint, moveSneak;
    public float moveConstant, sprintConstant, sneakConstant;
    public Vector3 pointRef;
    public Transform movePoint;
    public List<Vector3> moveHist = new List<Vector3>();
    public LayerMask noPass;
    public int partyCount = 4;

    public GameObject memTemplate;
    GameObject memSpawn;
    public AudioSource walkAudi;
    private int walkAudiCount;

    public CinemachineVirtualCamera vcam;
    public float camMax, camMin;
    private bool camZoom;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        spritestate = GetComponent<SpriteRenderer>();
        walkAudi = GetComponent<AudioSource>();


        movePoint.parent = null;
        moveConstant = moveSpeed;
        moveSprint = moveSpeed*sprintConstant;
        moveSneak = moveSpeed*sneakConstant;

        moveHist = new List<Vector3>{movePoint.position, movePoint.position, movePoint.position};

        // Party Members Spawning
        for (int x = 0; x<partyCount; x++) {
            memSpawn = GameObject.Instantiate(memTemplate);
            memSpawn.name = $"Follower {x+1}";
            // memSpawn.order = x;
            memSpawn.GetComponent<Follower>().order = x+1;
        }
        
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

        // Controls Movement Speed
        if(Input.GetMouseButton(1)){moveSpeed = moveSneak;}
        else if(Input.GetKey(KeyCode.LeftShift)){moveSpeed = moveSprint;}
        else{moveSpeed = moveConstant;}

        // Controls Camera Scale
        // if (Input.GetKey(KeyCode.Q) && personalCamera.orthographicSize <= 8) {
        //     personalCamera.orthographicSize += 1;
        // }
        if(Input.GetKey(KeyCode.Q)) // Controlls CamZoom
		{
            camZoom = true;
			if(vcam.m_Lens.OrthographicSize >= camMax)
			{
                vcam.m_Lens.OrthographicSize = camMax;
			} else{
                vcam.m_Lens.OrthographicSize = vcam.m_Lens.OrthographicSize + camMax*Time.deltaTime*2;
            }
		} else
		{
            camZoom = false;
			if(vcam.m_Lens.OrthographicSize <= camMin)
			{
                vcam.m_Lens.OrthographicSize = camMin;
			} else{
                vcam.m_Lens.OrthographicSize = vcam.m_Lens.OrthographicSize - camMin*Time.deltaTime*2;
            }
		}


        if (Vector3.Distance(transform.position, movePoint.position) <= movementInputDelay && !camZoom){

            pointRef = movePoint.position;
            
            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f){
                if(!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), .2f, noPass)){
                    movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                    if (Input.GetAxisRaw("Horizontal") < 0f) {spritestate.flipX = true;}
                    else {spritestate.flipX = false;}
                    anim.Play("Walk Left");
                }
            }
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f){
                if(!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), .2f, noPass)){
                    movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                    if (Input.GetAxisRaw("Vertical") > 0f) {
                        anim.Play("Walk Up");
                    } else if (Input.GetAxisRaw("Vertical") < 0f) {
                        anim.Play("Walk Down");
                    }
                }
            }
            if (!walkAudi.isPlaying && walkAudiCount <=0 ) {walkAudi.Play(); walkAudiCount+=1; walkAudi.Play();}
            else {walkAudi.Stop(); walkAudiCount = 0;}

            if (movePoint.position != pointRef){moveHist.Add(pointRef);}
            if (moveHist.Count > partyCount){moveHist.RemoveAt(0);}
        }
    }
}
