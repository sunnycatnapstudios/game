using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cinemachine;

public class Player : MonoBehaviour
{

    public bool infSprint;
    [HideInInspector] public Animator anim;
    [HideInInspector] public SpriteRenderer spritestate;

    public float moveSpeed = 5f, movementInputDelay = 0.05f, moveConstant, sprintConstant = 1.7f, sneakConstant = .5f;
    [HideInInspector] public float moveSprint, moveSneak;
    private Vector3 pointRef;
    [HideInInspector] public Transform movePoint;
    public List<Vector3> moveHist = new List<Vector3>();

    public LayerMask noPass, NPC;

    public AudioSource walkAudi;
    public int walkAudiCount;

    public ParticleSystem partiSystem;

    public CinemachineVirtualCamera vcam;
    public float camMax, camMin;
    public bool isZooming;

    public float currStamina, maxStamina = 100, sprintCost = 35;
    public Image staminaBar;
    private Coroutine recharge;
    public bool recharging, isMoving;
    public bool faceLeft, faceRight, faceUp, faceDown = true;
    int animCount;

    // New Input controls
    private PlayerInputActions playerInputActions;
    private InputAction movement, move;
    public Vector2 moveInput;
    public Vector2 lastInput;
    public Vector2 playerInput;

    private PartyManager partyManager;

    public bool isPlayerInControl;

    
    private IEnumerator RechargeStamina() {
        yield return new WaitForSeconds(1f);
        while (currStamina < maxStamina) {
            currStamina += (sprintCost*3f)*Time.deltaTime;
            if (currStamina > maxStamina) {recharging = false; currStamina = maxStamina;}
            staminaBar.fillAmount = currStamina/maxStamina;
            yield return new WaitForSeconds(.01f);
        }
    }

    void viewMap()
    {
        isZooming = Input.GetKey(KeyCode.Q);
        float targetSize = isZooming ? camMax : camMin;

        vcam.m_Lens.OrthographicSize = Mathf.MoveTowards(vcam.m_Lens.OrthographicSize,targetSize,targetSize * Time.deltaTime * 2);
    }


    void Start()
    {
        anim = GetComponent<Animator>();
        spritestate = GetComponent<SpriteRenderer>();
        walkAudi = GetComponent<AudioSource>();
        partyManager = GetComponent<PartyManager>();

        // // Sets Up Variables to prevent confusion
        // moveSpeed = 5f; sprintConstant = 1.7f; sneakConstant = .5f; movementInputDelay = .1f;
        // partyCount = 4;
        // camMax = 10; camMin = 6;
        // maxStamina = 100f; sprintCost = 35f;


        movePoint.parent = null;
        moveConstant = moveSpeed; moveSprint = moveSpeed*sprintConstant; moveSneak = moveSpeed*sneakConstant;

        currStamina = maxStamina;

        moveHist = new List<Vector3>();
        for (int i = 0; i<partyManager.partyCount; i++) {moveHist.Add(movePoint.position);}
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


        // Movement Controls V2
        Vector3 startRef = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed*Time.deltaTime);
        Vector3 endRef = transform.position;

        isMoving = startRef!=endRef;

        // Controls Movement Speed
        if (!isPlayerInControl)
        {
            if(Input.GetMouseButton(1)){moveSpeed = moveSneak;}
            else if(Input.GetKey(KeyCode.LeftShift) && !recharging){

                if (currStamina>0 && !recharging && isMoving)
                {
                    moveSpeed = moveSprint;
                    if (animCount<=0){partiSystem.Play(); animCount+=1;}
                    if (!infSprint) {currStamina -= sprintCost*Time.deltaTime;}

                    if (currStamina<0) {currStamina = 0; recharging = true;}
                    staminaBar.fillAmount = currStamina/maxStamina;

                    if (recharge != null) {StopCoroutine(recharge);}
                    recharge = StartCoroutine(RechargeStamina());

                } else{moveSpeed = moveConstant; animCount=0; partiSystem.Stop();}
            } else{moveSpeed = moveConstant; animCount=0; partiSystem.Stop();}
        }

    
        viewMap();

        if (Vector3.Distance(transform.position, movePoint.position) <= movementInputDelay && !isZooming){

            pointRef = movePoint.position;
            
            playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector3 moveDir = Vector3.zero;
            
            if (playerInput.x != 0 && lastInput.x == 0) {
                lastInput = new Vector2(playerInput.x, 0f);
            }
            else if (playerInput.y != 0 && lastInput.y == 0) {
                lastInput = new Vector2(0f, playerInput.y);
            }

            if (playerInput.x * lastInput.x == 0-1f || playerInput.y * lastInput.y == 0-1f) {lastInput = playerInput;}

            if (playerInput == Vector2.zero) {lastInput = Vector2.zero;}
            moveDir = new Vector3(lastInput.x, lastInput.y, 0f);

            // if (playerInput!=Vector2.zero)
            if (moveDir != Vector3.zero)
            {
                // Vector3 moveDir = new Vector3(playerInput.x, playerInput.y, 0f).normalized;

                if (!Physics2D.OverlapCircle(movePoint.position+moveDir, .2f, noPass) && 
                    !Physics2D.OverlapCircle(movePoint.position+moveDir, .2f, NPC))
                {
                    movePoint.position+=moveDir;

                    // if (playerInput.x!=0)
                    if (moveDir.x!=0)
                    {
                        spritestate.flipX = moveDir.x<0;
                        partiSystem.transform.eulerAngles = new Vector3(0f, moveDir.x < 0 ? 90f : -90f, 90f);

                        faceLeft = moveDir.x < 0; faceRight = moveDir.x > 0;
                        faceUp = faceDown = false;

                        anim.Play("Walk Left");
                    }
                    else if (moveDir.y!=0)
                    {
                        bool movingUp = moveDir.y > 0;
                        anim.Play(movingUp ? "Walk Up" : "Walk Down");
                        partiSystem.transform.eulerAngles = new Vector3(movingUp ? 90f : -90f, 0f, 0f);

                        faceUp = movingUp; faceDown = !movingUp;
                        faceLeft = faceRight = false;
                    }
                }
            }
        
            if (movePoint.position != pointRef){moveHist.Add(pointRef);}
            if (moveHist.Count > partyManager.partyCount){moveHist.RemoveAt(0);}

        }
        if (!walkAudi.isPlaying && walkAudiCount <=0 && isMoving) {
            walkAudi.Play(); walkAudiCount+=1; walkAudi.Play();
        }
        else if (!isMoving) {walkAudi.Stop(); walkAudiCount = 0;}
    }
}
