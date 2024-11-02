using BbxCommon;
using System;

namespace Dcg
{
    public class TaskTestContext : TaskContextBase
    {
        public string DebugContent;

        public enum EField
        {
            DebugContent,
        }

        public override Type GetFieldEnumType()
        {
            return typeof(EField);
        }

        protected override void RegisterFields()
        {
            RegisterString(EField.DebugContent, DebugContent);
        }
    }
}
