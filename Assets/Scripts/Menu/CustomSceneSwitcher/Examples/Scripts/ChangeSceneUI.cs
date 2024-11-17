using CustomSceneSwitcher.Switcher;
using CustomSceneSwitcher.Switcher.Data;
using UnityEngine;

namespace CustomSceneSwitcher.Examples.Scripts
{
    public class ChangeSceneUI : MonoBehaviour
    {
        [SerializeField] private SceneChangeData sceneChangeData;
        
        public void ChangeScene()
        {
            SceneSwitcher.ChangeScene(sceneChangeData);
        }
    }
}
