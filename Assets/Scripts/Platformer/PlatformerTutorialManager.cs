using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlatformerTutorialManager : MonoBehaviour
{
    public Player player;
    [SerializeField] private int playerID = 0;

    [Header("Panels")]
    [SerializeField] GameObject textPanel;

    [Header("Specific Objects")]
    public List<GameObject> instructions;
    public List<GameObject> inputPrompts;

    private PlatformerTutorialUIState currentUIState;

    [Header("UI Transition State")]
    public bool uiTextTransition;
    public float fadeInSpeed;
    public int instructionIndex;
    public bool endTutorial;

    // Start is called before the first frame update
    void Start()
    {
        // Get Instruction Text Mesh objects
        for (int i = 0; i < textPanel.transform.childCount - 1; i++)
        {
            instructions.Add(textPanel.transform.GetChild(i).gameObject);
        }

        // Get input prompts text mesh objects
        for (int i = 0; i < textPanel.transform.GetChild(textPanel.transform.childCount - 1).childCount; i++)
        {
            inputPrompts.Add(textPanel.transform.GetChild(textPanel.transform.childCount - 1).GetChild(i).gameObject);
        }

        player = ReInput.players.GetPlayer(playerID);

        instructionIndex = 0;

        endTutorial = false;

        UpdateInputPrompts();

        ChangeUIState(new PlatformerTutorialUITransitionState());

        player.controllers.maps.SetMapsEnabled(true, "Tutorial");
    }

    // Update is called once per frame
    void Update()
    {
        currentUIState.Update();

        if (!uiTextTransition)
        {
            if (player.GetButtonDown("Next"))
            {
                if (instructionIndex + 1 < instructions.Count)
                {
                    instructionIndex++;
                    ChangeInstructionPanel(instructionIndex - 1);
                }

            }

            if (player.GetButtonDown("Prev"))
            {
                if (instructionIndex - 1 >= 0)
                {
                    instructionIndex--;
                    ChangeInstructionPanel(instructionIndex + 1);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        currentUIState.FixedUpdate();
    }

    private void ChangeInstructionPanel(int prevIndex)
    {
        instructions[prevIndex].SetActive(false);
        instructions[instructionIndex].SetActive(true);

        UpdateInputPrompts();

        uiTextTransition = true;
        ChangeUIState(new PlatformerTutorialUITransitionState());
    }

    private void UpdateInputPrompts()
    {
        if (instructionIndex == 0)
        {
            inputPrompts[1].SetActive(false);
        }

        else if (instructionIndex >= 1)
        {
            inputPrompts[1].SetActive(true);
            if (instructionIndex == instructions.Count - 1)
            {
                endTutorial = true;
                inputPrompts[0].SetActive(false);
            }
            else if (endTutorial)
            {
                endTutorial = false;
                inputPrompts[0].SetActive(true);
            }
        }
    }

    public void ChangeUIState(PlatformerTutorialUIState state)
    {
        //Debug.Log("Changing State!");
        if (currentUIState != null)
        {
            //Debug.Log("Exiting State!");
            currentUIState.Exit();
        }
        currentUIState = state;
        currentUIState.Enter(this);
    }
}
