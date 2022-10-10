using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnEnterTrigger : MonoBehaviour
{
    [SerializeField] RespawnScript respawner;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            respawner.Respawn();
        }
    }
}
