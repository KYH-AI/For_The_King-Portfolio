using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_Stop : MonoBehaviour
{
    void Stop()
    {
        GetComponent<ParticleSystem>().Stop();
    }
}
