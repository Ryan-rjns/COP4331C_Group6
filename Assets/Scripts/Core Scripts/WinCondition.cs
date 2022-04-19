using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A signal lister that triggers GameManager.Win() when it becomes true
// (Just attach this script to an empty GameObject)
public class WinCondition : SignalListner
{
    // failsafe to make sure Win() isn't called more than once
    private bool alreadyWon = false;
    
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!alreadyWon && IsSignaled())
        {
            alreadyWon = true;
            GameManager.Win();
        }
    }
}
