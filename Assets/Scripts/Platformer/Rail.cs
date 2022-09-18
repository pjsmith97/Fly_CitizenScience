using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Rail : MonoBehaviour
{
    public Transform[] nodes;
    public bool railCompleted;

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
            railCompleted = true;
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

    public Vector3 SegmentForward(int seg)
    {
        return nodes[seg].forward; 
    }
}
