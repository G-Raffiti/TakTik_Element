using System.Collections;
using Cells;
using DataBases;
using UnityEngine;

namespace _Instances
{
    [CreateAssetMenu(fileName = "DataBase", menuName = "Scriptable Object/DataBase/Data Base")]
    public class DataBase : ScriptableObject
    {
        public static DataBase Instance;
        public static DataBaseMonster Monster;
        public static DataBaseRelic Relic;
        public static DataBaseCell Cell;
        public static DataBaseAffix Affix;
        public static DataBaseGear Gear;
        public static DataBaseBoard Board;
        public static DataBaseSkill Skill;
        public static DataBaseElement Element;

        [SerializeField] private DataBaseMonster dataMonster;
        [SerializeField] private DataBaseRelic dataRelic;
        [SerializeField] private DataBaseCell dataCell;
        [SerializeField] private DataBaseAffix dataAffix;
        [SerializeField] private DataBaseGear dataGear;
        [SerializeField] private DataBaseBoard dataBoard;
        [SerializeField] private DataBaseSkill dataSkill;
        [SerializeField] private DataBaseElement dataElement;

        public void OnEnable()
        {
            InstantiateDataBases();
        }

        public void InstantiateDataBases()
        {
            Monster = dataMonster;
            Relic = dataRelic;
            Cell = dataCell;
            Affix = dataAffix;
            Gear = dataGear;
            Board = dataBoard;
            Skill = dataSkill;
            Element = dataElement;
        }
    }
}