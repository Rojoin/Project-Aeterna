using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextCardIndicator : MonoBehaviour
{
    [SerializeField] private List<GameObject> nextCarUI;

    [SerializeField] private List<GameObject> toChange;

    public void CheckNextCard(int toNextCard) 
    {
        switch (toNextCard) 
        {
            case 1:

                nextCarUI[0].SetActive(false);
                toChange[0].SetActive(true);

                break;

            case 2:

                nextCarUI[1].SetActive(false);
                toChange[1].SetActive(true);

                break;

            case 3:

                nextCarUI[2].SetActive(false);
                toChange[2].SetActive(true);

                break;

            default:
                break;
        }
    }

    public void RestartIndicatorCard()
    {
        for (int i = 0; i < nextCarUI.Count; i++) 
        {
            nextCarUI[i].SetActive(true);
            toChange[i].SetActive(false);
        }
    }
}
