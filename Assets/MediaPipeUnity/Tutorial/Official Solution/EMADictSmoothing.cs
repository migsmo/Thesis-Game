using System;
using System.Collections.Generic;

namespace MediaPipeUnity.Tutorial.Official_Solution
{
    public class EMADictSmoothing
    {
        private int _windowSize;
        private float _alpha;

        private List<Dictionary<string, float>> _dataInWindow;

        public EMADictSmoothing(int windowSize = 10, float alpha = 0.2f)
        {
            _windowSize = windowSize;
            _alpha = alpha;

            _dataInWindow = new List<Dictionary<string, float>>();
        }

        public Dictionary<string, float> Smooth(Dictionary<string, int> data)
        {
            // Add new data to the beginning of the window for simpler code.
            Dictionary<string, float> newData = new Dictionary<string, float>();
            foreach (var item in data)
            {
                newData.Add(item.Key, item.Value);
            }
            _dataInWindow.Insert(0, newData);
            _dataInWindow = _dataInWindow.GetRange(0, Math.Min(_dataInWindow.Count, _windowSize));

            // Get all keys.
            HashSet<string> keys = new HashSet<string>();
            foreach (var windowData in _dataInWindow)
            {
                foreach (var key in windowData.Keys)
                {
                    keys.Add(key);
                }
            }

            // Get smoothed values.
            Dictionary<string, float> smoothedData = new Dictionary<string, float>();
            foreach (var key in keys)
            {
                float factor = 1.0f;
                float topSum = 0.0f;
                float bottomSum = 0.0f;
                foreach (var windowData in _dataInWindow)
                {
                    float value = windowData.ContainsKey(key) ? windowData[key] : 0.0f;

                    topSum += factor * value;
                    bottomSum += factor;

                    // Update factor.
                    factor *= (1.0f - _alpha);
                }

                smoothedData[key] = topSum / bottomSum;
            }

            return smoothedData;
        }
    }
}