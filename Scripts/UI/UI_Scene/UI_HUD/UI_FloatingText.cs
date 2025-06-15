using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_FloatingText : MonoBehaviour
{
    private float moveSpeed;
    private float alphaSpeed;
    private float destroyTime;
    TextMeshProUGUI text;
    Color alpha;
    public string word;

    // Update is called once per frame
    void Update()
    {
        text.rectTransform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
       // transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0)); // 텍스트 위치

        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed); // 텍스트 알파값
        text.color = alpha;
    }

    public void Init(string word)
    {
        this.gameObject.SetActive(true);
        moveSpeed = 10.0f;
        alphaSpeed = 3.0f;
        destroyTime = 2.0f;
        if(text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
        }   
        alpha = text.color;
        alpha.a = 1;
        text.color = alpha;
        text.text = word;
        Invoke("DestroyObject", destroyTime);
    }
    private void DestroyObject()
    {
        this.gameObject.SetActive(false);
    }
}