using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MainMenuTutorialUIState
{
    protected MenuTutorialStateHelper menuHelper;

    public virtual void Enter(MenuTutorialStateHelper helper)
    {
        menuHelper = helper;
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
