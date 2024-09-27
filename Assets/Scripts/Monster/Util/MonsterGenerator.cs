using System.Collections.Generic;
using System.Linq;
using MonsterBattleArena.Monster;
using UnityEngine;

namespace MonsterBattleArena.Util
{
    public class MonsterGenerator
    {
        /*
        *   NOTE: 
        *       GetRandomPart(MonsterPartType) can cause performance issue if we have a lot of mosnter parts
        *       since we're operating with index on an enumerable
        */

        /// <summary>
        /// Generate monster with random attributes
        /// </summary>
        /// <returns></returns>
        public static MonsterData GenerateRandom()
        {
            MonsterData monsterData = new MonsterData(RandomNameGenerator.GetMonsterName());

            SetMonsterPart(monsterData, MonsterPartType.Head);
            SetMonsterPart(monsterData, MonsterPartType.Body);
            SetMonsterPart(monsterData, MonsterPartType.LeftArm);
            SetMonsterPart(monsterData, MonsterPartType.RightArm);
            SetMonsterPart(monsterData, MonsterPartType.LeftLeg);
            SetMonsterPart(monsterData, MonsterPartType.RightLeg);

            return monsterData;
        }

        private static MonsterPart GetRandomPart(MonsterPartType type)
        {
            IEnumerable<MonsterPart> parts = ResourceDatabase.LoadAll<MonsterPart>(type);
            int index = Random.Range(0, parts.Count());

            return parts.ElementAt(index);
        }

        private static void SetMonsterPart(MonsterData monsterData, MonsterPartType type)
        {
            MonsterPart part = GetRandomPart(type);
            int level = Random.Range(0, part.TotalLevel);

            monsterData.SetMonsterPart(part, level);
        }
    }
}
