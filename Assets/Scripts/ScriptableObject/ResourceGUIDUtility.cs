using System.Text.RegularExpressions;
using UnityEngine;

namespace MonsterBattleArena
{
    public static class ResourceGUIDUtility
    {
        private const string REGEX_GUID_PATTERN = "[A-Fa-f0-9]{4}_[A-Fa-f0-9]{8}";
        private const string REGEX_GUID_PATTERN_APPENDED = "[A-Fa-f0-9]{4}_[A-Fa-f0-9]{8}_[A-Fa-f0-9]{2}";
        private const string REGEX_LEVEL_PATTERN = "[A-Fa-f0-9]{2}(?=$)";
        private const string REGEX_TYPE_PATTERN = "^[A-Fa-f0-9]{4}";

        private const int LENGTH_GUID = 13;
        private const int LENGTH_GUID_APPENDED = 16;

        private const string X2 = "X2";
        private const string X4 = "X4";
        private const string X8 = "X8";

        /*
        *   NOTE:
        *       GUID Format
        *
        *       04FA_00005F7E           : Raw asset's guid
        *       04FA_00005F7E_F3        : Assets guid with trailing level index
        *
        *       (0)_{1}_{0}
        *
        *       0: Asset's type
        *       1: Asset's guid
        *       2: Asset's level
        */

        /// <summary>
        /// Create asset guid from instance id
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static string CreateGUID(GameResource asset)
        {
            int guid = asset.GetInstanceID();
            int hash = GameResourceUtility.GetTypeHash(asset.GetType());

            return string.Format("{0}_{1}", hash.ToString(X4), guid.ToString(X8));
        }

        /// <summary>
        /// Append level to guid
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string AppendLevelToGUID(string guid, int level)
        {
            if (IsFormatted(guid, REGEX_GUID_PATTERN_APPENDED, LENGTH_GUID_APPENDED))
            {
                return Regex.Replace(guid, REGEX_LEVEL_PATTERN, level.ToString(X2));
            }

            if (IsFormatted(guid, REGEX_GUID_PATTERN, LENGTH_GUID))
            {
                return string.Format("{0}_{1}", guid, level.ToString(X2));
            }

            Debug.LogError("GUID not well formed!\n" + guid);
            return guid;
        }

        /// <summary>
        /// Get level appended to guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static int GetLevelFromGUID(string guid)
        {
            if (IsFormatted(guid, REGEX_GUID_PATTERN_APPENDED, LENGTH_GUID_APPENDED))
            {
                string hex = Regex.Match(guid, REGEX_LEVEL_PATTERN).Value;
                return System.Convert.ToInt32(hex);
            }
            else if (IsFormatted(guid, REGEX_GUID_PATTERN, LENGTH_GUID))
            {
                return 0;
            }

            Debug.LogError("GUID not well formed!\n" + guid);
            return 0;
        }

        /// <summary>
        /// Get type hash appended to guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static int GetTypeHashFromGUID(string guid)
        {
            if (IsFormatted(guid, REGEX_GUID_PATTERN_APPENDED, LENGTH_GUID_APPENDED) || IsFormatted(guid, REGEX_GUID_PATTERN, LENGTH_GUID))
            {
                string hex = Regex.Match(guid, REGEX_TYPE_PATTERN).Value;
                return System.Convert.ToInt32(hex, 16);
            }

            Debug.LogError("GUID not well formed!\n" + guid);
            return -1;
        }

        /// <summary>
        /// Get asset id from guid
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string GetIDFromGUID(string guid)
        {
            if (IsFormatted(guid, REGEX_GUID_PATTERN_APPENDED, LENGTH_GUID_APPENDED) || IsFormatted(guid, REGEX_GUID_PATTERN, LENGTH_GUID))
            {
                return Regex.Match(guid, REGEX_GUID_PATTERN).Value;
            }

            Debug.LogError("GUID not well formed!\n" + guid);
            return guid;
        }

        private static bool IsFormatted(string guid, string pattern, int patternLength)
        {
            if (string.IsNullOrEmpty(guid))
                return false;

            Match result = Regex.Match(guid, pattern);
            return result.Success && result.Length == patternLength;
        }
    }
}
