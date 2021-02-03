using System.Collections.Generic;
using UnityEngine;

namespace Nidavellir.FoxIt
{
    public class GameStart : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> m_startingClipQueue;

        // Start is called before the first frame update
        private void Start()
        {
            MusicPlayer.Instance.QueueClips(this.m_startingClipQueue);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
#if UNITY_EDITOR
            Cursor.lockState = CursorLockMode.None;
#endif
        }
    }
}