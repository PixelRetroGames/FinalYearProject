using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRunestoneCommandPlay : MenuRunestoneCommand
{
    public GameStateManager manager;

    public override void Execute()
    {
        manager.StartGame();
    }
}
