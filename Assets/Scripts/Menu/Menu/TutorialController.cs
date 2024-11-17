using CustomSceneSwitcher.Switcher;
using CustomSceneSwitcher.Switcher.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private SceneChangeData GoToGame;

    public void LoadGame() 
    {
        SceneSwitcher.ChangeScene(GoToGame);
    }
}
