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

        protected override void RegisterFields()
        {
            RegisterField(EField.Num, Num, (fieldInfo, context) => { Num = ReadInt(fieldInfo, context); });
        }

        protected override void OnConditionCollect()
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
