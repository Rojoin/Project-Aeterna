using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        public bool isUsingController;
    }
}