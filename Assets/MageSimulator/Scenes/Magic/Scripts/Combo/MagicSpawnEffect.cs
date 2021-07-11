using MageSimulator.Combo.Effects;
using UnityEngine;

namespace MageSimulator.Scenes.Magic.Scripts.Combo
{
    public class MagicSpawnEffect : ComboEffect
    {
        public Vector3 position;
        public Quaternion rotation;
        public GameObject magic;

        protected override void Initialize()
        {
            Instantiate(magic, position, rotation);
        }
    }
}