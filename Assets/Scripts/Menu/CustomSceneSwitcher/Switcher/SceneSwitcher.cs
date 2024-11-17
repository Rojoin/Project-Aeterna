using System.Threading.Tasks;
using CustomSceneSwitcher.Switcher.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using CustomSceneSwitcher.Switcher.Data;
using Object = UnityEngine.Object;

namespace CustomSceneSwitcher.Switcher
{
    public static class SceneSwitcher
    {
        private static AsyncOperation _changeSceneOperation;
        private static TransitionControl _currentTransitionControl;
        private static LoadingBarBase _currentLoadingBar;

        private static SceneChangeData _currentSceneChangeData;
        private static bool isChangingScene = false;

        public static async void ChangeScene(SceneChangeData sceneChangeData)
        {
            if (!isChangingScene)
            {
                isChangingScene = true;
                _currentSceneChangeData = sceneChangeData;

                _changeSceneOperation =
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_currentSceneChangeData.SceneReference);
                _changeSceneOperation.allowSceneActivation = false;


                if (_currentSceneChangeData.TransitionControlPrefab)
                {
                    _currentTransitionControl = Object.Instantiate(_currentSceneChangeData.TransitionControlPrefab);
                    Object.DontDestroyOnLoad(_currentTransitionControl);

                    _currentTransitionControl.OnStateEnter += OnTransitionStateEntered;
                    _currentTransitionControl.OnStateExit += OnTransitionStateExited;

                    _currentLoadingBar = _currentSceneChangeData.LoadingBarPrefab
                        ? Object.Instantiate(_currentSceneChangeData.LoadingBarPrefab,
                            _currentTransitionControl.transform)
                        : null;

                    _currentTransitionControl.SetTransitionSpeed(_currentSceneChangeData.TransitionSpeed);
                    _currentTransitionControl.SetLoopTime(_currentSceneChangeData.MinTransitionTime);
                    _currentTransitionControl.StartTransition();

                    float currentTime = 0;
                    while (_changeSceneOperation.progress < .9f || _currentTransitionControl.IsLocked())
                    {
                        if (_currentLoadingBar)
                        {
                            UpdateTransitionBarProgress(currentTime, _currentSceneChangeData.MinTransitionTime);
                            currentTime = Mathf.Clamp(currentTime + Time.deltaTime, 0f,
                                _currentSceneChangeData.MinTransitionTime);
                        }

                        await Task.Yield();
                    }

                    _currentTransitionControl.StopTransition();
                }

                _changeSceneOperation.allowSceneActivation = true;
            }
        }

        private static void OnTransitionStateEntered(SceneSwitcherEnumState state)
        {
            bool loadingBarBasedOnTransition = _currentSceneChangeData.LoadingBarVisibility ==
                                               LoadingBarVisibility.OnTransitionStartAndEnd;

            bool loadingBarBasedOnLoop = _currentSceneChangeData.LoadingBarVisibility ==
                                         LoadingBarVisibility.OnLoopStartAndEnd;

            if (state == SceneSwitcherEnumState.Showing)
            {
                if (_currentLoadingBar && loadingBarBasedOnTransition)
                    _currentLoadingBar.Show();
            }
            else if (state == SceneSwitcherEnumState.Hiding)
            {
                if (_currentLoadingBar && loadingBarBasedOnTransition)
                    _currentLoadingBar.Hide();
            }
            else if (state == SceneSwitcherEnumState.LoopShow)
            {
                if (_currentLoadingBar && loadingBarBasedOnLoop)
                    _currentLoadingBar.Show();
            }
        }

        private static void OnTransitionStateExited(SceneSwitcherEnumState state)
        {
            bool loadingBarBasedOnLoop = _currentSceneChangeData.LoadingBarVisibility ==
                                         LoadingBarVisibility.OnLoopStartAndEnd;

            if (state == SceneSwitcherEnumState.LoopShow)
            {
                if (_currentLoadingBar && loadingBarBasedOnLoop)
                    _currentLoadingBar.Hide();
                isChangingScene = false;
            }
            else if (state == SceneSwitcherEnumState.Hiding)
            {
                _currentTransitionControl.OnStateEnter -= OnTransitionStateEntered;
                _currentTransitionControl.OnStateExit -= OnTransitionStateExited;
                Object.Destroy(_currentTransitionControl.gameObject);
                isChangingScene = false;
            }
        }

        private static void UpdateTransitionBarProgress(float currentTime, float minTime)
        {
            float minProgress = currentTime / minTime;
            float realProgress = _changeSceneOperation.progress / .9f;

            float progress = Mathf.Min(minProgress, realProgress);
            _currentLoadingBar.SetProgress(progress);
        }
    }
}