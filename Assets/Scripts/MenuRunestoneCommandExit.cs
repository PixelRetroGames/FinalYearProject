using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRunestoneCommandExit : MenuRunestoneCommand 
{
    public override void Execute()
    {
        QuitGame();
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
