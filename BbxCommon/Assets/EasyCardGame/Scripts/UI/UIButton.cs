using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace CardGame.UI 
{
    [RequireComponent (typeof (Button))]
    public class UIButton : MonoBehaviour {
#pragma warning disable CS0649
        [SerializeField] private Input.InputActions inputActions;
        [SerializeField] private Button button;
#pragma warning restore CS0649

        private void OnValidate() {
            button = GetComponent<Button>();
        }

        private void OnEnable() {
            inputActions.OnSelect += Select;
        }

        private void OnDisable() {
            inputActions.OnSelect -= Select;
        }

        private void Select() {
            ExecuteEvents.Execute(button.gameObject, 
                new BaseEventData(EventSystem.current), 
                ExecuteEvents.submitHandler);
        }
    }
}
