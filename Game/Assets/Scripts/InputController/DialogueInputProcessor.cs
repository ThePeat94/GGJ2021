using UnityEngine;

namespace Nidavellir.FoxIt.InputController
{
    public class DialogueInputProcessor : MonoBehaviour
    {
        private PlayerInput m_playerInput;

        public bool ContinueDialogueTriggered => this.m_playerInput.Dialogue.ContinueConversation.triggered;

        private void Awake()
        {
            this.m_playerInput = new PlayerInput();
        }

        private void OnEnable()
        {
            this.m_playerInput?.Enable();
        }

        private void OnDisable()
        {
            this.m_playerInput?.Disable();
        }
    }
}