using System;
using System.Collections.Generic;

namespace BbxCommon
{
    internal static class TypeIdCounter<TBase>
    {
        internal static int CurId;
    }

    /// <summary>
    /// As we need to store some member object instances such as components and data items, the easiest way
    /// is creating a <see cref="Dictionary{TKey, TValue}"/> with <see cref="Type"/> keys. However, reflection
    /// and hash calculation are both with expensive overhead. In some cases, cosider storing them in a
    /// <see cref="List{T}"/> is perhaps better. To implement this, each type needs a unique index to sign
    /// which slot of the <see cref="List{T}"/> it should be in, so there is <see cref="ClassTypeId{TBase, TDerived}"/>.
    /// </summary>
    public static class ClassTypeId<TBase, TDerived> where TDerived : TBase
    {
        private static int m_Id;
        private static bool m_Inited;

        public static int Id
        {
            get
            {
                if (m_Inited == false)
                {
                    m_Id = TypeIdCounter<TBase>.CurId++;
                    m_Inited = true;
                }
                return m_Id;
            }
        }

        public static int GetId() => Id;
    }

    public static class ClassTypeId<TBase>
    {
        public static int GetId<TDerived>() where TDerived : TBase
        {
            return ClassTypeId<TBase, TDerived>.Id;
        }

        public static int GetId(Type type)
        {
            if (type.IsSubclassOf(typeof(TBase)) == false && type != typeof(TBase))
            {
                DebugApi.LogError("Type '"+ type.FullName + "is not derived from" + typeof(TBase).FullName + ".");
                return -1;
            }
            var classDeclare = typeof(ClassTypeId<,>).MakeGenericType(typeof(TBase), type);
            var method = classDeclare.GetMethod("GetId", Array.Empty<Type>());
            return (int)method.Invoke(null, null);
        }

        public static int GetId(object obj)
        {
            return GetId(obj.GetType());
        }
    }
}
