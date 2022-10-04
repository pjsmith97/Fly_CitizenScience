using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerInputMapManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] int playerID = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
        player.controllers.maps.SetMapsEnabled(true, "Default");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
