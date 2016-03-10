//
// Kino/Bloom v2 - Bloom filter for Unity
//
// Copyright (C) 2015, 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEngine;
using UnityEditor;

namespace Kino
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Bloom))]
    public class BloomEditor : Editor
    {
        SerializedProperty _threshold;
        SerializedProperty _softKnee;
        SerializedProperty _radius;
        SerializedProperty _intensity;
        SerializedProperty _highQuality;
        SerializedProperty _antiFlicker;

        const int _curveResolution = 128;
        Vector3[] _rectVertices;
        Vector3[] _curveVertices;
        Vector3[] _lineVertices;

        void OnEnable()
        {
            _threshold = serializedObject.FindProperty("_threshold");
            _softKnee = serializedObject.FindProperty("_softKnee");
            _radius = serializedObject.FindProperty("_radius");
            _intensity = serializedObject.FindProperty("_intensity");
            _highQuality = serializedObject.FindProperty("_highQuality");
            _antiFlicker = serializedObject.FindProperty("_antiFlicker");

            _rectVertices = new Vector3[4];
            _curveVertices = new Vector3[_curveResolution];
            _lineVertices = new Vector3[2];
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (!serializedObject.isEditingMultipleObjects) {
                EditorGUILayout.Space();
                DrawGraph((Bloom)target);
                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(_threshold);
            EditorGUILayout.PropertyField(_softKnee);
            EditorGUILayout.PropertyField(_radius);
            EditorGUILayout.PropertyField(_intensity);
            EditorGUILayout.PropertyField(_highQuality);
            EditorGUILayout.PropertyField(_antiFlicker);

            serializedObject.ApplyModifiedProperties();
        }

        void DrawGraph(Bloom bloom)
        {
            var rect = GUILayoutUtility.GetRect(128, 64+16);

            // Parameters
            var threshold = bloom.threshold;
            var knee = threshold * bloom.softKnee;
            var param1 = 0.25f / Mathf.Max(knee, 1e-5f);
            var param2 = threshold - knee;

            // Background
            _rectVertices[0] = new Vector3(rect.x, rect.y);
            _rectVertices[1] = new Vector3(rect.xMax, rect.y);
            _rectVertices[2] = new Vector3(rect.xMax, rect.yMax);
            _rectVertices[3] = new Vector3(rect.x, rect.yMax);

            Handles.DrawSolidRectangleWithOutline(
                _rectVertices, Color.white * 0.1f, Color.white * 0.4f
            );

            // Knee range
            _rectVertices[0] = PointInRect(rect, threshold - knee, 0);
            _rectVertices[1] = PointInRect(rect, threshold + knee, 0);
            _rectVertices[2] = PointInRect(rect, threshold + knee, 2);
            _rectVertices[3] = PointInRect(rect, threshold - knee, 2);

            Handles.DrawSolidRectangleWithOutline(
                _rectVertices, Color.white * 0.25f, Color.clear
            );

            // Vertical lines
            Handles.color = Color.white * 0.4f;
            for (var i = 1; i < 5; i++)
            {
                _lineVertices[0] = PointInRect(rect, i, 0);
                _lineVertices[1] = PointInRect(rect, i, 2);
                Handles.DrawAAPolyLine(2.0f, _lineVertices);
            }

                _lineVertices[0] = PointInRect(rect, 0, 1);
                _lineVertices[1] = PointInRect(rect, 5, 1);
                Handles.DrawAAPolyLine(2.0f, _lineVertices);

            // Threshold line
            _lineVertices[0] = PointInRect(rect, threshold, 0);
            _lineVertices[1] = PointInRect(rect, threshold, 2);
            Handles.color = Color.white * 0.8f;
            Handles.DrawAAPolyLine(2.0f, _lineVertices);

            Handles.color = Color.white * 0.9f;

            for (var i = 0; i < _curveResolution; i++)
            {
                var x = 5.0f * i / (_curveResolution - 1);
                var y1 = Mathf.Max(0, x - threshold);
                var y2 = x - param2;
                y2 = param1 * y2 * y2;

                var y = Mathf.Abs(x - threshold) < knee ? y2 : y1;
                y *= bloom.intensity;
                if (y > 1.5f)
                _curveVertices[i] = _curveVertices[i - 1];
                    else
                _curveVertices[i] = PointInRect(rect, x, y);
            }

            Handles.DrawAAPolyLine(2.0f, _curveVertices);
        }

        Vector3 PointInRect(Rect rect, float x, float y)
        {
            x = Mathf.Lerp(rect.x, rect.xMax, x / 5);
            y = Mathf.Lerp(rect.y, rect.yMax, 1 - y / 1.5f);
            return new Vector3(x, y, 1);
        }
    }
}
