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

        protected string m_baseText = "Yahaha! You found me!";
        protected string m_foundAll = "You have found us all! You are now perfectly prepared for the last fight.";
        protected string m_remainingFoxesText = "There are {0} foxes left. Find us all and you will defeat the evil more easily.";

        public Upgrade Upgrade { get; protected set; }
        public string Name => this.m_name;
        public Sprite Icon => this.m_icon;
        
        protected string GetBaseDialogueText()
        {
            var activeCount = FindObjectsOfType<Minifox>().Length - 1;
            if (activeCount > 0) return string.Format(this.m_baseText + " " + this.m_remainingFoxesText, activeCount);

            return this.m_baseText + " " + this.m_foundAll;
        }
        public DialogueData GetDiaglogueData()
        {
            var activeCount = FindObjectsOfType<Minifox>().Length - 1;
            return activeCount > 0 ? this.m_foundOneDialogueData : this.m_foundAllDialogueData;
        }

        public void TriggerAction(string actionName)
        {
            Debug.Log("Trigger lol " + actionName);
        }

        public void ShowUI()
        {
            this.m_ui.SetActive(true);
        }

        public void HideUI()
        {
            this.m_ui.SetActive(false);
        }
    }
}