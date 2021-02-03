using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Scriptables;
using UnityEngine;

public class CameraSoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioPlayerSettings m_settings;
    [SerializeField] private AudioSource m_audioSource;

    private static CameraSoundPlayer s_instance;

    public static CameraSoundPlayer Instance
    {
        get
        {
            if (s_instance == null)
                s_instance = FindObjectOfType<CameraSoundPlayer>();

            return s_instance;
        }
    }

    public void PlayClipAtCam(AudioClip clip)
    {
        this.m_audioSource.clip = clip;
        this.m_audioSource.volume = this.m_settings.Volume;
        this.m_audioSource.Play();
    }
    
    public void PlayClipAtCam(AudioClip clip, float volume)
    {
        this.m_audioSource.clip = clip;
        this.m_audioSource.volume = volume;
        this.m_audioSource.Play();
    }
}