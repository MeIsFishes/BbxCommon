using UnityEngine;
using UnityEngine.Video;

namespace CardGame.Textures {
    public class TwoDTexture : BaseTexture {
        public TwoDTexture (Texture texture) {
            this.texture = texture;
        }

        public override void SetMaterial(string shaderTextureName, Renderer renderer) {
            var videoPlayer = renderer.GetComponent<VideoPlayer>();
            if (videoPlayer != null) {
                Object.Destroy(videoPlayer);
            }

            if (Application.isPlaying) {
                renderer.material.SetTexture(shaderTextureName, (Texture2D) texture);
            } else {
                renderer.sharedMaterial.SetTexture(shaderTextureName, (Texture2D)texture);
            }
        }

        public override Texture GetPreview() {
            return (Texture) texture;
        }
    }
}
