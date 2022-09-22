using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialTransitionState : PhotoCheckTutorialUIState
{
    public override void Enter(TutorialPanelManager tutorial)
    {
        base.Enter(tutorial);

        tutorialManager.uiTextTransition = true;

        tutorialManager.instructions[tutorialManager.instructionIndex].SetActive(true);

        var tempTextColor = tutorialManager.instructions[tutorialManager.instructionIndex].GetComponent<TextMeshProUGUI>().color;
        tempTextColor.a = 0;
        tutorialManager.instructions[tutorialManager.instructionIndex].GetComponent<TextMeshProUGUI>().color = tempTextColor;

        if (tutorialManager.uiImageTransition)
        {
            foreach (GameObject obj in tutorialManager.GetActiveDiagram())
            {
                var tempImageColor = obj.GetComponent<Image>().color;
                tempImageColor.a = 0;
                obj.GetComponent<Image>().color = tempImageColor;
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        float Del = Time.deltaTime;

        if (tutorialManager.uiTextTransition)
        {
            var tempTextColor = tutorialManager.instructions[tutorialManager.instructionIndex].GetComponent<TextMeshProUGUI>().color;
            if (tempTextColor.a < 1)
            {
                tempTextColor.a += tutorialManager.fadeInSpeed * Del;
                tutorialManager.instructions[tutorialManager.instructionIndex].GetComponent<TextMeshProUGUI>().color = tempTextColor;
            }
            else
            {
                tutorialManager.uiTextTransition = false;
            }
        }

        if (tutorialManager.uiImageTransition)
        {
            int doneImages = 0;
            foreach (GameObject obj in tutorialManager.GetActiveDiagram())
            {
                var tempImageColor = obj.GetComponent<Image>().color;
                if(tempImageColor.a < 1)
                {
                    tempImageColor.a += tutorialManager.fadeInSpeed * Del;
                    obj.GetComponent<Image>().color = tempImageColor;
                }
                if (tempImageColor.a >= 1)
                {
                    doneImages += 1;
                }
            }

            if(doneImages >= tutorialManager.GetActiveDiagram().Count)
            {
                tutorialManager.uiImageTransition = false;
            }
        }

        if(!tutorialManager.uiImageTransition && !tutorialManager.uiTextTransition)
        {
            tutorialManager.ChangeUIState(new TutorialReadingState());
        }
    }
}
