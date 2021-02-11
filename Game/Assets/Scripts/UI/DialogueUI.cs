using System;
using System.Collections;
using System.Collections.Generic;
using Nidavellir.FoxIt.Dialogue;
using Nidavellir.FoxIt.EventArgs;
using Nidavellir.FoxIt.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nidavellir.FoxIt.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_dialogueText;
        [SerializeField] private TextMeshProUGUI m_talkerName;
        [SerializeField] private Image m_icon;
        [SerializeField] private GameObject m_dialoguePane;
        [SerializeField] private GameObject m_playerChoicesParent;
        [SerializeField] private GameObject m_playerChoiceButtonPrefab;

        private EventHandler<PlayerMadeChoiceEvent> m_playerMadeChoice;
        
        private Coroutine m_fadeInTextCoroutine;
        
        public event EventHandler<PlayerMadeChoiceEvent> PlayerMadeChoice
        {
            add => this.m_playerMadeChoice += value;
            remove => this.m_playerMadeChoice -= value;
        } 
        
        public bool IsCurrentlyFadingTextIn() => this.m_fadeInTextCoroutine != null;

        public void ShowDialogueText(string text, ITalkable talker)
        {
            this.m_dialoguePane.SetActive(true);
            this.StopFadingInText();
            this.m_fadeInTextCoroutine = this.StartCoroutine(this.FadeInText(text));
            this.m_talkerName.text = talker.Name;
            this.m_icon.sprite = talker.Icon;
        }

        public void ShowTextImmediatly(string text, ITalkable talker)
        {
            this.StopFadingInText();
            this.m_dialogueText.text = text;
            this.m_talkerName.text = talker.Name;
            this.m_icon.sprite = talker.Icon;
        }
        
        public void ShowPlayerChoices(IEnumerable<DialogueNode> playerChoices)
        {
            Cursor.visible = true;
            this.ClearCurrentPlayerChoices();
            foreach (var playerChoice in playerChoices)
            {
                var newBtn = Instantiate(this.m_playerChoiceButtonPrefab, this.m_playerChoicesParent.transform).GetComponent<PlayerChoiceButton>();
                newBtn.ChoiceText = playerChoice.Texts[0];
                newBtn.ChoiceButton.onClick.AddListener(() =>
                {
                    this.m_playerMadeChoice?.Invoke(this, new PlayerMadeChoiceEvent(playerChoice.Id));
                });
            }
        }

        public void HidePlayerChoices()
        {
            this.ClearCurrentPlayerChoices();
            Cursor.visible = false;
        }
        
        public void Close()
        {
            this.StopFadingInText();
            this.m_dialoguePane.SetActive(false);
        }

        private IEnumerator FadeInText(string text)
        {
            this.m_dialogueText.text = string.Empty;
            var textChars = text.ToCharArray();
            foreach (var textChar in textChars)
            {
                yield return new WaitForSeconds(0.1f);
                this.m_dialogueText.text += textChar;
            }

            this.m_fadeInTextCoroutine = null;
        }

        private void StopFadingInText()
        {
            if (this.m_fadeInTextCoroutine != null)
            {
                this.StopCoroutine(this.m_fadeInTextCoroutine);
                this.m_fadeInTextCoroutine = null;
            }
        }

        private void ClearCurrentPlayerChoices()
        {
            foreach (Transform child in this.m_playerChoicesParent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}