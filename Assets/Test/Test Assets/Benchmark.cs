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

    bool HighQuality {
        get { return _mode != 2; }
    }

    void Start()
    {
        ReplaceComponents();
    }

    void Update()
    {
        if (_frameCount < 200)
        {
            _frameCount++;

            if (_frameCount == 0)
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
            "Quality: " + HighQuality + "\n" +
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
            bloom.highQuality = HighQuality;
            bloom.intensity = 0;
        }

        _frameCount = -2;
    }
}
