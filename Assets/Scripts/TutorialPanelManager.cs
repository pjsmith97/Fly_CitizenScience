using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;
using TMPro;

/***************************************************************************************
*    Title: TutorialPanelManager
*    Author: Philip Smith
*    Date: December, 2022
*    Edit Author: Philip Smith
*    Code version: 1.0
*    Description: Class that governs the panels in Classification Tutorial slideshow
*
***************************************************************************************/
public class TutorialPanelManager : MonoBehaviour
{
    // Type indicating what diagrams are on screen
    public enum Diagrams
    {
        edit, //edit state diagrams
        flyBody, // fly body photos
        flyCloseUp, // fly close ups
        none // no diagrams
    }

    public Diagrams activeDiagram; //Active diagram on screen

    public Player player; //ReWire player object
    [SerializeField] private int playerID = 0; //Rewire playerID

    [Header("Panels")]
    [SerializeField] GameObject textPanel; // Text panel
    [SerializeField] GameObject diagramPanel; //Diagram panel
    [SerializeField] GameObject labels; // Diagram labels

    [Header("Specific Objects")]
    public List<GameObject> instructions; //instruction text objects
    public List<GameObject> inputPrompts; // button input label prompts
    public List<GameObject> exampleScreenshots; // Screenshot objects
    public List<GameObject> flyBodyPhotos; // fly body photos
    public List<GameObject> flyCloseUpPhotos; // fly close up photos
    public List<GameObject> editLabels; // labels for edit diagram
    public List<GameObject> photoLabels; //labels for photos

    private PhotoCheckTutorialUIState currentUIState;

    [Header("UI Transition State")]
    public bool uiTextTransition; // is the text UI in mid transition
    public bool uiImageTransition; // is the image UI in mid transition
    public float fadeInSpeed; // speed that UI fades in
    public int instructionIndex; // instruction panel index
    public bool endTutorial; // end classification slide show

    // Start is called before the first frame update
    void Start()
    {
        // Get Instruction Text Mesh objects
        for (int i = 0; i < textPanel.transform.childCount - 1; i++)
        {
            instructions.Add(textPanel.transform.GetChild(i).gameObject);
        }

        // Get input prompts text mesh objects
        for(int i = 0; i < textPanel.transform.GetChild(textPanel.transform.childCount - 1).childCount; i++)
        {
            inputPrompts.Add(textPanel.transform.GetChild(textPanel.transform.childCount - 1).GetChild(i).gameObject);
        }

        // Get exmaple screenshots
        for(int i = 0; i < diagramPanel.transform.GetChild(0).childCount; i++)
        {
            exampleScreenshots.Add(diagramPanel.transform.GetChild(0).GetChild(i).gameObject);
        }

        // Get fly body photos
        for (int i = 0; i < diagramPanel.transform.GetChild(1).childCount; i++)
        {
            flyBodyPhotos.Add(diagramPanel.transform.GetChild(1).GetChild(i).gameObject);
        }

        // Get fly close up photos
        for (int i = 0; i < diagramPanel.transform.GetChild(2).childCount; i++)
        {
            flyCloseUpPhotos.Add(diagramPanel.transform.GetChild(2).GetChild(i).gameObject);
        }

        // Get Edit diagram labels
        for (int i = 0; i < labels.transform.GetChild(0).childCount; i++)
        {
            editLabels.Add(labels.transform.GetChild(0).GetChild(i).gameObject);
        }

        // Get photo labels
        for (int i = 0; i < labels.transform.GetChild(1).childCount; i++)
        {
            photoLabels.Add(labels.transform.GetChild(1).GetChild(i).gameObject);
        }

        player = ReInput.players.GetPlayer(playerID); // Set up ReWired player

        activeDiagram = Diagrams.edit; // edit diagram set

        instructionIndex = 0;

        endTutorial = false;

        uiImageTransition = true;
        
        // Set up diagrams and text objects
        StartDiagramState();
        UpdateInputPrompts();

        ChangeUIState(new TutorialTransitionState());

        player.controllers.maps.SetMapsEnabled(true, "Tutorial"); // Set up ReWired controller map
    }

