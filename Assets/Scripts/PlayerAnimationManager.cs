using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] private int playerID = 0;
    [SerializeField] private Player player;

    [SerializeField] private Animator anim;
    private PlayerMovement playerMovement;

    private bool enteredWallMove;
    private bool enteredAirborne;

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
        playerMovement = GetComponent<PlayerMovement>();

        enteredWallMove = false;
        enteredAirborne = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        float XMov = player.GetAxis("Horizontal");
        float YMov = player.GetAxis("Vertical");

        if(playerMovement.CurrentState == PlayerMovement.PlayerStates.grounded)
        {
            PlayerMovement.GroundStates groundState = playerMovement.CurrentGroundState;

            if (enteredWallMove)
            {
                NoWallMovement();
            }

            if (enteredAirborne)
            {
                anim.SetBool("airborne", false);
                enteredAirborne = false;
            }

            if (anim.GetBool("onRail"))
            {
                anim.SetBool("onRail", false);
            }

            if (Mathf.Abs(XMov) > 0 || Mathf.Abs(YMov) > 0)
            {
                if (anim.GetBool("isIdle"))
                {
                    anim.SetBool("isIdle", false);
                }

                anim.SetBool("slowForward", groundState == PlayerMovement.GroundStates.slowForward);
                anim.SetBool("fastForward", groundState == PlayerMovement.GroundStates.fastForward);
                anim.SetBool("slowRight", groundState == PlayerMovement.GroundStates.slowRight);
                anim.SetBool("slowLeft", groundState == PlayerMovement.GroundStates.slowLeft);
                anim.SetBool("fastRight", groundState == PlayerMovement.GroundStates.fastRight);
                anim.SetBool("fastLeft", groundState == PlayerMovement.GroundStates.fastLeft);
                anim.SetBool("back", groundState == PlayerMovement.GroundStates.back);
            }

            else
            {
                anim.SetBool("isIdle", groundState == PlayerMovement.GroundStates.idle);
                NoGroundMovementState();
            }
        }
        else if(playerMovement.CurrentState == PlayerMovement.PlayerStates.onWall)
        {
            NoGroundMovementState();

            PlayerMovement.WallRunStates wallRunState = playerMovement.CurrentWallRunState;

            if (!enteredWallMove)
            {
                anim.SetBool("airborne", true);
                enteredWallMove = true;
            }
            else
            {
                anim.SetBool("airborne", false);
            }

            /*if (enteredAirborne)
            {
                enteredAirborne = false;
            }*/

            anim.SetBool("wallRunRight", wallRunState == PlayerMovement.WallRunStates.rightWall);
            anim.SetBool("wallRunLeft", wallRunState == PlayerMovement.WallRunStates.leftWall);

            
        }
        else if (playerMovement.CurrentState == PlayerMovement.PlayerStates.airborne)
        {
            
            if (!enteredAirborne)
            {
                enteredAirborne = true;
                Debug.Log("AIRBORNE");
            }

            NoGroundMovementState();

            if (anim.GetBool("isIdle"))
            {
                anim.SetBool("isIdle", false);
            }

            if (anim.GetBool("onRail"))
            {
                anim.SetBool("onRail", false);
            }

            NoWallMovement();
            anim.SetBool("airborne", true);
            
        }
        else if(playerMovement.CurrentState == PlayerMovement.PlayerStates.onRail)
        {
            NoWallMovement();
            NoGroundMovementState();
            if (anim.GetBool("airborne"))
            {
                anim.SetBool("airborne", false);
            }
            if (!anim.GetBool("onRail"))
            {
                anim.SetBool("onRail", true);
            }
        }
    }

    private void NoGroundMovementState()
    {
        if (anim.GetBool("slowForward"))
        {
            anim.SetBool("slowForward", false);
        }
        if (anim.GetBool("fastForward"))
        {
            anim.SetBool("fastForward", false);
        }
        if (anim.GetBool("slowRight"))
        {
            anim.SetBool("slowRight", false);
        }
        if (anim.GetBool("slowLeft"))
        {
            anim.SetBool("slowLeft", false);
        }
        if (anim.GetBool("fastRight"))
        {
            anim.SetBool("fastRight", false);
        }
        if (anim.GetBool("fastLeft"))
        {
            anim.SetBool("fastLeft", false);
        }
        if (anim.GetBool("back"))
        {
            anim.SetBool("back", false);
        }
    }

    private void NoWallMovement()
    {
        if (anim.GetBool("wallRunLeft"))
        {
            anim.SetBool("wallRunLeft", false);
            //anim.SetBool("airborne", true);
        }
        if (anim.GetBool("wallRunRight"))
        {
            anim.SetBool("wallRunRight", false);
            //anim.SetBool("airborne", true);
        }

        enteredWallMove = false;
    }
}
