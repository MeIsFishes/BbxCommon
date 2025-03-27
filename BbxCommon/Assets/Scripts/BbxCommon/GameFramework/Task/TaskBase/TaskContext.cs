using System;
using System.Collections.Generic;
using BbxCommon.Internal;

namespace BbxCommon
{
    public abstract class TaskContextBase : PooledObject
    {
        #region Interfaces
        private bool m_Inited = false;

        internal void Init()
        {
            if (m_Inited)
                return;
            RegisterFields();
            m_Inited = true;
        }
        #endregion

        #region Virtual Functions
        public abstract Type GetFieldEnumType();
        protected abstract void RegisterFields();
        #endregion

        #region Register and Get Field
        private struct RegisteredField
        {
            public object Value;
            public Type Type;

            public RegisteredField(object value, Type type)
            {
                Value = value;
                Type = type;
            }
        }
        private Dictionary<string, RegisteredField> m_Fields = new();

        internal TaskContextExportInfo GenerateExportInfo()
        {
            Init();
            var res = new TaskContextExportInfo();
            res.TaskContextTypeName = this.GetType().Name;
            foreach (var pair in m_Fields)
            {
                var exportFieldInfo = new TaskExportFieldInfo();
                exportFieldInfo.FieldName = pair.Key;
                exportFieldInfo.TypeInfo = TaskApi.GenerateTaskTypeInfo(pair.Value.Type);
                res.FieldInfos.Add(exportFieldInfo);
            }
            return res;
        }

        protected void RegisterBool<T>(T fieldEnum, bool value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = new RegisteredField(value, typeof(bool));
        }

        protected void RegisterShort<T>(T fieldEnum, short value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = new RegisteredField(value, typeof(short));
        }

        protected void RegisterUshort<T>(T fieldEnum, ushort value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = new RegisteredField(value, typeof(ushort));
        }

        protected void RegisterInt<T>(T fieldEnum, int value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = new RegisteredField(value, typeof(int));
        }

        protected void RegisterUint<T>(T fieldEnum, uint value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = new RegisteredField(value, typeof(uint));
        }

        protected void RegisterLong<T>(T fieldEnum, long value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = new RegisteredField(value, typeof(long));
        }

        protected void RegisterUlong<T>(T fieldEnum, ulong value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = new RegisteredField(value, typeof(ulong));
        }

        protected void RegisterFloat<T>(T fieldEnum, float value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = new RegisteredField(value, typeof(float));
        }

        protected void RegisterDouble<T>(T fieldEnum, double value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = new RegisteredField(value, typeof(double));
        }

        protected void RegisterString<T>(T fieldEnum, string value) where T : Enum
        {
            m_Fields[fieldEnum.ToString()] = new RegisteredField(value, typeof(string));
        }

        protected void RegisterObject<TEnum, TObject>(TEnum fieldEnum, TObject value) where TEnum : Enum
        {
            m_Fields[fieldEnum.ToString()] = new RegisteredField(value, typeof(TObject));
        }

        internal object GetValue(string key)
        {
            return m_Fields[key].Value;
        }
        #endregion
    }
}
