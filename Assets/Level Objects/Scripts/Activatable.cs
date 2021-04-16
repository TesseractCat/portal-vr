using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Activatable : MonoBehaviour
{
    public virtual void Activate() { }
    public virtual void Deactivate() { }
}
