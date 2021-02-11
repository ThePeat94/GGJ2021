using Nidavellir.FoxIt.Dialogue;
using UnityEngine;

namespace Nidavellir.FoxIt.Interfaces
{
    public interface ITalkable
    {
        public string Name { get; }
        public Sprite Icon { get; }
        
        public DialogueData GetDiaglogueData();
        public void TriggerAction(string actionName);
        public void ShowUI();
        public void HideUI();
    }
}