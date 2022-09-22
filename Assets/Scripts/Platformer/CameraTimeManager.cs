using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class CameraTimeManager : MonoBehaviour
{

    [SerializeField] private int playerID = 0;
    [SerializeField] private Player player;

    [SerializeField] private float slowMoScale;

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
            Time.timeScale = slowMoScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        else if (player.GetButtonUp("Aim"))
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }
}
