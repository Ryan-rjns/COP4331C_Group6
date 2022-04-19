using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalZone : Signal
{
    public List<Entity> targetEntites;

    private bool signal = false;
    public override bool IsSignaled() => signal;

    public void OnTriggerEnter(Collider other)
    {
        CheckTrigger(other.gameObject, true);
    }
    public void OnTriggerExit(Collider other)
    {
        CheckTrigger(other.gameObject,false);
    }
    private void CheckTrigger(GameObject obj, bool enter)
    {
        Entity hitEntity = obj.GetComponentInParent<Entity>();
        if (hitEntity != null && targetEntites.Contains(hitEntity))
        {
            signal = enter;
        }
    }
}
