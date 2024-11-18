using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menu
{
    public class ScrollFix : ScrollRect, ISelectHandler

    {
        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            OnBeginDrag(eventData);
        }

        public void OnSelect(BaseEventData eventData)
        {
        }
    }
}