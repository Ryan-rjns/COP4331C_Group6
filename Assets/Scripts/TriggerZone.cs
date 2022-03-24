using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this script to a GameObject with one or more colliders
public class TriggerZone : MonoBehaviour
{
    public List<Entity> targetEntites;
    public Flag MainFlag { get; private set; } = new Flag();

    public void OnTriggerEnter(Collider other)
    {
        Entity hitEntity;
        other.gameObject.TryGetComponent(out hitEntity);
        if(hitEntity != null && targetEntites.Contains(hitEntity))
        {
            MainFlag.Value = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Entity hitEntity;
        other.gameObject.TryGetComponent(out hitEntity);
        if (hitEntity != null && targetEntites.Contains(hitEntity))
        {
            MainFlag.Value = false;
        }
    }
}
