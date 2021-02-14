using System.Linq;
using Nidavellir.FoxIt.Dialogue;
using Nidavellir.FoxIt.Interfaces;
using Nidavellir.FoxIt.UI;
using Nidavellir.FoxIt.Upgrades;
using UnityEngine;

namespace Nidavellir.FoxIt.Minifox
{
    public abstract class Minifox : MonoBehaviour, ITalkable
    {
        [SerializeField] protected PlayerHud m_playerHud;
        [SerializeField] protected string m_upgradeText;
        [SerializeField] protected string m_name;
        [SerializeField] protected Sprite m_icon;
        [SerializeField] protected DialogueData m_foundOneDialogueData;
        [SerializeField] protected DialogueData m_foundAllDialogueData;
        [SerializeField] private GameObject m_ui;
        [SerializeField] private Transform m_talkableViewPoint;

        public Upgrade Upgrade { get; protected set; }
        public string Name => this.m_name;
        public Sprite Icon => this.m_icon;
        public Transform Viewpoint => this.m_talkableViewPoint;

        public DialogueData GetDiaglogueData()
        {
            var activeCount = FindObjectsOfType<Minifox>().Length - 1;
            return activeCount > 0 ? this.m_foundOneDialogueData : this.m_foundAllDialogueData;
        }

        public void TriggerAction(string actionName)
        {
            if (string.IsNullOrEmpty(actionName))
                return;

            var availableTriggers = this.GetComponents<DialogueTrigger>().Where(trigger => trigger.TriggerName.Equals(actionName));
            foreach (var availableTrigger in availableTriggers)
                availableTrigger.Trigger();
        }

        public void ShowUI()
        {
            this.m_ui.SetActive(true);
        }

        public void HideUI()
        {
            this.m_ui.SetActive(false);
        }

        public void ApplyUpgradeTrigger()
        {
            this.Upgrade.ApplyUpgrade();
        }
    }
}