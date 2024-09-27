using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBattleArena
{
    public static class Backstack
    {
        private static Stack<Action> BackstackBuffer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeOnLoad()
        {
            if (BackstackBuffer != null)
            {
                Debug.Log("Cleared " + BackstackBuffer.Count + " backstack(s) from the buffer.");
            }

            BackstackBuffer = new Stack<Action>();
            Debug.Log("Backstack buffer initialized.");
        }

        public static void RegisterBackstack(Action backstack)
        {
            if (backstack == null)
            {
                Debug.Log("Cannot register backstack because the value is null");
                return;
            }

            BackstackBuffer.Push(backstack);
        }

        public static bool PopBackstack()
        {
            if (BackstackBuffer.Count <= 0)
                return false;
                
            BackstackBuffer.Pop().Invoke();
            return true;
        }

        public static void ClearBackstack()
        {
            BackstackBuffer.Clear();
        }
    }
}
