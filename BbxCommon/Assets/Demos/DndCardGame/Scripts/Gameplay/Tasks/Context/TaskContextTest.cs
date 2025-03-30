using System;
using System.Collections.Generic;
using BbxCommon;

namespace Dcg
{
    public class TaskContextTest : TaskContextBase
    {
        public string DebugContent;
        public int Num;
        public long Content1;
        public double Content2;
        public List<int> Content3 = new List<int>();

        public enum EField
        {
            DebugContent,
            Num,
            BlackLong,
            BlackDouble,
            BlackObject,
        }

        public override Type GetFieldEnumType()
        {
            return typeof(EField);
        }

        protected override void RegisterFields()
        {
            RegisterString(EField.DebugContent, DebugContent);
            RegisterInt(EField.Num, Num);
            RegisterLong(EField.BlackLong, Content1);
            RegisterDouble(EField.BlackDouble, Content2);
            RegisterObject(EField.BlackObject, Content3);
        }

        protected override void OnContextCollect()
        {
            Content1 = 0;
            Content2 = 0;
            Content3.Clear();
            DebugContent = string.Empty;
            Num = 0;
        }
    }
}
