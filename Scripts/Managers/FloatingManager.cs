using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingManager :MonoBehaviour
{
    GameObject _floatingText;
    private Camera _main;
    
    public void Init()
    {
        _floatingText = Managers.Resource.LoadResource<GameObject>("FloatingText");
        _main = Camera.main;
    }
    public void FloatingText(string Damage, Vector3 target, bool isCr = false)
    {
        GameObject hudText = Instantiate(_floatingText, target , _main.transform.rotation); // 생성할 텍스트 오브젝트

        hudText.GetComponent<DamageText>().damage = Damage;
        if (isCr)
        {
            hudText.GetComponent<TextMeshPro>().color = Color.red;
        }
        else
        {
            hudText.GetComponent<TextMeshPro>().color = Color.white;
        }
    }
}
