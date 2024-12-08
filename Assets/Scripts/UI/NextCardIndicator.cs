using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;

public class NextCardIndicator : MonoBehaviour
{
    private static readonly int OnReset = Animator.StringToHash("OnReset");
    private static readonly int OnGainCard = Animator.StringToHash("OnGainCard");
    [SerializeField] private List<GameObject> nextCarUIGO;
    private List<UIParticleAttractor> nextCarUI = new List<UIParticleAttractor>();
    private List<Animator> animators = new List<Animator>();

    [SerializeField] private float timeUntilChange;
    private int toActivate = 0;

    public void CheckNextCard(int toNextCard)
    {
        StartCoroutine(ShowStar(toNextCard));
    }

    private void OnEnable()
    {
        foreach (GameObject attractor in nextCarUIGO)
        {
            animators.Add(attractor.GetComponent<Animator>());
            nextCarUI.Add(attractor.GetComponent<UIParticleAttractor>());
        }
    }

    private IEnumerator ShowStar(int toNextCard)
    {
        toActivate = toNextCard;
        //yield return new WaitForSeconds(timeUntilChange);
        switch (toNextCard)
        {
            case 1:

                nextCarUI[0].enabled = true;

                break;

            case 2:

                nextCarUI[1].enabled = true;

                break;

            case 3:

                nextCarUI[2].enabled = true;

                break;

            default:
                break;
        }

        yield break;
    }

    public void ActivateIcon()
    {
        int activate = toActivate - 1 >= 0 ? toActivate - 1 : 0;
        animators[activate].SetTrigger(OnGainCard);
        nextCarUI[activate].enabled = false;
        
        if (activate == nextCarUIGO.Count -1)
        {
            Invoke(nameof(RestartIndicatorCard),timeUntilChange);
        }
    }

    public void RestartIndicatorCard()
    {
        for (int i = 0; i < nextCarUI.Count; i++)
        {
            nextCarUI[i].enabled = false;
            animators[i].SetTrigger(OnReset);
        }
    }
}