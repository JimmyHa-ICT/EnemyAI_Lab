using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace HUST.EnemyAILab.Shared.Utility
{
    public sealed class DemoSceneReset : MonoBehaviour
    {
        private void Update()
        {
            if (Keyboard.current?.rKey.wasPressedThisFrame == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
