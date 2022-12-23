using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

/***************************************************************************************
*    Title: PlayerMovement
*    Author: Slug Glove
*    Date: November 14th, 2020
*    Edit: December, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Availability: https://www.youtube.com/watch?v=ZxWfkOhl6bQ&list=PLbT7sIsvd6RUPai-zx8wQ83QP8DG38Y9W&index=26
*    Description: Manages the logic of the player character's movement. Relies on PlayerCollision to help with
*                 logic.
*
***************************************************************************************/
public class PlayerMovement : MonoBehaviour
{
    public enum PlayerStates
    {
        grounded,
        airborne,
        onRail,
        onWall,
        ledgeGrab
    }

    /***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Ground states for player animation
*
***************************************************************************************/

    public enum GroundStates
    {
        idle,
        slowForward,
        slowRight,
        slowLeft,
        fastForward,
        fastRight,
        fastLeft,
        back
    }

    /***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Wall run states for player animation
*
***************************************************************************************/
    public enum WallRunStates
    {
        vertical,
        leftWall,
        rightWall
    }

    public PlayerStates CurrentState;

    public GroundStates CurrentGroundState;
    public WallRunStates CurrentWallRunState;

    [SerializeField] private int playerID = 0;
    [SerializeField] private Player player;

    [Header("Physics")]
    public float MaxSpeed;
    public float MovingBackSpeed;

    [Range(0,1)]
    public float InAirControl;

    public float CurrentSpeed;
    public Vector3 CurrentVel;

    public float Acceleration;
    public float Decceleration;
    public float DirectionalControl;

    private float AirborneTimer;
    private float GroundedTimer;

    private float AgilityControl; // for how much the player can adjust to any movement (0 when sliding since we lose control of velocity)

    [Header ("Jumping")]
    public float JumpForce;

    [Header("Turning")]
    public float TurnSpeed;
    public float TurnSpeedAirborne;
    public float TurnSpeedOnWalls;
    public float LookUpSpeed;
    public Camera HeadCam;
    private float YTurn;
    private float XTurn;
    public float MaxLookAngle;
    public float MinLookAngle;

    [Header("WallRunning")]
    public float MaxWallRunTime; // how long player can run on walls
    private float WallRunTimer = 0; // wall run timer
    public float WallRunUpwardsMovement = 4; // how much player runs up the wall
    public float WallRunAcceleration = 2;
    private Vector3 WallJumpDirection;
    public Vector3 WallNormal;
    public Vector3 WallRight;

    [Header("Ledge Grabs")]
    public float PullUpTotalTime = 0.5f;
    private Vector3 OriginPos;
    private Vector3 LedgePos;
    private Vector3 OriginForward;
    private float PullUpTimer;
    private bool tryingToGrab = false;

    /***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Sets rail slide variables and animation boolean. Some testing code
*
***************************************************************************************/
    [Header("Rail Slide")]
    public Rail currentRail;
    public int currentRailSeg;
    private float railTransition;
    private bool railCompleted;

    [Header("Animation Bool")]
    public bool fast;

    [Header("Testing")]
    public float LookUpTimer = 0;

    /***************************************************************************************
*   Edit end
*
***************************************************************************************/

    private PlayerCollision Collision;
    private Rigidbody Rgbody;

    /***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Set up Animator object. Test code for wall jump logic
*
***************************************************************************************/
    private Animator Anim; 

    private Vector3 camFixedForward;
    private Vector3 beforeWalJForward;
    private Vector3 afterWalJForward;
    private Vector3 forwardAxis;

    /***************************************************************************************
*   Edit end
*
***************************************************************************************/

    // Start is called before the first frame update
    void Start()
    {
        Collision = GetComponent<PlayerCollision>();
        Rgbody = GetComponent<Rigidbody>();

        /***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Set up Animator object.
*
***************************************************************************************/
        Anim = GetComponent<Animator>();

        player = ReInput.players.GetPlayer(playerID); //ReWired Set up

/***************************************************************************************
*   Edit end
*
***************************************************************************************/

        AgilityControl = 1;

        fast = false;
    }

