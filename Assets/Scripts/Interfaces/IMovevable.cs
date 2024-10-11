using UnityEngine;

public interface IMovevable 
{
    public void Move(Vector3 direction,float speed,float maxTime, AnimationCurve curve);
}