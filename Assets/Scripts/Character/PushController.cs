using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushController : MonoBehaviour
{
   [SerializeField] private Rigidbody _rigidbody;
   [SerializeField] private Vector3 _PushVelocity;

   public void Push(Vector3 target)
   {
      
      //_rigidbody.velocity = new Vector3(_PushVelocity.x * -target.x,_PushVelocity.y,_PushVelocity.z * -target.z);
      Vector3 diference = (transform.position - target);
      _rigidbody.linearVelocity = new Vector3(_PushVelocity.x * diference.x,_PushVelocity.y,_PushVelocity.z * diference.z);
   }
}
