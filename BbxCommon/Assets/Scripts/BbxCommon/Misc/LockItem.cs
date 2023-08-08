using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    /// <summary>
    /// <para>
    /// <see cref="LockItem"/> is for locking an instance.
    /// </para><para>
    /// Once an another object request locking the instance, it can call <see cref="LockItem.Lock"/> to get
    /// a key, and then request unlocking with the given key <see cref="LockItemKey"/>. Call <see cref="LockItem.IsLocked"/>
    /// to check if there are slots unlocked.
    /// </para><para>
    /// There are <see cref="LockItem.m_Generation"/>s for <see cref="LockItem"/>. If you want to unlock all
    /// slots, call <see cref="LockItem.UpdateGeneration"/> instead of clearing locking requests. With genaration
    /// flag, other objects may still hold their keys, but they cannot unlock slots any more. That means keys
    /// can only unlock slots if their <see cref="LockItemKey.Generation"/> and <see cref="LockItemKey.Key"/>
    /// all match.
    /// </para>
    /// </summary>
    public class LockItem : PooledObject
    {
        public bool IsLocked => m_Keys.Count > 0;

        private HashSet<ulong> m_Keys = new();
        private ulong m_Generation;

        private UniqueIdGenerator m_KeyGenerator = new();
        private UniqueIdGenerator m_GenerationGenerator = new(1);

        public LockItemKey Lock()
        {
            var key = m_KeyGenerator.GenerateId();
            m_Keys.Add(key);
            return new LockItemKey(m_Generation, key);
        }

        public void Unlock(LockItemKey key)
        {
            if (key.Generation == m_Generation)
            {
#if UNITY_EDITOR
                if (m_Keys.Contains(key.Key) == false)
                    Debug.LogWarning("Current key " + key.Key + " has already been removed! That's unexpected!");
                else
#endif
                    m_Keys.Remove(key.Key);
            }
        }

        public void UpdateGeneration()
        {
            m_Keys.Clear();
            m_KeyGenerator.ResetCounter();
            m_Generation = m_GenerationGenerator.GenerateId();
        }

        public override void OnCollect()
        {
            m_Keys.Clear();
            m_Generation = 0;
            m_KeyGenerator.ResetCounter();
            m_GenerationGenerator.ResetCounter(1);
        }
    }

    public struct LockItemKey
    {
        public ulong Generation;
        public ulong Key;

        public LockItemKey(ulong generation, ulong key)
        {
            Generation = generation;
            Key = key;
        }
    }
}
