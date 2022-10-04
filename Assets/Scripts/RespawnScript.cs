using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    [SerializeField] Transform respawnPt;
    [SerializeField] GameObject player;

    
    public void Respawn()
    {
        player.transform.position = respawnPt.position;
        player.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
