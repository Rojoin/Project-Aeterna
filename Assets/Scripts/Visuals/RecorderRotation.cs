using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visuals
{
    public class RecorderRotation : MonoBehaviour
    {
        private static readonly int ChangeFace = Animator.StringToHash("ChangeFace");
        public float rotationSpeed = 3.5f;
        public List<Transform> objectToRotate = new List<Transform>();
        public List<Material> materialsToChange = new List<Material>();
        public List<MeshRenderer> meshesToChange = new List<MeshRenderer>();
        private Animator animator;

        private void OnEnable()
        {
            animator = GetComponent<Animator>();
        }

        public void Update()
        {
        }

        [ContextMenu("Rotate Object")]
        public void Rotate()
        {
            StartCoroutine(RotateModels());
        }

        private IEnumerator RotateModels()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            float rotationValue = 0;
            bool changeFace = false;
            animator.SetBool(ChangeFace, changeFace);
            while (rotationValue < 360)
            {
                rotationValue += rotationSpeed * Time.deltaTime;
                foreach (Transform transform1 in objectToRotate)
                {
                    transform1.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                }

                if (rotationValue > 180 && !changeFace)
                {
                    changeFace = true;
                    animator.SetBool(ChangeFace, changeFace);
                }

                yield return null;
            }
            yield return new WaitForSecondsRealtime(0.5f);
            rotationValue = 0;
            while (rotationValue < 360)
            {
                rotationValue += rotationSpeed * Time.deltaTime;
                foreach (Transform transform1 in objectToRotate)
                {
                    transform1.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                }

                if (rotationValue > 180 && changeFace)
                {
                    changeFace = false;
                    animator.SetBool(ChangeFace, changeFace);
                }

                yield return null;
            }
            yield break;
        }
    }
}