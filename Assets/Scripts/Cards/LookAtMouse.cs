using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LookAtMouse : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform canvasRectTransform;

    private float targetAngle;

    [Header("Setup")]
    [SerializeField] private float minRotation;
    [SerializeField] private float maxRotation;

    [SerializeField] private float rotationSpeed;

    public bool canMove = true;

    void Start()
    {
        canvasRectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (canMove)
        SelectableCardMovement();
    }

    public void SelectableCardMovement() 
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        canMove = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canMove = true;
    }

    private void OnDisable()
    {
        EventTrigger trigger = GetComponent<EventTrigger>();
        if (trigger != null)
        {
            trigger.triggers.RemoveAll(t => t.eventID == EventTriggerType.PointerEnter);
            trigger.triggers.RemoveAll(t => t.eventID == EventTriggerType.PointerExit);
        }
    }
}
