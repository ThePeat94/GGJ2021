using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Scriptables;
using UnityEngine;

public class CameraSoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioPlayerSettings m_settings;

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
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, this.m_settings.Volume);
    }
    
    public void PlayClipAtCam(AudioClip clip, float volume)
    {
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, volume);
    }
}
