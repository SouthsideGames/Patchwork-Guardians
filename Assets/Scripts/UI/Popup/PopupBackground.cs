using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace MonsterBattleArena.UI
{
    internal static class PopupBackground
    {
        private static Controller _Controller { get; set; }

        private static void TryCreateInstance()
        {
            if (_Controller != null)
                return;

            GameObject instance = new GameObject("PopupBackground.Controller");
            _Controller = instance.AddComponent<Controller>();
        }

        public static YieldInstruction Show(UIPanel target)
        {
            TryCreateInstance();
            return _Controller.Show(target.transform);
        }

        public static YieldInstruction Hide()
        {
            if (_Controller == null)
                return new YieldInstruction();

            return _Controller.Hide();
        }

        private class Controller : MonoBehaviour
        {
            public Image Image;
            public CanvasGroup Group;
            public RectTransform RTransform;

            private void Awake()
            {
                Image = gameObject.AddComponent<Image>();
                Group = gameObject.AddComponent<CanvasGroup>();
                RTransform = GetComponent<RectTransform>();

                Group.alpha = 0;
                
                RTransform.anchorMin = new Vector2(0, 0);
                RTransform.anchorMax = new Vector2(1, 1);

                DontDestroyOnLoad(gameObject);
            }

            public YieldInstruction Show(Transform target)
            {
                Image.color = UISettings.PopupBackgroundColor;
                SetTarget(target);

                return Group.DOFade(1.0f, UISettings.PopupTransitionDuration)
                    .WaitForCompletion();
            }

            public YieldInstruction Hide()
            {
                return Group.DOFade(0.0f, UISettings.PopupTransitionDuration)
                    .OnComplete(() => 
                    {
                        transform.SetParent(null);
                        DontDestroyOnLoad(gameObject);
                    })
                    .WaitForCompletion();
            }

            private void SetTarget(Transform t)
            {
                int index = t.GetSiblingIndex();
                transform.SetParent(t.parent);
                transform.SetSiblingIndex(index);

                RTransform.offsetMin = new Vector2(0, 0);
                RTransform.offsetMax = new Vector2(1, 1);
            }
        }
    }
}
