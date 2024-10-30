using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Menu
{
    public class ScrollFix : ScrollRect {
        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            OnBeginDrag(eventData);
        }
    }
}