using System;
using System.Collections.Generic;
using BbxCommon;
using UnityEngine;

namespace Dcg
{
    public class TaskNodeDebugLog : TaskBase
    {
        public string Content;
        public long Content1;
        public double Content2;
        public List<int> Content3 = new List<int>();

        public enum EField
        {
            Content,
            BlackLong,
            BlackDouble,
            BlackObject,
        }

        protected override Type GetFieldEnumType()
        {
            return typeof(EField);
        }

        protected override void RegisterFields()
        {
            RegisterField(EField.Content, Content);
            RegisterField(EField.BlackLong, Content1);
            RegisterField(EField.BlackDouble, Content2);
            RegisterField(EField.BlackObject, Content3);
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
                case (int)EField.BlackLong:
                    Content1 = ReadLong(fieldInfo, context);
                    break;
                case (int)EField.BlackDouble:
                    Content2 = ReadDouble(fieldInfo, context);
                    break;
                case (int)EField.BlackObject:
                    Content3 = ReadValue<List<int>>(fieldInfo, context);
                    break;
            }
        }

        protected override void OnEnter()
        {
            //打印list内容
            var s = "";
            foreach (var i in Content3)
            {
                s += i + " ";
            }
            DebugApi.Log(TimeApi.Time.ToString() + ": " + Content + " " + Content1 + " " + Content2 + " " + s + "count: " + Content3.Count);
        }
    }
}
