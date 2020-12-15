using UnityEngine;

namespace MageSimulator.Scripts.Utils
{
    public class AroundMover : MonoBehaviour
    {
        public Transform target;
        public float speed;
        private float _axisT = 0f;

        private void Update()
        {
            // ターゲットのほうを向く
            var axis = new Vector3(Mathf.Sin(_axisT * 6), Mathf.Sin(_axisT * 2), Mathf.Sin(_axisT));
            transform.RotateAround(target.position, axis, speed * Time.deltaTime);
            transform.LookAt(target);
            _axisT = (_axisT + Mathf.PI / 60 * Time.deltaTime) % (Mathf.PI * 2);
        }
    }
}
