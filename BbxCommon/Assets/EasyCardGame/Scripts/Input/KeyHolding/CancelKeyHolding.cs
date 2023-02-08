namespace CardGame.Input {
    public class CancelKeyHolding : KeyHolding {

        private void OnEnable() {
            inputActions.OnCancel += StartHold;
            inputActions.OnCancelUp += EndHold;
        }

        private void OnDisable() {
            inputActions.OnCancel -= StartHold;
            inputActions.OnCancelUp -= EndHold;

            EndHold();
        }
    }
}
