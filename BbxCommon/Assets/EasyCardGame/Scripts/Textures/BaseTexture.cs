using UnityEngine;

namespace CardGame.Textures {
    public abstract class BaseTexture {
        public string name => texture.name;
        protected Object texture;
        public abstract void SetMaterial(string shaderTextureName, Renderer renderer);
        public abstract Texture GetPreview();
    }
}
