using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuState
{
    protected MenuController menu;

    public virtual void Enter(MenuController newMenu)
    {
        menu = newMenu;
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
