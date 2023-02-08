using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;

namespace CardGame.Textures {
    public static class TextureCollectionReader {
        public static Dictionary<string, Collection> Readers = new Dictionary<string, Collection>() {
            { "Avatars", new Collection ("Avatars") },
            { "DeckTextures", new Collection ("DeckTextures") }
        };
        
        public static void ReadAll () {
            foreach (var e in Readers) {
                e.Value.Read();
            }
        }

        public class Collection {
            private string folder;
            public Collection (string folder) {
                this.folder = folder;
            }
            public bool IsLoaded => Textures != null;
            public Dictionary<string, BaseTexture> Textures;

            /// <summary>
            /// Reads texture folder. Return null if all is readed successfully. 
            /// Returns textures name if there is a duplication.
            /// </summary>
            /// <returns>Duplicated image name</returns>
            public string Read() {
                Textures = new Dictionary<string, BaseTexture>();

                var textures = Resources.LoadAll<Texture2D>(folder);
                var movies = Resources.LoadAll<VideoClip>(folder);

                string error = null;

                foreach (var texture in textures) {
                    var newTexture = new TwoDTexture(texture);

                    if (Textures.ContainsKey(texture.name)) {
                        error = texture.name;
                    } else {
                        Textures.Add(texture.name, newTexture);
                    }
                }

                foreach (var texture in movies) {
                    var newTexture = new MovieTexture(texture);

                    if (Textures.ContainsKey(texture.name)) {
                        error = texture.name;
                    } else {
                        Textures.Add(texture.name, newTexture);
                    }
                }

                return error;
            }
        }
    }
}
