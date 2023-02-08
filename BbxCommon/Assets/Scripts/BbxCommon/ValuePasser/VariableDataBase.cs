using System;

namespace BbxCommon.ValuePasserInternal
{
    public abstract class VariableDataBase : PooledObject
    {
        public abstract Type CurrentValueType(int valueEnum);
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
    public class VariableDataAttribute : Attribute
    {
        public Type[] VariableTypes;

        public VariableDataAttribute(params Type[] variableType)
        {
            VariableTypes = variableType;
        }
    }

    public interface IVariableWriter<T>
    {
        void SetValue(T value);
    }

    public interface IVariableReader<T>
    {
        T GetValue(int valueEnum);
    }
}
