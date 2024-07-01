using UnityEngine;

public class SwCollision : SwBasic
{
    //collisions
    [SerializeField] private bool ColissionEnter;
    [SerializeField] private bool ColissionExit;
    [SerializeField] private bool TiggerEnter;
    [SerializeField] private bool TriggerExit;

    private void OnCollisionEnter(Collision collision)
    {
        if (ColissionEnter)
        {
            OnPlayAudio();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (ColissionExit)
        {
            OnPlayAudio();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TiggerEnter)
        {
            OnPlayAudio();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (TriggerExit)
        {
            OnPlayAudio();
        }
    }
}
