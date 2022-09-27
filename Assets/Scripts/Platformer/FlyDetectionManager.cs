using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class FlyDetectionManager : MonoBehaviour
{
    [SerializeField] private int playerID = 0;
    [SerializeField] private Player RewiredPlayer;

    [Header("Camera")]
    [SerializeField] private Image cameraLens;
    [SerializeField] private CameraLensManager lensManager;
    
    [Header("Particles")]
    [SerializeField] private ParticleSystem particles;

    [Header("Score")]
    [SerializeField] private FlyScoreManager flyCounter;


    public bool seen;
    public bool photoTaken;
    private Camera mainCamera;
    //private Renderer thisRenderer;
    private float oldColorG;
    

    // Start is called before the first frame update
    void Start()
    {
        RewiredPlayer = ReInput.players.GetPlayer(playerID);
        mainCamera = Camera.main;
        //thisRenderer = GetComponent<ParticleSystem>().;
        particles = this.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        oldColorG = cameraLens.color.g;
        seen = false;
        photoTaken = false;
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
                    if (viewPos.z < 15 && particles.GetComponent<ParticleSystemRenderer>().material.color != Color.cyan && !photoTaken)
                    {
                        seen = true;
                        Debug.Log("You see me!!!");
                    }
                    else if((viewPos.z >= 15 || photoTaken) && particles.GetComponent<ParticleSystemRenderer>().material.color == Color.cyan)
                    {
                        seen = false;
                        //Debug.Log("I'm hidden ;)");
                    }

                    else if (particles.GetComponent<ParticleSystemRenderer>().material.color == Color.cyan)
                    {
                        if (RewiredPlayer.GetButtonDown("TakePhoto"))
                        {
                            Debug.Log("Picture Taken!");
                            photoTaken = true;
                            seen = false;
                            cameraLens.color = new Vector4(cameraLens.color.r, oldColorG, cameraLens.color.b, cameraLens.color.a);
                            FlyScoreManager.flyPhotos += 1;
                        }
                    }
                }

                else if (particles.GetComponent<ParticleSystemRenderer>().material.color == Color.cyan)
                {
                    seen = false;
                    //Debug.Log("I'm hidden ;)");
                }


            }

            else if (particles.GetComponent<ParticleSystemRenderer>().material.color == Color.cyan)
            {
                seen = false;
                //Debug.Log("I'm hidden ;)");
            }
        }

        else if (RewiredPlayer.GetButtonUp("Aim"))
        {
            if (particles.GetComponent<ParticleSystemRenderer>().material.color == Color.cyan)
            {
                seen = false;
                cameraLens.color = new Vector4(cameraLens.color.r, oldColorG, cameraLens.color.b, cameraLens.color.a);
                Debug.Log("No more photos...");
            }
        }

        if (seen)
        {
            particles.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", Color.cyan);
            cameraLens.color = new Vector4(cameraLens.color.r, 255, cameraLens.color.b, cameraLens.color.a);
        }
        else if(particles.GetComponent<ParticleSystemRenderer>().material.color == Color.white && photoTaken)
        {
            particles.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", Color.cyan);
        }
        else if(particles.GetComponent<ParticleSystemRenderer>().material.color == Color.cyan && !photoTaken)
        {
            particles.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", Color.white);
            cameraLens.color = new Vector4(cameraLens.color.r, oldColorG, cameraLens.color.b, cameraLens.color.a);
        }
    }
}
