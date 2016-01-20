using UnityEngine;
using UnityEditor;

namespace SkyboxPlus
{
    public class GradientsMaterialEditor : ShaderGUI
    {
        MaterialProperty _baseColor;
        MaterialProperty _exposure;

        MaterialProperty _switch2;
        MaterialProperty _switch3;
        MaterialProperty _switch4;

        MaterialProperty _direction1;
        MaterialProperty _direction2;
        MaterialProperty _direction3;
        MaterialProperty _direction4;

        MaterialProperty _color1;
        MaterialProperty _color2;
        MaterialProperty _color3;
        MaterialProperty _color4;

        MaterialProperty _exponent1;
        MaterialProperty _exponent2;
        MaterialProperty _exponent3;
        MaterialProperty _exponent4;

        bool _initial = true;

        void FindProperties(MaterialProperty[] props)
        {
            _baseColor = FindProperty("_BaseColor", props);
            _exposure = FindProperty("_Exposure", props);

            _switch2 = FindProperty("_Switch2", props);
            _switch3 = FindProperty("_Switch3", props);
            _switch4 = FindProperty("_Switch4", props);

            _direction1 = FindProperty("_Direction1", props);
            _direction2 = FindProperty("_Direction2", props);
            _direction3 = FindProperty("_Direction3", props);
            _direction4 = FindProperty("_Direction4", props);

            _color1 = FindProperty("_Color1", props);
            _color2 = FindProperty("_Color2", props);
            _color3 = FindProperty("_Color3", props);
            _color4 = FindProperty("_Color4", props);

            _exponent1 = FindProperty("_Exponent1", props);
            _exponent2 = FindProperty("_Exponent2", props);
            _exponent3 = FindProperty("_Exponent3", props);
            _exponent4 = FindProperty("_Exponent4", props);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            FindProperties(properties);
            if (ShaderPropertiesGUI(materialEditor) || _initial)
                foreach (Material m in materialEditor.targets)
                    UpdateMaterial(m);
            _initial = false;
        }

        bool ShaderPropertiesGUI(MaterialEditor materialEditor)
        {
            EditorGUI.BeginChangeCheck();

            materialEditor.ShaderProperty(_baseColor, "Base Color");
            materialEditor.ShaderProperty(_exposure, "Exposure");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Gradient 1");
            Vector3Property(materialEditor, _direction1, "Direction");
            materialEditor.ShaderProperty(_color1, "Color");
            materialEditor.ShaderProperty(_exponent1, "Exponent");
            EditorGUILayout.Space();

            materialEditor.ShaderProperty(_switch2, "Gradient 2");
            if (_switch2.floatValue > 0)
            {
                Vector3Property(materialEditor, _direction2, "Direction");
                materialEditor.ShaderProperty(_color2, "Color");
                materialEditor.ShaderProperty(_exponent2, "Exponent");
                EditorGUILayout.Space();
            }

            materialEditor.ShaderProperty(_switch3, "Gradient 3");
            if (_switch3.floatValue > 0)
            {
                Vector3Property(materialEditor, _direction3, "Direction");
                materialEditor.ShaderProperty(_color3, "Color");
                materialEditor.ShaderProperty(_exponent3, "Exponent");
                EditorGUILayout.Space();
            }

            materialEditor.ShaderProperty(_switch4, "Gradient 4");
            if (_switch4.floatValue > 0)
            {
                Vector3Property(materialEditor, _direction4, "Direction");
                materialEditor.ShaderProperty(_color4, "Color");
                materialEditor.ShaderProperty(_exponent4, "Exponent");
            }

            return EditorGUI.EndChangeCheck();
        }

        void Vector3Property(MaterialEditor materialEditor, MaterialProperty prop, string label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            var newValue = EditorGUILayout.Vector3Field(label, prop.vectorValue);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck()) prop.vectorValue = newValue;
        }

        void UpdateMaterial(Material m)
        {
            Vector3 d1 = m.GetVector("_Direction1");
            Vector3 d2 = m.GetVector("_Direction2");
            Vector3 d3 = m.GetVector("_Direction3");
            Vector3 d4 = m.GetVector("_Direction4");
            m.SetVector("_NormalizedVector1", d1.normalized);
            m.SetVector("_NormalizedVector2", d2.normalized);
            m.SetVector("_NormalizedVector3", d3.normalized);
            m.SetVector("_NormalizedVector4", d4.normalized);
        }
    }
}
