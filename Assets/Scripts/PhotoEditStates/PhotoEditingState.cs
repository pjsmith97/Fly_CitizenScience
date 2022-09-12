using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************************************
*    
*    
***************************************************************************************/
public class PhotoEditingState : PhotoAnalysisState
{
    public override void Enter(PhotoAnalysisController analysis)
    {
        base.Enter(analysis);

        photoAnalysis.editingStateHelper.player.controllers.maps.SetMapsEnabled(true, "PhotoAnalysis");
        photoAnalysis.editingStateHelper.player.controllers.maps.SetMapsEnabled(false, "Default");

        photoAnalysis.editingStateHelper.editingUI.SetActive(true);

        photoAnalysis.SetPanelTitle("Edit Mode");
    }

    public override void Update()
    {
        photoAnalysis.editingStateHelper.HelperUpdate();
        if (photoAnalysis.editingStateHelper.done)
        {
            photoAnalysis.ChangeState(new PhotoDecisionState());
            photoAnalysis.editingStateHelper.done = false;
        }
    }

    public override void FixedUpdate()
    {
        photoAnalysis.editingStateHelper.HelperFixedUpdate();
    }

    public override void Exit()
    {
        base.Exit();

        photoAnalysis.editingStateHelper.player.controllers.maps.SetMapsEnabled(false, "PhotoAnalysis");
        photoAnalysis.editingStateHelper.editingUI.SetActive(false);
    }
}
