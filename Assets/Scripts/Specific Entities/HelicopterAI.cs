using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterAI : Helicopter
{
    
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        FlyUp(0.5f);
        FlyForward(1f);
        // TODO: State Machines, timers, triggers, AI states, etc., etc., etc.
    }
}
