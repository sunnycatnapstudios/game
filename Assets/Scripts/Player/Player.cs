using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cinemachine;

public class Player : MonoBehaviour
{

    // public float Speed;
    public bool infSprint;
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

    public ParticleSystem partiSystem;

    public CinemachineVirtualCamera vcam;
    public float camMax, camMin;
    public bool isZooming;

    public float currStamina, maxStamina = 100, sprintCost = 35;
    public Image staminaBar;
    private Coroutine recharge;
    private Coroutine checkKey;
    public bool recharging, isMoving;
    [HideInInspector] public bool faceLeft, faceRight, faceUp, faceDown = true;
    int animCount;

    public float horizontalInput, verticalInput;
    public string lastInput;
    public bool up_,down_, left_, right_;

    // New Input controls
    private PlayerInputActions playerInputActions;
    private InputAction movement, move;
    public Vector2 moveInput;

    // private void Awake()
    // {
    //     playerInputActions = new PlayerInputActions();

    // }

    // private void OnEnable()
    // {
    //     movement = playerInputActions.Player.Movement;
    //     movement.Enable();

    //     playerInputActions.Player.Sprint.performed += DoJump;
    //     playerInputActions.Player.Sprint.Enable();

    //     move = playerInputActions.Player.Move;
    //     move.performed += DoMove;
    //     move.Enable();
    // }

    // private void OnDisable()
    // {
    //     movement.Disable();
    //     playerInputActions.Player.Sprint.Enable();
        
    //     move.canceled -= DoMove;
    //     move.Enable();
    //     // move.Disable();
    // }

    // private void DoJump(InputAction.CallbackContext obj)
    // {
    //     Debug.Log("Idk lol");
    // }

    // private void DoMove(InputAction.CallbackContext obj)
    // {
    //     // moveInput = move.ReadValue<Vector2>();
    //     // Debug.Log("Last Key Pressed " + moveInput);
    //     Debug.Log("Last Key Pressed " + obj);
    // }

    // private void FixedUpdate()
    // {
    //     // Debug.Log("Movement Values "+ movement.ReadValue<Vector2>());
    //     // Debug.Log("Last Key Pressed " + move.ReadValue<Vector2>());
    //     // Debug.Log("Last Key Pressed " + moveInput);
    // }


    
    private IEnumerator RechargeStamina() {
        yield return new WaitForSeconds(1f);
        while (currStamina < maxStamina) {
            currStamina += (sprintCost*3f)*Time.deltaTime;
            if (currStamina > maxStamina) {recharging = false; currStamina = maxStamina;}
            staminaBar.fillAmount = currStamina/maxStamina;
            yield return new WaitForSeconds(.01f);
        }
    }

