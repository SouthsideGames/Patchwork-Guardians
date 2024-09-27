namespace MonsterBattleArena
{
    public static class GameResourceUtility
    {
        /// <summary>
        /// Get the asset's unique hash
        /// </summary>
        /// <returns></returns>
        public static int GetResourceTypeHash<T>() where T : GameResource
        {
            return GetTypeHash(typeof(T));
        }

        /// <summary>
        /// Get the asset's unique hash
        /// </summary>
        /// <returns></returns>
        public static int GetTypeHash(System.Type type)
        {
            string str = type.ToString();
            return HashFromString(str);
        }

        // TODO: This is not 100% unique
        public static int HashFromString(string typeString)
        {
            int h = 0;
            for (int i = 0; i < typeString.Length; i++)
            {
                char c = typeString[i];
                bool isHex = (c >= '0' && c <= 9) || 
                    (c >= 'a' && c <= 'f') ||
                    (c >= 'A' && c <= 'F');

                h += isHex ? System.Convert.ToInt32(c) : 1;
            }

            return h;
        }
    }
}