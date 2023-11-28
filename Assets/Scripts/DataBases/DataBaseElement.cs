using System.Collections.Generic;
using _ScriptableObject;
using Stats;
using UnityEngine;

namespace DataBases
{
    [CreateAssetMenu(fileName = "DataBase_Element", menuName = "Scriptable Object/DataBase/Element")]
    public class DataBaseElement : ScriptableObject
    {
        [SerializeField] private List<Element> elements;
        public Dictionary<EElement, Element> Elements => GetElements();
        private Dictionary<EElement, Element> GetElements()
        {
            Dictionary<EElement, Element> _ret = new Dictionary<EElement, Element>();
            elements.ForEach(_element => _ret.Add(_element.Type, _element));
            return _ret;
        }
    }
}