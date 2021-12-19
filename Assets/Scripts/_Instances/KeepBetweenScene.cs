using UnityEngine;

namespace _Instances
{
    public enum EBattleState
    {
        LOOT,
        FIGHT,
        BOSS
    }
    
    public class KeepBetweenScene : MonoBehaviour
    {
        [SerializeField] private int stage;
        private static GameObject instance;
        public static int Stage { get; private set; }
        public const int BattlePerStage = 3;
        public static int BattleNumber = 0;
        public static EBattleState currentState = EBattleState.LOOT;

        public static void NextStage()
        {
            Stage += 1;
            BattleNumber = 0;
        }

        private static void UpdateCurrentState()
        {
            if (BattleNumber == BattlePerStage)
            {
                currentState = EBattleState.BOSS;
            }
            else
            {
                currentState = EBattleState.FIGHT;
            }
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
            if (instance == null)
                instance = gameObject;
            else
                Destroy(gameObject);
        }
        
        [ContextMenu("set stage")]
        public void setStage()
        {
            Stage = stage;
        }
    }

    
}