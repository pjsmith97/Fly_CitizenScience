using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/***************************************************************************************
*    Title: PlayerCollision
*    Author: Slug Glove
*    Date: November 14th, 2020
*    Edit: December, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Availability: https://www.youtube.com/watch?v=ZxWfkOhl6bQ&list=PLbT7sIsvd6RUPai-zx8wQ83QP8DG38Y9W&index=26
*    Description: Manages the logic of the player character's proximity triggers. Helps PlayerMovement.
*
***************************************************************************************/
public class PlayerCollision : MonoBehaviour
{

    public float FloorCheckRadius; //radius of the detection for the floor
    public float RailCheckRadius; //radius of the detection for the floor
    public float bottomOffset; //bottom offset from player center
    public float railBottomOffset; //EDIT: bottom offset from player center
    public float WallCheckRadius; //radius of the detection for the wall
    public float frontOffset; //front offset from the player center
    public float RoofCheckRadius; // the amount we check before standing up
    public float upOffset; //offset upwards from player center

    public float ForwardLedgeCheckPos; //position in front of the player where we check for ledges
    public float UpwardLedgeCheckPos; //position above the player where we check for ledges
    public float LedgeCheckDistance; // distance from raycast before player can grab ledge

    public LayerMask FloorLayers; // layers to stand on
    public LayerMask WallLayers; // layers to wall run on
    public LayerMask LedgeGrabLayers; // layers to grab onto
    public LayerMask BouncyLayers; // EDIT: layers to bounce on
    public LayerMask RailLayers; // EDIT: layers to slide on rail

    //private float rayUpOffset = 0.2f; //Offset of ray to determine wall normal

    private Vector3 collisionCamForward;

    [Header("Wall Run Cam Test")]
    public Vector3 wallDir;
    public Vector3 camWallRight;
    public Vector3 onDrawCamRight;
    public Vector3 fixedCamRight;
    public float dot;

    private void FixedUpdate()
    {
        fixedCamRight = GetComponent<PlayerMovement>().HeadCam.transform.right;
    }

    public bool CheckFloor(Vector3 Dir)
    {
        Vector3 pos = transform.position + (Dir * bottomOffset); //position of floor checker
        Collider[] ColHit = Physics.OverlapSphere(pos, FloorCheckRadius, FloorLayers); // check for floor below
        if(ColHit.Length > 0)
        {
            // there is something to stand on
            return true;
        }

        //Debug.Log("Not touching ground!");
        return false;
    }

    public bool CheckBouncy(Vector3 Dir)
    {
        Vector3 pos = transform.position + (Dir * bottomOffset); //position of floor checker
        Collider[] ColHit = Physics.OverlapSphere(pos, FloorCheckRadius, BouncyLayers); // check for floor below
        if (ColHit.Length > 0)
        {
            // there is a bouncy surface below
            return true;
        }

        return false;
    }

    public bool CheckRails(Vector3 Dir, out Rail newRail, out int startingSeg)
    {
        Vector3 pos = transform.position + (Dir * railBottomOffset); //position of floor checker
        Collider[] ColHit = Physics.OverlapSphere(pos, RailCheckRadius, RailLayers); // check for floor below
        if (ColHit.Length > 0)
        {
            // there is a rail below

            //Assign new rail
            newRail = ColHit[0].gameObject.GetComponent<Rail>();
            //Find the closest segment on the rail
            startingSeg = ClosestRailSegment(newRail, ColHit[0].ClosestPoint(transform.position));
            //
            return true;
        }
        newRail = null;
        startingSeg = -1;
        return false;
    }

    private int ClosestRailSegment(Rail newRail, Vector3 collisionPt)
    {
        int nodeslength = newRail.nodes.Length;

        int seg = -1;
        float segDist = 9999;

        for(int i = 0; i < nodeslength; i++)
        {
            float mag = Vector3.Magnitude(collisionPt - newRail.nodes[i].position);
            if(mag < segDist)
            {
                seg = i;
                segDist = mag;
            }
        }

        return seg;
    }

