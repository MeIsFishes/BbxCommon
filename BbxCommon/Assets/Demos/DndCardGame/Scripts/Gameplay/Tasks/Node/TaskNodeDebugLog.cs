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

        protected override void RegisterFields()
        {
            RegisterField(EField.Content, Content, (fieldInfo, context) => { Content = ReadString(fieldInfo, context); });
        }

        protected override void OnTaskCollect()
        {
            Content = string.Empty;
        }

        protected override void OnEnter()
        {
            DebugApi.Log("TaskNodeDebugLog: " + Content);
        }
    }
}