    // Update is called once per frame
    void Update()
    {
        currentUIState.Update();

        if (!uiTextTransition)
        {
            if (player.GetButtonDown("Next"))
            {
                if(instructionIndex + 1 < instructions.Count)
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

            if (endTutorial)
            {
                if (player.GetButtonDown("End"))
                {
                    inputPrompts[2].GetComponent<TextMeshProUGUI>().color = Color.cyan;
                    FinishTutorial();
                    SceneManager.LoadScene("PhotoAnalysis");
                }
            }
        }
    }

    private void FixedUpdate()
    {
        currentUIState.FixedUpdate();
    }

    /***************************************************************************************
*    Title: ExitDiagram State
*    
*    Description: Sets all objects in current diagram to inactive
*
***************************************************************************************/
    public void ExitDiagramState()
    {
        List<GameObject> exitList = GetActiveDiagram();


        foreach (GameObject obj in exitList)
        {
            obj.SetActive(false);
        }
    }

/***************************************************************************************
*    Title: StartDiagramState
*    
*    Description: Sets the necessary diagram objects to active beased on what diagram state is active
*
***************************************************************************************/
    public void StartDiagramState()
    {
        // All diagram states except for edit set all objects in their diagram lists to active
        if(activeDiagram != Diagrams.edit)
        {
            foreach (GameObject obj in GetActiveDiagram())
            {
                obj.SetActive(true);
            }
        }

        // If in edit state, set specific objects in diagram list to active
        else
        {
            if(instructionIndex == 0)
            {
                GetActiveDiagram()[0].SetActive(true);
            }

            if (instructionIndex == 1)
            {
                GetActiveDiagram()[1].SetActive(true);
            }

            if (instructionIndex == 2)
            {
                GetActiveDiagram()[2].SetActive(true);
            }
        }
    }

    /***************************************************************************************
*    Title: ExitLabelState
*    
*    Description: Sets labels that are no longer necessary to inactive. prevIndex is the previous instruction index
*
***************************************************************************************/
    public void ExitLabelState(int prevIndex)
    {
        if(prevIndex == 1)
        {
            // set edit labels to inactive
            foreach (GameObject obj in editLabels)
            {
                obj.SetActive(false);
            }
        }

        else if(prevIndex == 3 && instructionIndex < 3)
        {
            // set photo labels to inactive
            foreach (GameObject obj in photoLabels)
            {
                obj.SetActive(false);
            }
        }
    }

    /***************************************************************************************
*    Title: StartLabelState
*    
*    Description: Sets labels to active based on the inctruction index
*
***************************************************************************************/
    public void StartLabelState()
    {
        if (instructionIndex == 1)
        {
            // set edit labels to active
            foreach(GameObject obj in editLabels)
            {
                obj.SetActive(true);
            }
        }

        else if(instructionIndex >= 3)
        {
            // set photo labels to active
            foreach (GameObject obj in photoLabels)
            {
                obj.SetActive(true);
            }
        }
    }

    /***************************************************************************************
*    Title: GetActiveDiagram
*    
*    Description: Returns the game object list based on whatever diagram state is active
*
***************************************************************************************/
    public List<GameObject> GetActiveDiagram()
    {
        if (activeDiagram == Diagrams.edit)
        {
            return exampleScreenshots;
        }

        else if (activeDiagram == Diagrams.flyBody)
        {
            return flyBodyPhotos;
        }

        else if (activeDiagram == Diagrams.flyCloseUp)
        {
            return flyCloseUpPhotos;
        }

        else
        {
            // If no diagram state is active, return an empty list
            return new List<GameObject>();
        }
    }

    /***************************************************************************************
*    Title: ChangeInstructionPanel
*    
*    Description: Iterates through each instruction panel text. Sets previous instruction panel at prevIndex to inactive.
*                 Makes calls to update input prompts, labels, and diagrams. Sets UI state to the transition state.
*
***************************************************************************************/
    private void ChangeInstructionPanel(int prevIndex)
    {
        instructions[prevIndex].SetActive(false);
        instructions[instructionIndex].SetActive(true); 

        UpdateInputPrompts(); // Update input prompts 

        uiTextTransition = true;
        ExitLabelState(prevIndex); // Update labels
        ChangeDiagram(); // Update diagrams
        ChangeUIState(new TutorialTransitionState()); // Update UI state
    }

    /***************************************************************************************
*    Title: UpdateInputPrompts
*    
*    Description: Changes input prompts at the bottom of instructions panel.
*
***************************************************************************************/
    private void UpdateInputPrompts()
    {
        // If at the starting panel, Back input is inactive
        if(instructionIndex == 0)
        {
            inputPrompts[1].SetActive(false);
        }

        else if(instructionIndex >= 1)
        {
            inputPrompts[1].SetActive(true); // Set Back button to Active

            // If at the last instruction panel, replace Next with prompt to end tutorial
            if (instructionIndex == instructions.Count - 1)
            {
                endTutorial = true;
                inputPrompts[0].SetActive(false);
                inputPrompts[2].SetActive(true);
            }

            // If the player exits this panel state, replace end prompt back with Next
            else if (endTutorial)
            {
                endTutorial = false;
                inputPrompts[0].SetActive(true);
                inputPrompts[2].SetActive(false);
            }
        }
    }

    /***************************************************************************************
*    Title: ChangeDiagram
*    
*    Description: Changes diagrams based on instruction index
*
***************************************************************************************/
    private void ChangeDiagram()
    {
        Diagrams oldDiagram = activeDiagram;
        Diagrams newDiagram = Diagrams.none;

        if(instructionIndex >= 0 && instructionIndex <= 2)
        {
            newDiagram = Diagrams.edit;
        }

        else if(instructionIndex == 3)
        {
            newDiagram = Diagrams.flyBody;
        }

        else if(instructionIndex >= 4)
        {
            newDiagram = Diagrams.flyCloseUp;
        }

        // If the index is changing the diagram, start Ui image transition and exit diagram state to start new on
        if(oldDiagram != newDiagram)
        {
            uiImageTransition = true;
            ExitDiagramState();
            activeDiagram = newDiagram;
            StartDiagramState();
        }

        // If in edit diagram state, change diagrams
        else if(activeDiagram == Diagrams.edit)
        {
            uiImageTransition = true;
            ExitDiagramState();
            StartDiagramState();
        }

        StartLabelState(); // Update labels
    }

    /***************************************************************************************
*    Title: ChangeUIState
*    
*    Description: Updates UI finite state machine 
*
***************************************************************************************/
    public void ChangeUIState(PhotoCheckTutorialUIState state)
    {
        //Debug.Log("Changing State!");
        if (currentUIState != null)
        {
            //Debug.Log("Exiting State!");
            currentUIState.Exit(); // Exit state
        }
        currentUIState = state;
        currentUIState.Enter(this); //Enter new state
    }

    /***************************************************************************************
*    Title: FinishTutorial
*    
*    Description: Initializes serialization to say that player has finished the full tutorial
*
***************************************************************************************/
    public void FinishTutorial()
    {
        GetComponent<TutorialSaveManager>().SaveData(true);
    }
}
