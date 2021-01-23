#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MageSimulator.Utils.Scripts
{
    [CustomEditor(typeof(RandomValueGenerator))]
    public class RandomValueGeneratorEditor : Editor
    {
        private RandomValueGenerator _target;

        private void Awake()
        {
            _target = target as RandomValueGenerator;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            _target.type = (RandomValueGenerator.GenerateType)EditorGUILayout.EnumPopup("生成方法", _target.type);

            _target.min = EditorGUILayout.FloatField("最小値", _target.min);
            _target.max = EditorGUILayout.FloatField("最大値", _target.max);

            switch (_target.type)
            {
                case RandomValueGenerator.GenerateType.Velocity:
                    _target.speed = EditorGUILayout.FloatField("速度", _target.speed);
                    break;

                case RandomValueGenerator.GenerateType.Acceleration:
                    _target.accel = EditorGUILayout.FloatField("加速度", _target.accel);
                    _target.maxSpeed = EditorGUILayout.FloatField("速度の最大値", _target.maxSpeed);
                    break;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("onGenerate"), new GUIContent("生成時の動作"));
            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(_target);
        }
    }
}

#endif