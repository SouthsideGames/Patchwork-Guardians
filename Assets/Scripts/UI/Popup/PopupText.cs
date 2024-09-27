using UnityEngine;
using UnityEngine.Pool;
using MonsterBattleArena.UI;
using MonsterBattleArena.Monster;
using DG.Tweening;
using TMPro;

namespace MonsterBattleArena
{
    public static class PopupText
    {
        public enum TextType
        {
            Default, Heal, Damage, Critical
        }

        private static Transform PoolContainer;
        private static IObjectPool<TMP_Text> Pool;

        public static void Show(string text, Vector2 worldPos, PopupConfig config)
        {
            TryInitObjectPool();
            
            PopupTextSettings settings = UISettings.PopupText;

            TMP_Text instance = Pool.Get();

            worldPos += Random.insideUnitCircle * config.Radius;
            instance.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-config.MaxAngle, config.MaxAngle));
            instance.SetText(text);
            instance.transform.position = worldPos;

            DoAnimate(instance, config.TextType, config.Delay, settings);
        }

        private static void DoAnimate(TMP_Text target, TextType type, float delay, PopupTextSettings settings)
        {    
            switch (type)
            {
                case TextType.Heal:
                    target.color = settings.PositiveColor;
                    DefaultAnimation(target, delay);
                    break;
                case TextType.Damage:
                case TextType.Critical:
                    target.color = settings.NegativeColor;
                    DefaultAnimation(target, delay);
                    break;
                case TextType.Default:
                    target.color = settings.DefaultColor;
                    DefaultAnimation(target, delay);
                    break;
            }
        }

        private static void DefaultAnimation(TMP_Text target, float delay)
        {
            RectTransform rt = target.rectTransform;
            Sequence sequence = DOTween.Sequence();
            if (delay > 0)
                sequence.AppendInterval(delay);

            sequence.Append(rt.DOLocalMoveY(rt.rect.height * 2, 1.0f).SetEase(Ease.OutCirc));
            sequence.Join(target.DOFade(0, 1.2f).SetEase(Ease.InCubic));
            sequence.OnComplete(() => Pool.Release(target));
        }

        private static void TryInitObjectPool()
        {
            if (PoolContainer == null)
            {
                PoolContainer = new GameObject("PopupTextContainer").transform;
                Object.DontDestroyOnLoad(PoolContainer.gameObject);
            }

            if (Pool == null)
            {
                Pool = new ObjectPool<TMP_Text>(
                    OnCreatePoolItem, 
                    x => x.gameObject.SetActive(true), 
                    OnReleasePoolItem, 
                    x => Object.Destroy(x));   
            }
        }

        private static TMP_Text OnCreatePoolItem()
        {
            PopupTextSettings settings = UISettings.PopupText;
            TMP_Text instance = Object.Instantiate(settings.Template);
            instance.transform.SetParent(PoolContainer);
            return instance;
        }

        private static void OnReleasePoolItem(TMP_Text item)
        {
            item.transform.position = Vector2.zero;
            item.transform.rotation = Quaternion.identity;
            item.color = Color.white;

            item.gameObject.SetActive(false);
        }

        public struct PopupConfig
        {
            public TextType TextType;
            public float Radius;
            public int MaxAngle;
            public float Delay;
        }
    }
}