    public bool CheckWalls(Vector3 Dir)
    {
        Vector3 pos = transform.position + (Dir * frontOffset); //position of wall checker in front
        Collider[] ColHit = Physics.OverlapSphere(pos, WallCheckRadius, WallLayers); // check for walls
        if (ColHit.Length > 0)
        {
/***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Sends ray out to the collided Wall. Retrieves info about wall's normal. Determines based on camera
*                 direction the direction the player runs towards.
*
***************************************************************************************/
            // Find closest point on the wall the player is near
            Vector3 collisionPt = ColHit[0].ClosestPoint(transform.position);

            //Create a ray pointing at another nearby position on the wall
            Vector3 rayStart = transform.position;
            Vector3 rayDir = collisionPt - rayStart;
            Ray ray = new Ray(rayStart, rayDir);

            // Cast the ray at the point on the wall
            RaycastHit hitInfo = new RaycastHit();
            ColHit[0].Raycast(ray, out hitInfo, WallCheckRadius);

            // Return the normal
            GetComponent<PlayerMovement>().WallNormal = hitInfo.normal;
            GetComponent<PlayerMovement>().WallRight = ColHit[0].transform.right;

            Vector3 newDir;
            Quaternion WalltoCamAngle;

            //Determine if wall is on the right or left
            if (Vector3.Dot(GetComponent<PlayerMovement>().WallRight,
                GetComponent<PlayerMovement>().HeadCam.transform.forward) < 0)
            {
                GetComponent<PlayerMovement>().CurrentWallRunState = PlayerMovement.WallRunStates.rightWall;
                newDir = -GetComponent<PlayerMovement>().WallRight;

                //Debug.Log(GetComponent<PlayerMovement>().CurrentWallRunState);
            }
            else
            {
                GetComponent<PlayerMovement>().CurrentWallRunState = PlayerMovement.WallRunStates.leftWall;
                newDir = GetComponent<PlayerMovement>().WallRight;

                //Debug.Log(GetComponent<PlayerMovement>().CurrentWallRunState);
            }

            wallDir = rayDir;


            //Debug.Log(GetComponent<PlayerMovement>().CurrentWallRunState);

            camWallRight = GetComponent<PlayerMovement>().HeadCam.transform.right;
            dot = Vector3.Dot(rayDir, GetComponent<PlayerMovement>().HeadCam.transform.right);

            Vector3 oldCamForward = GetComponent<PlayerMovement>().HeadCam.transform.forward;

            transform.forward = newDir;

            WalltoCamAngle = Quaternion.FromToRotation(oldCamForward, 
                GetComponent<PlayerMovement>().HeadCam.transform.forward);
            

            GetComponent<PlayerMovement>().HeadCam.transform.rotation *= Quaternion.Inverse(WalltoCamAngle);

            

            if (rayDir.magnitude >= 0.5 * WallCheckRadius)
            {
                transform.position += rayDir * (0.1f * WallCheckRadius);
            }

            collisionCamForward = GetComponent<PlayerMovement>().HeadCam.transform.forward;
            //Debug.Log("Collsion Cam Forward: " + collisionCamForward);

/***************************************************************************************
*   Edit end
*
***************************************************************************************/

            // there is a wall in front of player
            return true;
        }

        return false;
    }

    public Vector3 CheckLedges()
    {
        Vector3 RayPos = transform.position + (transform.forward * ForwardLedgeCheckPos) + (transform.up * UpwardLedgeCheckPos);

        RaycastHit Hit;
        if(Physics.Raycast(RayPos, -transform.up, out Hit, LedgeCheckDistance, LedgeGrabLayers))
        {
            return Hit.point;
        }

/***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Second ledge ray to accomodate for when the farther edge ray misses the ledge
*
***************************************************************************************/

        RayPos = transform.position + (transform.forward * ForwardLedgeCheckPos * 0.725f) + (transform.up * UpwardLedgeCheckPos);
        if (Physics.Raycast(RayPos, -transform.up, out Hit, LedgeCheckDistance, LedgeGrabLayers))
        {
            return Hit.point;
        }

/***************************************************************************************
*   Edit end
*
***************************************************************************************/

        return Vector3.zero;
    }

    /***************************************************************************************
*
*    Description: Draws Gizmos on selected in editor mode
*
***************************************************************************************/

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = transform.position + (-transform.up * bottomOffset);
        Gizmos.DrawSphere(pos, FloorCheckRadius);

        Gizmos.color = Color.green;
        Vector3 posRail = transform.position + (-transform.up * railBottomOffset);
        Gizmos.DrawSphere(posRail, RailCheckRadius);


        Gizmos.color = Color.cyan;
        Vector3 pos3 = transform.position + (transform.forward * ForwardLedgeCheckPos) + (transform.up * UpwardLedgeCheckPos);
        Gizmos.DrawLine(pos3, pos3 - (transform.up) * LedgeCheckDistance);

        Vector3 pos4 = transform.position + (transform.forward * ForwardLedgeCheckPos*0.725f) + (transform.up * UpwardLedgeCheckPos);
        Gizmos.DrawLine(pos4, pos4 - (transform.up) * LedgeCheckDistance);
    }

    private void OnDrawGizmos()
    {
       /* Gizmos.color = Color.red;
        Gizmos.DrawLine(GetComponent<PlayerMovement>().HeadCam.transform.position,
            GetComponent<PlayerMovement>().HeadCam.transform.position + 
            GetComponent<PlayerMovement>().HeadCam.transform.forward * 100);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(GetComponent<PlayerMovement>().HeadCam.transform.position,
            GetComponent<PlayerMovement>().HeadCam.transform.position +
            collisionCamForward * 100);

        onDrawCamRight = GetComponent<PlayerMovement>().HeadCam.transform.right;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(GetComponent<PlayerMovement>().HeadCam.transform.position,
            GetComponent<PlayerMovement>().HeadCam.transform.position +
            camWallRight * 100);*/
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
