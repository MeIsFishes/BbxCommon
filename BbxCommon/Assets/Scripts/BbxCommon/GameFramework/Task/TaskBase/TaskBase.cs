using System;
using System.Collections;
using System.Collections.Generic;
using BbxCommon.Internal;

namespace BbxCommon
{
    public enum ETaskRunState
    {
        None,
        Running,
        Succeeded,
        Failed,
    }

    public abstract class TaskBase : PooledObject
    {
        #region Lifecycle
        public bool IsRunning { get; private set; }

        private int m_TypeId;
        private bool m_RequiredStop = false;
        private bool m_BlockEnter = false;

        // EnterCondition: Check once when task executes Enter().
        // Condition: Check every frame. If failed, the task will be stopped and return failed.
        // ExitCondition: Check every frame. If succeeded, the task will be stopped and return succeeded.
        private TaskConnectPoint m_EnterCondition = new();
        private TaskConnectPoint m_Conditions = new();
        private TaskConnectPoint m_ExitConditions = new();

        public TaskBase()
        {
            m_TypeId = TaskDeserialiser.GetTaskTypeId(GetType());
            RegisterFields();
        }

        public void Run()
        {
            TaskManager.Instance.RunTask(this);
        }

        /// <summary>
        /// Force the task to stop and return <see cref="ETaskRunState.Succeeded"/>.
        /// </summary>
        public void Stop()
        {
            m_RequiredStop = true;
        }

        public void AddEnterCondition(TaskConditionBase condition)
        {
            m_EnterCondition.Tasks.Add(condition);
        }

        public void AddCondition(TaskConditionBase condition)
        {
            m_Conditions.Tasks.Add(condition);
        }

        public void AddExitCondition(TaskConditionBase condition)
        {
            m_ExitConditions.Tasks.Add(condition);
        }

        internal void Enter()
        {
            IsRunning = true;
            m_RequiredStop = false;
            m_BlockEnter = false;
            for (int i = 0; i < m_EnterCondition.Tasks.Count; i++)
            {
                var condition = m_EnterCondition.Tasks[i];
                condition.Enter();
                var state = condition.Update(0);
                condition.Exit();
                if (state == ETaskRunState.Failed)
                {
                    m_BlockEnter = true;
                    return;
                }
            }
            for (int i = 0; i < m_Conditions.Tasks.Count; i++)
            {
                m_Conditions.Tasks[i].Enter();
            }
            OnEnter();
        }

        public bool CanEnter()
        {
            for (int i = 0; i < m_EnterCondition.Tasks.Count; i++)
            {
                var condition = m_EnterCondition.Tasks[i];
                condition.Enter();
                var state = condition.Update(0);
                condition.Exit();
                if (state == ETaskRunState.Failed)
                {
                    m_BlockEnter = true;
                    return false;
                }
            }
            return true;
        }

        internal ETaskRunState Update(float deltaTime)
        {
            if (m_RequiredStop)
                return ETaskRunState.Succeeded;
            if (m_BlockEnter)
                return ETaskRunState.Failed;
            for (int i = 0; i < m_Conditions.Tasks.Count; i++)
            {
                if (m_Conditions.Tasks[i].Update(deltaTime) == ETaskRunState.Failed)
                {
                    return ETaskRunState.Failed;
                }
            }
            for (int i = 0; i < m_ExitConditions.Tasks.Count; i++)
            {
                if (m_ExitConditions.Tasks[i].Update(deltaTime) == ETaskRunState.Succeeded)
                {
                    return ETaskRunState.Succeeded;
                }
            }
            return OnUpdate(deltaTime);
        }

        internal void Exit()
        {
            IsRunning = false;
            if (m_BlockEnter)
                return;
            for (int i = 0; i < m_Conditions.Tasks.Count; i++)
            {
                m_Conditions.Tasks[i].Exit();
            }
            for (int i = 0; i < m_ExitConditions.Tasks.Count; i++)
            {
                m_ExitConditions.Tasks[i].Exit();
            }
            OnExit();
        }

        internal void OnNodeSucceeded() { OnSucceeded(); }
        internal void OnNodeFailed() { OnFailed(); }

        protected virtual void OnEnter() { }
        protected virtual ETaskRunState OnUpdate(float deltaTime) { return ETaskRunState.Succeeded; }
        protected virtual void OnExit() { }
        protected virtual void OnSucceeded() { }
        protected virtual void OnFailed() { }

