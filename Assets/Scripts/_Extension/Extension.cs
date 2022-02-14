using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Extension
{
    public static class Extension
    {
        public static void Shuffle<T>(this IList<T> _list)
        {
            int count = _list.Count;
            int last = count - 1;
            for (int i = 0; i < last; i++)
            {
                int randomIndex = Random.Range(i, count);
                T tempValue = _list[i];
                _list[i] = _list[randomIndex];
                _list[randomIndex] = tempValue;
            }
        }

        public static T GetRandom<T>(this IList<T> _list)
        {
            int max = _list.Count;
            return _list[Random.Range(0, max)];
        }
        
        public static int Max(this int _scoreDamage, int _damage)
        {
            return _scoreDamage < _damage ? _damage : _scoreDamage;
        }

        public static T GetKeyOfMaxValue<T>(this IDictionary<T, int> _dictionary)
        {
            T max = _dictionary.First().Key;
            foreach (KeyValuePair<T, int> pair in _dictionary)
            {
                if (pair.Value > _dictionary[max]) max = pair.Key;
            }

            return max;
        }
        
        public static T GetKeyOfMaxValue<T>(this IDictionary<T, float> _dictionary)
        {
            T max = _dictionary.First().Key;
            foreach (KeyValuePair<T, float> pair in _dictionary)
            {
                if (pair.Value > _dictionary[max]) max = pair.Key;
            }

            return max;
        }
        
        public static T GetKeyOfMinValue<T>(this IDictionary<T, int> _dictionary)
        {
            T max = _dictionary.First().Key;
            foreach (KeyValuePair<T, int> pair in _dictionary)
            {
                if (pair.Value < _dictionary[max]) max = pair.Key;
            }

            return max;
        }
        
        public static T GetKeyOfMinValue<T>(this IDictionary<T, float> _dictionary)
        {
            T max = _dictionary.First().Key;
            foreach (KeyValuePair<T, float> pair in _dictionary)
            {
                if (pair.Value < _dictionary[max]) max = pair.Key;
            }

            return max;
        }

        public static void Clear(this Transform _transform)
        {
            while (_transform.childCount > 0)
            {
                GameObject.DestroyImmediate(_transform.GetChild(0).gameObject);
            }
        }
    }
}