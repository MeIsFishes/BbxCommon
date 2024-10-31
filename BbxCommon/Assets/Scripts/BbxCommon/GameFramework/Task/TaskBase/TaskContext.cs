using System;
using System.Collections.Generic;

namespace BbxCommon
{
    public abstract class TaskContextBase
    {
        #region Register and Get Field
        private Dictionary<string, object> m_Fields = new();

        protected void RegisterBool<T>(T fieldEnum, bool value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = value;
        }

        protected void RegisterShort<T>(T fieldEnum, short value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = value;
        }

        protected void RegisterUshort<T>(T fieldEnum, ushort value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = value;
        }

        protected void RegisterInt<T>(T fieldEnum, int value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = value;
        }

        protected void RegisterUint<T>(T fieldEnum, uint value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = value;
        }

        protected void RegisterLong<T>(T fieldEnum, long value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = value;
        }

        protected void RegisterUlong<T>(T fieldEnum, ulong value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = value;
        }

        protected void RegisterFloat<T>(T fieldEnum, float value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = value;
        }

        protected void RegisterDouble<T>(T fieldEnum, double value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = value;
        }

        protected void RegisterString<T>(T fieldEnum, string value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = value;
        }

        protected void RegisterObject<T>(T fieldEnum, object value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = value;
        }

        internal object GetValue(string key)
        {
            return m_Fields[key];
        }
        #endregion
    }
}