    private IEnumerator MovementDirection()
    {
        up_ = ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)));
        down_ = ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)));
        left_ = ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)));
        right_ = ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)));

        if (up_) {lastInput = "Up";}
        if (down_) {lastInput = "Down";}
        if (left_) {lastInput = "Left";}
        if (right_) {lastInput = "Right";}
        yield return new WaitForSeconds(.2f);
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

        // checkKey = MovementDirection();

        // if (checkKey != null) {StopCoroutine(checkKey);}
        // checkKey = StartCoroutine(MovementDirection());

        movePoint.parent = null;
        moveConstant = moveSpeed; moveSprint = moveSpeed*sprintConstant; moveSneak = moveSpeed*sneakConstant;

        currStamina = maxStamina;

        moveHist = new List<Vector3>{movePoint.position, movePoint.position, movePoint.position};

        // Party Members Spawning
        for (int x = 0; x<partyCount; x++) {
            memSpawn = GameObject.Instantiate(memTemplate);
            memSpawn.name = $"Follower {x+1}";
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


        // Movement Controls V2
        Vector3 startRef = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed*Time.deltaTime);
        Vector3 endRef = transform.position;

        isMoving = startRef!=endRef;

        // Controls Movement Speed
        if(Input.GetMouseButton(1)){moveSpeed = moveSneak;}
        else if(Input.GetKey(KeyCode.LeftShift) && !recharging){

            if (currStamina>0 && !recharging && isMoving){
                // if (Input.GetKeyDown(KeyCode.LeftShift)) {partiSystem.Play();}

                moveSpeed = moveSprint;
                if (animCount<=0){partiSystem.Play(); animCount+=1;}
                if (!infSprint) {currStamina -= sprintCost*Time.deltaTime;}

                if (currStamina<0) {currStamina = 0; recharging = true;}
                staminaBar.fillAmount = currStamina/maxStamina;

                if (recharge != null) {StopCoroutine(recharge);}
                recharge = StartCoroutine(RechargeStamina());

            } else{moveSpeed = moveConstant; animCount=0; partiSystem.Stop();}
        
        } else{moveSpeed = moveConstant; animCount=0; partiSystem.Stop();}
        // if (Input.GetKeyUp(KeyCode.LeftShift)){partiSystem.Stop();}

    
        viewMap();

        if (Vector3.Distance(transform.position, movePoint.position) <= movementInputDelay && !isZooming){

            pointRef = movePoint.position;

            float horizontalInput = Input.GetAxisRaw("Horizontal"), verticalInput = Input.GetAxisRaw("Vertical");

            // bool up_ = ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)));
            // bool down_ = ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)));
            // bool left_ = ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)));
            // bool right_ = ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)));
            // string lastInput = "";

            // if (Mathf.Abs(horizontalInput) == 1f) {lastInput = "Horizontal";}
            // if (Mathf.Abs(verticalInput) == 1f) {lastInput = "Vertical";}

            // if (up_||down_) {lastInput = "Horizontal";}
            // if (left_||right_) {lastInput = "Vertical";}

            // if (up_) {lastInput = "Up";}
            // if (down_) {lastInput = "Down";}
            // if (left_) {lastInput = "Left";}
            // if (right_) {lastInput = "Right";}

            // if (up_) {lastInput = "Vertical";}
            // if (down_) {lastInput = "Vertical";}
            // if (left_) {lastInput = "Horizontal";}
            // if (right_) {lastInput = "Horizontal";}

            // if (left_||right_) {lastInput = "Horizontal";}
            // if (up_||down_) {lastInput = "Vertical";}

            // if (lastInput == "Up"){movePoint.position += new Vector3(0, verticalInput, 0);}
            // else if (lastInput == "Down"){movePoint.position += new Vector3(0, verticalInput, 0);}

            // else if (lastInput == "Left"){movePoint.position += new Vector3(horizontalInput, 0, 0);}
            // else if (lastInput == "Right"){movePoint.position += new Vector3(horizontalInput, 0, 0);}

            // if (lastInput == "Horizontal" && !Physics2D.OverlapCircle(movePoint.position + new Vector3(horizontalInput, 0, 0), .2f, noPass)) {
            //     movePoint.position += new Vector3(horizontalInput, 0, 0);
            //     if (horizontalInput<0) {
            //         spritestate.flipX = true;
            //         partiSystem.transform.eulerAngles = new Vector3(0, 90f, 90f);
            //         faceLeft = true; faceUp=faceRight=faceDown=false;
            //     } else {
            //         spritestate.flipX = false;
            //         partiSystem.transform.eulerAngles = new Vector3(0f, -90f, 90f);
            //         faceRight = true; faceUp=faceLeft=faceDown=false;
            //     }
            //     anim.Play("Walk Left");
            // }
            // if (lastInput == "Vertical" && !Physics2D.OverlapCircle(movePoint.position + new Vector3(0, verticalInput, 0), .2f, noPass)) {
            //     movePoint.position += new Vector3(0, verticalInput, 0);
            //     if (verticalInput>0) {
            //         partiSystem.transform.eulerAngles = new Vector3(90f, 0, 0);
            //         faceUp = true; faceDown=faceLeft=faceRight=false;
            //         anim.Play("Walk Up");
            //     } else if (verticalInput<0) {
            //         partiSystem.transform.eulerAngles = new Vector3(-90f, 0, 0);
            //         faceDown = true; faceUp=faceLeft=faceRight=false;
            //         anim.Play("Walk Down");
            //     }
            // }
            


            if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1f){
                if(!Physics2D.OverlapCircle(movePoint.position + new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f), .2f, noPass)){
                    movePoint.position += new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);
                    if (Input.GetAxisRaw("Horizontal") < 0f) {
                        spritestate.flipX = true;
                        partiSystem.transform.eulerAngles = new Vector3(0f, 90f, 90f);
                        faceLeft = true; faceUp=faceRight=faceDown=false;
                    } else {
                        spritestate.flipX = false;
                        partiSystem.transform.eulerAngles = new Vector3(0f, -90f, 90f);
                        faceRight = true; faceUp=faceLeft=faceDown=false;
                    }
                    anim.Play("Walk Left");
                }
            }
            else if (Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1f){
                if(!Physics2D.OverlapCircle(movePoint.position + new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f), .2f, noPass)){
                    movePoint.position += new Vector3(0f, Input.GetAxisRaw("Vertical"), 0f);
                    if (Input.GetAxisRaw("Vertical") > 0f) {
                        anim.Play("Walk Up");
                        partiSystem.transform.eulerAngles = new Vector3(90f, 0f, 0f);
                        faceUp = true; faceDown=faceLeft=faceRight=false;
                    } else if (Input.GetAxisRaw("Vertical") < 0f) {
                        anim.Play("Walk Down");
                        partiSystem.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
                        faceDown = true; faceUp=faceLeft=faceRight=false;
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
