﻿using Coffee.UIExtensions;
using UnityEngine;

namespace UI
{
    public class UIParticleController : MonoBehaviour
    {
        [SerializeField] private Vector3ChannelSO OnParticleCalled;
        private RectTransform rectTransform;
        private UIParticle particleSystem;
        [SerializeField] private Canvas canvas;

        public void OnEnable()
        {
            OnParticleCalled.Subscribe(SpawnParticle);
            rectTransform = GetComponent<RectTransform>();
            particleSystem = GetComponent<UIParticle>();
        }

        private void OnDisable()
        {
            OnParticleCalled.Unsubscribe(SpawnParticle);
        }

        
        private void SpawnParticle(Vector3 obj)
        {
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(obj);
            rectTransform.localPosition = new Vector3(0, 0, 0); 
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, null, out var uiPosition);

            rectTransform.localPosition = uiPosition;
            particleSystem.Play();
        }
    }
}