    // Update is called once per frame
    private void Update()
    {
        /*float XMov = Input.GetAxis("Horizontal");
        float YMov = Input.GetAxis("Vertical");*/

        float XMov = player.GetAxis("Horizontal");
        float YMov = player.GetAxis("Vertical");


/***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Reset Rigibody components based on game state. This is so players don't fall while wall running and fixes
*                 weird bug that made camera turn while ledge grabbing.
*
***************************************************************************************/
        if (CurrentState != PlayerStates.onWall)
        {
            Rgbody.useGravity = true;
        }

        if(CurrentState != PlayerStates.ledgeGrab)
        {
            Rgbody.freezeRotation = false;
        }
/***************************************************************************************
*   Edit end
*
***************************************************************************************/

        if (CurrentState == PlayerStates.grounded)
        {
            // check for jump
            if (player.GetButtonDown("Jump"))
            {
                JumpUp();
            }

            //check for ground
            bool checkGround = Collision.CheckFloor(-transform.up);
            if (!checkGround)
            {
                InAir();
            }

            Vector3 newLedge = Collision.CheckLedges();
            if (newLedge != Vector3.zero)
            {
                OriginForward = transform.forward; // EDIT: keeps track of forward when grabbing
                LedgeGrab(newLedge);
                //Debug.Log("Grab!");
            }
        }
        else if (CurrentState == PlayerStates.airborne)
        {
            // check for ledges
            if (tryingToGrab)
            {
                Vector3 newLedge = Collision.CheckLedges();
                if (newLedge != Vector3.zero)
                {
                    OriginForward = transform.forward; // EDIT: keeps track of forward when grabbing
                    LedgeGrab(newLedge);
                    //Debug.Log("Grab!");
                }
            }

            // check for walls to run on
            bool wallExists = CheckWall(XMov, YMov);

            if (wallExists && AirborneTimer > 1f)
            {
                //Debug.ClearDeveloperConsole();
                Debug.Log("Wall found");               
                WallRun();
                return;
            }

            // check for ground to land
            bool checkGround = Collision.CheckFloor(-transform.up);
            //check for bouncy surface
            bool checkBouncy = Collision.CheckBouncy(-transform.up);
            // Check for rails to slide on
            bool checkRail = Collision.CheckRails(-transform.up, out currentRail, out currentRailSeg);

            if (checkGround && AirborneTimer > 0.2f)
            {
                OnGround();
            }

/***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Checks for bouncy layer. If true, initializes bouncy jump. 
*    
*                 Checks for rail after. If true, set player state to onRail.
*                 
*                 Both times need to check to make sure player has been in air for long enough time.
*                 
*
***************************************************************************************/
            else if (checkBouncy && AirborneTimer > 0.2f)
            {
                BouncyJump();
            }


            else if (checkRail && AirborneTimer > 0.4f)
            {
                OnRail();
            }
        }

/***************************************************************************************
*   Edit end
*
***************************************************************************************/

        else if (CurrentState == PlayerStates.ledgeGrab)
        {
            // stop velocity 
            Rgbody.velocity = Vector3.zero;


        }

        else if (CurrentState == PlayerStates.onWall)
        {
            //Debug.Log("Camera Update 1: " + HeadCam.transform.forward);

/***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: New wall jump mechanic. Resets object rotation to fit the camera after exiting the wall
*                 state.
*
***************************************************************************************/

            // check for jump
            if (player.GetButtonDown("Jump"))
            {
                Debug.Log("Before Jump: " + HeadCam.transform.forward);
                beforeWalJForward = HeadCam.transform.forward;
                WallJump();
                Debug.Log("After Jump: " +  HeadCam.transform.forward);
                afterWalJForward = HeadCam.transform.forward;
                //Vector3 camDir = HeadCam.transform.forward;
                ResetObjectRotation();
                //Debug.Log("After Obj Rotate: " + HeadCam.transform.forward);
                //transform.r
                return;
            }

            bool checkGround = Collision.CheckFloor(-transform.up);
            if (checkGround)
            {
                OnGround();
                ResetObjectRotation();
                return;
            }

/***************************************************************************************
*   Edit end
*
***************************************************************************************/

            //Debug.Log("Forward 2: " + transform.forward);

            //Debug.Log("Camera Update 2: " + HeadCam.transform.forward);
        }

/***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Rail slide logic. When the player reaches the end of the rail or presses jump, 
*                 initiate jump function and reset player object's rotation and velocity.
*
***************************************************************************************/
        else if (CurrentState == PlayerStates.onRail)
        {
            if (railCompleted || player.GetButtonDown("Jump"))
            {
                ResetObjectRotation();
                ResetObjectVelocity();
                JumpUp();
                currentRail.railCompleted = false;
                railCompleted = false;
                /*Debug.Log("Off Rail!");
                Debug.Log("Cam forward: " + HeadCam.transform.forward);
                Debug.Log("Cam rotation: " + HeadCam.transform.rotation);*/
                return;
            }
        }

/***************************************************************************************
*   Edit end
*
***************************************************************************************/
    }

