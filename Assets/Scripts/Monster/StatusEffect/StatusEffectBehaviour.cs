namespace MonsterBattleArena.Monster
{
    [System.Serializable]
    public abstract class StatusEffectBehaviour
    {
        protected BattleUnit BattleUnit { get; private set; }

        public void Init(BattleUnit battleUnit)
        {
            BattleUnit = battleUnit;
        }

        /// <summary>
        /// Called once when applied
        /// </summary>
        public virtual void OnApply()
        {}
        
        /// <summary>
        /// Called each round end
        /// </summary>
        public virtual void OnUpdate()
        {}

        /// <summary>
        /// Called once when removed or when the duration ends
        /// </summary>
        public virtual void OnRemove() 
        {}

        /// <summary>
        /// Create copy of this behaviour
        /// </summary>
        /// <returns></returns>
        public abstract StatusEffectBehaviour Copy();
    }
}
