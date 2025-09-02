using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BbxCommon.Internal;

namespace BbxCommon
{
    public abstract class TaskContextBase : PooledObject
    {
        #region Interfaces
        internal int TypeId;
        internal TaskBridgeGroupInfo BindingTaskGroupInfo;
        private bool m_Inited;

        internal void Init(TaskBridgeGroupInfo taskGroupInfo)
        {
            if (m_Inited == false)
            {
                TypeId = ClassTypeId<TaskContextBase>.GetId(this.GetType());
            }
            BindingTaskGroupInfo = taskGroupInfo;
            RegisterFields();
            m_Inited = true;
            TaskDeserialiser.GetContextData(TypeId).Inited = true;
        }
        #endregion

        #region override
        protected sealed override void OnAllocate()
        {
            OnContextAllocate();
        }

        protected virtual void OnContextAllocate() { }

        protected sealed override void OnCollect()
        {
            OnContextCollect();
            BindingTaskGroupInfo = null;
            m_blackBoardDoubleData.Clear();
            m_blackBoardLongData.Clear();
            m_blackBoardObjectData.Clear();
        }

        protected virtual void OnContextCollect() { }

        #endregion
        
        #region BlackBoard

        /// <summary>
        /// 用于引用类型
        /// </summary>
        private Dictionary<string, object> m_blackBoardObjectData = new();
        /// <summary>
        /// 用于所有整数类型
        /// </summary>
        private Dictionary<string, long> m_blackBoardLongData = new();
        /// <summary>
        /// 用于所有浮点型
        /// </summary>
        private Dictionary<string, double> m_blackBoardDoubleData = new();
        
        /// <summary>
        /// 提供一个兜底的获取方法，如果不存在则抛出异常
        /// </summary>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public T GetBlackBoardValue<T>(string key)
        {
            // 检查可空类型的实际类型（如int?）
            Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            
            // 判断类型优先级：引用类型> long兼容类型 > double兼容类型 
            if (!targetType.IsValueType) // 引用类型
            {
                if (m_blackBoardObjectData.TryGetValue(key, out object objVal) && objVal is T typedObj)
                {
                    return typedObj;
                }
            }
            else if (IsLongCompatibleType(targetType))
            {
                if (m_blackBoardLongData.TryGetValue(key, out long longVal))
                {
                    return ConvertToType<T>(longVal);
                }
            }
            else if (IsDoubleCompatibleType(targetType))
            {
                if (m_blackBoardDoubleData.TryGetValue(key, out double doubleVal))
                {
                    return ConvertToType<T>(doubleVal);
                }
            }
    
            throw new KeyNotFoundException($"Key '{key}' not found in compatible dictionary");
        }

        public object GetBlackBoardObjectValue(string key)
        {
            if (m_blackBoardObjectData.TryGetValue(key, out object value))
            {
                return value;
            }

            return null;
        }
        
        public long GetBlackBoardLongValue(string key)
        {
            if (m_blackBoardLongData.TryGetValue(key, out long value))
            {
                return value;
            }

            return 0;
        }
        
        public double GetBlackBoardDoubleValue(string key)
        {
            if (m_blackBoardDoubleData.TryGetValue(key, out double value))
            {
                return value;
            }

            return 0;
        }

        public void SetBlackBoardValue<T>(string key, T value)
        {
            // 获取实际类型（处理可空类型）
            Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            // 类型判断优先级：long兼容类型 > double兼容类型 > 引用类型
            if (IsLongCompatibleType(targetType))
            {
                SetBlackBoardLongValue(key, ConvertToType<long>(value));
            }
            else if (IsDoubleCompatibleType(targetType))
            {
                SetBlackBoardDoubleValue(key, ConvertToType<double>(value));
            }
            else if (!targetType.IsValueType) // 引用类型
            {
                SetBlackBoardObjectValue(key,value);
            }
            else
            {
                throw new InvalidOperationException($"Unsupported type: {typeof(T)}");
            }
        }
        
