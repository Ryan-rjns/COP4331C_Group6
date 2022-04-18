using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCounter : MonoBehaviour
{
    public static Dictionary<string, Flag> Flags { get; private set; } = new Dictionary<string, Flag>();
    public static Flag GetFlag(string targetName)
    {
        if (Flags.ContainsKey(targetName)) return Flags[targetName];
        Flag flag = new Flag();
        Flags[targetName] = flag;
        return flag;
    }
    private static void SetFlag(string triggerName, bool value)
    {
        if (Flags.ContainsKey(triggerName))
        {
            Flags[triggerName].Value = value;
        }
    }

    public List<GameObject> targetUnits;
    public string triggerName = "NoName";

    void Update()
    {
        // Count how many targets are still alive
        int count = 0;
        foreach(GameObject unit in targetUnits)
        {
            if (unit != null) count++;
        }
        // If no targets are alive, trigger the flag
        if (count == 0 && !GetFlag(triggerName).Value)
        {
            Debug.Log($"KillCounter {triggerName}: All targets were killed");
            SetFlag(triggerName, true);
        }
    }
}