        protected sealed override void OnAllocate()
        {
            OnTaskAllocate();
        }

        protected sealed override void OnCollect()
        {
            m_EnterCondition.Reset();
            m_Conditions.Reset();
            m_ExitConditions.Reset();
            OnTaskCollect();
        }

        protected virtual void OnTaskAllocate() { }
        protected virtual void OnTaskCollect() { }
        #endregion

        #region Read Field Info

        #region Common
        public void ReadFieldInfo(int fieldEnum, TaskBridgeFieldInfo fieldInfo, TaskContextBase context)
        {
            m_RegisteredFieldList[fieldEnum].FieldCallback(fieldInfo, context);
        }

        protected T ReadValue<T>(TaskBridgeFieldInfo fieldInfo, TaskContextBase context)
        {
            var res = default(T);
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    DebugApi.LogWarning("Task Value: Use other functions which meets the type required instead!");
                    break;
                case ETaskFieldValueSource.Context:
                    res = (T)context.GetConstValue(fieldInfo.ConstValue.IntValue).ObjectValue;
                    break;
                case ETaskFieldValueSource.Blackboard:
                    res = (T)context.GetBlackBoardObjectValue(fieldInfo.ConstValue.StringValue);
                    break;
            }
            if (res == null)
                res = (T)Activator.CreateInstance(typeof(T));
            return res;
        }
        #endregion

