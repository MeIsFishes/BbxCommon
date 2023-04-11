using UnityEngine;
using UnityEditor;

namespace BbxCommon
{
    #region RandomFunc
    public static class RandomFunc
    {
        /// <summary>
        /// Input a probability in [0, 1], return if it hits.
        /// </summary>
        public static bool IsSucceededRatio(float probability)
        {
            var rand = Random.Range(0f, 1f);
            if (rand < probability)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Input a probability in percentage, return if it hits.
        /// </summary>
        public static bool IsSucceededPercentage(float probability)
        {
            var rand = Random.Range(0f, 100f);
            if (rand < probability)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Input a normal distribution factor, return a random result in range [0f, 1f].
        /// </summary>
        /// <param name="factor"> Normal distribution factor, must be greater than 0.
        /// When it's less than 1, the random result will more likely to be closer to 0,
        /// and if it's greater than 1, the result will more likely to be closer to 1. </param>
        /// <returns> A randomly created result represents the probability in percentage. </returns>
        public static float NormalDistributionProbability(float factor)
        {
            var res = Random.Range(0f, 1f);
            res = Mathf.Pow(res, factor);
            res = 1 - res;
            return res;
        }
    }
    #endregion

    #region AssetFunc
#if UNITY_EDITOR
    public static class AssetFunc
    {
        public static TAsset LoadOrCreateAsset<TAsset>(string path) where TAsset : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<TAsset>(path);
            if (asset != null)
                return asset;
            else
            {
                asset = ScriptableObject.CreateInstance<TAsset>();
                AssetDatabase.CreateAsset(asset, path);
                return asset;
            }
        }
    }
#endif
    #endregion
}
