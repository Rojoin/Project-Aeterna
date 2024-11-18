using System;
using System.Collections;
using Coffee.UIEffects;
using CustomChannels;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public enum PlayerPortraitStates
    {
        None,
        Normal,
        InBattle,
        Hit,
        LowHealth
    }

    public class PlayerPortrait : MonoBehaviour
    {
        public PlayerPortraitChannelSO OnPortraitChangeChannel;
        [SerializeField] private float whiteEffectTime = 0.2f;
        public AnimationCurve whiteAnimationCurve;
        private PlayerPortraitStates currentState = PlayerPortraitStates.Normal;
        public Sprite defaultPortrait;
        public Sprite inBattlePortrait;
        public Sprite hitPortrait;
        public Sprite lowHealthPortrait;
        private Image portrait;
        private UIEffect effectPortrait;
        private Coroutine hitPlayer;
        private Coroutine lowHealthPlayer;

        void OnEnable()
        {
            portrait = GetComponent<Image>();
            effectPortrait = GetComponent<UIEffect>();
            portrait.sprite = defaultPortrait;
            OnPortraitChangeChannel.Subscribe(ChangePortrait);
        }

        void OnDisable()
        {
            OnPortraitChangeChannel.Unsubscribe(ChangePortrait);
        }

        private void ChangePortrait(PlayerPortraitStates obj)
        {
            switch (obj)
            {
                case PlayerPortraitStates.None:
                    break;
                case PlayerPortraitStates.Normal:
                    if (currentState != PlayerPortraitStates.LowHealth)
                    {
                        portrait.sprite = defaultPortrait;
                        currentState = PlayerPortraitStates.Normal;
                    }

                    break;
                case PlayerPortraitStates.InBattle:
                    portrait.sprite = inBattlePortrait;
                    currentState = PlayerPortraitStates.InBattle;
                    break;
                case PlayerPortraitStates.Hit:
                    OnPlayerHit();
                    break;
                case PlayerPortraitStates.LowHealth:
                    portrait.sprite = lowHealthPortrait;
                    currentState = PlayerPortraitStates.LowHealth;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
            }
        }

        private void OnPlayerHit()
        {
            portrait.sprite = hitPortrait;

            if (hitPlayer != null)
            {
                StopCoroutine(hitPlayer);
            }

            hitPlayer = StartCoroutine(PlayerHitAnim());
        }

        public IEnumerator PlayerHitAnim()
        {
            float timer = 0;
            while (timer < whiteEffectTime)
            {
                timer += Time.deltaTime;
                effectPortrait.colorFactor = whiteAnimationCurve.Evaluate(timer / whiteEffectTime);
                yield return null;
            }

            effectPortrait.effectFactor = 0.0f;
            ChangePortrait(currentState);
        }
    }
}