        #region Base Type
        protected bool ReadBool(TaskBridgeFieldInfo fieldInfo, TaskContextBase context)
        {
            // init
            bool res = default;
            if (fieldInfo.Inited == false)
            {
                var str = fieldInfo.ConstValue.StringValue;
                switch (fieldInfo.ValueSource)
                {
                    case ETaskFieldValueSource.Value:
                        if (bool.TryParse(str, out res) == false)
                        {
                            DebugApi.LogError("Task value parse failed! Task: " + this.GetType().Name + ", fieldEnumValue: " + fieldInfo.FieldEnumValue +
                                ", required type: bool, content: " + str);
                        }
                        fieldInfo.ConstValue.BoolValue = res;
                        break;
                    case ETaskFieldValueSource.Context:
                        fieldInfo.ConstValue.IntValue = context.GetStrIndex(fieldInfo.ConstValue.StringValue);
                        break;
                    case ETaskFieldValueSource.Blackboard:
                        break;
                }
            }
            // get value
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = fieldInfo.ConstValue.BoolValue;
                    break;
                case ETaskFieldValueSource.Context:
                    res = context.GetConstValue(fieldInfo.ConstValue.IntValue).BoolValue;
                    break;
                case ETaskFieldValueSource.Blackboard:
                    res = context.GetBlackBoardLongValue(fieldInfo.ConstValue.StringValue) > 0;
                    break;
            }
            return res;
        }

        protected short ReadShort(TaskBridgeFieldInfo fieldInfo, TaskContextBase context)
        {
            // init
            short res = default;
            if (fieldInfo.Inited == false)
            {
                var str = fieldInfo.ConstValue.StringValue;
                switch (fieldInfo.ValueSource)
                {
                    case ETaskFieldValueSource.Value:
                        if (short.TryParse(str, out res) == false)
                        {
                            DebugApi.LogError("Task value parse failed! Task: " + this.GetType().Name + ", fieldEnumValue: " + fieldInfo.FieldEnumValue +
                                ", required type: short, content: " + str);
                        }
                        fieldInfo.ConstValue.ShortValue = res;
                        break;
                    case ETaskFieldValueSource.Context:
                        fieldInfo.ConstValue.IntValue = context.GetStrIndex(fieldInfo.ConstValue.StringValue);
                        break;
                    case ETaskFieldValueSource.Blackboard:
                        break;
                }
            }
            // get value
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = fieldInfo.ConstValue.ShortValue;
                    break;
                case ETaskFieldValueSource.Context:
                    res = context.GetConstValue(fieldInfo.ConstValue.IntValue).ShortValue;
                    break;
                case ETaskFieldValueSource.Blackboard:
                    res = (short)context.GetBlackBoardLongValue(fieldInfo.ConstValue.StringValue);
                    break;
            }
            return res;
        }

        protected ushort ReadUshort(TaskBridgeFieldInfo fieldInfo, TaskContextBase context)
        {
            // init
            ushort res = default;
            if (fieldInfo.Inited == false)
            {
                var str = fieldInfo.ConstValue.StringValue;
                switch (fieldInfo.ValueSource)
                {
                    case ETaskFieldValueSource.Value:
                        if (ushort.TryParse(str, out res) == false)
                        {
                            DebugApi.LogError("Task value parse failed! Task: " + this.GetType().Name + ", fieldEnumValue: " + fieldInfo.FieldEnumValue +
                                ", required type: ushort, content: " + str);
                        }
                        fieldInfo.ConstValue.UshortValue = res;
                        break;
                    case ETaskFieldValueSource.Context:
                        fieldInfo.ConstValue.IntValue = context.GetStrIndex(fieldInfo.ConstValue.StringValue);
                        break;
                    case ETaskFieldValueSource.Blackboard:
                        break;
                }
            }
            // get value
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = fieldInfo.ConstValue.UshortValue;
                    break;
                case ETaskFieldValueSource.Context:
                    res = context.GetConstValue(fieldInfo.ConstValue.IntValue).UshortValue;
                    break;
                case ETaskFieldValueSource.Blackboard:
                    res = (ushort)context.GetBlackBoardLongValue(fieldInfo.ConstValue.StringValue);
                    break;
            }
            return res;
        }

        protected int ReadInt(TaskBridgeFieldInfo fieldInfo, TaskContextBase context)
        {
            // init
            int res = default;
            if (fieldInfo.Inited == false)
            {
                var str = fieldInfo.ConstValue.StringValue;
                switch (fieldInfo.ValueSource)
                {
                    case ETaskFieldValueSource.Value:
                        if (int.TryParse(str, out res) == false)
                        {
                            DebugApi.LogError("Task value parse failed! Task: " + this.GetType().Name + ", fieldEnumValue: " + fieldInfo.FieldEnumValue +
                                ", required type: int, content: " + str);
                        }
                        fieldInfo.ConstValue.IntValue = res;
                        break;
                    case ETaskFieldValueSource.Context:
                        fieldInfo.ConstValue.IntValue = context.GetStrIndex(fieldInfo.ConstValue.StringValue);
                        break;
                    case ETaskFieldValueSource.Blackboard:
                        break;
                }
            }
            // get value
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = fieldInfo.ConstValue.IntValue;
                    break;
                case ETaskFieldValueSource.Context:
                    res = context.GetConstValue(fieldInfo.ConstValue.IntValue).IntValue;
                    break;
                case ETaskFieldValueSource.Blackboard:
                    res = (int)context.GetBlackBoardLongValue(fieldInfo.ConstValue.StringValue);
                    break;
            }
            return res;
        }

        protected uint ReadUint(TaskBridgeFieldInfo fieldInfo, TaskContextBase context)
        {
            // init
            uint res = default;
            if (fieldInfo.Inited == false)
            {
                var str = fieldInfo.ConstValue.StringValue;
                switch (fieldInfo.ValueSource)
                {
                    case ETaskFieldValueSource.Value:
                        if (uint.TryParse(str, out res) == false)
                        {
                            DebugApi.LogError("Task value parse failed! Task: " + this.GetType().Name + ", fieldEnumValue: " + fieldInfo.FieldEnumValue +
                                ", required type: uint, content: " + str);
                        }
                        fieldInfo.ConstValue.UintValue = res;
                        break;
                    case ETaskFieldValueSource.Context:
                        fieldInfo.ConstValue.IntValue = context.GetStrIndex(fieldInfo.ConstValue.StringValue);
                        break;
                    case ETaskFieldValueSource.Blackboard:
                        break;
                }
            }
            // get value
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = fieldInfo.ConstValue.UintValue;
                    break;
                case ETaskFieldValueSource.Context:
                    res = context.GetConstValue(fieldInfo.ConstValue.IntValue).UintValue;
                    break;
                case ETaskFieldValueSource.Blackboard:
                    res = (uint)context.GetBlackBoardLongValue(fieldInfo.ConstValue.StringValue);
                    break;
            }
            return res;
        }

        protected long ReadLong(TaskBridgeFieldInfo fieldInfo, TaskContextBase context)
        {
            // init
            long res = default;
            if (fieldInfo.Inited == false)
            {
                var str = fieldInfo.ConstValue.StringValue;
                switch (fieldInfo.ValueSource)
                {
                    case ETaskFieldValueSource.Value:
                        if (long.TryParse(str, out res) == false)
                        {
                            DebugApi.LogError("Task value parse failed! Task: " + this.GetType().Name + ", fieldEnumValue: " + fieldInfo.FieldEnumValue +
                                ", required type: long, content: " + str);
                        }
                        fieldInfo.ConstValue.LongValue = res;
                        break;
                    case ETaskFieldValueSource.Context:
                        fieldInfo.ConstValue.IntValue = context.GetStrIndex(fieldInfo.ConstValue.StringValue);
                        break;
                    case ETaskFieldValueSource.Blackboard:
                        break;
                }
            }
            // get value
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = fieldInfo.ConstValue.LongValue;
                    break;
                case ETaskFieldValueSource.Context:
                    res = context.GetConstValue(fieldInfo.ConstValue.IntValue).LongValue;
                    break;
                case ETaskFieldValueSource.Blackboard:
                    res = context.GetBlackBoardLongValue(fieldInfo.ConstValue.StringValue);
                    break;
            }
            return res;
        }

        protected ulong ReadUlong(TaskBridgeFieldInfo fieldInfo, TaskContextBase context)
        {
            // init
            ulong res = default;
            if (fieldInfo.Inited == false)
            {
                var str = fieldInfo.ConstValue.StringValue;
                switch (fieldInfo.ValueSource)
                {
                    case ETaskFieldValueSource.Value:
                        if (ulong.TryParse(str, out res) == false)
                        {
                            DebugApi.LogError("Task value parse failed! Task: " + this.GetType().Name + ", fieldEnumValue: " + fieldInfo.FieldEnumValue +
                                ", required type: ulong, content: " + str);
                        }
                        fieldInfo.ConstValue.UlongValue = res;
                        break;
                    case ETaskFieldValueSource.Context:
                        fieldInfo.ConstValue.IntValue = context.GetStrIndex(fieldInfo.ConstValue.StringValue);
                        break;
                    case ETaskFieldValueSource.Blackboard:
                        break;
                }
            }
            // get value
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = fieldInfo.ConstValue.UlongValue;
                    break;
                case ETaskFieldValueSource.Context:
                    res = context.GetConstValue(fieldInfo.ConstValue.IntValue).UlongValue;
                    break;
                case ETaskFieldValueSource.Blackboard:
                    res = (ulong)context.GetBlackBoardLongValue(fieldInfo.ConstValue.StringValue);
                    break;
            }
            return res;
        }

        protected float ReadFloat(TaskBridgeFieldInfo fieldInfo, TaskContextBase context)
        {
            // init
            float res = default;
            if (fieldInfo.Inited == false)
            {
                var str = fieldInfo.ConstValue.StringValue;
                switch (fieldInfo.ValueSource)
                {
                    case ETaskFieldValueSource.Value:
                        if (float.TryParse(str, out res) == false)
                        {
                            DebugApi.LogError("Task value parse failed! Task: " + this.GetType().Name + ", fieldEnumValue: " + fieldInfo.FieldEnumValue +
                                ", required type: float, content: " + str);
                        }
                        fieldInfo.ConstValue.FloatValue = res;
                        break;
                    case ETaskFieldValueSource.Context:
                        fieldInfo.ConstValue.IntValue = context.GetStrIndex(fieldInfo.ConstValue.StringValue);
                        break;
                    case ETaskFieldValueSource.Blackboard:
                        break;
                }
            }
            // get value
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = fieldInfo.ConstValue.FloatValue;
                    break;
                case ETaskFieldValueSource.Context:
                    res = context.GetConstValue(fieldInfo.ConstValue.IntValue).FloatValue;
                    break;
                case ETaskFieldValueSource.Blackboard:
                    res = (float)context.GetBlackBoardDoubleValue(fieldInfo.ConstValue.StringValue);
                    break;
            }
            return res;
        }

        protected double ReadDouble(TaskBridgeFieldInfo fieldInfo, TaskContextBase context)
        {
            // init
            double res = default;
            if (fieldInfo.Inited == false)
            {
                var str = fieldInfo.ConstValue.StringValue;
                switch (fieldInfo.ValueSource)
                {
                    case ETaskFieldValueSource.Value:
                        if (double.TryParse(str, out res) == false)
                        {
                            DebugApi.LogError("Task value parse failed! Task: " + this.GetType().Name + ", fieldEnumValue: " + fieldInfo.FieldEnumValue +
                                ", required type: double, content: " + str);
                        }
                        fieldInfo.ConstValue.DoubleValue = res;
                        break;
                    case ETaskFieldValueSource.Context:
                        fieldInfo.ConstValue.IntValue = context.GetStrIndex(fieldInfo.ConstValue.StringValue);
                        break;
                    case ETaskFieldValueSource.Blackboard:
                        break;
                }
            }
            // get value
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = fieldInfo.ConstValue.DoubleValue;
                    break;
                case ETaskFieldValueSource.Context:
                    res = context.GetConstValue(fieldInfo.ConstValue.IntValue).DoubleValue;
                    break;
                case ETaskFieldValueSource.Blackboard:
                    res = context.GetBlackBoardDoubleValue(fieldInfo.ConstValue.StringValue);
                    break;
            }
            return res;
        }

        protected T ReadEnum<T>(TaskBridgeFieldInfo fieldInfo, TaskContextBase context) where T : Enum
        {
            T res = default;
            // init
            if (fieldInfo.Inited == false)
            {
                var str = fieldInfo.ConstValue.StringValue;
                switch (fieldInfo.ValueSource)
                {
                    case ETaskFieldValueSource.Value:
                        if (Enum.TryParse(typeof(T), str, out var obj) == false)
                        {
                            DebugApi.LogError("Task value parse failed! Task: " + this.GetType().Name + ", fieldEnumValue: " + fieldInfo.FieldEnumValue +
                                ", required type: " + typeof(T).Name + ", content: " + str);
                        }
                        fieldInfo.ConstValue.ObjectValue = (T)obj;
                        break;
                    case ETaskFieldValueSource.Context:
                        fieldInfo.ConstValue.IntValue = context.GetStrIndex(fieldInfo.ConstValue.StringValue);
                        break;
                    case ETaskFieldValueSource.Blackboard:
                        break;
                }
            }
            // get value
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = (T)fieldInfo.ConstValue.ObjectValue;
                    break;
                case ETaskFieldValueSource.Context:
                    res = (T)context.GetConstValue(fieldInfo.ConstValue.IntValue).ObjectValue;
                    break;
                case ETaskFieldValueSource.Blackboard:
                    res = context.GetBlackBoardValue<T>(fieldInfo.ConstValue.StringValue);
                    break;
            }
            return res;
        }

        protected string ReadString(TaskBridgeFieldInfo fieldInfo, TaskContextBase context)
        {
            // init
            string res = default;
            if (fieldInfo.Inited == false)
            {
                var str = fieldInfo.ConstValue.StringValue;
                switch (fieldInfo.ValueSource)
                {
                    case ETaskFieldValueSource.Value:
                        break;
                    case ETaskFieldValueSource.Context:
                        fieldInfo.ConstValue.IntValue = context.GetStrIndex(fieldInfo.ConstValue.StringValue);
                        break;
                    case ETaskFieldValueSource.Blackboard:
                        break;
                }
            }
            // get value
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = fieldInfo.ConstValue.StringValue;
                    break;
                case ETaskFieldValueSource.Context:
                    res = context.GetConstValue(fieldInfo.ConstValue.IntValue).StringValue;
                    break;
                case ETaskFieldValueSource.Blackboard:
                    res = (string)context.GetBlackBoardObjectValue(fieldInfo.ConstValue.StringValue);
                    break;
            }
            return res;
        }
        #endregion

        #region Special Type
        protected List<T> ReadList<T>(TaskBridgeFieldInfo fieldInfo, TaskContextBase context, List<T> res, bool isConnectPoint = false)
        {
            res ??= new List<T>();
            res.Clear();
            // init
            if (fieldInfo.Inited == false)
            {
                var str = fieldInfo.ConstValue.StringValue;
                switch (fieldInfo.ValueSource)
                {
                    case ETaskFieldValueSource.Value:
                        if (fieldInfo.ConstValue.StringValue.IsNullOrEmpty()) // default value
                            return res;
                        var elements = fieldInfo.ConstValue.StringValue.Split(TaskExportCrossVariable.ListElementSplit, StringSplitOptions.RemoveEmptyEntries);
                        if (res is List<bool> boolList)
                        {
                            for (int i = 0; i < elements.Length; i++)
                            {
                                if (bool.TryParse(elements[i], out var val))
                                {
                                    boolList.Add(val);
                                }
                            }
                        }
                        else if (res is List<short> shortList)
                        {
                            for (int i = 0; i < elements.Length; i++)
                            {
                                if (short.TryParse(elements[i], out var val))
                                {
                                    shortList.Add(val);
                                }
                            }
                        }
                        else if (res is List<ushort> ushortList)
                        {
                            for (int i = 0; i < elements.Length; i++)
                            {
                                if (ushort.TryParse(elements[i], out var val))
                                {
                                    ushortList.Add(val);
                                }
                            }
                        }
                        else if (res is List<int> intList)
                        {
                            for (int i = 0; i < elements.Length; i++)
                            {
                                if (int.TryParse(elements[i], out var val))
                                {
                                    intList.Add(val);
                                }
                            }
                            if (isConnectPoint)
                            {
                                for (int i = 0; i < intList.Count; i++)
                                {
                                    intList[i] = context.BindingTaskGroupInfo.ReorderedIndexDic[intList[i]];
                                }
                            }
                        }
                        else if (res is List<uint> uintList)
                        {
                            for (int i = 0; i < elements.Length; i++)
                            {
                                if (uint.TryParse(elements[i], out var val))
                                {
                                    uintList.Add(val);
                                }
                            }
                        }
                        else if (res is List<long> longList)
                        {
                            for (int i = 0; i < elements.Length; i++)
                            {
                                if (long.TryParse(elements[i], out var val))
                                {
                                    longList.Add(val);
                                }
                            }
                        }
                        else if (res is List<ulong> ulongList)
                        {
                            for (int i = 0; i < elements.Length; i++)
                            {
                                if (ulong.TryParse(elements[i], out var val))
                                {
                                    ulongList.Add(val);
                                }
                            }
                        }
                        else if (res is List<float> floatList)
                        {
                            for (int i = 0; i < elements.Length; i++)
                            {
                                if (float.TryParse(elements[i], out var val))
                                {
                                    floatList.Add(val);
                                }
                            }
                        }
                        else if (res is List<double> doubleList)
                        {
                            for (int i = 0; i < elements.Length; i++)
                            {
                                if (double.TryParse(elements[i], out var val))
                                {
                                    doubleList.Add(val);
                                }
                            }
                        }
                        else if (res is List<string> stringList)
                        {
                            for (int i = 0; i < elements.Length; i++)
                            {
                                stringList.Add(elements[i]);
                            }
                        }
                        else if (typeof(T).IsEnum)
                        {
                            var addMethod = res.GetType().GetMethod("Add");
                            for (int i = 0; i < elements.Length; i++)
                            {
                                if (Enum.TryParse(typeof(T), elements[i], out var val))
                                {
                                    addMethod.Invoke(res, new object[] { val });
                                }
                            }
                        }
                        else
                        {
                            DebugApi.LogWarning("Task Value: Parse custom types for collections are currently not supported!");
                        }
                        fieldInfo.ConstValue.ObjectValue = res;
                        break;
                    case ETaskFieldValueSource.Context:
                        fieldInfo.ConstValue.IntValue = context.GetStrIndex(fieldInfo.ConstValue.StringValue);
                        break;
                    case ETaskFieldValueSource.Blackboard:
                        break;
                }
            }
            // get value
            switch (fieldInfo.ValueSource)
            {
                case ETaskFieldValueSource.Value:
                    res = (List<T>)fieldInfo.ConstValue.ObjectValue;
                    break;
                case ETaskFieldValueSource.Context:
                    res.AddList((List<T>)context.GetConstValue(fieldInfo.ConstValue.IntValue).ObjectValue);
                    break;
                case ETaskFieldValueSource.Blackboard:
                    res.AddList((List<T>)context.GetBlackBoardObjectValue(fieldInfo.ConstValue.StringValue));
                    break;
            }
            return res;
        }
        #endregion

        #region Task Connect Point
        /// <summary>
        /// For task instances should be deserialized first, it can't read refrences during deserialization. We cache those
        /// connect point and then initialize them after deserialization is done.
        /// </summary>
        private List<TaskConnectPoint> m_CachedConnectPoint = new();

        /// <summary>
        /// Read task refrences for child nodes. Most frequently used in low-level drive nodes, such as Sequence, Selector in
        /// behavior tree.
        /// </summary>
        protected TaskConnectPoint ReadConnectPoint(TaskBridgeFieldInfo fieldInfo, TaskContextBase context)
        {
            var res = new TaskConnectPoint();
            res.TaskRefrenceIds = ReadList(fieldInfo, context, res.TaskRefrenceIds, true);
            m_CachedConnectPoint.Add(res);
            return res;
        }

        internal void InitConnectPoint(List<TaskBase> taskList)
        {
            if (m_CachedConnectPoint.Count == 0)
                return;
            for (int i = 0; i < m_CachedConnectPoint.Count; i++)
            {
                var connectPoint = m_CachedConnectPoint[i];
                for (int j = 0; j < connectPoint.TaskRefrenceIds.Count; j++)
                {
                    connectPoint.Tasks.Add(taskList[connectPoint.TaskRefrenceIds[j]]);
                }
                connectPoint.TaskRefrenceIds.Clear();
            }
            m_CachedConnectPoint.Clear();
        }
        #endregion

        #endregion

        #region Register Field Info
        internal class RegisteredField
        {
            internal string Name;
            internal TaskExportTypeInfo TypeInfo;
            internal Action<TaskBridgeFieldInfo, TaskContextBase> FieldCallback;
        }

        private List<RegisteredField> m_RegisteredFieldList = new();

        internal TaskExportInfo GenerateExportInfo()
        {
            RegisterFields();
            var res = new TaskExportInfo();
            res.TaskTypeName = this.GetType().Name;
            res.TaskFullTypeName = this.GetType().FullName;
            // tags
            var attributes = this.GetType().GetCustomAttributes(true);
            bool overriden = false;
            foreach (var attribute in attributes)
            {
                if (attribute is TaskTagAttribute tagAttribute)
                {
                    res.Tags.AddRange(tagAttribute.Tags);
                    overriden = tagAttribute.SetTag == TaskTagAttribute.ESetTag.Override;
                }
            }
            if (overriden == false)
            {
                if (this is TaskDurationBase)
                {
                    res.Tags.Add(TaskExportCrossVariable.TaskTagAction);
                    res.Tags.Add(TaskExportCrossVariable.TaskTagDuration);
                }
                else if (this is TaskOnceBase)
                {
                    res.Tags.Add(TaskExportCrossVariable.TaskTagAction);
                    res.Tags.Add(TaskExportCrossVariable.TaskTagOnce);
                }
                else if (this is TaskConditionBase)
                {
                    res.Tags.Add(TaskExportCrossVariable.TaskTagCondition);
                }
                else // hasn't been derrived
                {
                    res.Tags.Add(TaskExportCrossVariable.TaskTagAction);
                    res.Tags.Add(TaskExportCrossVariable.TaskTagNormal);
                }
            }
            // fields
            foreach (var field in m_RegisteredFieldList)
            {
                var exportFieldInfo = new TaskExportFieldInfo();
                exportFieldInfo.FieldName = field.Name;
                exportFieldInfo.TypeInfo = field.TypeInfo;
                res.FieldInfos.Add(exportFieldInfo);
            }
            return res;
        }

        protected abstract void RegisterFields();

        protected void RegisterField<TEnum, TObj>(TEnum fieldEnum, TObj obj, Action<TaskBridgeFieldInfo, TaskContextBase> fieldCallback) where TEnum : Enum
        {
            var fieldEnumString = fieldEnum.ToString();
            var fieldEnumValue = TaskDeserialiser.GetTaskFieldEnum(m_TypeId, fieldEnumString);
            if (m_RegisteredFieldList.Count < fieldEnumValue + 1)
                m_RegisteredFieldList.ModifyCount(fieldEnumValue + 1);
            if (m_RegisteredFieldList[fieldEnumValue] == null)
                m_RegisteredFieldList[fieldEnumValue] = new();
            var field = m_RegisteredFieldList[fieldEnumValue];
            field.Name = fieldEnumString;
            field.TypeInfo = TaskApi.GenerateTaskTypeInfo(typeof(TObj), obj);
            field.FieldCallback = fieldCallback;
        }
        #endregion
    }

    #region Task Extension
    public static class TaskExtension
    {
        public static TaskValueInfo CreateTaskValueInfo<T>(this TaskGroupInfo groupInfo, int id)
        {
            var info = new TaskValueInfo();
            info.FullTypeName = typeof(T).FullName;
            groupInfo.TaskInfos[id] = info;
            return info;
        }

        public static TaskValueInfo CreateTaskTimelineValueInfo(this TaskGroupInfo groupInfo, int id, float duration)
        {
            var info = groupInfo.CreateTaskValueInfo<TaskTimeline>(id);
            info.AddFieldInfo(TaskTimeline.EField.Duration, duration);
            return info;
        }
    }
    #endregion
}
