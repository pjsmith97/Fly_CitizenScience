using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class TutorialPanelManager : MonoBehaviour
{
    public enum Diagrams
    {
        edit,
        flyBody,
        flyCloseUp
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

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitDiagramState()
    {
        List<GameObject> exitList = new List<GameObject>();

        if(activeDiagram == Diagrams.edit)
        {
            exitList = exampleScreenshots;
        }

        else if (activeDiagram == Diagrams.flyBody)
        {
            exitList = flyBodyPhotos;
        }

        else if(activeDiagram == Diagrams.flyCloseUp)
        {
            exitList = flyCloseUpPhotos;
        }

        foreach (GameObject obj in exitList)
        {
            obj.SetActive(false);
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
}
