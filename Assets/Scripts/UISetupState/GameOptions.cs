using UnityEngine;
using UnityEngine.Serialization;
using UserInterface;

namespace UISetupState
{
    public class GameOptions : MonoBehaviour
    {
        [FormerlySerializedAs("Manager")]
        [SerializeField] private UIManager manager;

        public void IsLifeBarActive(bool _isActive)
        {
            manager.MenuActiveLifeBar(_isActive);
            foreach (LifeBar _bar in FindObjectsOfType<LifeBar>())
            {
                _bar.Activate(_isActive);
            }
        }

        public void IsCompleteSats(bool _isActive)
        {
            manager.MenuActiveCompleteStats(_isActive);
        }
    }
}