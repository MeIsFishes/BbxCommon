using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CardGame.GameFunctions {
    public class GameFunctionQuery {
        private List<BaseFunction> functions;

        private Coroutine coroutine;
        public GameFunctionQuery () {
            functions = new List<BaseFunction>();
        }

        public void AddToQuery (BaseFunction function) {
            functions.Add(function);
        }

        public void Play (MonoBehaviour coroutineHolder) {
            coroutine = coroutineHolder.StartCoroutine(Loop());
        }

        public void Reset (MonoBehaviour coroutineHolder) {
            coroutineHolder.StopCoroutine(coroutine);
            foreach (var f in functions) {
                f.Clear();
            }

            functions.Clear();

            Play(coroutineHolder);
        }

        IEnumerator Loop () {
            bool isPlaying = false;
            while (true) {
                if (!isPlaying && functions.Count > 0) {
                    isPlaying = true;

                    functions[0].Trigger(() => {
                        isPlaying = false;
                        functions.RemoveAt(0);
                    });
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}