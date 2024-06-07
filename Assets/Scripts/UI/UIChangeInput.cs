using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UIChangeInput : MonoBehaviour
{
    [SerializeField] private BoolChannelSO OnControllersChange;
    [SerializeField] private TextMeshProUGUI textContainer;
    [SerializeField] private string controllerText;
    [SerializeField] private string keyboardText;

    // Update is called once per frame
    private void OnEnable()
    {
        OnControllersChange.Subscribe(ChangeInput);
    }

    private void OnDisable()
    {
        OnControllersChange.Unsubscribe(ChangeInput);
    }

    private void ChangeInput(bool obj)
    {
        controllerText = controllerText.Replace("ENTER","\n");
        keyboardText = keyboardText.Replace("ENTER","\n");
        textContainer.text = obj ? controllerText : keyboardText;
    }
}