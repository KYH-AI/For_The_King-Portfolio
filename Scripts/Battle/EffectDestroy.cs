using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroy : MonoBehaviour
{
   public void Destroyed()
    {
        Destroy(transform.parent.gameObject);
    }
}
