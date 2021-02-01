using UnityEngine;
using UnityEngine.Animations;

namespace MageSimulator.Fields.Start.Scripts
{
    public class CrossSceneConstraint : MonoBehaviour
    {
        public ParentConstraint follower;
        public GuidReference target;
        public Vector3 positionOffset;
        public Vector3 rotationOffset;

        private void Start()
        {
            for (var i = 0; i < follower.sourceCount; i++)
                follower.RemoveSource(i);
            follower.AddSource(new ConstraintSource {sourceTransform = target.gameObject.transform, weight = 1f});
            follower.SetRotationOffset(0, rotationOffset);
            follower.SetTranslationOffset(0, positionOffset);
        }
    }
}