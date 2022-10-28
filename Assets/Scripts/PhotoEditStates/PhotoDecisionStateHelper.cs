using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Rewired;
using TMPro;


public class PhotoDecisionStateHelper : MonoBehaviour
{
    public Player player;
    [SerializeField] private int playerID = 0;

    public PhotoManager photoManager;

    //public EventSystem eventSystem;

    public bool done = false;
    public bool back = false;
    public bool hint = false;

    public Dictionary<string, GameObject> buttonOptions;
    public Dictionary<int, string> buttonIndexOptions;
    public Dictionary<int, GameObject> symbolIndexOptions;
    public Dictionary<int, GameObject> textIndexOptions;


    [Header("Buttons")]
    private string buttonDecision;    
    public Transform buttons;

    public int buttonIndex;
    private int buttonCount;

    [SerializeField] int incrementSpeed;
    private float incrementTimer = 1;

    public float guessingTimer = 0;

    [Header("Decision UI")]
    public GameObject decisionUI;
    public GameObject symbolsUI;
    public GameObject textUI;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI helpText;
    public GameObject hintUI;
    public int symbolIndex;
    [SerializeField] private float likelyAlpha;
    [SerializeField] private float symbolAlphaSpeed;
    private float symbolAlphaTimer;
    public bool likelySymbol; // if the symbol is likely male or likely female
    public float symbolMaxAlpha;

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(playerID);

        buttonIndex = 0;

        buttonCount = buttons.childCount;

        buttonIndexOptions = new Dictionary<int, string>();

        buttonIndexOptions[0] = "female";
        buttonIndexOptions[1] = "likelyFemale";
        buttonIndexOptions[2] = "male";
        buttonIndexOptions[3] = "likelyMale";
        buttonIndexOptions[4] = "cantSee";

        buttonOptions = new Dictionary<string, GameObject>();
        int i = 0;
        foreach (Transform child in buttons)
        {
            buttonOptions[buttonIndexOptions[i]] = child.gameObject;
            i++;
        }

        symbolIndexOptions = new Dictionary<int, GameObject>();
        i = 0;
        foreach (Transform child in symbolsUI.transform)
        {
            symbolIndexOptions[i] = child.gameObject;
            child.gameObject.SetActive(false);
            i++;
        }

        textIndexOptions = new Dictionary<int, GameObject>();
        i = 0;
        foreach (Transform child in textUI.transform)
        {
            textIndexOptions[i] = child.gameObject;
            child.gameObject.SetActive(false);
            i++;
        }

        symbolIndex = 0;

        symbolMaxAlpha = 1;

        likelySymbol = false;

        photoManager = GetComponent<PhotoManager>();

        //symbolIndexOptions[0].SetActive(true);
    }

    // Update is called once per frame
    public void HelperUpdate()
    {
        if (!hint)
        {
            if (player.GetButtonDown("Select"))
            {
                done = true;
                //var sendTask = photoManager.SendSpiPollInfo(buttonIndexOptions[buttonIndex], (int) guessingTimer);
            }

            else if (player.GetButtonDown("Back"))
            {
                back = true;
            }

            else if (player.GetButtonDown("Help"))
            {
                hint = true;
                hintUI.SetActive(true);
            }
        }

        else if (player.GetButtonDown("Help"))
        {
            hint = false;
            hintUI.SetActive(false);
        }
    }

    public void HelperFixedUpdate()
    {
        if (!hint)
        {
            float Del = Time.deltaTime;
            float leftStickVal = player.GetAxis("ButtonSelection");

            guessingTimer += Del;

            int timerMinuteNum = Mathf.FloorToInt(guessingTimer / 60);
            int timerSecondNum = Mathf.FloorToInt(guessingTimer % 60);
            string timerMinute = timerMinuteNum < 10 ? "0" + timerMinuteNum : "" + timerMinuteNum;
            string timerSecond = timerSecondNum < 10 ? "0" + timerSecondNum : "" + timerSecondNum;

            string timerString = timerMinute + ":" + timerSecond;

            timerText.text = timerString;


            if (leftStickVal > 0.1 || leftStickVal < -0.1)
            {
                if (incrementTimer == 1)
                {
                    // If this a positive or negative increment
                    bool posIncrement;

                    // Increment button index appropriately to the input
                    if (leftStickVal > 0)
                    {
                        posIncrement = true;
                    }
                    else
                    {
                        posIncrement = false;
                    }

                    IncrementButton(posIncrement);
                }

                incrementTimer += incrementSpeed * Del;
            }

            else
            {
                incrementTimer = 1;
            }

            // Set up symbol image alpha fade in
            if (likelySymbol)
            {
                symbolMaxAlpha = likelyAlpha;
            }

            else
            {
                symbolMaxAlpha = 1;
            }

            if (symbolIndexOptions[symbolIndex].GetComponent<RawImage>().color.a < symbolMaxAlpha)
            {
                var tempColor = symbolIndexOptions[symbolIndex].GetComponent<RawImage>().color;
                tempColor.a = Mathf.Lerp(0, symbolMaxAlpha, symbolAlphaTimer);
                symbolIndexOptions[symbolIndex].GetComponent<RawImage>().color = tempColor;

                symbolAlphaTimer += symbolAlphaSpeed * Del;
            }
        }
    }

    public void SetSymbolFadeTimer(float newTime)
    {
        symbolAlphaTimer = newTime;
    }

    private void IncrementButton(bool positiveIncrement)
    {
        if (positiveIncrement)
        {
            if (buttonIndex < buttonCount - 1)
            {
                symbolIndexOptions[symbolIndex].SetActive(false);
                textIndexOptions[buttonIndex].SetActive(false);
                buttonIndex++;
            }
        }
        else
        {
            if (buttonIndex > 0)
            {
                symbolIndexOptions[symbolIndex].SetActive(false);
                textIndexOptions[buttonIndex].SetActive(false);
                buttonIndex--;
            }
        }

        if (buttonIndex == 0 || buttonIndex == 1)
        {
            symbolIndex = 0;
        }

        else if (buttonIndex == 2 || buttonIndex == 3)
        {
            symbolIndex = 1;
        }

        else if (buttonIndex == 4)
        {
            symbolIndex = 2;
        }


        if (buttonIndex == 1 || buttonIndex == 3)
        {
            likelySymbol = true;
        }
        else
        {
            likelySymbol = false;
        }

        var tempColor = symbolIndexOptions[symbolIndex].GetComponent<RawImage>().color;
        tempColor.a = 0;
        symbolIndexOptions[symbolIndex].GetComponent<RawImage>().color = tempColor;

        symbolAlphaTimer = 0;

        symbolIndexOptions[symbolIndex].SetActive(true);

        textIndexOptions[buttonIndex].SetActive(true);

        // Change selected button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(buttonOptions[buttonIndexOptions[buttonIndex]]);

        // Reset increment timer
        incrementTimer = 0;
    }
}
