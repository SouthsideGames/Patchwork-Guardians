using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;

namespace MonsterBattleArena.Monster
{
    public class MonsterRig : MonoBehaviour
    {
        /*
        *   TODO: 
        *       - Create proper animation handler for more complex animations
        */

        [SerializeField] private SpriteLibrary _spriteLibrary;
        [SerializeField] private SortingGroup _sortingGroup;
        [SerializeField] private Animator _animator;

        private System.Action _onAttackCompleted;
        private System.Action _onDealDamage;

        public Bounds GetBounds()
        {
            Bounds bounds = new Bounds();

            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }

        /// <summary>
        /// Updates the displayed sprites on the rig
        /// </summary>
        /// <param name="monsterData">source</param>
        public void UpdateMonsterPart(MonsterData monsterData)
        {
            ApplyOverride(monsterData, MonsterPartType.Head);
            ApplyOverride(monsterData, MonsterPartType.Body);
            ApplyOverride(monsterData, MonsterPartType.LeftArm);
            ApplyOverride(monsterData, MonsterPartType.RightArm);
            ApplyOverride(monsterData, MonsterPartType.LeftLeg);
            ApplyOverride(monsterData, MonsterPartType.RightLeg);
        }

        /// <summary>
        /// Set the sorting order to 1
        /// </summary>
        public void SetToForeground()
        {
            _sortingGroup.sortingOrder = 1;
        }
        
        /// <summary>
        /// Resets the sorting order to 0
        /// </summary>
        public void ResetSortingOrder()
        {
            _sortingGroup.sortingOrder = 0;
        }

        /// <summary>
        /// Apply the sprite override for the monster part
        /// </summary>
        /// <param name="part"></param>
        private void ApplyOverride(MonsterData monsterData, MonsterPartType partType)
        {
            MonsterData.PartData? part = monsterData?.GetMonsterPart(partType);
            string partSlot = ResolvePartType(partType);

            if (part == null || part?.Id == null)
            {
                _spriteLibrary.RemoveOverride(partSlot, "Entry");
                return;
            }

            MonsterPart monsterPart = ResourceDatabase.Load<MonsterPart>(part?.Id);

            // NOTE: Label can be used for more variations in the future
            // TODO: Compare combined and individual spriteLib performance 
            //      see Assets/Arts/Sprites/DefaultSpriteLib which as smaller size
            _spriteLibrary.AddOverride(monsterPart.Sprite, partSlot, "Entry");
        }

        private string ResolvePartType(MonsterPartType type)
        {
            switch (type)
            {
                case MonsterPartType.Head:
                    return "head";
                case MonsterPartType.Body:
                    return "body";
                case MonsterPartType.LeftArm:
                    return "leftArm";
                case MonsterPartType.RightArm:
                    return "rightArm";
                case MonsterPartType.LeftLeg:
                    return "leftLeg";
                case MonsterPartType.RightLeg:
                    return "rightLeg";
                default:
                    return "NULL";
            }
        }
    
        public void TriggerAttackAnimation(System.Action onAttackCompleted, System.Action onDealDamage)
        {
            _onDealDamage = onDealDamage;
            _onAttackCompleted = onAttackCompleted;
            _animator.SetTrigger("Attack");
        }

        public void TriggerDeathAnimation()
        {
            _animator.SetTrigger("Death");
        }

        public void OnAttackCompleted()
        {
            _onAttackCompleted?.Invoke();
            _onAttackCompleted = null;
        }
    
        public void OnDealDamage()
        {
            _onDealDamage?.Invoke();
            _onDealDamage = null;
        }
    }
}
