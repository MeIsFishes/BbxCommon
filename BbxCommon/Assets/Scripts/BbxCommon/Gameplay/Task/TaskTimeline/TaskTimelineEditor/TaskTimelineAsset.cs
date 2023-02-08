using System;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    [Serializable]
    public class TaskTimelineAsset : ScriptableObject
    {
        public List<CreateTaskData> CreateTaskDatas = new List<CreateTaskData>();
    }
}
