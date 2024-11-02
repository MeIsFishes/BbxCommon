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
            DebugApi.Log(TimeApi.Time.ToString() + ": " + Content);
        }

        public enum EField
        {
            Content,
        }

        protected override Type GetFieldEnumType()
        {
            return typeof(EField);
        }

        protected override void RegisterField()
        {
            RegisterField(EField.Content, Content);
        }

        public override void ReadFieldInfo(int fieldEnum, TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            switch (fieldEnum)
            {
                case (int)EField.Content:
                    Content = ReadString(fieldInfo, context);
                    break;
            }
        }
    }
}
