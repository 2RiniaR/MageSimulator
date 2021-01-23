using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace MageSimulator.Utils.Scripts
{
    public class RandomValueGenerator : MonoBehaviour
    {
        public enum GenerateType
        {
            Constant,
            Velocity,
            Acceleration
        }

        public GenerateType type;
        public float min = 0f;
        public float max = 1f;
        public float speed= 1f;
        public float accel = 1f;
        public float maxSpeed = 1f;
        public UnityEvent<float> onGenerate;
        private float _currentConstant = 0f;
        private float _currentVelocity = 0f;

        private void Start()
        {
            _currentConstant = (min + max) / 2f;
        }

        private void Update()
        {
            switch (type)
            {
                case GenerateType.Constant:
                    onGenerate.Invoke(Random.Range(min, max));
                    break;

                case GenerateType.Velocity:
                    _currentConstant += Random.Range(-speed * Time.deltaTime, speed * Time.deltaTime);
                    _currentConstant = Mathf.Clamp(_currentConstant, min, max);
                    onGenerate.Invoke(_currentConstant);
                    break;

                case GenerateType.Acceleration:
                    _currentVelocity += Random.Range(-accel * Time.deltaTime, accel * Time.deltaTime);
                    _currentVelocity = Mathf.Clamp(_currentVelocity, -maxSpeed, maxSpeed);
                    _currentConstant += _currentVelocity * Time.deltaTime;
                    _currentConstant = Mathf.Clamp(_currentConstant, min, max);
                    onGenerate.Invoke(_currentConstant);
                    break;
            }
        }
    }
}
