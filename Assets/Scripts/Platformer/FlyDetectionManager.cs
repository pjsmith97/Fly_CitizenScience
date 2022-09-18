using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class FlyDetectionManager : MonoBehaviour
{
    [SerializeField] private GameObject PlayerCharacter;
    [SerializeField] private int playerID = 0;
    [SerializeField] private Player RewiredPlayer;
    private Renderer thisRenderer;

    // Start is called before the first frame update
    void Start()
    {
        RewiredPlayer = ReInput.players.GetPlayer(playerID);
        thisRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (RewiredPlayer.GetButton("Aim"))
        {
            if (thisRenderer.isVisible)
            {
                thisRenderer.material.SetColor("_Color", Color.cyan);
                Debug.Log("You see me!!!");
            }

            else if (thisRenderer.material.color == Color.cyan)
            {
                thisRenderer.material.SetColor("_Color", Color.white);
                Debug.Log("I'm hidden ;)");
            }
        }

        else if (RewiredPlayer.GetButtonUp("Aim"))
        {
            if (thisRenderer.material.color == Color.cyan)
            {
                thisRenderer.material.SetColor("_Color", Color.white);
                Debug.Log("No more photos...");
            }
        }

    }
}
