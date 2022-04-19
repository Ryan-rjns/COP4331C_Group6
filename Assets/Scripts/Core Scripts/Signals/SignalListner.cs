using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalListner : Signal
{
    // Register all signals that are being watched
    public List<Signal> signals;
    // Display name for this listner
    public string displayName = "SignalListner";
    // If this is >= 0, it determines what objective slot to display in
    public int objectiveSlot = -1;

    // Returns the number of valid signals that match the given state
    // If the state is null, returns the total number of valid signals
    public int NumSignals(bool? state = null)
    {
        int count = 0;
        foreach (Signal signal in signals)
        {
            if (signal == null) continue;
            if(state == null || state == signal.IsSignaled()) count++;
        }
        return count;
    }

    public override bool IsSignaled()
    {
        return NumSignals() == NumSignals(true);
    }

    public string DisplayString()
    {
        int totalSignals = NumSignals();
        int activeSignals = NumSignals(true);
        return $"{displayName}: {activeSignals}/{totalSignals}{(activeSignals==totalSignals?" (DONE)":"")}";
    }

    protected override void Update()
    {
        base.Update();
        if (objectiveSlot >= 0 && objectiveSlot < HUD.objectives.Count)
        {
            HUD.objectives[objectiveSlot].text = DisplayString();
        }
    }
}
