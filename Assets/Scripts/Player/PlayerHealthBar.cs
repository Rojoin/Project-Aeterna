using System;
using Coffee.UIEffects;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Image circleBar;
    [SerializeField] private Image normalBar;
    [SerializeField] private UIShiny specialBarShinny;
    [SerializeField] private Image specialBar;
    [SerializeField] private FloatChannelSO specialBarChannel;

    [SerializeField] private PlayerEntitySO player;

    [SerializeField] private float circlePercentage = 0.3f;

    [SerializeField] private float circleFillAmount = 0.75f;

    private float fillAmount;

    public float FillAmount
    {
        get => fillAmount;
        set
        {
            fillAmount = value;
            UpdateSlider();
        }
    }

    private void OnEnable()
    {
       specialBarChannel.Subscribe(UpdateSpecialSlider);
       specialBarShinny.Play();
    } 
    private void OnDisable()
    {
       specialBarChannel.Unsubscribe(UpdateSpecialSlider);
    }

    private void UpdateSpecialSlider(float obj)
    {
        specialBar.fillAmount = obj;
        if (obj >= 1)
        {
            specialBar.fillAmount = 1;
            specialBarShinny.Play();
        }
        else
        {
            specialBarShinny.Stop();
        }
    }

    private void Start()
    {
        CircleFill();
        NormalFill();
    }

    private void UpdateSlider()
    {
        CircleFill();
        NormalFill();
    }

    public void CircleFill()
    {
        float healthPercentage = player.health / player.maxHealth;
        float circleFill = healthPercentage / circlePercentage;

        circleFill *= circleFillAmount;

        circleFill = Mathf.Clamp(circleFill, 0, circleFillAmount);

        circleBar.fillAmount = circleFill;
    }

    public void NormalFill()
    {
        float circleAmount = circlePercentage * player.maxHealth;

        float extraHealth = player.health - circleAmount;
        float extraTotalHealth = player.maxHealth - circleAmount;

        float extraFill = extraHealth / extraTotalHealth;

        extraFill = Mathf.Clamp(extraFill, 0, 1);

        normalBar.fillAmount = extraFill;
    }
}
