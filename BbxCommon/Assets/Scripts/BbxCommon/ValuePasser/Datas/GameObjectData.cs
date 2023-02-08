using System;
using UnityEngine;

namespace BbxCommon.ValuePasserInternal
{
    [VariableData(typeof(GameObject)), Serializable]
    public class GameObjectData : VariableDataBase, IVariableWriter<GameObject>, IVariableReader<GameObject>, IVariableReader<Vector3>
    {
        public enum EValueType
        {
            Self,
            Position,
            Scale,
            Forward,
        }

        private GameObject m_Value;
        public EValueType ValueType;

        void IVariableWriter<GameObject>.SetValue(GameObject value)
        {
            m_Value = value;
        }

        GameObject IVariableReader<GameObject>.GetValue(int valueEnum)
        {
            if ((EValueType)valueEnum == EValueType.Self)
                return m_Value;
            return null;
        }

        Vector3 IVariableReader<Vector3>.GetValue(int valueEnum)
        {
            switch ((EValueType)valueEnum)
            {
                case EValueType.Position:
                    return m_Value.transform.position;
                case EValueType.Scale:
                    return m_Value.transform.localScale;
                case EValueType.Forward:
                    return m_Value.transform.forward;
            }
            return Vector3.zero;
        }

        public override Type CurrentValueType(int valueEnum)
        {
            switch ((EValueType)valueEnum)
            {
                case EValueType.Self:
                    return typeof(GameObject);
                case EValueType.Position:
                case EValueType.Scale:
                case EValueType.Forward:
                    return typeof(Vector3);
            }
            return null;
        }
    }
}
