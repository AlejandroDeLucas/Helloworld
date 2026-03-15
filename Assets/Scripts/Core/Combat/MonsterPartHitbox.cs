using UnityEngine;

namespace TinyHunter.Core.Combat
{
    public class MonsterPartHitbox : MonoBehaviour
    {
        [SerializeField] private string partId;
        public string PartId => partId;
    }
}
