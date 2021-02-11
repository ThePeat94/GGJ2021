using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nidavellir.FoxIt.UI
{
    public class PlayerChoiceButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_textToDisplay;
        [SerializeField] private Button m_button;

        public string ChoiceText
        {
            get => this.m_textToDisplay.text;
            set => this.m_textToDisplay.text = value;
        }

        public Button ChoiceButton => this.m_button;
    }
}