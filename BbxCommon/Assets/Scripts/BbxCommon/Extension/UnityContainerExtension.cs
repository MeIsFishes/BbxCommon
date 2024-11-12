using System;
using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;

namespace BbxCommon
{
    public static class UnityContainerExtension
    {
        #region UnityEngine
        #region Vector
        public static Vector2 SetX(this Vector2 vector, float value)
        {
            return new Vector2(value, vector.y);
        }

        public static Vector2 SetY(this Vector2 vector, float value)
        {
            return new Vector2(vector.x, value);
        }

        public static Vector2 AddX(this Vector2 vector, float value)
        {
            return new Vector2(vector.x + value, vector.y);
        }

        public static Vector2 AddY(this Vector2 vector, float value)
        {
            return new Vector2(vector.x, vector.y + value);
        }

        public static Vector3 AsVector3XY(this Vector2 vector)
        {
            return new Vector3(vector.x, vector.y, 0);
        }

        public static Vector3 AsVector3XZ(this Vector2 vector)
        {
            return new Vector3(vector.x, 0, vector.y);
        }

        public static Vector3 AsVector3YZ(this Vector2 vector)
        {
            return new Vector3(0, vector.x, vector.y);
        }

        public static Vector3 SetX(this Vector3 vector, float value)
        {
            return new Vector3(value, vector.y, vector.z);
        }

        public static Vector3 SetY(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, value, vector.z);
        }

        public static Vector3 SetZ(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y, value);
        }

        public static Vector3 SetXY(this Vector3 vector, float x, float y)
        {
            return new Vector3(x, y, vector.z);
        }

        public static Vector3 SetXZ(this Vector3 vector, float x, float z)
        {
            return new Vector3(x, vector.y, z);
        }

        public static Vector3 SetYZ(this Vector3 vector, float y, float z)
        {
            return new Vector3(vector.x, y, z);
        }

        public static Vector3 AddX(this Vector3 vector, float value)
        {
            return new Vector3(vector.x + value, vector.y, vector.z);
        }

        public static Vector3 AddY(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y + value, vector.z);
        }

        public static Vector3 AddZ(this Vector3 vector, float value)
        {
            return new Vector3(vector.x, vector.y, vector.z + value);
        }

        public static Vector2 AsVector2XY(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }

        public static Vector2 AsVector2XZ(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public static Vector2 AsVector2YZ(this Vector3 vector)
        {
            return new Vector2(vector.y, vector.z);
        }
        #endregion

        #region Color
        public static Color SetR(this Color color, float value)
        {
            return new Color(value, color.g, color.b, color.a);
        }

        public static Color SetG(this Color color, float value)
        {
            return new Color(color.r, value, color.b, color.a);
        }

        public static Color SetB(this Color color, float value)
        {
            return new Color(color.r, color.g, value, color.a);
        }

        public static Color SetA(this Color color, float value)
        {
            return new Color(color.r, color.g, color.b, value);
        }
        #endregion

        #region Quaternion
        public static Quaternion SetX(this Quaternion quaternion, float value)
        {
            return new Quaternion(value, quaternion.y, quaternion.z, quaternion.w);
        }

        public static Quaternion SetY(this Quaternion quaternion, float value)
        {
            return new Quaternion(quaternion.x, value, quaternion.z, quaternion.w);
        }

        public static Quaternion SetZ(this Quaternion quaternion, float value)
        {
            return new Quaternion(quaternion.x, quaternion.y, value, quaternion.w);
        }

        public static Quaternion SetW(this Quaternion quaternion, float value)
        {
            return new Quaternion(quaternion.x, quaternion.y, quaternion.z, value);
        }

        public static float4 AsFloat4(this Quaternion quaternion)
        {
            return new float4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }
        #endregion

        #region GameObject
        public static bool HasComponent<TComp>(this GameObject gameObject) where TComp : Component
        {
            return gameObject.GetComponent<TComp>() != null;
        }

        public static bool HasComponent(this GameObject gameObject, Type type)
        {
            return gameObject.GetComponent(type) != null;
        }

        public static TComp AddMissingComponent<TComp>(this GameObject gameObject) where TComp : Component
        {
            if (gameObject.TryGetComponent<TComp>(out var comp) == false)
                return gameObject.AddComponent<TComp>();
            else
                return comp;
        }

        public static Component AddMissingComponent(this GameObject gameObject, Type type)
        {
            if (gameObject.TryGetComponent(type, out var comp) == false)
                return gameObject.AddComponent(type);
            else
                return comp;
        }

        public static void Destroy(this GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }
        #endregion

        #region Transform
        public static void LookAtIgnoreY(this Transform transform, Vector3 target)
        {
            target = target.SetY(transform.position.y);
            transform.rotation = Quaternion.LookRotation(target -  transform.position);
        }
        #endregion

        #region Component
        public static void Destroy(this Component component)
        {
            UnityEngine.Object.Destroy(component);
        }
        #endregion
        #endregion

        #region DOTS
        #region float3
        public static float3 SetX(this float3 target, float value)
        {
            return new float3(value, target.y, target.z);
        }

        public static float3 SetY(this float3 target, float value)
        {
            return new float3(target.x, value, target.z);
        }

        public static float3 SetZ(this float3 target, float value)
        {
            return new float3(target.x, target.y, value);
        }

        public static float3 AddX(this float3 target, float value)
        {
            return new float3(target.x + value, target.y, target.z);
        }

        public static float3 AddY(this float3 target, float value)
        {
            return new float3(target.x, target.y + value, target.z);
        }

        public static float3 AddZ(this float3 target, float value)
        {
            return new float3(target.x, target.y, target.z + value);
        }
        #endregion
        #endregion

        #region Collections
        /// <summary>
        /// Add HashSet's members to current List.
        /// </summary>
        public static void AddHashSet<T>(this List<T> list, SerializableHashSet<T> set)
        {
            foreach (var m in set)
            {
                list.Add(m);
            }
        }

        /// <summary>
        /// Add Dictionary's key to the List.
        /// </summary>
        public static void AddDicKey<TKey, TValue>(this List<TKey> list, SerializableDic<TKey, TValue> dic)
        {
            foreach (var pair in dic)
            {
                list.Add(pair.Key);
            }
        }

        /// <summary>
        /// Add Dictionary's value to the List.
        /// </summary>
        public static void AddDicValue<TKey, TValue>(this List<TValue> list, SerializableDic<TKey, TValue> dic)
        {
            foreach (var pair in dic)
            {
                list.Add(pair.Value);
            }
        }

        /// <summary>
        /// Add HashSet's members to the HashSet.
        /// </summary>
        public static void AddHashSet<T>(this HashSet<T> set, SerializableHashSet<T> addSet)
        {
            foreach (var m in addSet)
            {
                set.Add(m);
            }
        }

        /// <summary>
        /// Add Dictionary's key to the HashSet.
        /// </summary>
        public static void AddDicKey<TKey, TValue>(this HashSet<TKey> set, SerializableDic<TKey, TValue> dic)
        {
            foreach (var pair in dic)
            {
                set.Add(pair.Key);
            }
        }

        /// <summary>
        /// Add Dictionary's value to the HashSet.
        /// </summary>
        public static void AddDicValue<TKey, TValue>(this HashSet<TValue> set, SerializableDic<TKey, TValue> dic)
        {
            foreach (var pair in dic)
            {
                set.Add(pair.Value);
            }
        }
        #endregion
    }
}
