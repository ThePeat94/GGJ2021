using Nidavellir.FoxIt.Scriptables;
using UnityEngine;

namespace Nidavellir.FoxIt
{
    public class CameraSoundPlayer : MonoBehaviour
    {
        private static CameraSoundPlayer s_instance;
        [SerializeField] private AudioPlayerSettings m_settings;
        [SerializeField] private AudioSource m_audioSource;

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
}