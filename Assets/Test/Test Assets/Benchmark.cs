using UnityEngine;
using Kino;

public class Benchmark : MonoBehaviour
{
    [SerializeField] int _iteration = 20;

    int _mode;
    float _timeStart;
    float _averageTime;
    int _frameCount;

    bool AntiFlicker {
        get { return _mode == 0; }
    }

    Bloom.QualityLevel Quality {
        get { return (Bloom.QualityLevel)Mathf.Min(2, 3 - _mode); }
    }

    void Start()
    {
        ReplaceComponents();
    }

    void Update()
    {
        if (_frameCount < 100)
        {
            _frameCount++;

            if (_frameCount == 2)
                _timeStart = Time.time;
            else
                _averageTime = (Time.time - _timeStart) * 1000 / _frameCount;
        }

        if (Input.GetMouseButtonDown(0))
        {
            _mode = (_mode + 1) % 4;
            ReplaceComponents();
        }
    }

    void OnGUI()
    {
        GUI.Label(
            new Rect(0, 0, Screen.width, Screen.height),
            "Click the screen to switch test mode.\n" +
            "Screen Resolution: " + Screen.currentResolution + "\n" +
            "Quality: " + Quality + "\n" +
            "Anti Flicker: " + AntiFlicker + "\n" +
            "Average Time: " + _averageTime + " ms\n" +
            "Frame Count: " + _frameCount);
    }

    void ReplaceComponents()
    {
        var camera = Camera.main.gameObject;

        foreach (var c in camera.GetComponents<Bloom>())
            Destroy(c);

        for (var i = 0; i < _iteration; i++)
        {
            var bloom = camera.AddComponent<Bloom>();
            bloom.antiFlicker = AntiFlicker;
            bloom.quality = Quality;
        }

        _frameCount = 0;
    }
}
