using System;
using BbxCommon;

namespace Dcg
{
    public class TaskContextTest : TaskContextBase
    {
        public string DebugContent;
        public int Num;

        public enum EField
        {
            DebugContent,
            Num,
        }

        public override Type GetFieldEnumType()
        {
            return typeof(EField);
        }

        protected override void RegisterFields()
        {
            RegisterString(EField.DebugContent, DebugContent);
            RegisterInt(EField.Num, Num);
        }
    }
}
