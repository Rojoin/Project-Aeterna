using UnityEngine;

namespace CustomSceneSwitcher.Switcher
{
    public abstract class LoadingBarBase : MonoBehaviour
    {
        public abstract void Show();
        public abstract void Hide();
        public abstract void SetProgress(float progress);
    }
}
