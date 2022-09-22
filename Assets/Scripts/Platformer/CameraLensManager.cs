using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class CameraLensManager : MonoBehaviour
{
    [SerializeField] private int playerID = 0;
    [SerializeField] private Player player;

    [SerializeField] private Image cameraLens;
    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetButton("Aim"))
        {
            cameraLens.gameObject.SetActive(true);
        }

        else if (player.GetButtonUp("Aim"))
        {
            cameraLens.gameObject.SetActive(false);
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
            return hitInfo.transform.gameObject;
        }
        else
        {
            return null;
        }
    }
}
