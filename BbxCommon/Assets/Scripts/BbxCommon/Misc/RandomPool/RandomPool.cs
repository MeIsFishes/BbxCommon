using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    public interface IRandomPoolDebug
    {
        string GetDebugOutputName();
    }

    // RandomPool<T> helps you to build up a random result generator with given weight.
    // To simplify designing, it supports pool-in-pool and mixing of final target with pool-in-pool.
    // For example, you can build a pool like this:
    //         P
    //    /   /  \      \
    //   t1  t2  t3     P1
    //              /   /    \
    //             t4  t5    P2
    //                      /  \
    //                     t6  t7
    // In graph above, P represents pool, and t represents final target with type T.
    // In pool P, it will try to hit t1, t2, t3 or P1, while hits P1, P1 will try to hit t4, t5 or P2, until hit a final target.

    public class RandomPool<T> : PooledObject
    {
        private struct RandomItem<TItem>
        {
            public TItem Item;
            public int Weight;

            public RandomItem(TItem item, int weight)
            {
                Item = item;
                Weight = weight;
            }
        }

        private List<RandomItem<T>> m_RandomItems = new();
        private Dictionary<T, int> m_ItemIndexes = new();
        private List<RandomItem<RandomPool<T>>> m_RandomPools = new();
        private Dictionary<RandomPool<T>, int> m_PoolIndexes = new();
        private int m_TotalWeight;

        /// <summary>
        /// Get a result with type <typeparamref name="T"/>.
        /// </summary>
        public T Rand()
        {
            if (m_TotalWeight <= 0)
            {
                Debug.LogWarning("There is no item in the RandomPool. Returned a default value.");
                return default;
            }
            var rand = Random.Range(0, m_TotalWeight);
            for (int i = 0; i < m_RandomItems.Count; i++)
            {
                rand -= m_RandomItems[i].Weight;
                if (rand < 0)
                    return m_RandomItems[i].Item;
            }
            for (int i = 0; i <= m_RandomPools.Count; i++)
            {
                rand -= m_RandomPools[i].Weight;
                if (rand < 0)
                    return m_RandomPools[i].Item.Rand();
            }
            return default;
        }

        public void SetWeight(T item, int weight)
        {
            if (weight <= 0)
            {
                Debug.LogError("Weight must be at least 1!");
                return;
            }
            if (m_ItemIndexes.TryGetValue(item, out var index))
            {
                m_TotalWeight -= m_RandomItems[index].Weight;
                m_RandomItems[index] = new RandomItem<T>(item, weight);
                m_TotalWeight += weight;
            }
            else
            {
                m_RandomItems.Add(new RandomItem<T>(item, weight));
                m_ItemIndexes.Add(item, m_RandomItems.Count - 1);
                m_TotalWeight += weight;
            }
        }

        /// <summary>
        /// If hit a <see cref="RandomPool{T}"/>, it will request a final result in it.
        /// </summary>
        public void SetWeight(RandomPool<T> pool, int weight)
        {
            if (weight <= 0)
            {
                Debug.LogError("Weight must be at least 1!");
                return;
            }
            if (m_PoolIndexes.TryGetValue(pool, out var index))
            {
                m_TotalWeight -= m_RandomPools[index].Weight;
                m_RandomPools[index] = new RandomItem<RandomPool<T>>(pool, weight);
                m_TotalWeight += weight;
            }
            else
            {
                m_RandomPools.Add(new RandomItem<RandomPool<T>>(pool, weight));
                m_PoolIndexes.Add(pool, m_RandomPools.Count - 1);
                m_TotalWeight += weight;
            }
        }

        public override void OnCollect()
        {
            m_RandomItems.Clear();
            m_ItemIndexes.Clear();
            m_RandomPools.Clear();
            m_PoolIndexes.Clear();
        }
    }
}
