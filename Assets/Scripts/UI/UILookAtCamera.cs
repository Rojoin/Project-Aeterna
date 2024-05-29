using System;
using UnityEngine;

namespace UI
{
    public class UILookAtCamera : MonoBehaviour
    {
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            transform.rotation = Quaternion.LookRotation(_camera.transform.forward);
        }
    }
}