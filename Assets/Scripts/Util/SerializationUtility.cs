using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace MonsterBattleArena.Util
{
    internal static class SerializationUtility
    {
        /// <summary>
        /// Serialize an object to a file
        /// </summary>
        /// <param name="o">target object</param>
        /// <param name="file">file name</param>
        /// <param name="path">relative path</param>
        public static void Serialize(object o, string file, string path)
        {
            string json = JsonConvert.SerializeObject(o, Formatting.Indented, new JsonSerializerSettings {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            File.WriteAllText(GetPath(file, path), json);
        }

        /// <summary>
        /// Deserialize an object from a file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file">file name</param>
        /// <param name="path">relative path</param>
        /// <returns></returns>
        public static T Deserialize<T>(string file, string path)
        {
            string json = File.ReadAllText(GetPath(file, path));
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static bool IsFileExists(string file, string path)
        {
            return File.Exists(GetPath(file, path));
        }

        private static string GetPath(string file, string path)
        {
            string dir = Path.Combine(Application.persistentDataPath, path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return Path.Combine(dir, file);
        }
    }
}