        public void SetBlackBoardObjectValue(string key, object value)
        {
            if (m_blackBoardObjectData.ContainsKey(key))
            {
                m_blackBoardObjectData[key] = value;
            }
            else
            {
                m_blackBoardObjectData.Add(key, value);
            }
        }
        
        public void SetBlackBoardLongValue(string key, long value)
        {
            if (m_blackBoardObjectData.ContainsKey(key))
            {
                m_blackBoardLongData[key] = value;
            }
            else
            {
                m_blackBoardLongData.Add(key, value);
            }
        }
        
        public void SetBlackBoardDoubleValue(string key, double value)
        {
            if (m_blackBoardObjectData.ContainsKey(key))
            {
                m_blackBoardDoubleData[key] = value;
            }
            else
            {
                m_blackBoardDoubleData.Add(key, value);
            }
        }
        private static T ConvertToType<T>(object value)
        {
            Type targetType = typeof(T);
            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                // 处理枚举类型转换
                if (underlyingType.IsEnum)
                {
                    return (T)Enum.ToObject(underlyingType, value);
                }

                return (T)Convert.ChangeType(value, underlyingType);
            }
            catch (InvalidCastException ex)
            {
                throw new InvalidCastException(
                    $"Cannot convert value {value} of type {value.GetType()} to {typeof(T)}", ex);
            }
        }
        
        private static bool IsLongCompatibleType(Type type)
        {
            // 包含枚举类型及其基础类型
            if (type.IsEnum)
            {
                Type underlyingType = Enum.GetUnderlyingType(type);
                return underlyingType == typeof(long) ||
                       underlyingType == typeof(int) ||
                       underlyingType == typeof(short) ||
                       underlyingType == typeof(byte) ||
                       underlyingType == typeof(ulong) ||
                       underlyingType == typeof(uint) ||
                       underlyingType == typeof(ushort) ||
                       underlyingType == typeof(sbyte);
            }
    
            // 标准整数类型
            return type == typeof(long) || 
                   type == typeof(int) ||
                   type == typeof(short) ||
                   type == typeof(byte) ||
                   type == typeof(ulong) ||
                   type == typeof(uint) ||
                   type == typeof(ushort) ||
                   type == typeof(sbyte) ||
                   type == typeof(char);
        }
    
        private static bool IsDoubleCompatibleType(Type type)
        {
            return type == typeof(double) || 
                   type == typeof(float) ||
                   type == typeof(decimal);
        }
        
        #endregion
        
        #region Virtual Functions
        public abstract Type GetFieldEnumType();
        protected abstract void RegisterFields();
        #endregion

        #region Register and Get Field
        private struct RegisteredField
        {
            public TaskBridgeConstValue Value;

            public RegisteredField(object value)
            {
                Value = new() { ObjectValue = value };
            }

            public RegisteredField(bool value)
            {
                Value = new() { BoolValue = value };
            }

            public RegisteredField(byte value)
            {
                Value = new() { ByteValue = value };
            }

            public RegisteredField(short value)
            {
                Value = new() { ShortValue = value };
            }

            public RegisteredField(ushort value)
            {
                Value = new() { UshortValue = value };
            }

            public RegisteredField(int value)
            {
                Value = new() { IntValue = value };
            }

            public RegisteredField(uint value)
            {
                Value = new() { UintValue = value };
            }

            public RegisteredField(long value)
            {
                Value = new() { LongValue = value };
            }

            public RegisteredField(ulong value)
            {
                Value = new() { UlongValue = value };
            }

            public RegisteredField(float value)
            {
                Value = new() { FloatValue = value };
            }

            public RegisteredField(double value)
            {
                Value = new() { DoubleValue = value };
            }

            public RegisteredField(string value)
            {
                Value = new() { StringValue = value };
            }
        }

        private List<RegisteredField> m_FieldList = new();

        internal TaskContextExportInfo GenerateExportInfo()
        {
            RegisterFields();
            var res = new TaskContextExportInfo();
            res.TaskContextTypeName = this.GetType().Name;
            foreach (var pair in TaskDeserialiser.GetContextData(TypeId).FieldTypeDic)
            {
                var exportFieldInfo = new TaskExportFieldInfo();
                exportFieldInfo.FieldName = pair.Key;
                exportFieldInfo.TypeInfo = TaskApi.GenerateTaskTypeInfo(pair.Value);
                res.FieldInfos.Add(exportFieldInfo);
            }
            return res;
        }

