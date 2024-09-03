using UnityEngine;
using ScriptableObjects.Entities;

namespace StateMachine
{
    public abstract class YukinkoState : BaseStateFSM
    {
        protected Material materialBody;
        protected Material materialFace;
        protected ShootingEnemySO enemyConfig;
        protected static float enterDefenseTimer = 0.0f;
        protected Animator _animator;
        protected YukinkoState(params object[] data) : base(data)
        {
           materialBody = data[1] as Material;
           materialFace = data[2] as Material;
           enemyConfig = data[3] as ShootingEnemySO;
           _animator = data[4] as Animator;
        }
        
    }
}