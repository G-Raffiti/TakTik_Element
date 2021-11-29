using UnityEngine;

namespace _Instances
{
    public class KeepBetweenScene : MonoBehaviour
    {
        [SerializeField] private int stage;
        private static GameObject instance;
        public static int Stage = 0;
        private const int BattlePerStage = 3;
        public static int BattleBeforeBoss = BattlePerStage;
        public static int BattleNumber = 0;

        public static void NextStage()
        {
            Stage += 1;
            BattleBeforeBoss = BattlePerStage;
            BattleNumber = 0;
        }

        public static void StartBattle()
        {
            BattleBeforeBoss -= 1;
            BattleNumber += 1;
            Debug.Log($"Stage = {Stage}, Battle number = {BattlePerStage - BattleBeforeBoss}");
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