namespace CardGame.Input {
    public class ReturnKeyHolding : KeyHolding {

        private void OnEnable() {
            inputActions.OnReturn += StartHold;
            inputActions.OnReturnUp += EndHold;
        }

        private void OnDisable() {
            inputActions.OnReturn -= StartHold;
            inputActions.OnReturnUp -= EndHold;
        }
    }
}
