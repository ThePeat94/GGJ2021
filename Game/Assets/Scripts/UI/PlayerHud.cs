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
    
    // Start is called before the first frame update
    void Start()
    {
        this.m_healthBar.maxValue = this.m_player.HealthController.MaxValue;
        this.m_healthBar.minValue = 0;
        this.m_healthBar.value = this.m_player.HealthController.CurrentValue;
        
        this.m_player.HealthController.ResourceValueChanged += HealthControllerOnResourceValueChanged;
        this.m_player.HealthController.MaxValueChanged += HealthControllerOnMaxValueChanged;
        this.m_ammoText.text = $"{this.m_player.QuiverController.CurrentValue}/{this.m_player.QuiverController.MaxValue}";
        this.m_player.QuiverController.MaxValueChanged += this.ArrowControllerValueChanged;
        this.m_player.QuiverController.ResourceValueChanged += this.ArrowControllerValueChanged;
    }

    private void HealthControllerOnMaxValueChanged(object sender, ResourceValueChangedEventArgs e)
    {
        this.m_healthBar.maxValue = e.NewValue;
    }

    private void ArrowControllerValueChanged(object sender, ResourceValueChangedEventArgs e)
    {
        this.m_ammoText.text = $"{this.m_player.QuiverController.CurrentValue}/{this.m_player.QuiverController.MaxValue}";
    }

    private void HealthControllerOnResourceValueChanged(object sender, ResourceValueChangedEventArgs e)
    {
        this.m_healthBar.value = e.NewValue;
    }
}
