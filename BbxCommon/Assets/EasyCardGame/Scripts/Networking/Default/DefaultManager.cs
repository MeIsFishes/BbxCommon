using UnityEngine;

namespace CardGame.Networking.Default {
    public class DefaultManager : MonoBehaviour {
        // Start is called before the first frame update
        void Start() {
            Gateway.Instance = new DefaultBridge();
        }
    }
}
