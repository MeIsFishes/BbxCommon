using System;
using BbxCommon;

namespace Dcg
{
    public class TaskConditionGreaterThan5 : TaskConditionBase
    {
        public int Num;

        public enum EField
        {
            Num,
        }

        protected override Type GetFieldEnumType()
        {
            return typeof(EField);
        }

        protected override void RegisterFields()
        {
            RegisterField(EField.Num, Num);
        }

        public override void ReadFieldInfo(int fieldEnum, TaskBridgeFieldInfo fieldInfo, TaskContextBase context)
        {
            Num = ReadInt(fieldInfo, context);
        }

        public override void OnCollect()
        {
            Num = 0;
        }

        protected override EConditionState OnConditionUpdate(float deltaTime)
        {
            if (Num > 5)
                return EConditionState.Succeeded;
            else
                return EConditionState.Failed;
        }
    }
}
