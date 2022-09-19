using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/***************************************************************************************
*    
*    
***************************************************************************************/
public class PhotoDecisionState : PhotoAnalysisState
{
    public override void Enter(PhotoAnalysisController analysis)
    {
        base.Enter(analysis);

        // Change Rewired button map
        photoAnalysis.decisionStateHelper.player.controllers.maps.SetMapsEnabled(true, "PhotoDecision");
        
        // Set Decision UI to active
        photoAnalysis.decisionStateHelper.decisionUI.SetActive(true);
        photoAnalysis.decisionStateHelper.symbolIndexOptions[photoAnalysis.decisionStateHelper.symbolIndex].SetActive(true);

        // Set first symbol alpha to 0
        var tempColor = photoAnalysis.decisionStateHelper.symbolIndexOptions[photoAnalysis.decisionStateHelper.symbolIndex].GetComponent<RawImage>().color;
        tempColor.a = 0;
        photoAnalysis.decisionStateHelper.symbolIndexOptions[photoAnalysis.decisionStateHelper.symbolIndex].GetComponent<RawImage>().color = tempColor;

        // Set Symbol fade in timer to 0
        photoAnalysis.decisionStateHelper.SetSymbolFadeTimer(0);

        // Set Decision option text to Active
        photoAnalysis.decisionStateHelper.textIndexOptions[photoAnalysis.decisionStateHelper.buttonIndex].SetActive(true);

        // Set selected Button Option 
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(
            photoAnalysis.decisionStateHelper.buttonOptions[photoAnalysis.decisionStateHelper.buttonIndexOptions[photoAnalysis.decisionStateHelper.buttonIndex]]);

        // Set Panel Title
        photoAnalysis.SetPanelTitle("Decision Mode");
    }

    public override void Update()
    {
        photoAnalysis.decisionStateHelper.HelperUpdate();
        if (photoAnalysis.decisionStateHelper.done)
        {
            photoAnalysis.decisionStateHelper.done = false;
            photoAnalysis.ChangeState(new PhotoConfirmationState());
        }

        if (photoAnalysis.decisionStateHelper.back)
        {
            photoAnalysis.decisionStateHelper.back = false;
            photoAnalysis.ChangeState(new PhotoEditingState());
        }
        
    }

    public override void FixedUpdate()
    {
        photoAnalysis.decisionStateHelper.HelperFixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();

        EventSystem.current.SetSelectedGameObject(null);
        /*EventSystem.current.SetSelectedGameObject(
            photoAnalysis.decisionStateHelper.buttonOptions[photoAnalysis.decisionStateHelper.buttonIndexOptions[photoAnalysis.decisionStateHelper.buttonIndex]]);*/

        photoAnalysis.decisionStateHelper.player.controllers.maps.SetMapsEnabled(false, "PhotoDecision");
        photoAnalysis.decisionStateHelper.decisionUI.SetActive(false);
        /*photoAnalysis.decisionStateHelper.symbolIndexOptions[photoAnalysis.decisionStateHelper.symbolIndex].SetActive(false);
        photoAnalysis.decisionStateHelper.textIndexOptions[photoAnalysis.decisionStateHelper.buttonIndex].SetActive(false);*/
        
    }
}
