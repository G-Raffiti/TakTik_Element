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
            int _count = _list.Count;
            int _last = _count - 1;
            for (int _i = 0; _i < _last; _i++)
            {
                int _randomIndex = Random.Range(_i, _count);
                T _tempValue = _list[_i];
                _list[_i] = _list[_randomIndex];
                _list[_randomIndex] = _tempValue;
            }
        }

        public static T GetRandom<T>(this IList<T> _list)
        {
            int _max = _list.Count;
            return _list[Random.Range(0, _max)];
        }
        
        public static int Max(this int _scoreDamage, int _damage)
        {
            return _scoreDamage < _damage ? _damage : _scoreDamage;
        }

        public static T GetKeyOfMaxValue<T>(this IDictionary<T, int> _dictionary)
        {
            T _max = _dictionary.First().Key;
            foreach (KeyValuePair<T, int> _pair in _dictionary)
            {
                if (_pair.Value > _dictionary[_max]) _max = _pair.Key;
            }

            return _max;
        }
        
        public static T GetKeyOfMaxValue<T>(this IDictionary<T, float> _dictionary)
        {
            T _max = _dictionary.First().Key;
            foreach (KeyValuePair<T, float> _pair in _dictionary)
            {
                if (_pair.Value > _dictionary[_max]) _max = _pair.Key;
            }

            return _max;
        }
        
        public static T GetKeyOfMinValue<T>(this IDictionary<T, int> _dictionary)
        {
            T _max = _dictionary.First().Key;
            foreach (KeyValuePair<T, int> _pair in _dictionary)
            {
                if (_pair.Value < _dictionary[_max]) _max = _pair.Key;
            }

            return _max;
        }
        
        public static T GetKeyOfMinValue<T>(this IDictionary<T, float> _dictionary)
        {
            T _max = _dictionary.First().Key;
            foreach (KeyValuePair<T, float> _pair in _dictionary)
            {
                if (_pair.Value < _dictionary[_max]) _max = _pair.Key;
            }

            return _max;
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