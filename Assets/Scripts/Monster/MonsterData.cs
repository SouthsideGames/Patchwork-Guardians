using System.Collections.Generic;
using MonsterBattleArena.Util;
using Newtonsoft.Json;

namespace MonsterBattleArena.Monster
{
    public class MonsterData
    { 
        /*
        *   NOTE: 
        *       - Stores total attribute to simplify query for statistics panel see StatsPanel.cs:145 
        *         and battle calculation 
        */

        [JsonProperty("id")] private string _id;
        [JsonProperty("name")] private string _name;

        [JsonProperty("head")] private string _head;
        [JsonProperty("body")] private string _body;
        [JsonProperty("leftArm")] private string _leftArm;
        [JsonProperty("rightArm")] private string _rightArm;
        [JsonProperty("leftLeg")] private string _leftLeg;
        [JsonProperty("rightLeg")] private string _rightLeg;

        [JsonIgnore] public string Id { get => _id; }
        [JsonIgnore] public string Name { get => _name; set => _name = value; }

        [JsonIgnore]
        public IEnumerable<PartData> EnumerateParts
        {
            get
            {
                yield return GetMonsterPart(MonsterPartType.Head);
                yield return GetMonsterPart(MonsterPartType.Body);
                yield return GetMonsterPart(MonsterPartType.LeftArm);
                yield return GetMonsterPart(MonsterPartType.RightArm);
                yield return GetMonsterPart(MonsterPartType.LeftLeg);
                yield return GetMonsterPart(MonsterPartType.RightLeg);
            }    
        }

        public MonsterData(string name)
        {
            _id = System.Guid.NewGuid().ToString();
            _name = name;
        }

        public void SetMonsterPart(MonsterPart part, int level)
        {
            if (part == null)
                return;

            string id = ResourceGUIDUtility.AppendLevelToGUID(part.Id, level);

            switch (part.PartType)
            {
                case MonsterPartType.Head:
                    _head = id;
                    break;
                case MonsterPartType.Body:
                    _body = id;
                    break;
                case MonsterPartType.LeftArm:
                    _leftArm = id;
                    break;
                case MonsterPartType.RightArm:
                    _rightArm = id;
                    break;
                case MonsterPartType.LeftLeg:
                    _leftLeg = id;
                    break;
                case MonsterPartType.RightLeg:
                    _rightLeg = id;
                    break;
            }
        }
        
        public void RemoveMonsterPart(MonsterPartType partType)
        {
            switch (partType)
            {
                case MonsterPartType.Head:
                    _head = string.Empty;
                    break;
                case MonsterPartType.Body:
                    _body = string.Empty;
                    break;
                case MonsterPartType.LeftArm:
                    _leftArm = string.Empty;
                    break;
                case MonsterPartType.RightArm:
                    _rightArm = string.Empty;
                    break;
                case MonsterPartType.LeftLeg:
                    _leftLeg = string.Empty;
                    break;
                case MonsterPartType.RightLeg:
                    _rightLeg = string.Empty;
                    break;
            }
        }

        public PartData GetMonsterPart(MonsterPartType partType)
        {
            switch (partType)
            {
                case MonsterPartType.Head:
                    return GetPartData(_head);
                case MonsterPartType.Body:
                    return GetPartData(_body);
                case MonsterPartType.LeftArm:
                    return GetPartData(_leftArm);
                case MonsterPartType.RightArm:
                    return GetPartData(_rightArm);
                case MonsterPartType.LeftLeg:
                    return GetPartData(_leftLeg);
                case MonsterPartType.RightLeg:
                    return GetPartData(_rightLeg);
                default:
                    return new PartData(null, -1);
            }
        }

        private PartData GetPartData(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return new PartData(null, 0);
            }

            string id = ResourceGUIDUtility.GetIDFromGUID(guid);
            int level = ResourceGUIDUtility.GetLevelFromGUID(guid);
            return new PartData(id, level);
        }

        public struct PartData
        {
            private string _id;
            private int _level;

            public string Id { get => _id; }
            public int Level { get => _level; }
            public bool IsNull { get => string.IsNullOrEmpty(_id); }

            public PartData(string id, int level)
            {
                _id = id;
                _level = level;
            }
        }
    }
}
