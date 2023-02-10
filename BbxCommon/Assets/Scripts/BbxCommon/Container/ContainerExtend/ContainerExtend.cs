using UnityEngine;
using System.Collections.Generic;

namespace BbxCommon
{
    public static class ContainerExtend
    {
        #region List
        /// <summary>
        /// Shuffle elements to random order.
        /// </summary>
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var index = Random.Range(0, list.Count);
                T k = list[i];
                list[i] = list[index];
                list[index] = k;
            }
        }
        #endregion
    }
}
