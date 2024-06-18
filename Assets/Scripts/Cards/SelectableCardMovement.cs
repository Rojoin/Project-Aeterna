using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableCardMovement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;

    [SerializeField] private GameObject cardInformation;

    [Header("Setup")]
    [SerializeField] private float minRotation;
    [SerializeField] private float maxRotation;

    [SerializeField] private float rotationSpeed;

    [SerializeField] private float xCardRotation;

    [SerializeField] private Button button;

    [SerializeField] private bool isSelected = false;

    private RectTransform rectTransform;

    private float targetAngle;

    private bool canMove = true;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SelectableCardMovementWithMouse()
    {
        if (canMove)
        {
            Vector3 mousePos = Input.mousePosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePos, null, out Vector2 localMousePos);

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

            transform.rotation = Quaternion.Euler(xCardRotation, currentAngle, 0);
        }
    }

    public void SelectableCardMovementWithGamepad()
    {
        rotationSpeed = 2;

        if (!IsButtonSelected(button))
        {
            isSelected = false;

            Vector3 targetPosition = EventSystem.current.currentSelectedGameObject.transform.position;

            Vector3 direction = rectTransform.position - targetPosition;

            targetAngle = Mathf.Clamp(Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg, minRotation, maxRotation);

            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            rectTransform.rotation = Quaternion.Slerp(rectTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        else 
        { 
            isSelected = true;

            transform.rotation = Quaternion.Euler(xCardRotation, 0, 0);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        canMove = false;
        isSelected = true;
        cardInformation.SetActive(true);
        transform.rotation = Quaternion.Euler(xCardRotation, 0, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        canMove = true;
        isSelected = false;
        cardInformation.SetActive(false);
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

    bool IsButtonSelected(Button button)
    {
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (selectedObject != null && selectedObject == button.gameObject)
        {
            return true;
        }

        return false;
    }

    public bool IsSelected() 
    {
        if (isSelected) 
        {
            return true;
        }

        else 
        {
            return false;
        }
    }
}
