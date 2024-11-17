using UnityEngine;
using CustomSceneSwitcher.Switcher.External;

namespace CustomSceneSwitcher.Switcher.Data
{
    [System.Serializable]
    public struct SceneChangeData
    {
        public SceneReference SceneReference => sceneReference;
        [SerializeField] private SceneReference sceneReference;

        public TransitionControl TransitionControlPrefab => transitionControlPrefab;
        [SerializeField] private TransitionControl transitionControlPrefab;

        public float TransitionSpeed => transitionSpeed;
        [SerializeField] private float transitionSpeed;

        public float MinTransitionTime => minTransitionTime;
        [SerializeField] private float minTransitionTime;

        public LoadingBarBase LoadingBarPrefab => loadingBarPrefab;
        [SerializeField] private LoadingBarBase loadingBarPrefab;
        
        public LoadingBarVisibility LoadingBarVisibility => loadingBarVisibility;
        [SerializeField] private LoadingBarVisibility loadingBarVisibility;
    }
}
