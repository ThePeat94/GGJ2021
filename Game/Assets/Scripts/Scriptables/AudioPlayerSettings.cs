using UnityEngine;

namespace Nidavellir.FoxIt.Scriptables
{
    [CreateAssetMenu(fileName = "Audio Player Settings", menuName = "Data/Audio/Audio Player Settings", order = 0)]
    public class AudioPlayerSettings : ScriptableObject
    {
        [SerializeField] [Range(0f, 1f)] private float m_volume;

        public float Volume => this.m_volume;
    }
}