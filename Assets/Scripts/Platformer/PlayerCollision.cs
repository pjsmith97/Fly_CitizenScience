using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    public float FloorCheckRadius; //radius of the detection for the floor
    public float RailCheckRadius; //radius of the detection for the floor
    public float bottomOffset; //bottom offset from player center
    public float railBottomOffset; //bottom offset from player center
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
    public LayerMask BouncyLayers; // layers to bounce on
    public LayerMask RailLayers; // layers to slide on rail

    //private float rayUpOffset = 0.2f; //Offset of ray to determine wall normal

    public bool CheckFloor(Vector3 Dir)
    {
        Vector3 pos = transform.position + (Dir * bottomOffset); //position of floor checker
        Collider[] ColHit = Physics.OverlapSphere(pos, FloorCheckRadius, FloorLayers); // check for floor below
        if(ColHit.Length > 0)
        {
            // there is something to stand on
            return true;
        }

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

        return Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = transform.position + (-transform.up * bottomOffset);
        Gizmos.DrawSphere(pos, FloorCheckRadius);

        Gizmos.color = Color.green;
        Vector3 posRail = transform.position + (-transform.up * railBottomOffset);
        Gizmos.DrawSphere(posRail, RailCheckRadius);

        Gizmos.color = Color.red;
        Vector3 pos2 = transform.position + (transform.forward * frontOffset);
        Gizmos.DrawSphere(pos2, WallCheckRadius);

        Gizmos.color = Color.cyan;
        Vector3 pos3 = transform.position + (transform.forward * ForwardLedgeCheckPos) + (transform.up * UpwardLedgeCheckPos);
        Gizmos.DrawLine(pos3, pos3 - (transform.up)*LedgeCheckDistance);
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
