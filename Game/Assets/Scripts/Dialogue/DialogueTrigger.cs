using UnityEngine;
using UnityEngine.Events;

namespace Nidavellir.FoxIt.Dialogue
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private string m_triggerName;
        [SerializeField] private UnityEvent m_trigger;
        [SerializeField] private bool m_oneTimeTrigger;

        private bool m_isTriggered;

        public string TriggerName => this.m_triggerName;

        public void Trigger()
        {
            if (this.m_isTriggered)
                return;

            this.m_isTriggered = true;
            this.m_trigger?.Invoke();
        }
    }
}