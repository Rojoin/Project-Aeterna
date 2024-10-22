using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Visuals
{
    public class ToriOpacity : MonoBehaviour
    {
        private static readonly int Transparency = Shader.PropertyToID("_Transparency");
        private Material toChange;
        [SerializeField] private float screenHeightThreshold;
        
        [SerializeField] private float transparency = 0.5f;

        private void FixedUpdate()
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            
            if (screenPos.y < Screen.height * screenHeightThreshold)
            {
                foreach (MeshRenderer child in GetComponentsInChildren<MeshRenderer>())
                {
                    ChangeOpacity(transparency,child.material);
                }
            }
            else
            {
                foreach (MeshRenderer child in GetComponentsInChildren<MeshRenderer>())
                {
                    ChangeOpacity(0.0f,child.material);
                }
            }

          
        }

        void Update()
        {
        }

        void ChangeOpacity(float opacity,Material material)
        {
            material.SetFloat(Transparency, opacity);
        }
    }
}