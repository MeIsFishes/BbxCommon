using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace CardGame.Loaders {
    [CreateAssetMenu(fileName = "Pool", menuName = "CardGame/Create Pool", order = 1)]
    public class Pool : ScriptableObject {
        public class ObjectPool {
            private readonly Object[] objectList;
            private int step;
            private int size;
            public ObjectPool (int size, Object[] objectList) {
                this.size = size;
                this.objectList = objectList;
            }

            public Object Get (bool dontCount = false) {
                var select = objectList[step];

                if (!dontCount) {
                    if (++step >= size) {
                        step = 0;
                    }
                }

                return select;
            }
        }

        [System.NonSerialized]
        private Dictionary<string, ObjectPool> pools;
        [System.NonSerialized]
        private int count;

        public bool isLoaded => count > 0;
        public int Count => count;

        public ObjectPool[] GetAllObjects () {
            return pools.Values.ToArray();
        }

        /// <summary>
        /// load a resources folder into the pool.
        /// </summary>
        /// <param name="folderName">folder name in resources.</param>
        public void LoadFolder<T>(string folderName, int poolSize, Transform holder) where T : MonoBehaviour {
            pools = new Dictionary<string, ObjectPool>();

            var loaded = Resources.LoadAll<T>(folderName);
            foreach (var e in loaded) {
                var objectList = new Object[poolSize];
                for (int i=0; i<poolSize; i++) {
                    objectList[i] = Instantiate(e, holder);
                    objectList[i].name = e.name;
                }

                var objectPool = new ObjectPool(poolSize, objectList);
                pools.Add(e.name, objectPool);
            }

            count = loaded.Length;
        }

        /// <summary>
        /// Used to load single data like texture, mesh.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="folderName"></param>
        public void LoadFolderWithoutInstantiate <T> (string folderName) where T : Object {
            pools = new Dictionary<string, ObjectPool>();

            var loaded = Resources.LoadAll<T>(folderName);
            foreach (var e in loaded) {
                var objectList = new Object[1];
                objectList[0] = e;

                var objectPool = new ObjectPool(1, objectList);
                pools.Add(e.name, objectPool);
            }

            count = loaded.Length;
        }

        /// <summary>
        /// load a single object from resources at given path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="poolSize"></param>
        public void LoadSingleObject <T> (string path, int poolSize, Transform holder) where T : MonoBehaviour {
            pools = new Dictionary<string, ObjectPool>();

            var loaded = Resources.Load<T>(path);

            var objectList = new Object[poolSize];
            for (int i = 0; i < poolSize; i++) {
                objectList[i] = Instantiate(loaded, holder);
                objectList[i].name = loaded.name;
            }

            var objectPool = new ObjectPool(poolSize, objectList);
            pools.Add(loaded.name, objectPool);

            count = 1;
        }

        public T Get<T>(string name) where T : Object {
            if (pools.ContainsKey(name)) {
                return (T) pools[name].Get();
            }

            return null;
        }

        public T Get<T>(int index) where T : Object {
            return (T)pools.ElementAt(index).Value.Get ();
        }

        public T GetRandom<T>(bool dontCount = false) where T : Object {
            return (T) pools.Values.ElementAt(Random.Range(0, count)).Get (dontCount);
        }

        public T[] GetCollection <T>() where T : Object {
            int count = pools.Count;
            var result = new T[count];
            for (int i=0; i<count; i++) {
                result[i] = (T)pools.ElementAt(i).Value.Get();
            }

            return result;
        }

        public void Dispose () {
            pools.Clear();
            count = 0;
        }
    }
}
