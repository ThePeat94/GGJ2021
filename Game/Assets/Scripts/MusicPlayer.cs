using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource m_audioSource;
    [SerializeField] private AudioClip m_defaultTheme;
    
    private static MusicPlayer s_instance;

    private bool m_shallLoop;

    public static MusicPlayer Instance
    {
        get
        {
            if (s_instance == null)
                s_instance = FindObjectOfType<MusicPlayer>();

            return s_instance;
        }
    }

    private Queue<AudioClip> m_clipQueue;

    private void Awake()
    {
        this.m_clipQueue = new Queue<AudioClip>();
        this.m_audioSource = this.GetComponent<AudioSource>();
        this.m_shallLoop = false;
    }

    private void Update()
    {
        if (this.m_audioSource.isPlaying)
            return;

        var clipToPlay = this.m_audioSource.clip;
        if (this.m_clipQueue.Count > 0)
            clipToPlay = this.m_clipQueue.Dequeue();

        if (clipToPlay != this.m_audioSource.clip || this.m_shallLoop)
        {
            this.m_audioSource.clip = clipToPlay;
            this.m_audioSource.Play();
        }
    }

    public void  ClearQueue()
    {
        this.m_clipQueue.Clear();
    }

    public void QueueClip(AudioClip clipToQueue)
    {
        this.m_clipQueue.Enqueue(clipToQueue);
    }

    public void QueueClips(IEnumerable<AudioClip> clipsToQueue)
    {
        foreach (var clipToQueue in clipsToQueue)
            this.m_clipQueue.Enqueue(clipToQueue);
    }

    public void PlayInstant(AudioClip clip, bool shallLoop = true)
    {
        this.m_clipQueue.Clear();
        this.m_audioSource.clip = clip;
        this.m_audioSource.Play();
        this.m_shallLoop = shallLoop;
    }

    public void PlayDefault()
    {
        this.m_clipQueue.Clear();
        this.m_audioSource.clip = this.m_defaultTheme;
        this.m_shallLoop = true;
        this.m_audioSource.Play();
    }
}
