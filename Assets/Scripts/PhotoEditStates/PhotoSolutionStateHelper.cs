using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Rewired;

public class PhotoSolutionStateHelper : MonoBehaviour
{
    [Header("Rewired Player")]
    public Player player;
    [SerializeField] private int playerID = 0;

    [Header("Photo Manager")]
    public PhotoManager photoManager;

    [Header("UI Objects")]
    public GameObject solutionUI;
    public GameObject solutionImageUI;
    public TextMeshProUGUI solutionTextUI;
    public List<GameObject> imageList;

    [Header("Variables")]
    public string solution;
    public bool correct;
    public bool done = false;

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);

        correct = false;

        solutionImageUI = solutionUI.transform.GetChild(1).gameObject;

        imageList = new List<GameObject>();

        foreach (Transform t in solutionImageUI.transform)
        {
            imageList.Add(t.gameObject);
        }

        solutionTextUI = solutionUI.transform.GetChild(2).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();

        photoManager = GetComponent<PhotoManager>();
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
