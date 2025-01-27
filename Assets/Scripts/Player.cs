using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class Player : MonoBehaviour
{

    // public float Speed;
    [HideInInspector] public Animator anim;
    [HideInInspector] public SpriteRenderer spritestate;

    public float moveSpeed = 5, movementInputDelay = 0.05f, moveConstant, sprintConstant, sneakConstant;
    [HideInInspector] public float moveSprint, moveSneak;
    private Vector3 pointRef;
    [HideInInspector] public Transform movePoint;
    public List<Vector3> moveHist = new List<Vector3>();
    public LayerMask noPass;
    public int partyCount = 4;

    public GameObject memTemplate;
    public AudioSource walkAudi;
    private int walkAudiCount;
    GameObject memSpawn;

    public CinemachineVirtualCamera vcam;
    public float camMax, camMin;
    public bool camZoom;

    private float currStamina, maxStamina = 100;
    public float sprintCost = 35;
    public Image staminaBar;
    private Coroutine recharge;
    public bool recharging;

    private IEnumerator RechargeStamina() {
        yield return new WaitForSeconds(1f);
        while (currStamina < maxStamina) {
            currStamina += (sprintCost*1.5f)*Time.deltaTime;
            if (currStamina > maxStamina) {recharging = false; currStamina = maxStamina;}
            staminaBar.fillAmount = currStamina/maxStamina;
            yield return new WaitForSeconds(.01f);
        }
    }

    void viewMap()
    {
        if(Input.GetKey(KeyCode.Q)) // Controlls CamZoom
		{
            camZoom = true;
			if(vcam.m_Lens.OrthographicSize >= camMax){vcam.m_Lens.OrthographicSize = camMax;}
            else{vcam.m_Lens.OrthographicSize = vcam.m_Lens.OrthographicSize + camMax*Time.deltaTime*2;}
		} else
		{
            camZoom = false;
			if(vcam.m_Lens.OrthographicSize <= camMin){vcam.m_Lens.OrthographicSize = camMin;}
            else{vcam.m_Lens.OrthographicSize = vcam.m_Lens.OrthographicSize - camMin*Time.deltaTime*2;}
		}
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        spritestate = GetComponent<SpriteRenderer>();
        walkAudi = GetComponent<AudioSource>();


        movePoint.parent = null;
        moveConstant = moveSpeed;
        moveSprint = moveSpeed*sprintConstant;
        moveSneak = moveSpeed*sneakConstant;

        currStamina = maxStamina;

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
        else if(Input.GetKey(KeyCode.LeftShift)){

            if (currStamina>0 && !recharging){
                moveSpeed = moveSprint;
                currStamina -= sprintCost*Time.deltaTime;
                if (currStamina<0) {currStamina = 0; recharging = true;}
                staminaBar.fillAmount = currStamina/maxStamina;

                if (recharge != null) {StopCoroutine(recharge);}
                recharge = StartCoroutine(RechargeStamina());

            } else{moveSpeed = moveConstant;}

        }else{moveSpeed = moveConstant;}


        viewMap();
        // if(Input.GetKey(KeyCode.Q)) // Controlls CamZoom
		// {
        //     camZoom = true;
		// 	if(vcam.m_Lens.OrthographicSize >= camMax){vcam.m_Lens.OrthographicSize = camMax;}
        //     else{vcam.m_Lens.OrthographicSize = vcam.m_Lens.OrthographicSize + camMax*Time.deltaTime*2;}
		// } else
		// {
        //     camZoom = false;
		// 	if(vcam.m_Lens.OrthographicSize <= camMin){vcam.m_Lens.OrthographicSize = camMin;}
        //     else{vcam.m_Lens.OrthographicSize = vcam.m_Lens.OrthographicSize - camMin*Time.deltaTime*2;}
		// }


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
