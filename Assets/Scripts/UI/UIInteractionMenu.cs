using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIInteractionMenu : MonoBehaviour
    {
        public BoolChannelSO OnControlSchemeChange;
        
        public Sprite controllerInteractionSprite;
        public Sprite controllerAlternativeInteractionSprite;
        public Sprite keyboardInteractionSprite;
        public Sprite keyboardAlternativeInteractionSprite;
        
        public List<Image> interactionImages;
        public List<Image> alternativeInteractionImages;

        private void OnEnable()
        {
            OnControlSchemeChange.Subscribe(OnControllerChange);
        }

        private void OnDisable()
        {
            OnControlSchemeChange.Unsubscribe(OnControllerChange);
        }
        private void OnControllerChange(bool value)
        {
            foreach (Image interactionImage in interactionImages)
            {
                interactionImage.sprite = value ? controllerInteractionSprite : keyboardInteractionSprite;
            }
            foreach (Image interactionImage in alternativeInteractionImages)
            {
                interactionImage.sprite = value ? controllerAlternativeInteractionSprite : keyboardAlternativeInteractionSprite;
            }
        }
    }
}