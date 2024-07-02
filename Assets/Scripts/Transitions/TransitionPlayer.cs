using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPlayer : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;

    [SerializeField] private float transitionTime;
    public IEnumerator StartTransition()
    {
        transitionAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);
    }
}