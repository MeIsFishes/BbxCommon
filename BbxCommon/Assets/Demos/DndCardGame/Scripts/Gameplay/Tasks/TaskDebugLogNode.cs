using BbxCommon;
using BbxCommon.Internal;
using System;

namespace Dcg
{
    public class TaskDebugLogNode : TaskBase
    {
        public string Content;

        protected override void OnEnter()
        {
            DebugApi.Log(Content);
        }

        public enum EField
        {
            Content,
        }

        public override Type GetFieldEnumType()
        {
            return typeof(EField);
        }

        public override void ReadFieldInfo(int fieldEnum, TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            Content = ReadString(fieldInfo, context);
        }
    }
}
