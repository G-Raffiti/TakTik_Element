using EndConditions;
using UnityEngine;

namespace _Instances
{
    
    public class BattleStage : MonoBehaviour
    {
        [SerializeField] private int stage;
        private static GameObject _instance;
        public static int Stage { get; private set; }
        public const int BattlePerStage = 3;
        public static int BattleNumber = -1;
        public static EConditionType currentState = EConditionType.LootBox;

        public static void NextStage()
        {
            Stage += 1;
            BattleNumber = 0;
        }

        private static void UpdateCurrentState()
        {

            if (Stage == 0 && BattleNumber == BattlePerStage)
            {
                currentState = EConditionType.Last;
                return;
            }
            
            if (BattleNumber == BattlePerStage)
                currentState = EConditionType.Boss;
            else
                currentState = EConditionType.Death;
        }

        public static void EndBattle()
        {
            BattleNumber += 1;
            
            if (BattleNumber > BattlePerStage)
                NextStage();
            
            UpdateCurrentState();
        }
        
        private void Start() 
        {
            DontDestroyOnLoad(gameObject.transform);
            if (_instance == null)
                _instance = gameObject;
            else
                Destroy(gameObject);
        }
        
        [ContextMenu("set stage")]
        public void SetStage()
        {
            Stage = stage;
        }

        public static void EndGame()
        {
            Stage = -1;
            BattleNumber = -1;
        }
    }

    
}