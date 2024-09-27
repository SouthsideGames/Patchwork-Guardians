using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using System.Collections;

namespace MonsterBattleArena.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanel : UIBehaviour
    {
        public RectTransform RTransform { get; private set; }
        public CanvasGroup Group { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            Group = GetComponent<CanvasGroup>();
            RTransform = GetComponent<RectTransform>();
        }

        public virtual YieldInstruction Show(Action onComplete, bool useTransition = true)
        {
            IEnumerator exec()
            {
                PopupBackground.Show(this);

                if (!useTransition)
                {
                    transform.localScale = Vector2.one;
                    Group.alpha = 1.0f;
                }
                else
                {
                    Sequence sequence = DOTween.Sequence();
                    sequence.Append(RTransform.DOScale(Vector2.one, UISettings.PopupTransitionDuration));
                    sequence.Join(Group.DOFade(1.0f, UISettings.PopupTransitionDuration));
                    yield return sequence.WaitForCompletion();
                }

                onComplete?.Invoke();
            }

            gameObject.SetActive(true);
            return StartCoroutine(exec());
        }

        public virtual YieldInstruction Hide(Action onComplete, bool useTransition = true)
        {
            IEnumerator exec()
            {
                PopupBackground.Hide();

                if (!useTransition)
                {
                    transform.localScale = Vector2.zero;
                    Group.alpha = 0.0f;
                }
                else
                {
                    Sequence sequence = DOTween.Sequence();
                    sequence.Append(RTransform.DOScale(Vector2.zero, UISettings.PopupTransitionDuration));
                    sequence.Join(Group.DOFade(0.0f, UISettings.PopupTransitionDuration));
                    yield return sequence.WaitForCompletion();
                }

                onComplete?.Invoke();
                gameObject.SetActive(false);
            }

            return StartCoroutine(exec());
        }
    }
}
