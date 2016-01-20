using UnityEngine;
using UnityEditor;

namespace SkyboxPlus
{
    public class CubemapMaterialEditor : ShaderGUI
    {
        MaterialProperty _cubemap;
        MaterialProperty _tint;
        MaterialProperty _euler;
        MaterialProperty _exposure;
        MaterialProperty _saturation;
        MaterialProperty _lod;
        MaterialProperty _lodLevel;

        static GUIContent _textCubemap = new GUIContent("Cubemap");

        bool _initial = true;

        void FindProperties(MaterialProperty[] props)
        {
            _cubemap = FindProperty("_Tex", props);
            _tint = FindProperty("_Tint", props);
            _euler = FindProperty("_Euler", props);
            _exposure = FindProperty("_Exposure", props);
            _saturation = FindProperty("_Saturation", props);
            _lod = FindProperty("_Lod", props);
            _lodLevel = FindProperty("_LodLevel", props);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            FindProperties(properties);
            if (ShaderPropertiesGUI(materialEditor) || _initial)
                foreach (Material m in materialEditor.targets)
                    SetMatrix(m);
            _initial = false;
        }

        bool ShaderPropertiesGUI(MaterialEditor materialEditor)
        {
            EditorGUI.BeginChangeCheck();

            materialEditor.TexturePropertySingleLine(_textCubemap, _cubemap, _tint);
            Vector3Property(materialEditor, _euler, "Rotation");
            materialEditor.ShaderProperty(_exposure, "Exposure");
            materialEditor.ShaderProperty(_saturation, "Saturation");

            materialEditor.ShaderProperty(_lod, "Specify MIP Level");
            if (_lod.hasMixedValue || _lod.floatValue > 0)
            {
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(_lodLevel, "Level");
                EditorGUI.indentLevel--;
            }

            return EditorGUI.EndChangeCheck();
        }

        static void SetMatrix(Material material)
        {
            var r = material.GetVector("_Euler");
            var q = Quaternion.Euler(r.x, r.y, r.z);
            var m = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
            material.SetVector("_Rotation1", m.GetRow(0));
            material.SetVector("_Rotation2", m.GetRow(1));
            material.SetVector("_Rotation3", m.GetRow(2));
        }

        void Vector3Property(MaterialEditor materialEditor, MaterialProperty prop, string label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            var newValue = EditorGUILayout.Vector3Field(label, prop.vectorValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck()) prop.vectorValue = newValue;
        }
    }
}
