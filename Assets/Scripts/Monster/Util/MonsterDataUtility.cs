using System.Collections.Generic;

namespace MonsterBattleArena.Monster
{
    public static class MonsterDataUtility
    {
        public static IEnumerable<MonsterPart> GetMonsterPartAssets(MonsterData monsterData)
        {
            foreach (MonsterData.PartData partData in monsterData.EnumerateParts)
            {
                if (string.IsNullOrEmpty(partData.Id))
                    continue;

                yield return ResourceDatabase.Load<MonsterPart>(partData.Id);
            }
        }

        public static int GetTotalAttributeValue(MonsterData monsterData, AttributeType attributeType)
        {
            int total = 0;

            foreach (MonsterData.PartData part in monsterData.EnumerateParts)
            {
                if (string.IsNullOrEmpty(part.Id))
                    continue;

                MonsterPart asset = ResourceDatabase.Load<MonsterPart>(part.Id);
                foreach (MonsterAttribute attr in asset.GetAttributes(part.Level))
                {
                    if (attr.AttributeType == attributeType)
                    {
                        total += attr.Value;
                    }
                }
            }

            return total;
        }

        public static IEnumerable<MonsterAbility> GetAbilities(MonsterData mosnterData)
        {
            foreach (MonsterPart part in GetMonsterPartAssets(mosnterData))
            {
                yield return part.Ability;
            }
        }

        public static IEnumerable<(Synergy, int)> GetActiveSynergies(MonsterData monsterData)
        {
            HashSet<(Synergy, int)> actives = new HashSet<(Synergy, int)>();

            foreach (string syn in GetAvailableSynergies(monsterData))
            {
                Synergy synergy = ResourceDatabase.Load<Synergy>(syn);
                if (synergy.SynergyCount <= 0)
                {
                    actives.Add((synergy, -1));
                    continue;
                }

                /* 
                *   NOTE:
                *       Start from behind, since the last index of the synergy would check
                *       if all body part has the same synergy.
                *
                *       If all the body part have the same synergy, then we can exit the loops 
                *       since there's only 1 possible synergy, we don't need to check further.
                */ 
                bool shouldBreak = false;
                for (int i = synergy.SynergyCount - 1; i >= 0; i--)
                {
                    int index = i;
                    Synergy.SynergyWrapper wrapper = synergy.Synergies[index];

                    bool active = true;
                    if (wrapper.RequiredParts.Length > 0)
                    {
                        foreach (MonsterPartType partType in wrapper.RequiredParts)
                        {
                            MonsterPart asset = ResourceDatabase.Load<MonsterPart>(monsterData.GetMonsterPart(partType).Id);
                            if (asset.Synergy.Id != synergy.Id)
                            {
                                active = false;
                                break;
                            }
                        }
                    }

                    if (active)
                    {
                        actives.Add((synergy, index));

                        /*
                        *   The last tier of synergy is active
                        *   We can break out of the loops
                        */
                        if (index == synergy.SynergyCount - 1)
                        {
                            shouldBreak = true;
                        }
                    }
                }

                if (shouldBreak)
                    break;
            }

            return actives;
        }

        private static IEnumerable<string> GetAvailableSynergies(MonsterData monsterData)
        {
            HashSet<string> synergies = new HashSet<string>();
            foreach (MonsterPart part in GetMonsterPartAssets(monsterData))
            {
                synergies.Add(part.Synergy.Id);
            }

            return synergies;
        }
    }
}
