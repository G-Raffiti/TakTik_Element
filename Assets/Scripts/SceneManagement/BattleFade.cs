using System.Collections;
using System.Linq;
using _EventSystem.CustomEvents;
using _Instances;
using EndConditions;
using StateMachine;
using UnityEngine;
using UnityEngine.Serialization;
using Void = _EventSystem.CustomEvents.Void;

namespace SceneManagement
{
    public class BattleFade : MonoBehaviour
    {
        [FormerlySerializedAs("SceneManager")]
        [Header("UI Elements in BattleScene")]
        [SerializeField] private ChangeScene sceneManager;
        [SerializeField] private ShopChoiceUI shopChoiceUI;
        [SerializeField] private GameObject nextBattle;
        [SerializeField] private EndRunUI endRunUI;
        [SerializeField] private GameObject resurectionPool;
        
        [Header("Event Sender")]
        [SerializeField] private VoidEvent onBattleEndTrigger;
        [SerializeField] private VoidEvent onUIEnable;
        
        [Header("Event Listener")]
        [SerializeField] private BoolEvent onBattleIsOver;
        [SerializeField] private VoidEvent onQuitShop;
        [FormerlySerializedAs("nextBattle")]
        [FormerlySerializedAs("NextBattle")]
        [SerializeField] private VoidEvent onNextBattle;
        [SerializeField] private VoidEvent goToResurrection;
        
        private void Start()
        {
            onBattleIsOver.EventListeners += BattleIsOver_Catch;
            onQuitShop.EventListeners += StartNewBattle;
            onNextBattle.EventListeners += StartNewBattle;
            goToResurrection.EventListeners += GoToResurrection;
        }

        private void OnDestroy()
        {
            onBattleIsOver.EventListeners -= BattleIsOver_Catch;
            onQuitShop.EventListeners -= StartNewBattle;
            onNextBattle.EventListeners -= StartNewBattle;
            goToResurrection.EventListeners -= GoToResurrection;
        }

        private void GoToResurrection(Void _obj)
        {
            onBattleEndTrigger.Raise();
            sceneManager.LoadScene("Reborn");
        }

        private void StartNewBattle(Void _empty)
        {
            BattleStage.EndBattle();
            sceneManager.LoadScene("BattleScene");
            //Todo: Show this Log on screen (BattleStart)
            Debug.Log($"Battle: {BattleStage.BattleNumber} Stage: {BattleStage.Stage}");
        }

        private void BattleIsOver_Catch(bool _isWin)
        {
            StartCoroutine(BattleEnds(_isWin));
        }

        private IEnumerator BattleEnds(bool _isWin)
        {
            yield return new WaitWhile(() => BattleStateManager.instance.DeadThisTurn.Count > 0);
            onUIEnable.Raise();

            if (BattleStateManager.instance.EndCondition.Type == EConditionType.Last || !_isWin)
            {
                endRunUI.gameObject.SetActive(true);
                endRunUI.Open(_isWin);
                yield break;
            }
            
            YouWin();
        }

        public void GoToScoreScene()
        {
            sceneManager.LoadScene("ScoreScene");
        }

        private void YouWin()
        {
            if (PlayerData.GetInstance().Heroes.Any(_h => _h.isDead) && PlayerData.GetInstance().ResurrectionPoints > 0)
            {
                resurectionPool.SetActive(true);
                return;
            }
                
            switch (BattleStateManager.instance.EndCondition.Type)
            {
                case EConditionType.LootBox:
                    nextBattle.SetActive(true);
                    break;
                default:
                    shopChoiceUI.gameObject.SetActive(true);
                    break;
            }
        }

        public void GoToShop(string _shopName)
        {
            onBattleEndTrigger.Raise();
            sceneManager.LoadScene(_shopName);
        }
    }
}