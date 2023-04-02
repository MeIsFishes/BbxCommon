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
            var timeline = TaskTimeline.Create();   // ����timeline����ʼ����Ϣ
            timeline.Duration = 5;  // timeline����ĳ���ʱ��
            timeline.StopTasks = true;  // ����timeline����ʱ�Ƿ���ֹ��������task����Ϊfalse����ֻ���𴴽�����������ֹ
            timeline.Context = new ContextTest();   // ����һ��context��context��Ҫ�Ǽ�¼task����������ģ��ڲ�ͬ��Ӧ�ó����п���ʹ�ò�ͬ��context
            timeline.Context.Caster = gameObject;    // ��context��Ա��ֵ
            timeline.AddCreateTaskData(new CreateTaskData(
                CreatingTime: 0.0f, // ��ʲôʱ��㴴��
                CreatingTaskFunc: (TaskTimelineContextBase context) =>  // ʹ�������������������context
                {
                    ContextTest taskContext = (ContextTest)context;
                    /* ��Manager��ֱ�Ӵ���һ��task���ڵײ��У�task������timeline��tick�����ֱ�����Ե�Manager���,�����ƿ���ʵ��task����Ŀɲ�װ���ԣ�������task�ȿ���
                     * ���뵽timeline�����У�Ҳ�����ڳ���ĸ���С������������С� */
                    var task = TaskManager<TaskFlicker>.Instance.CreateTask();
                    task.Init(context.Caster, 0.5f, 2f);    // Լ���׳ɣ�ÿ��task��Ӧ��дһ��Init()�������Ա��ⲿ���ó�ʼ�����������ʾ��һ�����ʹ��context�����ʼ��
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
            timeline.Start();   // ������timeline֮��Ҫ�ֶ�start����ʱ���ἴʱ����CreatingTime = 0�Ĵ�������
        }
    }
}
