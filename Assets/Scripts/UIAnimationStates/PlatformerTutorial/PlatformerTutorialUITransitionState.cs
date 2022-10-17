using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlatformerTutorialUITransitionState : PlatformerTutorialUIState
{
    public override void Enter(PlatformerTutorialManager tutorial)
    {
        base.Enter(tutorial);

        tutorialManager.uiTextTransition = true;

        tutorialManager.instructions[tutorialManager.instructionIndex].SetActive(true);

        var tempTextColor = tutorialManager.instructions[tutorialManager.instructionIndex].GetComponent<TextMeshProUGUI>().color;
        tempTextColor.a = 0;
        tutorialManager.instructions[tutorialManager.instructionIndex].GetComponent<TextMeshProUGUI>().color = tempTextColor;

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

        if (!tutorialManager.uiTextTransition)
        {
            tutorialManager.ChangeUIState(new PlatformerTutorialReadingState());
        }
    }
}
