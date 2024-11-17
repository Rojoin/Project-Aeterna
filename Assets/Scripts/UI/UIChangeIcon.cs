using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIChangeIcon : MonoBehaviour
    {
        public BoolChannelSO OnControlSchemeChange;
        
        public Sprite controllerAttackSprite;
        public Sprite controllerMoveSprite;
        public Sprite controllerSpecialSprite;
        public Sprite controllerDashSprite;
        public Sprite keyboardAttackSprite;
        public Sprite keyboardMoveSprite;
        public Sprite keyboardSpecialSprite;
        public Sprite keyboardDashSprite;
        
        public Image moveImage;
        public Image attackImage;
        public Image specialImage;
        public Image dashImage;
        
        private void OnEnable()
        {
            OnControlSchemeChange.Subscribe(OnControllerChange);
        }
        private void OnDisable()
        {
            OnControlSchemeChange.Unsubscribe(OnControllerChange);
        }
        private void OnControllerChange(bool value)
        {
            moveImage.sprite = value ?controllerMoveSprite : keyboardMoveSprite;
            attackImage.sprite = value ?controllerAttackSprite : keyboardAttackSprite;
            specialImage.sprite= value ?controllerSpecialSprite : keyboardSpecialSprite;
            dashImage.sprite = value ?controllerDashSprite : keyboardDashSprite;
        }
    }
}