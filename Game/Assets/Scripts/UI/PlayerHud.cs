using System.Collections;
using System.Collections.Generic;
using EventArgs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHud : MonoBehaviour
{
    [SerializeField] private Slider m_healthBar;
    [SerializeField] private TextMeshProUGUI m_ammoText;
    [SerializeField] private PlayerController m_player;
    [SerializeField] private TextMeshProUGUI m_bossName;
    [SerializeField] private Slider m_bossHealthBar;
    [SerializeField] private GameObject m_playerBossHud;
    [SerializeField] private GameObject m_dialogueBox;
    [SerializeField] private TextMeshProUGUI m_dialogueText;
    [SerializeField] private GameObject m_youDiedPanel;
    [SerializeField] private GameObject m_youWonPanel;
    [SerializeField] private Image m_reloadImage;

    private Coroutine m_hideCoroutine;
    private Coroutine m_reloadCoroutine;
    
    public void ShowBossHud(MagmaBoss boss)
    {
        this.m_bossName.text = boss.Name;
        boss.HealthController.ResourceValueChanged += this.BossHealthChanged;
        this.m_bossHealthBar.maxValue = boss.HealthController.MaxValue;
        this.m_bossHealthBar.minValue = 0;
        this.m_bossHealthBar.value = boss.HealthController.CurrentValue;
        this.m_playerBossHud.SetActive(true);
    }

    public void ShowDialogue(string text)
    {
        if(this.m_hideCoroutine != null)
            StopCoroutine(this.m_hideCoroutine);
        
        this.m_dialogueText.text = text;
        this.m_dialogueBox.SetActive(true);

        this.m_hideCoroutine = this.StartCoroutine(this.HideDialogue());
    }

    public void ShowReloadUI(float duration)
    {
        this.StartCoroutine(ShowReloadAnimation(duration));
    }

    public void ShowWinScreen()
    {
        this.m_youWonPanel.SetActive(true);
    }

    private IEnumerator HideWinScreen()
    {
        yield return new WaitForSeconds(5f);
        this.m_youWonPanel.SetActive(false);
    }

    private IEnumerator ShowReloadAnimation(float duration)
    {
        var currentDuration = 0f;
        while(currentDuration < duration)
        {
            m_reloadImage.fillAmount = 1f - currentDuration / duration;
            yield return new WaitForEndOfFrame();
            currentDuration += Time.deltaTime;
        }
        m_reloadImage.fillAmount = 0f;
    }

    private IEnumerator HideDialogue()
    {
        yield return new WaitForSeconds(10f);
        this.m_dialogueBox.SetActive(false);
        this.m_hideCoroutine = null;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        this.m_healthBar.maxValue = this.m_player.HealthController.MaxValue;
        this.m_healthBar.minValue = 0;
        this.m_healthBar.value = this.m_player.HealthController.CurrentValue;
        
        this.m_player.HealthController.ResourceValueChanged += this.PlayerHealthChanged;
        this.m_player.HealthController.MaxValueChanged += this.PlayerMaxHealthChanged;
        this.m_ammoText.text = $"{this.m_player.QuiverController.CurrentValue}/{this.m_player.QuiverController.MaxValue}";
        this.m_player.QuiverController.MaxValueChanged += this.ArrowControllerValueChanged;
        this.m_player.QuiverController.ResourceValueChanged += this.ArrowControllerValueChanged;
    }
    
    private void BossHealthChanged(object sender, ResourceValueChangedEventArgs e)
    {
        this.m_bossHealthBar.value = e.NewValue;

        if (e.NewValue <= 0)
        {
            this.m_playerBossHud.SetActive(false);
        }
    }

    private void PlayerMaxHealthChanged(object sender, ResourceValueChangedEventArgs e)
    {
        this.m_healthBar.maxValue = e.NewValue;
    }

    private void ArrowControllerValueChanged(object sender, ResourceValueChangedEventArgs e)
    {
        this.m_ammoText.text = $"{this.m_player.QuiverController.CurrentValue}/{this.m_player.QuiverController.MaxValue}";
    }

    private void PlayerHealthChanged(object sender, ResourceValueChangedEventArgs e)
    {
        this.m_healthBar.value = e.NewValue;
        if (e.NewValue <= 0)
        {
            this.m_youDiedPanel.SetActive(true);
        }
    }
}
