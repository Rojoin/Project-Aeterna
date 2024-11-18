using System;
using CustomSceneSwitcher.Switcher;
using CustomSceneSwitcher.Switcher.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private SceneChangeData GoToGame;
    [SerializeField] private Button button;

    public void LoadGame() 
    {
        SceneSwitcher.ChangeScene(GoToGame);
    }

    public void Update()
    {
        EventSystem.current.SetSelectedGameObject(button.gameObject);
    }
}
