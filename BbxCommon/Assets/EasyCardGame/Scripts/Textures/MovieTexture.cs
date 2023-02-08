using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Rendering;

namespace CardGame.Textures {
    public class MovieTexture : BaseTexture {
        private VideoPlayer previewVideoPlayer;
        private Texture currentTexture;
        private bool previewIsReady;
        
        public MovieTexture (VideoClip texture) {
            this.texture = texture; 
        }

        public override void SetMaterial(string shaderTextureName, Renderer renderer) {
            var videoPlayer = renderer.GetComponent<VideoPlayer>();
            if (videoPlayer == null) {
                videoPlayer = renderer.gameObject.AddComponent<VideoPlayer>();
            }

            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
            videoPlayer.clip = (VideoClip) texture;
            videoPlayer.isLooping = true;
            videoPlayer.targetMaterialRenderer = renderer;
            videoPlayer.targetMaterialProperty = shaderTextureName;
            videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
            videoPlayer.Prepare();
            videoPlayer.StepForward();
            videoPlayer.Play();
        }

        public override Texture GetPreview() {
            if (!previewIsReady) {
                if (previewVideoPlayer == null) {
                    var previewer = new GameObject("videoPreview");

                    previewVideoPlayer = previewer.AddComponent<VideoPlayer>();

                    previewVideoPlayer.clip = (VideoClip)texture;
                    previewVideoPlayer.isLooping = true;
                    previewVideoPlayer.renderMode = VideoRenderMode.APIOnly;
                    previewVideoPlayer.sendFrameReadyEvents = true;

                    previewVideoPlayer.prepareCompleted += (source) => {
                        previewVideoPlayer.Pause();
                    };
                    previewVideoPlayer.frameReady += (source, frameIdx) => {
                        var texture = source.texture;

                        previewIsReady = true;

                        RenderTexture.active = (RenderTexture) texture;

                        var t2d = new Texture2D(texture.width, texture.height);
                        t2d.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
                        t2d.Apply();

                        currentTexture = t2d;

                        Object.DestroyImmediate(previewVideoPlayer.gameObject);
                    };

                    previewVideoPlayer.Prepare();
                    previewVideoPlayer.StepForward();
                }
            }

            return currentTexture;
        }
    }
}
