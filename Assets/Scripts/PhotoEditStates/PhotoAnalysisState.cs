using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PhotoAnalysisState
{
    protected PhotoAnalysisController photoAnalysis;

    public virtual void Enter(PhotoAnalysisController analysis)
    {
        photoAnalysis = analysis;
    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {



    }

    public virtual void Exit()
    {

    }


}