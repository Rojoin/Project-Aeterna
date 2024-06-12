using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    private RectTransform canvasRectTransform;

    private float targetAngle;

    [Header("Set Up")]
    [SerializeField] private float minRotation;
    [SerializeField] private float maxRotation;

    [SerializeField] private float rotationSpeed;

    void Start()
    {
        canvasRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, mousePos, null, out Vector2 localMousePos);

        Vector2 direction = ((RectTransform)transform).anchoredPosition - localMousePos;
        direction.Normalize();

        targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        targetAngle = Mathf.Clamp(targetAngle, minRotation, maxRotation);

        float currentAngle = transform.rotation.eulerAngles.y;
        float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

        float rotationAmount = rotationSpeed * Time.deltaTime;

        if (Mathf.Abs(angleDifference) > rotationAmount)
            currentAngle += Mathf.Sign(angleDifference) * rotationAmount;
        
        else
            currentAngle = targetAngle;
        
        transform.rotation = Quaternion.Euler(10, currentAngle, 0);
    }
}
