using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class CameraLensManager : MonoBehaviour
{
    [SerializeField] private int playerID = 0;
    [SerializeField] private Player player;

    [Header("Camera Lens")]
    [SerializeField] private Image cameraLens;
    [SerializeField] private float shutterSpeed;
    [SerializeField] private float shutterClosingScale;
    public bool shutterMoving;
    public bool shutterClosing;

    private Vector3 startingScale;

    // Testing
    public bool cameraRay;
    private float rayDist;
    private Vector3 rayDirection;
    public Vector3 currentFlyPos;

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
        shutterMoving = false;
        shutterClosing = false;
        startingScale = cameraLens.transform.localScale;

        cameraRay = false;
        rayDist = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetButton("Aim"))
        {
            cameraLens.gameObject.SetActive(true);

            if (player.GetButtonDown("TakePhoto"))
            {
                shutterMoving = true;
                shutterClosing = true;
            }
        }

        else if (player.GetButtonUp("Aim"))
        {
            cameraLens.gameObject.SetActive(false);
            shutterMoving = false;
            shutterClosing = false;
            //cameraRay = false;

            cameraLens.transform.localScale = new Vector3(startingScale.x, startingScale.y, startingScale.z);
        }
     }

    private void FixedUpdate()
    {
        float Del = Time.deltaTime;

        if (shutterMoving)
        {
            if (shutterClosing)
            {
                cameraLens.transform.localScale = new Vector3(cameraLens.transform.localScale.x - (Del * shutterSpeed),
                    cameraLens.transform.localScale.y - (Del * shutterSpeed), cameraLens.transform.localScale.z /*- (Del * shutterSpeed)*/);

                if(cameraLens.transform.localScale.x <= 0.5 * startingScale.x)
                {
                    shutterClosing = false;
                }
            }

            else
            {
                cameraLens.transform.localScale = new Vector3(cameraLens.transform.localScale.x + (Del * shutterSpeed),
                    cameraLens.transform.localScale.y + (Del * shutterSpeed), cameraLens.transform.localScale.z /*+ (Del * shutterSpeed)*/);

                if (cameraLens.transform.localScale.x >= startingScale.x)
                {
                    shutterMoving = false;
                }
            }
        }
    }

    public GameObject CheckObstruction(Vector3 flyPos)
    {
        //Create a ray pointing at fly
        Vector3 rayStart = transform.position;
        Vector3 rayDir = flyPos - rayStart;
        Ray ray = new Ray(rayStart, rayDir);

        // Cast the ray at the fly
        RaycastHit hitInfo = new RaycastHit();
        
        if(Physics.Raycast(ray, out hitInfo))
        {
            cameraRay = true;
            rayDirection = ray.direction;
            rayDist = hitInfo.distance;
            currentFlyPos = flyPos;
            return hitInfo.transform.gameObject;
        }
        else
        {
            cameraRay = false;
            return null;
        }
    }

    private void OnDrawGizmos()
    {
        /*if (cameraRay)
        {
            //Debug.Log("Drawing Ray: " + rayDirection);
            //Debug.Log("Position: " + currentFlyPos);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentFlyPos);
        }*/
        
    }
}