    private void FixedUpdate()
    {
        float Del = Time.deltaTime;

        float XMov = player.GetAxis("Horizontal");
        float YMov = player.GetAxis("Vertical");

        float CamX = player.GetAxis("Camera Horizontal");
        float CamY = player.GetAxis("Camera Vertical");

        LookUpDown(CamY, Del);


        if (CurrentState == PlayerStates.grounded)
        {
            // increase grounded timer
            if(GroundedTimer < 10)
            {
                GroundedTimer += Del;
            }

            //get the magnitude of our inputs
            float inputMag = new Vector2(XMov, YMov).normalized.magnitude;

            // get which speed to apply, forward or back?
            float targetSpd = Mathf.Lerp(MovingBackSpeed, MaxSpeed, YMov);

            LerpSpeed(inputMag, Del, targetSpd);

            MovePlayer(XMov, YMov, Del, 1);
            TurnPlayer(CamX, Del, TurnSpeed);

            /*if(AgilityControl < 1)
            {
                AgilityControl += Del 
            }*/
            if(AgilityControl > 1)
            {
                AgilityControl = 1;
            }
        }
        else if (CurrentState == PlayerStates.airborne)
        {
            if (AirborneTimer < 10)
            {
                AirborneTimer += Del;
            }

            // Move player, apply in air control float
            MovePlayer(XMov, YMov, Del, InAirControl);
            TurnPlayer(CamX, Del, TurnSpeedAirborne);
        }
        else if (CurrentState == PlayerStates.ledgeGrab)
        {
            // increment pull up timer
            PullUpTimer += Del;

            float pullUpLerp = PullUpTimer / PullUpTotalTime;

            if(pullUpLerp < 0.5)
            {
                // vertical movement
                float lamt = pullUpLerp * 2;

                Vector3 thisPullPos = new Vector3(OriginPos.x, LedgePos.y, OriginPos.z);
                transform.position = Vector3.Lerp(OriginPos, thisPullPos, lamt);
            }

            else if(pullUpLerp <= 1)
            {
                // horizontal movement
                if(OriginPos.y != LedgePos.y)
                {
                    OriginPos = new Vector3(transform.position.x, LedgePos.y, transform.position.z);
                }

                float lamt = (pullUpLerp - 0.5f) * 2;

                transform.position = Vector3.Lerp(OriginPos, LedgePos, lamt);
            }
            else
            {
                // check for ground to land
                bool checkGround = Collision.CheckFloor(-transform.up);
                // Check for rails to slide on
                bool checkRail = Collision.CheckRails(-transform.up, out currentRail, out currentRailSeg); // EDIT: check on rail

                if (checkGround)
                {
                    OnGround();
                }

/***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Check on Rail
*
***************************************************************************************/

                else if (checkRail)
                {
                    OnRail();
                }

/***************************************************************************************
*   Edit end
*
***************************************************************************************/
            }
        }
        else if (CurrentState == PlayerStates.onWall)
        {
            // increment wall run timer
            //WallRunTimer += Del;


/***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Player now turns camera instead of the whole game object while on the wall
*
***************************************************************************************/

            TurnCamera(CamX, Del, TurnSpeedOnWalls);

/***************************************************************************************
*   Edit end
*
***************************************************************************************/

            // check for walls to run on
            bool wallExists = CheckWall(XMov, YMov);

            if (!wallExists)
            {
                InAir();
                ResetObjectRotation();
                //Debug.Log("Dropping");
                return;
            }

            MovePlayerOnWall(YMov, Del);

            //Debug.Log("Camera Fixed: " + HeadCam.transform.rotation);
            camFixedForward = HeadCam.transform.forward;
        }


/***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Turn camera logic on rail. Interpolation of player movement on rail
*
***************************************************************************************/
        else if (CurrentState == PlayerStates.onRail)
        {
            TurnCamera(CamX, Del, TurnSpeed);
            //Debug.Log("Forward:" + transform.forward);

            // Move player along the rail until there is no more rail
            if (!railCompleted)
            {
                MovePlayerOnRail(Del);
            }
        }

/***************************************************************************************
*   Edit end
*
***************************************************************************************/

        CurrentVel = Rgbody.velocity;
    }

