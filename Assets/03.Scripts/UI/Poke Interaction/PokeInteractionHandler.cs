using UnityEngine;
using UnityEngine.Events;

namespace Oculus.Interaction
{
    public class PokeInteractionHandler : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IInteractableView))]
        private UnityEngine.Object _interactableView;
        private IInteractableView InteractableView { get; set; }

        // Inspector에서 할당 가능한 UnityEvent
        [SerializeField]
        private UnityEvent _onPokeAction;

        protected virtual void Awake()
        {
            InteractableView = _interactableView as IInteractableView;
        }

        protected virtual void OnEnable()
        {
            InteractableView.WhenStateChanged += OnStateChanged;
        }

        protected virtual void OnDisable()
        {
            InteractableView.WhenStateChanged -= OnStateChanged;
        }

        private void OnStateChanged(InteractableStateChangeArgs args)
        {
            if (args.NewState == InteractableState.Select)
            {
                PerformPokeAction();
            }
        }

        private void PerformPokeAction()
        {
            Debug.Log("Poke Interaction 발생!");
            _onPokeAction?.Invoke(); // UnityEvent 실행
        }
    }
}
