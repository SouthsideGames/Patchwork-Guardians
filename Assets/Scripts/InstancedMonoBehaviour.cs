using UnityEngine;

namespace MonsterBattleArena
{
    public abstract class InstancedMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _currentInstance;
        public static T Current => _currentInstance;

        protected virtual void Awake()
        {
            if (_currentInstance != null)
            {
                Debug.LogWarning("Multiple instance of " + typeof(T) + " found.");
                Destroy(this.gameObject);
                return;
            }

            _currentInstance = gameObject.GetComponent<T>();
        }

        protected virtual void OnDestroy()
        {
            _currentInstance = null;
        }
    }
}
