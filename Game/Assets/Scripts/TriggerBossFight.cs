using Nidavellir.FoxIt.Enemy;
using Nidavellir.FoxIt.UI;
using UnityEngine;

namespace Nidavellir.FoxIt
{
    public class TriggerBossFight : MonoBehaviour
    {
        [SerializeField] private MagmaBoss m_magmaBoss;
        [SerializeField] private PlayerHud m_playerHud;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerController>() != null)
            {
                this.m_magmaBoss.StartFight();
                this.m_playerHud.ShowBossHud(this.m_magmaBoss);
                Destroy(this.gameObject);
            }
        }
    }
}