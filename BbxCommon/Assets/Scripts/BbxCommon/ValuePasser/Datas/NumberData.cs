using System;

namespace BbxCommon.ValuePasserInternal
{
    [VariableData(typeof(int), typeof(uint), typeof(float))]
    public class NumberData : VariableDataBase, IVariableWriter<float>, IVariableWriter<int>, IVariableWriter<uint>,
        IVariableReader<float>, IVariableReader<int>, IVariableReader<uint>
    {
        public enum EValueType
        {
            Self,
        }

        private float m_Value;
        public EValueType ValueType;

        public override Type CurrentValueType(int valueEnum)
        {
            return null;
        }

        void IVariableWriter<float>.SetValue(float value)
        {
            m_Value = value;
        }

        void IVariableWriter<int>.SetValue(int value)
        {
            m_Value = value;
        }

        void IVariableWriter<uint>.SetValue(uint value)
        {
            m_Value = value;
        }

        float IVariableReader<float>.GetValue(int valueEnum)
        {
            return m_Value;
        }

        int IVariableReader<int>.GetValue(int valueEnum)
        {
            return (int)m_Value;
        }

        uint IVariableReader<uint>.GetValue(int valueEnum)
        {
            return (uint)m_Value;
        }
    }
}
