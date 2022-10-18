using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kino;
using Rewired;

public class RespawnScript : MonoBehaviour
{
    public Player playerRewired;
    [SerializeField] private int playerID = 0;

    [SerializeField] Transform respawnPt;
    [SerializeField] GameObject playerObject;

    [Header("Glitch Transition")]
    [Range(0f, 2f)]
    [SerializeField] float glitchCap;
    [SerializeField] DigitalGlitch dgtGlitch;
    [SerializeField] AnalogGlitch anGlitch;
    [SerializeField] float incrementSpeed;
    public bool glitching;
    public bool recovering;

    private void Start()
    {
        playerRewired = ReInput.players.GetPlayer(playerID);

        glitching = false;
        recovering = false;
    }

    private void Update()
    {
        if (glitching)
        {
            if (!recovering && anGlitch.scanLineJitter >= glitchCap && anGlitch.verticalJump >= glitchCap &&
                dgtGlitch.intensity >= glitchCap)
            {
                recovering = true;
                playerObject.transform.position = respawnPt.position;
                playerObject.transform.forward = Vector3.zero;
                playerObject.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }

            else if (recovering && anGlitch.scanLineJitter <= 0 &&
                    anGlitch.verticalJump <= 0 && dgtGlitch.intensity <= 0)
            {
                recovering = false;
                glitching = false;
                anGlitch.scanLineJitter = 0;
                anGlitch.verticalJump = 0;
                dgtGlitch.intensity = 0;

                if (!GetComponent<ExitMenuManager>().exitUI.activeInHierarchy)
                {
                    playerRewired.controllers.maps.SetMapsEnabled(true, "Default");
                }
                
            }
        }
        
    }

    private void FixedUpdate()
    {
        float Del = Time.deltaTime;

        if (glitching)
        {
            if (recovering)
            {
                Del *= -1;
            }
            
            anGlitch.scanLineJitter += incrementSpeed * Del;
            anGlitch.verticalJump += incrementSpeed * Del;
            dgtGlitch.intensity += incrementSpeed * Del;
        }
    }

    public void Respawn()
    {
        glitching = true;
        playerRewired.controllers.maps.SetMapsEnabled(false, "Default");
    }


}
