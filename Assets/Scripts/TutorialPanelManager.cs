using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;
using TMPro;

public class TutorialPanelManager : MonoBehaviour
{
    public enum Diagrams
    {
        edit,
        flyBody,
        flyCloseUp,
        none
    }

    public Diagrams activeDiagram;

    public Player player;
    [SerializeField] private int playerID = 0;

    [Header("Panels")]
    [SerializeField] GameObject textPanel;
    [SerializeField] GameObject diagramPanel;
    [SerializeField] GameObject labels;

    [Header("Specific Objects")]
    public List<GameObject> instructions;
    public List<GameObject> inputPrompts;
    public List<GameObject> exampleScreenshots;
    public List<GameObject> flyBodyPhotos;
    public List<GameObject> flyCloseUpPhotos;
    public List<GameObject> editLabels;
    public List<GameObject> photoLabels;

    private PhotoCheckTutorialUIState currentUIState;

    [Header("UI Transition State")]
    public bool uiTextTransition;
    public bool uiImageTransition;
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

        player = ReInput.players.GetPlayer(playerID);

        activeDiagram = Diagrams.edit;

        instructionIndex = 0;

        endTutorial = false;

        uiImageTransition = true;
        
        StartDiagramState();
        UpdateInputPrompts();

        ChangeUIState(new TutorialTransitionState());

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

    public void ExitDiagramState()
    {
        List<GameObject> exitList = GetActiveDiagram();


        foreach (GameObject obj in exitList)
        {
            obj.SetActive(false);
        }
    }

    public void StartDiagramState()
    {
        if(activeDiagram != Diagrams.edit)
        {
            foreach (GameObject obj in GetActiveDiagram())
            {
                obj.SetActive(true);
            }
        }
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

    public void ExitLabelState(int prevIndex)
    {
        if(prevIndex == 1)
        {
            foreach (GameObject obj in editLabels)
            {
                obj.SetActive(false);
            }
        }

        else if(prevIndex == 3 && instructionIndex < 3)
        {
            foreach (GameObject obj in photoLabels)
            {
                obj.SetActive(false);
            }
        }
    }

    public void StartLabelState()
    {
        if (instructionIndex == 1)
        {
            foreach(GameObject obj in editLabels)
            {
                obj.SetActive(true);
            }
        }

        else if(instructionIndex >= 3)
        {
            foreach (GameObject obj in photoLabels)
            {
                obj.SetActive(true);
            }
        }
    }

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
            return new List<GameObject>();
        }
    }

    private void ChangeInstructionPanel(int prevIndex)
    {
        instructions[prevIndex].SetActive(false);
        instructions[instructionIndex].SetActive(true);

        UpdateInputPrompts();

        uiTextTransition = true;
        ExitLabelState(prevIndex);
        ChangeDiagram();
        ChangeUIState(new TutorialTransitionState());
    }

    private void UpdateInputPrompts()
    {
        if(instructionIndex == 0)
        {
            inputPrompts[1].SetActive(false);
        }

        else if(instructionIndex >= 1)
        {
            inputPrompts[1].SetActive(true);
            if (instructionIndex == instructions.Count - 1)
            {
                endTutorial = true;
                inputPrompts[0].SetActive(false);
                inputPrompts[2].SetActive(true);
            }
            else if (endTutorial)
            {
                endTutorial = false;
                inputPrompts[0].SetActive(true);
                inputPrompts[2].SetActive(false);
            }
        }
    }

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

        if(oldDiagram != newDiagram)
        {
            uiImageTransition = true;
            ExitDiagramState();
            activeDiagram = newDiagram;
            StartDiagramState();
        }

        else if(activeDiagram == Diagrams.edit)
        {
            uiImageTransition = true;
            ExitDiagramState();
            StartDiagramState();
        }

        StartLabelState();
    }

    public void ChangeUIState(PhotoCheckTutorialUIState state)
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

    public void FinishTutorial()
    {
        GetComponent<TutorialSaveManager>().SaveData(true);
    }
}
