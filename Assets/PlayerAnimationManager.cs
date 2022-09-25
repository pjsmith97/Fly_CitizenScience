using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] private int playerID = 0;
    [SerializeField] private Player player;

    [SerializeField] private Animator anim;
    [SerializeField] PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
