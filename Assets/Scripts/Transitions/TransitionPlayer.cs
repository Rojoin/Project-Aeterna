using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPlayer : MonoBehaviour
{
    [SerializeField] private Animator transitionAnimator;

    [SerializeField] private float transitionTime;

    private void OnEnable()
    {
        transitionAnimator.SetTrigger("Start");
    }
    
    public IEnumerator StartTransition()
    {
       

        yield return new WaitForSeconds(transitionTime);
    }
}