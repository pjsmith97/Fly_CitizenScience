using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class FlyDetectionManager : MonoBehaviour
{
    [SerializeField] private GameObject PlayerCharacter;
    [SerializeField] private Image cameraLens;
    [SerializeField] private CameraLensManager lensManager;
    [SerializeField] private int playerID = 0;
    [SerializeField] private Player RewiredPlayer;
    private Camera mainCamera;
    private Renderer thisRenderer;
    private float oldColorG;

    // Start is called before the first frame update
    void Start()
    {
        RewiredPlayer = ReInput.players.GetPlayer(playerID);
        mainCamera = Camera.main;
        thisRenderer = GetComponent<Renderer>();
        oldColorG = cameraLens.color.g;
    }

    // Update is called once per frame
    void Update()
    {

        if (RewiredPlayer.GetButton("Aim"))
        {
            Vector3 viewPos = mainCamera.WorldToViewportPoint(transform.position);

            if (viewPos.x >= 0.4 && viewPos.x <= 0.6 && viewPos.y >= 0.4 && viewPos.y <= 0.6)
            {
                if (lensManager.CheckObstruction(transform.position) == this.gameObject)
                {
                    if (viewPos.z < 15)
                    {
                        thisRenderer.material.SetColor("_Color", Color.cyan);
                        cameraLens.color = new Vector4(cameraLens.color.r, 255, cameraLens.color.b, cameraLens.color.a);
                        Debug.Log("You see me!!!");
                    }
                    else
                    {

                    }
                }

                else if (thisRenderer.material.color == Color.cyan)
                {
                    thisRenderer.material.SetColor("_Color", Color.white);
                    cameraLens.color = new Vector4(cameraLens.color.r, oldColorG, cameraLens.color.b, cameraLens.color.a);
                    Debug.Log("I'm hidden ;)");
                }


            }

            else if (thisRenderer.material.color == Color.cyan)
            {
                thisRenderer.material.SetColor("_Color", Color.white);
                cameraLens.color = new Vector4(cameraLens.color.r, oldColorG, cameraLens.color.b, cameraLens.color.a);
                Debug.Log("I'm hidden ;)");
            }
        }

        else if (RewiredPlayer.GetButtonUp("Aim"))
        {
            if (thisRenderer.material.color == Color.cyan)
            {
                thisRenderer.material.SetColor("_Color", Color.white);
                cameraLens.color = new Vector4(cameraLens.color.r, oldColorG, cameraLens.color.b, cameraLens.color.a);
                Debug.Log("No more photos...");
            }
        }

    }
}
