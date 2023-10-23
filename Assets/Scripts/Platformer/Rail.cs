using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/***************************************************************************************
*    Title: Rail
*    Author: N3K EN
*    Date: September 23rd, 2016
*    Edit: December, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Availability: https://www.youtube.com/watch?v=URqjHIz6pts&list=PLbT7sIsvd6RUPai-zx8wQ83QP8DG38Y9W&index=28
*    Description: Rail Class. Handles rail node transforms and finds positions on the rail 
*
***************************************************************************************/
public class Rail : MonoBehaviour
{
    public Transform[] nodes; // rail nodes. Invisible game objects that tell the player object where to go on the rail
    public bool railCompleted; // is the rail completed

    private void Start()
    {
        nodes = GetComponentsInChildren<Transform>();
        railCompleted = false;
    }

    private void OnDrawGizmos()
    {
        
    }

    public Vector3 LinearPosition(int segment, float ratio)
    {
        Vector3 p1 = nodes[segment].position;
        Vector3 p2;
        if (segment+1 < nodes.Length)
        {
            p2 = nodes[segment + 1].position;
        }
        else
        {
            p2 = p1;

            /***************************************************************************************
*   Edit Author: Philip Smith
*
*    Description: Check if player reaches the last node of the rail
*
***************************************************************************************/
            railCompleted = true;

/***************************************************************************************
*   Edit end
*
***************************************************************************************/
        }

        return Vector3.Lerp(p1, p2, ratio);
    }

    public Quaternion Orientation (int segment, float ratio)
    {
        Quaternion q1 = nodes[segment].rotation;
        Quaternion q2;


        if (segment + 1 < nodes.Length)
        {
            q2 = nodes[segment + 1].rotation;
        }
        else
        {
            q2 = q1;
        }

        return Quaternion.Lerp(q1, q2, ratio);
    }

    /***************************************************************************************
*   Edit Author: Philip Smith
*
*	Description: Get forward direction of segment
*
***************************************************************************************/

    public Vector3 SegmentForward(int seg)
    {
        return nodes[seg].forward; 
    }


    /***************************************************************************************
    *   Edit end
    *
    ***************************************************************************************/
}
