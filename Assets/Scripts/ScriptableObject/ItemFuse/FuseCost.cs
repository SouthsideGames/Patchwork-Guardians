using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBattleArena
{
    [System.Serializable]
    public struct FuseCost
    {
        [SerializeField] private GameResource _resource;
        [SerializeField] private int _count;

        public string Resource { get => _resource?.Id; }
        public int Count { get => _count; }
    }
}
