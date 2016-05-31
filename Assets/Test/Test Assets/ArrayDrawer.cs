using UnityEngine;

[ExecuteInEditMode]
public class ArrayDrawer : MonoBehaviour
{
    [Space]
    [SerializeField] int _columns = 10;
    [SerializeField] float _interval = 1;
    [SerializeField] float _cellSize = 0.1f;

    [Space]
    [SerializeField, ColorUsage(false, true, 0, 20, 0.125f, 3)]
    Color _color1 = Color.green;

    [SerializeField, ColorUsage(false, true, 0, 20, 0.125f, 3)]
    Color _color2 = Color.red;

    [Space]
    [SerializeField] float _slideSpeed = 1;
    [SerializeField] float _blinkSpeed = 1;

    [SerializeField, HideInInspector] Mesh _mesh;
    [SerializeField, HideInInspector] Material _material;

    MaterialPropertyBlock _props;

    void Update()
    {
        if (_props == null)
            _props = new MaterialPropertyBlock();

        var dx = Mathf.Sin(_slideSpeed * Time.time) * _interval;
        var br = (Mathf.Cos(_blinkSpeed * Time.time) + 1) * 0.5f;

        var origin = transform.position + new Vector3(
                _interval * _columns * -0.5f + dx,
                _interval * _columns * -0.5f, 0);
        var rotation = transform.rotation;
        var scale = Vector3.one * _cellSize;

        for (var y = 0; y <= _columns; y++)
        {
            for (var x = 0; x <= _columns; x++)
            {
                var position = origin + new Vector3(x, y, 0) * _interval;
                var matrix = Matrix4x4.TRS(position, rotation, scale);

                var c1 = _color1 * (br * x / _columns);
                var c2 = _color2 * (br * y / _columns);
                _props.SetColor("_EmissionColor", c1 + c2);

                Graphics.DrawMesh(_mesh, matrix, _material, 0, null, 0, _props);
            }
        }
    }
}
