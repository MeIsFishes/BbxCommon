namespace CardGame.Input {
    public class SelectKeyHolding : KeyHolding {

        private void OnEnable() {
            inputActions.OnSelect += StartHold;
            inputActions.OnSelectUp += EndHold;
        }

        private void OnDisable() {
            inputActions.OnSelect -= StartHold;
            inputActions.OnSelectUp -= EndHold;
        }
    }
}
