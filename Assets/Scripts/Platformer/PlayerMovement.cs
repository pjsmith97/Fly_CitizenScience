using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;


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

    [Header("Rail Slide")]
    public Rail currentRail;
    public int currentRailSeg;
    private float railTransition;
    private bool railCompleted;

    [Header("Animation Bool")]
    public bool fast;

    [Header("Testing")]
    public float LookUpTimer = 0;

    private PlayerCollision Collision;
    private Rigidbody Rgbody;
    private Animator Anim;

    private Vector3 camFixedForward;
    private Vector3 beforeWalJForward;
    private Vector3 afterWalJForward;
    private Vector3 forwardAxis;

    // Start is called before the first frame update
    void Start()
    {
        Collision = GetComponent<PlayerCollision>();
        Rgbody = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();

        player = ReInput.players.GetPlayer(playerID);

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


        // Reset Rigibody components based on game state
        if(CurrentState != PlayerStates.onWall)
        {
            Rgbody.useGravity = true;
        }

        if(CurrentState != PlayerStates.ledgeGrab)
        {
            Rgbody.freezeRotation = false;
        }

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
                OriginForward = transform.forward;
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
                    OriginForward = transform.forward;
                    LedgeGrab(newLedge);
                    //Debug.Log("Grab!");
                }
            }

            // check for walls to run on
            bool wallExists = CheckWall(XMov, YMov);

            if (wallExists && AirborneTimer > 1f)
            {
                Debug.ClearDeveloperConsole();
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
            
            else if (checkBouncy && AirborneTimer > 0.2f)
            {
                BouncyJump();
            }

            else if (checkRail && AirborneTimer > 0.4f)
            {
                OnRail();
            }
        }
        else if (CurrentState == PlayerStates.ledgeGrab)
        {
            // stop velocity 
            Rgbody.velocity = Vector3.zero;


        }
        else if (CurrentState == PlayerStates.onWall)
        {
            //Debug.Log("Camera Update 1: " + HeadCam.transform.forward);

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

            //Debug.Log("Forward 2: " + transform.forward);

            //Debug.Log("Camera Update 2: " + HeadCam.transform.forward);
        }
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
                bool checkRail = Collision.CheckRails(-transform.up, out currentRail, out currentRailSeg);

                if (checkGround)
                {
                    OnGround();
                }

                else if (checkRail)
                {
                    OnRail();
                }
            }
        }
        else if (CurrentState == PlayerStates.onWall)
        {
            // increment wall run timer
            //WallRunTimer += Del;

            TurnCamera(CamX, Del, TurnSpeedOnWalls);

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

    private void WallJump()
    {

        //Debug.Log("Before Rotate Velocity: " + Rgbody.velocity);
        forwardAxis = transform.forward;
        

        if (Vector3.Dot(WallNormal, HeadCam.transform.forward) < 0)
        {
            HeadCam.transform.forward = Quaternion.AngleAxis(180, transform.forward) * HeadCam.transform.forward;
        }

        Vector3 tempVel = Quaternion.FromToRotation(Rgbody.velocity, 
            HeadCam.transform.forward) * Rgbody.velocity;

        Vector3 jumpVelocity = tempVel;
        jumpVelocity.y = 0;

        tempVel = jumpVelocity;

        //Debug.Log("After Rotate Velocity: " + tempVel);

        Rgbody.velocity = Vector3.zero;

        Debug.Log("Begin Magnitude: " + tempVel.magnitude);

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

    private void BouncyJump()
    {
        Vector3 jumpVelocity = Rgbody.velocity;
        jumpVelocity.y = 0;

        Rgbody.velocity = jumpVelocity;

        Rgbody.AddForce(transform.up * (JumpForce * 2), ForceMode.Impulse);

        tryingToGrab = true;

        InAir();
    }

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

        if(CurrentState == PlayerStates.grounded)
        {
            CheckGroundState(horizontal, vertical);
            /*Debug.Log("Ground State: " + CurrentGroundState);
            Debug.Log("Current Speed: " + CurrentSpeed);*/
        }

        // Test magnitude of axis
        //Debug.Log("Horizontal: " + horizontal + ", Vertical: " + vertical);
    }

    private void MovePlayerOnWall(float vertivalMov, float del)
    {
        // Get movement direction
        Vector3 MoveDir = transform.up * vertivalMov;
        MoveDir = MoveDir * WallRunUpwardsMovement;

        if (Vector3.Dot(HeadCam.transform.forward, WallRight) >= 0)
        {
            MoveDir += WallRight * CurrentSpeed;
        }
        else
        {
            MoveDir += -WallRight * CurrentSpeed;
        }

        

        Vector3 lerpAmount = Vector3.Lerp(Rgbody.velocity, MoveDir, WallRunAcceleration * del);
        Rgbody.velocity = lerpAmount;

        //Debug.Log("WallRun :" + CurrentWallRunState);
    }

    private void MovePlayerOnRail(float del)
    {
        railTransition += del * Mathf.Max(CurrentSpeed, 5f); // TODO test this with CurrentSpeed

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

    private void TurnPlayer(float xValue, float delta, float speed)
    {
        YTurn += (xValue * delta) * speed;

        transform.rotation = Quaternion.Euler(0, YTurn, 0);

        //Testing
        beforeWalJForward = Quaternion.Euler(0, (xValue * delta) * speed, 0) * beforeWalJForward;
        afterWalJForward = Quaternion.Euler(0, (xValue * delta) * speed, 0) * afterWalJForward;
        forwardAxis = Quaternion.Euler(0, (xValue * delta) * speed, 0) * forwardAxis;
    }

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

    private void ResetObjectRotation()
    {
        Vector3 levelForward = new Vector3(HeadCam.transform.forward.x, 0, HeadCam.transform.forward.z);
        transform.forward = Quaternion.FromToRotation(transform.forward, levelForward) * transform.forward;
        XTurn = 0;
        //YTurn = 0;
        HeadCam.transform.localRotation = Quaternion.Euler(XTurn, 0, 0);
        Debug.Log("Reset Rotation: " + HeadCam.transform.forward);
    }

    private void ResetObjectVelocity()
    {
        Vector3 levelForward = new Vector3(transform.forward.x, 0, transform.forward.z);
        Rgbody.velocity = Quaternion.FromToRotation(Rgbody.velocity, levelForward) * Rgbody.velocity;
    }

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
}
