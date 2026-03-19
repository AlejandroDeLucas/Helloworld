using System.Collections.Generic;
using TinyHunter.Data.Monsters;
using UnityEngine;

namespace TinyHunter.Core.Combat
{
    public class MonsterPartBreakSystem : MonoBehaviour
    {
        [System.Serializable]
        private class PartState
        {
            public MonsterPartDefinition Definition;
            public float CurrentDurability;
            public bool IsBroken;
        }

        [SerializeField] private MonsterDefinition monsterDefinition;
        [SerializeField] private bool debugPartBreakLogs = true;
        private readonly Dictionary<string, PartState> partStates = new();

        public delegate void PartBrokenHandler(MonsterPartDefinition part);
        public event PartBrokenHandler OnPartBroken;

        private void Awake()
        {
            foreach (var part in monsterDefinition.BreakableParts)
            {
                partStates[part.PartId] = new PartState
                {
                    Definition = part,
                    CurrentDurability = part.MaxDurability,
                    IsBroken = false
                };
            }
        }

        public void ApplyPartDamage(string partId, float damage)
        {
            if (!partStates.TryGetValue(partId, out var partState) || partState.IsBroken)
            {
                return;
            }

            partState.CurrentDurability -= damage * partState.Definition.BreakDamageMultiplier;
            if (partState.CurrentDurability > 0f) return;

            partState.IsBroken = true;
            if (debugPartBreakLogs)
            {
                Debug.Log($"Part broken: {partState.Definition.DisplayName} ({partState.Definition.PartId})");
            }

            OnPartBroken?.Invoke(partState.Definition);
        }

        public bool HasPart(string partId) => partStates.ContainsKey(partId);
    }
}
