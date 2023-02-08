using UnityEngine;

namespace CardGame.Loaders {
    public interface IRandomizable<T> {
        int Rate { get; set; }
        T Object { get; set; }
    }

    public struct Randomizable<T> : IRandomizable<T> {
        private  T Object;
        private int Rate;

        public Randomizable (T Object, int Rate) {
            this.Object = Object;
            this.Rate = Rate;
        }

        int IRandomizable<T>.Rate { 
            get => Rate;
            set => Rate = value;
        }

        T IRandomizable<T>.Object {
            get => Object;
            set => Object = value;
        }

        public T GetObject () {
            return Object;
        }
    }

    public class Randomizer<T> {
        private IRandomizable<T>[] randomizables;
        private bool calculated;
        private int total;
        private int count;
        private int step;

        public int Count => count;

        public Randomizer (int count) {
            this.count = count;

            step = 0;
            calculated = false;
            total = 0;
            randomizables = new IRandomizable<T>[count];
        }

        private void Calculate () {
            if (calculated) {
                return;
            }

            for (int i=0; i<count; i++) {
                total += randomizables[i].Rate;
            }

            calculated = true;
        }

        public void AddMember (T Object, int rate) {
            if (step >= count) {
                Debug.LogError("[Randomizer] Capacity is full.");
                return;
            }

            randomizables[step] = new Randomizable<T>(Object, rate);

            step++;
        }

        /// <summary>
        /// Randomly select
        /// </summary>
        /// <returns></returns>
        public T Select () {
            Calculate();

            // Get a random integer from 0 to PoolSize.
            int randomNumber = Random.Range(0, total);

            // Detect the item, which corresponds to current random number.
            int accumulatedProbability = 0;
            for (int i = 0; i < count; i++) {
                accumulatedProbability += randomizables[i].Rate;

                if (randomNumber <= accumulatedProbability)
                    return randomizables[i].Object;
            }

            return new Randomizable<T>().GetObject();
        }

        /// <summary>
        /// Pick from an index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T Pick (int index) {
            return randomizables[index].Object;
        }
    }
}
