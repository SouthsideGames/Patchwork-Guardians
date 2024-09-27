using System.Collections.Generic;
using UnityEngine;

namespace MonsterBattleArena
{
    public class ResourceDatabase
    {
        /*
        *   Manifest structure
        *   id;path;subtype
        *
        *   The object type hash is stored in the beggining of the id
        *   
        */

        private const string RESOURCE_MANIFEST_PATH = "manifest";
        private static Dictionary<string, ResourceDBWrapper> Db;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            new ResourceDatabase();
        }

        private ResourceDatabase()
        {
            Db = new Dictionary<string, ResourceDBWrapper>();

            int loaded = 0;
            TextAsset textAsset = Resources.Load<TextAsset>(RESOURCE_MANIFEST_PATH);
            if (textAsset == null)
            {
                Debug.LogError("Unable to find resource manifest! Please go to Tools/Update Resource Manifest to generate a new one.");
            }
            
            foreach (string entry in textAsset.text.Split('\n', '\r'))
            {
                if (string.IsNullOrEmpty(entry))
                    continue;

                string[] values = entry.Split(';');
                if (!IsResourcePathValid(values[1]))
                {
                    Debug.LogWarningFormat("Manifest entries contains invalid path: \"{0}\". The entry is removed. Please make sure the resource manifest is updated.", values[0]);
                    continue;
                }

                Db.Add(values[0], new ResourceDBWrapper
                {
                    Path = values[1],
                    MatchHash = System.Convert.ToInt32(values[2])
                });

                loaded++;
            }

            Debug.LogFormat("Loaded {0} resources.", loaded);
        }

        /// <summary>
        /// Load resource with guid
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T Load<T>(string id) where T : GameResource
        {
            if (string.IsNullOrEmpty(id))
                return null;

            string resId = ResourceGUIDUtility.GetIDFromGUID(id);
            
            if (!Db.ContainsKey(resId))
                Debug.LogError("Unable to find resource with id: " + resId + ". Please make sure the resource manifest is updated.");

            if (Db.TryGetValue(resId, out ResourceDBWrapper dbWrapper))
            {
                return Resources.Load<T>(dbWrapper.Path);
            }

            return null;
        }

        /// <summary>
        /// Load all resources of type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subType">the subType for filter</param>
        /// <returns></returns>
        public static IEnumerable<T> LoadAll<T>(object subType = null) where T : GameResource
        {
            int hash = GameResourceUtility.GetResourceTypeHash<T>();
            int typeHash = subType == null ? -1 : GameResourceUtility.HashFromString(subType.ToString());

            foreach (KeyValuePair<string, ResourceDBWrapper> kvp in Db)
            {
                int resHash = ResourceGUIDUtility.GetTypeHashFromGUID(kvp.Key);
                if (resHash != hash)
                    continue;

                if (typeHash != -1)
                {
                    if (typeHash == kvp.Value.MatchHash)
                        yield return Resources.Load<T>(kvp.Value.Path);
                }
                else
                {
                    yield return Resources.Load<T>(kvp.Value.Path);
                }
            }
        }

        private bool IsResourcePathValid(string path)
        {
            Object res = Resources.Load(path);
            bool valid = res != null;
            Resources.UnloadAsset(res);
            return valid;
        }

        private struct ResourceDBWrapper
        {
            public string Path;
            public int MatchHash;
        }
    }
}
