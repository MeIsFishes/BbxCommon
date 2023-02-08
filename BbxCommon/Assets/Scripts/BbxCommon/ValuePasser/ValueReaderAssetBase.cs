using System;
using UnityEngine;

namespace BbxCommon.ValuePasserInternal
{
    public class ValueReaderAssetBase : ScriptableObject
    {
        
    }

    [Serializable]
    public struct ValueReaderItem<T>
    {
        public enum EGetThrough
        {
            SetValue,
            VriablePass,
            NumberDic,
            ObjectDic,
        }

        public string VariableName;
        public EGetThrough GetThrough;
        public T Value;
        public string DicKey;

        public string VariableDataKey;
        public int VariableValueEnum;

        public ValueReaderItem(string variableName)
        {
            VariableName = variableName;
            GetThrough = EGetThrough.SetValue;
            Value = default(T);
            DicKey = null;
            VariableDataKey = "";
            VariableValueEnum = 0;
        }

        public T GetValue(ValuePasser valuePasser)
        {
            switch (GetThrough)
            {
                case EGetThrough.SetValue:
                    return Value;
                case EGetThrough.VriablePass:
                    var reader = valuePasser.VariablePasser.VariableDatas[VariableDataKey];
                    if (reader is IVariableReader<T> variableReader)
                        return variableReader.GetValue(VariableValueEnum);
                    break;
                case EGetThrough.NumberDic:
                    var number = valuePasser.NumberDic[DicKey];
                    if (number is IVariableReader<T> numberReader)
                        return numberReader.GetValue(VariableValueEnum);
                    break;
                case EGetThrough.ObjectDic:
                    var obj = valuePasser.ObjectDic[DicKey];
                    if (obj is T objRes)
                        return objRes;
                    break;
            }
            return default(T);
        }
    }
}