        protected void RegisterBool<T>(T fieldEnum, bool value) where T : Enum
        {
            var contextData = TaskDeserialiser.GetContextData(TypeId);
            var enumValue = fieldEnum.GetHashCode();
            if (m_FieldList.Count <= enumValue)
                m_FieldList.ModifyCount(enumValue + 1);
            if (contextData.Inited == false)
            {
                contextData.FieldTypeDic[fieldEnum.ToString()] = typeof(bool);
                contextData.FieldStrIndexDic[fieldEnum.ToString()] = enumValue;
            }
            m_FieldList[enumValue] = new RegisteredField(value);
        }

        protected void RegisterByte<T>(T fieldEnum, byte value) where T : Enum
        {
            var contextData = TaskDeserialiser.GetContextData(TypeId);
            var enumValue = fieldEnum.GetHashCode();
            if (m_FieldList.Count <= enumValue)
                m_FieldList.ModifyCount(enumValue + 1);
            if (contextData.Inited == false)
            {
                contextData.FieldTypeDic[fieldEnum.ToString()] = typeof(byte);
                contextData.FieldStrIndexDic[fieldEnum.ToString()] = enumValue;
            }
            m_FieldList[enumValue] = new RegisteredField(value);
        }

        protected void RegisterShort<T>(T fieldEnum, short value) where T : Enum
        {
            var contextData = TaskDeserialiser.GetContextData(TypeId);
            var enumValue = fieldEnum.GetHashCode();
            if (m_FieldList.Count <= enumValue)
                m_FieldList.ModifyCount(enumValue + 1);
            if (contextData.Inited == false)
            {
                contextData.FieldTypeDic[fieldEnum.ToString()] = typeof(short);
                contextData.FieldStrIndexDic[fieldEnum.ToString()] = enumValue;
            }
            m_FieldList[enumValue] = new RegisteredField(value);
        }

        protected void RegisterUshort<T>(T fieldEnum, ushort value) where T : Enum
        {
            var contextData = TaskDeserialiser.GetContextData(TypeId);
            var enumValue = fieldEnum.GetHashCode();
            if (m_FieldList.Count <= enumValue)
                m_FieldList.ModifyCount(enumValue + 1);
            if (contextData.Inited == false)
            {
                contextData.FieldTypeDic[fieldEnum.ToString()] = typeof(ushort);
                contextData.FieldStrIndexDic[fieldEnum.ToString()] = enumValue;
            }
            m_FieldList[enumValue] = new RegisteredField(value);
        }

        protected void RegisterInt<T>(T fieldEnum, int value) where T : Enum
        {
            var contextData = TaskDeserialiser.GetContextData(TypeId);
            var enumValue = fieldEnum.GetHashCode();
            if (m_FieldList.Count <= enumValue)
                m_FieldList.ModifyCount(enumValue + 1);
            if (contextData.Inited == false)
            {
                contextData.FieldTypeDic[fieldEnum.ToString()] = typeof(int);
                contextData.FieldStrIndexDic[fieldEnum.ToString()] = enumValue;
            }
            m_FieldList[enumValue] = new RegisteredField(value);
        }

        protected void RegisterUint<T>(T fieldEnum, uint value) where T : Enum
        {
            var contextData = TaskDeserialiser.GetContextData(TypeId);
            var enumValue = fieldEnum.GetHashCode();
            if (m_FieldList.Count <= enumValue)
                m_FieldList.ModifyCount(enumValue + 1);
            if (contextData.Inited == false)
            {
                contextData.FieldTypeDic[fieldEnum.ToString()] = typeof(uint);
                contextData.FieldStrIndexDic[fieldEnum.ToString()] = enumValue;
            }
            m_FieldList[enumValue] = new RegisteredField(value);
        }

