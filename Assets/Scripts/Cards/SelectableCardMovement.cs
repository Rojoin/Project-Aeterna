using ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SelectableCardMovement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler,
    IDeselectHandler
{
    [Header("References")] [SerializeField]
    private GameManager gameManager;

    [SerializeField] private GameObject cardInformation;

    [SerializeField] private GameSettings gameSettings;

    [Header("Setup")] [SerializeField] private float minRotation;
    [SerializeField] private float maxRotation;

    [SerializeField] private float rotationSpeed;

    [Header("Setup: for controller")] [SerializeField]
    private float xCardRotation;

    [SerializeField] private Button button;

    private RectTransform rectTransform;

    private float targetAngle;
    private float angle;

    public bool canMove = true;
    public bool isSelected = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!gameSettings.isUsingController)
        {
            SelectableCardMovementWithMouse();
        }

        else
        {
            SelectableCardMovementWithGamepad();
        }
    }

    public void SelectableCardMovementWithMouse()
    {
        if (canMove)
        {
            Vector3 mousePos = Input.mousePosition;

            if (mousePos.x < Screen.width / 2)
            {
                angle = maxRotation;
            }

            if (mousePos.x > Screen.width / 2)
            {
                angle = minRotation;
            }

            float currentAngle = Mathf.LerpAngle(rectTransform.eulerAngles.y, angle, rotationSpeed * Time.deltaTime);

            rectTransform.eulerAngles = new Vector3(0, currentAngle, 0);
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

            rectTransform.rotation =
                Quaternion.Slerp(rectTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        else
        {
            isSelected = true;

            transform.rotation = Quaternion.Euler(xCardRotation, 0, 0);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSelectCard();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnDeselectCard();
    }

    private void OnDeselectCard()
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
        return isSelected;
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnSelectCard();
    }

    private void OnSelectCard()
    {
        canMove = false;
        isSelected = true;
        cardInformation.SetActive(true);
        transform.rotation = Quaternion.Euler(xCardRotation, 0, 0);
        AkSoundEngine.PostEvent("Cards_Show_Play", gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnDeselectCard();
    }
}