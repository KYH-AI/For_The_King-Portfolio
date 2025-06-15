using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UI_FadeOut : MonoBehaviour
{
    public float animTime = 2f;         // Fade �ִϸ��̼� ��� �ð� (����:��).  
    private Image fadeImage;            // UGUI�� Image������Ʈ ���� ����.  
    private float start = 1f;           // Mathf.Lerp �޼ҵ��� ù��° ��.  
    private float end = 0f;             // Mathf.Lerp �޼ҵ��� �ι�° ��.  
    private float time = 3f;            // Mathf.Lerp �޼ҵ��� �ð� ��.  


    public bool stopIn = false; //false�϶� ����Ǵ°ǵ�, �ʱⰪ�� false�� �� ������ ���� �����Ҷ� ���̵������� ������...�װ� ������ true�� �ϸ��.

    void Awake()
    {
        // Image ������Ʈ�� �˻��ؼ� ���� ���� �� ����.  
        fadeImage = GetComponent<Image>();
    }


    void Update()
    {

        // ���������� = FadeIn �ִϸ��̼� ���.  
        if (stopIn == false && time <= animTime)
        {
            PlayFadeIn();
        }
        if (stopIn == true && time <= animTime)
        {
            PlayFadeOut();
        }


    }

    // ���->����
    void PlayFadeIn()
    {
        // ��� �ð� ���.  
        // 2��(animTime)���� ����� �� �ֵ��� animTime���� ������.  
        time += Time.deltaTime / animTime;

        // Image ������Ʈ�� ���� �� �о����.  
        Color color = fadeImage.color;
        // ���� �� ���.  
        color.a = Mathf.Lerp(start, end, time);
        // ����� ���� �� �ٽ� ����.  
        fadeImage.color = color;
        // Debug.Log(time);
    }

    // ����->���
    void PlayFadeOut()
    {
        // ��� �ð� ���.  
        // 2��(animTime)���� ����� �� �ֵ��� animTime���� ������.  
        time += Time.deltaTime / animTime;

        // Image ������Ʈ�� ���� �� �о����.  
        Color color = fadeImage.color;
        // ���� �� ���.  
        color.a = Mathf.Lerp(end, start, time);  //FadeIn���� �޸� start, end�� �ݴ��.
        // ����� ���� �� �ٽ� ����.  
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