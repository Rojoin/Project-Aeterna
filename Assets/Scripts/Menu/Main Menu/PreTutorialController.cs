using System;
using System.Collections;
using CustomSceneSwitcher.Switcher;
using CustomSceneSwitcher.Switcher.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Main_Menu
{
    public class PreTutorialController : MonoBehaviour
    {
        [SerializeField] private Button _playTutorialButton;
        [SerializeField] private Button _skipTutorialButton;
        [SerializeField] private SceneChangeData _firstLevel;

        public void Awake()
        {
            _playTutorialButton.onClick.AddListener(PlayTutorial);
            _skipTutorialButton.onClick.AddListener(PlayGame);
        }

        private IEnumerator Start()
        {
            yield return null;
            yield return null;
            EventSystem.current.GetComponent<EventSystem>().SetSelectedGameObject(_playTutorialButton.gameObject);
            _playTutorialButton.Select();
            yield break;
        }

        public void OnDestroy()
        {
            _playTutorialButton.onClick.RemoveAllListeners();
            _skipTutorialButton.onClick.RemoveAllListeners();
        }

        private void PlayTutorial()
        {
            //_playerStats.isTutorialOn = true;
            ChangeScene();
        }
        private void PlayGame()
        {
            //_playerStats.isTutorialOn = false;
            ChangeScene();
        }

        private void ChangeScene()
        {
            SceneSwitcher.ChangeScene(_firstLevel);
        }
    }
}