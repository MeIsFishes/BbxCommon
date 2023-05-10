using System.Collections.Generic;

namespace BbxCommon
{
    /// <summary>
    /// A class to generates non-duplicate IDs.
    /// </summary>
    public class UniqueIdGenerator : PooledObject
    {
        private uint m_Id = 0;

        public uint GenerateID()
        {
            return m_Id++;
        }

        public override void OnAllocate()
        {
            m_Id = 0;
        }

        // There are 2 ways to create a UniqueIDGenerator:
        // 1. Instantiate and manage an instance yourself.
        // 2. Create an instance by static function, then hold and visit the instance through the key.
        // Visiting generators through keys in type uint and string are both available, but notice those ones with keys in different types are managed separately.

        // -------------------- Generators with uint keys --------------------

        private static Dictionary<uint, UniqueIdGenerator> m_s_GeneratorUint = new Dictionary<uint, UniqueIdGenerator>();
        private static UniqueIdGenerator m_s_KeyGenerator;

        /// <summary>
        /// Create an UniqueIDGenerator instance and it will pass out the key.
        /// </summary>
        public static UniqueIdGenerator CreateGenerator(out uint key)
        {
            if (m_s_KeyGenerator == null)
                m_s_KeyGenerator = new UniqueIdGenerator();
            uint newId = m_s_KeyGenerator.GenerateID();
            var generator = ObjectPool<UniqueIdGenerator>.Alloc();
            m_s_GeneratorUint.Add(newId, generator);
            key = newId;
            return generator;
        }

        /// <summary>
        /// Get an existed generator by uint key.
        /// </summary>
        public static UniqueIdGenerator GetGenerator(uint key)
        {
            return m_s_GeneratorUint[key];
        }

        /// <summary>
        /// Destroy an existed generator by uint key.
        /// </summary>
        public static void DestroyGenerator(uint key)
        {
            m_s_GeneratorUint.Remove(key);
        }

        // -------------------- Generators with string keys --------------------

        private static Dictionary<string, UniqueIdGenerator> m_s_GeneratorString = new Dictionary<string, UniqueIdGenerator>();

        /// <summary>
        /// Create an UniqueIDGenerator instance with a string key.
        /// </summary>
        public static UniqueIdGenerator CreateGenerator(string key)
        {
            var generator = ObjectPool<UniqueIdGenerator>.Alloc();
            m_s_GeneratorString.Add(key, generator);
            return generator;
        }

        /// <summary>
        /// Get an existed generator by string key.
        /// </summary>
        public static UniqueIdGenerator GetGenerator(string key)
        {
            return m_s_GeneratorString[key];
        }

        /// <summary>
        /// Destroy an existed generator by string key.
        /// </summary>
        public static void DestroyGenerator(string key)
        {
            m_s_GeneratorString.Remove(key);
        }
    }
}
