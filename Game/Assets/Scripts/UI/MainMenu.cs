using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject m_StartMenu;

        public void BtnPlay_Click()
        {
            SceneManager.LoadScene(1);
        }

        public void BtnQuit_Click()
        {
            Application.Quit();
        }
    }
}
