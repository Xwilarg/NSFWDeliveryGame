using UnityEngine;
using UnityEngine.SceneManagement;

namespace NsfwDelivery.Manager
{
    public class MenuManager : MonoBehaviour
    {
        public void PlayGame()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
