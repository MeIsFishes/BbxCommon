using System;
using System.Collections.Generic;
using BbxCommon;
using UnityEngine;

namespace Dcg
{
    public class TaskNodeDebugLog : TaskBase
    {
        public string Content;

        public enum EField
        {
            Content,
        }

        protected override Type GetFieldEnumType()
        {
            return typeof(EField);
        }

        protected override void RegisterFields()
        {
            RegisterField(EField.Content, Content);
        }

        public override void OnCollect()
        {
            Content = string.Empty;
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

        protected override void OnEnter()
        {
            DebugApi.Log("TaskNodeDebugLog: " + Content);
        }
    }
}
