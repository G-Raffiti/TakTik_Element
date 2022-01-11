using System.Collections.Generic;
using UnityEngine;

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
        
        public static int Max(this int _scoreDamage, int _damage)
        {
            return _scoreDamage < _damage ? _damage : _scoreDamage;
        }
    }
}