    private void JumpUp()
    {
        Vector3 jumpVelocity = Rgbody.velocity;
        jumpVelocity.y = 0;

        Rgbody.velocity = jumpVelocity;

        Rgbody.AddForce(transform.up * JumpForce, ForceMode.Impulse);

        tryingToGrab = true;

        InAir();
    }


    /***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: New Function
*
***************************************************************************************/

    /***************************************************************************************
*    Title: WallJump
*    
*    Description: Alternate jump function when doing it from a wall. The direction of the jump is based on
*                 the camera's direction and the normal vector of the wall.
*
***************************************************************************************/
    private void WallJump()
    {

        //Debug.Log("Before Rotate Velocity: " + Rgbody.velocity);
        forwardAxis = transform.forward;
        
        // if camera is facing the wall, flip it 180 degrees to the mirror angle facing away from the wall
        if (Vector3.Dot(WallNormal, HeadCam.transform.forward) < 0)
        {
            HeadCam.transform.forward = Quaternion.AngleAxis(180, transform.forward) * HeadCam.transform.forward;
        }

        // Set temp velocity variable to the direction of the camera multiplied to the curent velocity
        Vector3 tempVel = Quaternion.FromToRotation(Rgbody.velocity, 
            HeadCam.transform.forward) * Rgbody.velocity;

        Vector3 jumpVelocity = tempVel;
        jumpVelocity.y = 0;

        tempVel = jumpVelocity;

        //Debug.Log("After Rotate Velocity: " + tempVel);

        Rgbody.velocity = Vector3.zero;


        Debug.Log("Begin Magnitude: " + tempVel.magnitude);

        // Multiply magnitude if too small. Fixes bug where wall jumps have too little force
        if (tempVel.magnitude < 10)
        {
            tempVel *=  10f / tempVel.magnitude;
        }

        Rgbody.AddForce(tempVel + transform.up * (JumpForce*1.5f) + WallNormal * (JumpForce*1.5f), 
            ForceMode.Impulse);

        //Debug.Log("End Velocity: " + tempVel);
        Debug.Log("End Magnitude: " + tempVel.magnitude);

        tryingToGrab = true;

        InAir();
    }


