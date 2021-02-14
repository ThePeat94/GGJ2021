using UnityEngine;
using UnityEngine.Events;

namespace Nidavellir.FoxIt.Dialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private string m_triggerName;
        [SerializeField] private UnityEvent m_trigger;

        public string TriggerName => this.m_triggerName;
        public UnityEvent Trigger => this.m_trigger;
    }
}