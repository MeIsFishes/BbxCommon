
using UnityEngine;

namespace CardGame.UI {
    public class CanvasAlphaForMeshes : MonoBehaviour {
        private CanvasGroup canvasGroup;
        
        [SerializeField] private MeshRenderer meshRenderer = default;
        [SerializeField] private string alphaProperty = default;
        [SerializeField] private float valueMultiplier = default;

        private void Awake() {
            canvasGroup = GetComponentInParent<CanvasGroup>();
        }

        // Update is called once per frame
        void Update() {
            if (meshRenderer != null && canvasGroup != null) {
                if (!meshRenderer.material.HasProperty (alphaProperty)) {
                    return;
                }

                float alpha = (1- canvasGroup.alpha) * valueMultiplier;
                meshRenderer.material.SetFloat (alphaProperty, alpha);
            }
        }
    }
}

