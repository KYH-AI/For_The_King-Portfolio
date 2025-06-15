using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UI_FadeOut : MonoBehaviour
{
    public float animTime = 2f;         // Fade 애니메이션 재생 시간 (단위:초).  
    private Image fadeImage;            // UGUI의 Image컴포넌트 참조 변수.  
    private float start = 1f;           // Mathf.Lerp 메소드의 첫번째 값.  
    private float end = 0f;             // Mathf.Lerp 메소드의 두번째 값.  
    private float time = 3f;            // Mathf.Lerp 메소드의 시간 값.  


    public bool stopIn = false; //false일때 실행되는건데, 초기값을 false로 한 이유는 게임 시작할때 페이드인으로 들어가려고...그게 싫으면 true로 하면됨.

    void Awake()
    {
        // Image 컴포넌트를 검색해서 참조 변수 값 설정.  
        fadeImage = GetComponent<Image>();
    }


    void Update()
    {

        // 투명해지는 = FadeIn 애니메이션 재생.  
        if (stopIn == false && time <= animTime)
        {
            PlayFadeIn();
        }
        if (stopIn == true && time <= animTime)
        {
            PlayFadeOut();
        }


    }

    // 흰색->투명
    void PlayFadeIn()
    {
        // 경과 시간 계산.  
        // 2초(animTime)동안 재생될 수 있도록 animTime으로 나누기.  
        time += Time.deltaTime / animTime;

        // Image 컴포넌트의 색상 값 읽어오기.  
        Color color = fadeImage.color;
        // 알파 값 계산.  
        color.a = Mathf.Lerp(start, end, time);
        // 계산한 알파 값 다시 설정.  
        fadeImage.color = color;
        // Debug.Log(time);
    }

    // 투명->흰색
    void PlayFadeOut()
    {
        // 경과 시간 계산.  
        // 2초(animTime)동안 재생될 수 있도록 animTime으로 나누기.  
        time += Time.deltaTime / animTime;

        // Image 컴포넌트의 색상 값 읽어오기.  
        Color color = fadeImage.color;
        // 알파 값 계산.  
        color.a = Mathf.Lerp(end, start, time);  //FadeIn과는 달리 start, end가 반대다.
        // 계산한 알파 값 다시 설정.  
        fadeImage.color = color;
    }

    public void FadeIn(float animTime,Color color)
    {
        time = 0;
        stopIn = false;
        this.animTime = animTime;
        fadeImage.color = color;
    }
    public void FadeOut(float animTime, Color color)
    {
        time = 0;
        stopIn = true;
        this.animTime = animTime;
        fadeImage.color = color;
    }

}