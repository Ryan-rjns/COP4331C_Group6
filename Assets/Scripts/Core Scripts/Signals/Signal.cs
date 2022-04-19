using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : MonoBehaviour
{
    // If true, the gameObject this script is attached to will disable when IsSignaled() becomes true
    public bool disableOnFinish = false;

    // Child classes should override this
    public virtual bool IsSignaled() => false;

    protected virtual void Update()
    {
        if (disableOnFinish && IsSignaled())
        {
            gameObject.SetActive(false);
        }
    }
}
