using System.Collections.Generic;
using System.IO;
using MonsterBattleArena.Monster;
using UnityEditor;
using UnityEngine;

namespace MonsterBattleArena.Editor
{
    public class ResourceManifestUpdater
    {
        /*
        *   TODO: Use automated asset processor to update the manifest
        *   NOTE: This will only scans for assets located under Resources/Monsters/{monster name}
        */

        [MenuItem("Tools/Update Resource Manifest")]
        public static void UpdateResourceManifest()
        {
            List<string> resources = new List<string>();

            DirectoryInfo resFolder = new DirectoryInfo(Path.Combine(Application.dataPath, "Resources"));
            foreach (DirectoryInfo dir in resFolder.GetDirectories("*.*", SearchOption.AllDirectories))
            {
                foreach (FileInfo file in dir.GetFiles("*.asset"))
                {
                    string relPath = GetRelativePath(file.FullName);
                    GameResource asset = AssetDatabase.LoadAssetAtPath<GameResource>(relPath);
                    if (asset != null)
                    {
                        string path = GetResourcesPath(relPath);
                        string entry;
                        if (asset is MonsterPart monsterPart)
                        {
                            int partTypeHash = GameResourceUtility.HashFromString(monsterPart.PartType.ToString());
                            entry = string.Format("{0};{1};{2}", asset.Id, path, partTypeHash);
                        }
                        else
                        {
                            entry = string.Format("{0};{1};{2}", asset.Id, path, -1);
                        }

                        resources.Add(entry);
                    }
                }
            }

            File.WriteAllLines(resFolder.FullName + "/manifest.txt", resources);
            AssetDatabase.Refresh();
        }

        private static string GetRelativePath(string path)
        {
            int index = path.IndexOf("Assets");
            return path.Substring(index, path.Length - index);
        }

        private static string GetResourcesPath(string path)
        {
            return path.Replace("Assets\\Resources\\", "").Replace(".asset", "");
        }
    }
}

