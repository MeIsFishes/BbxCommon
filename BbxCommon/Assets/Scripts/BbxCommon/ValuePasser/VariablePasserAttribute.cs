using System;

namespace BbxCommon
{
    /// <summary>
    /// Set the class you are willing to show in editor and set as reader by this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class VariableReaderAttribute : Attribute
    {
        public string ClassName;
    }

    /// <summary>
    /// Set the class you are willing to show in editor and set as writer by this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class VariableWriterAttribute : Attribute
    {
        public string ClassName;
    }

    /// <summary>
    /// Set variables in the class you are willing to show in editor and set as reader by this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = true)]
    public class VariableReaderMemberAttribute : Attribute
    {
        public Type MemberType;
        public string MemberName;
    }

    /// <summary>
    /// Set variables in the class you are willing to show in editor and set as reader by this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.GenericParameter, AllowMultiple = false, Inherited = true)]
    public class VariableWriterMemberAttribute : Attribute
    {
        public Type MemberType;
        public string MemberName;
    }
}
