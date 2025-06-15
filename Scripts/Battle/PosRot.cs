using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PosRot")]
public class PosRot : ScriptableObject
{
    [SerializeField] Vector3 _pos;
    [SerializeField] Quaternion _rot;
    public Vector3 Pos => _pos;
    public Quaternion Rot => _rot;

}