    /***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: New Function
*
***************************************************************************************/
    /***************************************************************************************
*    Title: BouncyJump
*    
*    Description: Alternate jump function when doing it from a bouncy surface. Multiplies JumpForce by 2
*
***************************************************************************************/
    private void BouncyJump()
    {
        Vector3 jumpVelocity = Rgbody.velocity;
        jumpVelocity.y = 0;

        Rgbody.velocity = jumpVelocity;

        Rgbody.AddForce(transform.up * (JumpForce * 2), ForceMode.Impulse);

        tryingToGrab = true;

        InAir();
    }

/***************************************************************************************
*   Edit end
*
***************************************************************************************/

    private void LerpSpeed(float Magnitude, float delta, float targetSpd)
    {
        // current speed multiplied by magnitude of inputs
        float SpdByMag = targetSpd * Magnitude;

        // moving or stopping?
        float thisAccel = Acceleration;
        if (Magnitude == 0)
        {
            thisAccel = Decceleration;
        }

        // lerp current speed
        CurrentSpeed = Mathf.Lerp(CurrentSpeed, SpdByMag, delta * thisAccel);
    }

    private void MovePlayer(float horizontal, float vertical, float delta, float ctrl)
    {
        Vector3 MoveDir = (transform.forward * vertical) + (transform.right * horizontal);
        MoveDir = MoveDir.normalized;

        // if there is no active input, continue in direction of velocity
        if(horizontal == 0 && vertical == 0)
        {
            MoveDir = Rgbody.velocity.normalized;
        }

       /* else
        {

        }*/

        // multiply direction by speed
        MoveDir = MoveDir * CurrentSpeed;
            
        MoveDir.y = Rgbody.velocity.y;
        
        //apply acceleration
        float Accel = (AgilityControl * DirectionalControl) * ctrl; // represents how much control the player has over movement
        Vector3 LerpVel = Vector3.Lerp(Rgbody.velocity, MoveDir, Accel * delta);

        Rgbody.velocity = LerpVel;

        /***************************************************************************************
        *   Edit Author: Philip Smith
        *
        *    Description: Check ground state for animation
        *
        ***************************************************************************************/

        if (CurrentState == PlayerStates.grounded)
        {


            CheckGroundState(horizontal, vertical);

        }

/***************************************************************************************
*   Edit end
*
***************************************************************************************/

        // Test magnitude of axis
        //Debug.Log("Horizontal: " + horizontal + ", Vertical: " + vertical);
    }

    private void MovePlayerOnWall(float vertivalMov, float del)
    {
        // Get movement direction
        Vector3 MoveDir = transform.up * vertivalMov;
        MoveDir = MoveDir * WallRunUpwardsMovement;

        /***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Move player based on where they're looking on wall
*
***************************************************************************************/

        if (Vector3.Dot(HeadCam.transform.forward, WallRight) >= 0)
        {
            MoveDir += WallRight * CurrentSpeed;
        }
        else
        {
            MoveDir += -WallRight * CurrentSpeed;
        }

        /***************************************************************************************
*   Edit end
*
***************************************************************************************/

        Vector3 lerpAmount = Vector3.Lerp(Rgbody.velocity, MoveDir, WallRunAcceleration * del);
        Rgbody.velocity = lerpAmount;

        //Debug.Log("WallRun :" + CurrentWallRunState);
    }


    /***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: New Function
*
***************************************************************************************/
    /***************************************************************************************
    *    Title: MovePlayerOnRail
    *    
    *    Description: Interpolates the player over the rail segment objects. Interacts with the rail class.
    *
    ***************************************************************************************/

