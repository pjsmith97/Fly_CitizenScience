using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/***************************************************************************************
*    Title: HeadBobManager
*    Author: Hero 3D
*    Date: May 18th, 2021
*    Edit: December, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Availability: https://youtube.com/watch?v=5MbR2qJK8Tc&si=EnSIkaIECMiOmarE
*    Description: Class that bobs the player's camera to simulate the head bob when moving
*
***************************************************************************************/
public class HeadBobManager : MonoBehaviour
{
    [SerializeField] private bool enableBob;

    [SerializeField, Range(0, 0.1f)] private float amplitude;
    [SerializeField, Range(0, 30)] private float frequency;

    [SerializeField] private Transform camera;

    private float toggleSpd = 3f;
    private Rigidbody rgbody;
    private Vector3 startPos;
    private PlayerMovement playerMovement;// EDIT: PlayerMovement object to keep track of player state
    
    // Start is called before the first frame update
    void Start()
    {
        rgbody = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
        startPos = camera.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!enableBob) return;

        //frequency = Mathf.Lerp(8, 12, playerMovement.CurrentSpeed / playerMovement.MaxSpeed);
        float speedBorder = (7f / 8f) * (float)playerMovement.MaxSpeed;
        if (playerMovement.CurrentSpeed <= speedBorder)
        {

            amplitude = 0.01f;
        }

        else
        {
            amplitude = 0.025f;
        }

        CheckMotion();
        ResetPosition();
    }

    private void PlayMotion(Vector3 motion)
    {
        camera.localPosition += motion;
    }

    private Vector3 StepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency) * amplitude;
        pos.x += Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;
        return pos;
    }

    private void CheckMotion()
    {
        float speed = new Vector3(rgbody.velocity.x, 0, rgbody.velocity.z).magnitude;

        if (speed < toggleSpd) return;

/***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Checks that player is on the ground before simulating head bob
*
***************************************************************************************/
        if (playerMovement.CurrentState != PlayerMovement.PlayerStates.grounded) return;

/***************************************************************************************
*   Edit end
*
***************************************************************************************/

        PlayMotion(StepMotion());
    }

    private void ResetPosition()
    {
        if (camera.localPosition == startPos) return;
        camera.localPosition = Vector3.Lerp(camera.localPosition, startPos, Time.deltaTime);
    }
}
