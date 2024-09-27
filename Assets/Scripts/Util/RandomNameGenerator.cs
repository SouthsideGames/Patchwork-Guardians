using UnityEngine;

namespace MonsterBattleArena.Util
{
    internal class RandomNameGenerator
    {
        private static string[] _names;

        public static string GetMonsterName()
        {
            TryInitializeNames();
            return _names[Random.Range(0, _names.Length)];
        }

        private static void TryInitializeNames()
        {
            if (_names != null)
                return;

            TextAsset asset = Resources.Load<TextAsset>("monster_names");
            _names = asset.text.Split(new char[]{ '\n', '\r'}, System.StringSplitOptions.RemoveEmptyEntries);

            Resources.UnloadAsset(asset);
        }
    }
}
