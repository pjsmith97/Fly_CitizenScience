using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnExitTrigger : MonoBehaviour
{
    [SerializeField] RespawnScript respawner;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            respawner.Respawn();
        }
    }
}