        protected void RegisterLong<T>(T fieldEnum, long value) where T : Enum
        {
            var contextData = TaskDeserialiser.GetContextData(TypeId);
            var enumValue = fieldEnum.GetHashCode();
            if (m_FieldList.Count <= enumValue)
                m_FieldList.ModifyCount(enumValue + 1);
            if (contextData.Inited == false)
            {
                contextData.FieldTypeDic[fieldEnum.ToString()] = typeof(long);
                contextData.FieldStrIndexDic[fieldEnum.ToString()] = enumValue;
            }
            m_FieldList[enumValue] = new RegisteredField(value);
        }

        protected void RegisterUlong<T>(T fieldEnum, ulong value) where T : Enum
        {
            var contextData = TaskDeserialiser.GetContextData(TypeId);
            var enumValue = fieldEnum.GetHashCode();
            if (m_FieldList.Count <= enumValue)
                m_FieldList.ModifyCount(enumValue + 1);
            if (contextData.Inited == false)
            {
                contextData.FieldTypeDic[fieldEnum.ToString()] = typeof(ulong);
                contextData.FieldStrIndexDic[fieldEnum.ToString()] = enumValue;
            }
            m_FieldList[enumValue] = new RegisteredField(value);
        }

        protected void RegisterFloat<T>(T fieldEnum, float value) where T : Enum
        {
            var contextData = TaskDeserialiser.GetContextData(TypeId);
            var enumValue = fieldEnum.GetHashCode();
            if (m_FieldList.Count <= enumValue)
                m_FieldList.ModifyCount(enumValue + 1);
            if (contextData.Inited == false)
            {
                contextData.FieldTypeDic[fieldEnum.ToString()] = typeof(float);
                contextData.FieldStrIndexDic[fieldEnum.ToString()] = enumValue;
            }
            m_FieldList[enumValue] = new RegisteredField(value);
        }

        protected void RegisterDouble<T>(T fieldEnum, double value) where T : Enum
        {
            var contextData = TaskDeserialiser.GetContextData(TypeId);
            var enumValue = fieldEnum.GetHashCode();
            if (m_FieldList.Count <= enumValue)
                m_FieldList.ModifyCount(enumValue + 1);
            if (contextData.Inited == false)
            {
                contextData.FieldTypeDic[fieldEnum.ToString()] = typeof(double);
                contextData.FieldStrIndexDic[fieldEnum.ToString()] = enumValue;
            }
            m_FieldList[enumValue] = new RegisteredField(value);
        }

        protected void RegisterString<T>(T fieldEnum, string value) where T : Enum
        {
            var contextData = TaskDeserialiser.GetContextData(TypeId);
            var enumValue = fieldEnum.GetHashCode();
            if (m_FieldList.Count <= enumValue)
                m_FieldList.ModifyCount(enumValue + 1);
            if (contextData.Inited == false)
            {
                contextData.FieldTypeDic[fieldEnum.ToString()] = typeof(string);
                contextData.FieldStrIndexDic[fieldEnum.ToString()] = enumValue;
            }
            m_FieldList[enumValue] = new RegisteredField(value);
        }

        protected void RegisterObject<TEnum, TObject>(TEnum fieldEnum, TObject value) where TEnum : Enum
        {
            var contextData = TaskDeserialiser.GetContextData(TypeId);
            var enumValue = fieldEnum.GetHashCode();
            if (m_FieldList.Count <= enumValue)
                m_FieldList.ModifyCount(enumValue + 1);
            if (contextData.Inited == false)
            {
                contextData.FieldTypeDic[fieldEnum.ToString()] = typeof(TEnum);
                contextData.FieldStrIndexDic[fieldEnum.ToString()] = enumValue;
            }
            m_FieldList[enumValue] = new RegisteredField(value);
        }

        internal TaskBridgeConstValue GetConstValue(int enumValue)
        {
            return m_FieldList[enumValue].Value;
        }

        internal int GetStrIndex(string str)
        {
            return TaskDeserialiser.GetContextData(TypeId).FieldStrIndexDic[str];
        }
        #endregion
    }
}