    private void MovePlayerOnRail(float del)
    {
        railTransition += del * Mathf.Max(CurrentSpeed, 5f); // increment progress over rail

        // if railTransition reaches over 1, move on to new segment
        if (railTransition > 1)
        {
            railTransition = 0;
            currentRailSeg++;
        }

        else if (railTransition < 0)
        {
            railTransition = 1;
            currentRailSeg--;
        }

        transform.position = currentRail.LinearPosition(currentRailSeg, railTransition);
        transform.position += new Vector3(0, 1, 0);
        transform.forward = currentRail.SegmentForward(currentRailSeg);

        railCompleted = currentRail.railCompleted;
    }


    /***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: New Function
*
***************************************************************************************/
    /***************************************************************************************
*    Title: CheckGroundState
*    
*    Description: Checks what state the player character is in on the gorund for the sake of animation,
*                 horizontal and vertical are the degrees of the controller analog sticks. They help determine
*                 whether the player character is goign forward, back, or sideways.
*
***************************************************************************************/
    private void CheckGroundState(float horizontal, float vertical)
    {

        if(Mathf.Abs(horizontal) >= 0.5 || Mathf.Abs(vertical) >= 0.5)
        {
            fast = true;
        }

        else
        {
            fast = false;
        }

        if (Mathf.Abs(horizontal) < 0.05 && Mathf.Abs(vertical) < 0.05)
        {
            CurrentGroundState = GroundStates.idle;
            return;
        }

        if(vertical > -0.4)
        {
            if (Mathf.Abs(horizontal) + 0.3 > Mathf.Abs(vertical))
            {
                if (fast)
                {
                    if(horizontal > 0)
                    {
                        CurrentGroundState = GroundStates.fastRight;
                    }
                    else
                    {
                        CurrentGroundState = GroundStates.fastLeft;
                    }
                }
                else
                {
                    if (horizontal > 0)
                    {
                        CurrentGroundState = GroundStates.slowRight;
                    }
                    else
                    {
                        CurrentGroundState = GroundStates.slowLeft;
                    }
                }
            }

            else
            {
                if (fast)
                {
                    CurrentGroundState = GroundStates.fastForward;
                }
                else
                {
                    CurrentGroundState = GroundStates.slowForward;
                }
            }
        }

        else
        {
            CurrentGroundState = GroundStates.back;
        }
        
        
    }

    /***************************************************************************************
*   Edit end
*
***************************************************************************************/

    private void TurnPlayer(float xValue, float delta, float speed)
    {
        YTurn += (xValue * delta) * speed;

        transform.rotation = Quaternion.Euler(0, YTurn, 0);

        //EDIT Testing
        beforeWalJForward = Quaternion.Euler(0, (xValue * delta) * speed, 0) * beforeWalJForward;
        afterWalJForward = Quaternion.Euler(0, (xValue * delta) * speed, 0) * afterWalJForward;
        forwardAxis = Quaternion.Euler(0, (xValue * delta) * speed, 0) * forwardAxis;
    }


    /***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: New Function
*
***************************************************************************************/
    /***************************************************************************************
*    Title: TurnCamera
*    
*    Description: For turning the camera independently of the player object. Used on rails and walls
*                 so player animation remains the same.
*
***************************************************************************************/
    private void TurnCamera(float xValue, float delta, float speed)
    {
        YTurn += (xValue * delta) * speed;

        HeadCam.transform.rotation = Quaternion.Euler(0, YTurn, 0);

        //Debug.Log("TC Cam Right: " + HeadCam.transform.right);

        //Testing
        beforeWalJForward = Quaternion.Euler(0, (xValue * delta) * speed, 0) * beforeWalJForward;
        afterWalJForward = Quaternion.Euler(0, (xValue * delta) * speed, 0) * afterWalJForward;
        forwardAxis = Quaternion.Euler(0, (xValue * delta) * speed, 0) * forwardAxis;
    }

    /***************************************************************************************
*   Edit end
*
***************************************************************************************/

