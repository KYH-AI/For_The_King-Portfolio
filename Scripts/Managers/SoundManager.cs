using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    [SerializeField]
    private  AudioSource _BGMSource; // ����� �ҽ� ������Ʈ
    [SerializeField]
    private AudioSource _SFXSource; // ����� �ҽ� ������Ʈ

    Dictionary<string, AudioClip> _SFXClips = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> _BGMClips = new Dictionary<string, AudioClip>();
    public void Init()
    {
        //_audioSource = GetComponent<AudioSource>();
    }

   


    // ����� Ŭ���� �̸����� �˻��Ͽ� �÷��� �Լ�
    public void PlaySFX(string clipName, AudioSource audioSource = null, float volume = 1f)
    {
        if (audioSource == null)
        {
            AudioClip clip = FindSFXByName(clipName);
            _SFXSource.PlayOneShot(clip, volume);
        }
        else
        {
            AudioClip clip = FindSFXByName(clipName);
            audioSource.PlayOneShot(clip, volume);
        }
        
    }
    public void PlayBGM(string clipName, float volume = 1f)
    {
        if (_BGMSource.isPlaying)
            _BGMSource.Stop();
        AudioClip clip = FindBGMByName("BGM/"+clipName);
        _BGMSource.clip = clip;
        _BGMSource.pitch = volume;
        _BGMSource.Play();
    }

    public void StopBGM()
    {
        if (_BGMSource.isPlaying)
            _BGMSource.Stop();
    }

    // �̸����� ����� Ŭ�� �˻� �Լ�
    private AudioClip FindSFXByName(string clipName)
    {
        if (_SFXClips.ContainsKey(clipName))
        {
            return _SFXClips[clipName];
        }
        else
        {
            AudioClip clip = Managers.Resource.LoadResource<AudioClip>(clipName);
            _SFXClips.Add(clipName, clip);
            return clip;
        }
    }
    private AudioClip FindBGMByName(string clipName)
    {
        if (_BGMClips.ContainsKey(clipName))
        {
            return _BGMClips[clipName];
        }
        else
        {
            AudioClip clip = Managers.Resource.LoadResource<AudioClip>(clipName);
            _BGMClips.Add(clipName, clip);
            return clip;
        }
    }

    public void ClearSFX()
    {
        _SFXClips.Clear();
    }
}

