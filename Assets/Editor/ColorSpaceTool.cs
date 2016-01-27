using UnityEngine;
using UnityEditor;

public class ColorSpaceTool
{
    [MenuItem("Test/Switch Color Space")]
    static void SwitchColorSpace()
    {
        PlayerSettings.colorSpace =
            PlayerSettings.colorSpace == ColorSpace.Gamma ?
            ColorSpace.Linear : ColorSpace.Gamma;

        // Force to reload assets.
        EditorApplication.SaveAssets();
    }
}