    private void LookUpDown(float yValue, float delta)
    {
        XTurn -= (yValue * delta) * LookUpSpeed;

        XTurn = Mathf.Clamp(XTurn, MinLookAngle, MaxLookAngle);

        HeadCam.transform.localRotation = Quaternion.Euler(XTurn, 0, 0);

        //LookUpTimer += delta;

        /*if (CurrentState == PlayerStates.onWall)
        {
            Debug.Log("Axis Y Val: " + yValue);
            Debug.Log("Xturn Val: " + XTurn);
            Debug.Log("LUD Cam Right: " + HeadCam.transform.right);
        }*/
    }

    private void ResetCameraRotation()
    {
        HeadCam.transform.forward = transform.forward;
    }


    /***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: New Function
*
***************************************************************************************/
    /***************************************************************************************
*    Title: ResetObjectRotation
*    
*    Description: Sets the player's object rotation to fit the camera
*
***************************************************************************************/
    private void ResetObjectRotation()
    {
        Vector3 levelForward = new Vector3(HeadCam.transform.forward.x, 0, HeadCam.transform.forward.z);
        transform.forward = Quaternion.FromToRotation(transform.forward, levelForward) * transform.forward;
        XTurn = 0;
        //YTurn = 0;
        HeadCam.transform.localRotation = Quaternion.Euler(XTurn, 0, 0);
        Debug.Log("Reset Rotation: " + HeadCam.transform.forward);
    }


    /***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: New Function
*
***************************************************************************************/
    /***************************************************************************************
*    Title: ResetObjectVelocity
*    
*    Description: Velocity set to object forward direction with neutrality in the y axis.
*
***************************************************************************************/
    private void ResetObjectVelocity()
    {
        Vector3 levelForward = new Vector3(transform.forward.x, 0, transform.forward.z);
        Rgbody.velocity = Quaternion.FromToRotation(Rgbody.velocity, levelForward) * Rgbody.velocity;
    }

    /***************************************************************************************
*   Edit end
*
***************************************************************************************/

    private bool CheckWall(float xVal, float yVal)
    {
        if(xVal == 0 && yVal == 0)
        {
            return false;
        }

        /*if(WallRunTimer > MaxWallRunTime)
        {
            return false;
        }*/

        Vector3 WallDirection = transform.forward * yVal + transform.right * xVal;
        WallDirection = WallDirection.normalized;

        bool wallExists = Collision.CheckWalls(WallDirection);

        return wallExists;
    }

    private void InAir()
    {
        AirborneTimer = 0;
        CurrentState = PlayerStates.airborne;
    }

    private void OnGround()
    {
        GroundedTimer = 0;
        //WallRunTimer = 0; // When grounded, reset wall run timer
        tryingToGrab = false;
        CurrentState = PlayerStates.grounded;
    }

    private void LedgeGrab(Vector3 ledgePos)
    {
        CurrentState = PlayerStates.ledgeGrab;

        LedgePos = ledgePos;
        OriginPos = transform.position;
        Rgbody.velocity = Vector3.zero;
        Rgbody.freezeRotation = true;
        PullUpTimer = 0;
    }


    /***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: New Function
*
***************************************************************************************/
    /***************************************************************************************
*    Title: OnRail
*    
*    Description: Set player state on rail
*
***************************************************************************************/
    private void OnRail()
    {
        CurrentState = PlayerStates.onRail;
        //currentRailSeg = 0;
        railTransition = 0;
        //transform.forward = currentRail.SegmentForward(currentRailSeg);
    }

    private void WallRun()
    {
        CurrentState = PlayerStates.onWall;
        Rgbody.useGravity = false;
    }



    /***************************************************************************************
*     
*    Description: Draw gizmos in editor
*
***************************************************************************************/
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(HeadCam.transform.position,
            HeadCam.transform.position + beforeWalJForward * 100);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(HeadCam.transform.position,
            HeadCam.transform.position + afterWalJForward * 100);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(HeadCam.transform.position,
            HeadCam.transform.position + forwardAxis * 100);
    }

    /***************************************************************************************
*   Edit end
*
***************************************************************************************/
}
