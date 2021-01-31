using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    [SerializeField] private List<AudioClip> m_startingClipQueue;
    
    // Start is called before the first frame update
    void Start()
    {
        MusicPlayer.Instance.QueueClips(this.m_startingClipQueue);
    }
}
