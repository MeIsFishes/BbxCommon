using UnityEngine;
using BbxCommon;

namespace Nnp
{
    public class TestTaskTimeline : MonoBehaviour
    {
        public class ContextTest : TaskTimelineContextBase
        {
            
        }

        void Start()
        {
            var timeline = TaskTimeline.Create();   // 创建timeline并初始化信息
            timeline.Duration = 5;  // timeline自身的持续时间
            timeline.StopTasks = true;  // 设置timeline结束时是否终止属于它的task，若为false，则只负责创建，不进行终止
            timeline.Context = new ContextTest();   // 创建一个context，context主要是记录task所需的上下文，在不同的应用场景中可能使用不同的context
            timeline.Context.Caster = gameObject;    // 给context成员赋值
            timeline.AddCreateTaskData(new CreateTaskData(
                CreatingTime: 0.0f, // 在什么时间点创建
                CreatingTaskFunc: (TaskTimelineContextBase context) =>  // 使用匿名函数创建具体的context
                {
                    ContextTest taskContext = (ContextTest)context;
                    /* 从Manager中直接创建一个task，在底层中，task并不由timeline来tick，而分别交予各自的Manager完成,这个设计可以实现task本身的可拆装特性，即单个task既可以
                     * 加入到timeline中运行，也可以在程序的各个小场景里独立运行。 */
                    var task = TaskManager<TaskFlicker>.Instance.CreateTask();
                    task.Init(context.Caster, 0.5f, 2f);    // 约定俗成，每个task都应该写一个Init()函数，以便外部调用初始化。这里简单演示了一下如何使用context参与初始化
                    return task;
                }));
            timeline.AddCreateTaskData(new CreateTaskData(
                CreatingTime: 4.0f,
                CreatingTaskFunc: (TaskTimelineContextBase context) =>
                {
                    ContextTest taskContext = (ContextTest)context;
                    var task = TaskManager<TaskFadeTo>.Instance.CreateTask();
                    task.Init(taskContext.Caster, 0f, 2f);
                    return task;
                }));
            timeline.Start();   // 创建了timeline之后要手动start，此时它会即时创建CreatingTime = 0的创建任务
        }
    }
}
