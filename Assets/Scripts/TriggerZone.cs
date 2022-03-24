using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this script to a GameObject with one or more colliders
public class TriggerZone : MonoBehaviour
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
        if(Flags.ContainsKey(triggerName))
        {
            Flags[triggerName].Value = value;
        }
    }

    public List<Entity> targetEntites;
    public string triggerName = "NoName";

    public void OnTriggerEnter(Collider other)
    {
        Entity hitEntity = other.gameObject.GetComponentInParent<Entity>();
        if(hitEntity != null && targetEntites.Contains(hitEntity))
        {
            SetFlag(triggerName,true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Entity hitEntity = other.gameObject.GetComponentInParent<Entity>();
        if (hitEntity != null && targetEntites.Contains(hitEntity))
        {
            SetFlag(triggerName, false);
        }
    }
}
