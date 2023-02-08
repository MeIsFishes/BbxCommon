using System;

namespace CardGame.Networking.Default {
    public class DefaultBridge : Gateway {
        private void ThrowNetworkingError () {
            throw new NotImplementedException("[DefaultBridge] Networking is not implemented on this gateway. Please check the documentation on https://www.easycardgame.com to implement Photon Networking, or to get information about creating new gateways.");
        }
        public override void Connect() {
            ThrowNetworkingError();
        }

        public override void QuickMatch() {
            ThrowNetworkingError();
        }
    }
}