using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using TMPro;

public class PhotoFinalResultStateHelper : MonoBehaviour
{
    [Header("Rewired Player")]
    public Player player;
    [SerializeField] private int playerID = 0;

    [Header ("UI")]
    public GameObject resultsUI;
    public GameObject searchHighScoreText;
    public GameObject classHighScoreText;
    public TextMeshProUGUI searchTimeText;
    public TextMeshProUGUI classCountText;
    public TextMeshProUGUI classTimeText;
    public GameObject checkMarkSymbols;
    public bool classHighScore;
    public bool searchHighScore;

    [Header("Progress Variable")]
    public bool done;

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);

        classHighScore = false;
        searchHighScore = false;
    }

    // Update is called once per frame
    public void HelperUpdate()
    {
        if (player.GetButtonDown("Select"))
        {
            done = true;
        }
    }
}
