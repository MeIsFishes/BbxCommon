using System.Collections.Generic;
using Godot;
using System.IO;
using System.Xml.Linq;
using BbxCommon;

/// <summary>
/// task 图源编辑器
/// </summary>
public partial class TaskGraphManager : GraphEdit
{
    [Export] public PackedScene BaseTaskScene; // 预制体：基础节点
    [Export] public string TaskType { get; set; }

    private PopupMenu _contextMenu;
    private BaseGraphTask _currentGraphNode;
    private EditorModel.NodeGraphSaveTargetData _saveTargetData;
    public EditorModel.NodeGraphSaveTargetData SaveTargetData
    {
        get
        {
            if (_saveTargetData == null)
            {
                _saveTargetData = EditorModel.NodeGraphSaveTarget;
            }
            return _saveTargetData;
        }
    }
    
    public override void _Ready()
    {
        // 初始化右键菜单
        _contextMenu = GetNode<PopupMenu>("ContextMenu");
        AddContextMenuItem();
        _contextMenu.IdPressed += OnContextMenuItemSelected;

        // 连接信号
        NodeSelected += OnNodeSelected;
        ConnectionRequest += OnConnectionRequested;
        DisconnectionRequest += OnDisconnectionRequested;
    }

    /// <summary>
    /// 通过配置反向创建节点图
    /// </summary>
    /// <param name="saveTargetData"></param>
    private void LoadConfig(EditorModel.NodeGraphSaveTargetData saveTargetData)
    {
        prefixIdGenerate.Clear();//根据配置重建id生成器
        foreach (var task in saveTargetData.NodeEditDataDictionary.Values)
        {
            var split = task.Name.Split("_");
            var prefix = split[0];
            var id = split[1];
            prefixIdGenerate.TryAdd(prefix, 0);
            if (prefixIdGenerate.TryGetValue(prefix, out int curId) && int.TryParse(id, out var taskId) && taskId > curId)
            {
                prefixIdGenerate[prefix] = taskId;
            }
            CreateTask(task.Name, task.Pos);
        }

        foreach (var line in saveTargetData.NodeLineEditDataSet)
        {
            ConnectNode(line.FromTask, line.FromPort, line.ToTask, line.ToPort);
        }
    }
    
    private void OnNodeSelected(Node node)
    {
        if (node is BaseGraphTask graphNode)
        {
            _currentGraphNode = graphNode;
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
        // 右键打开菜单
        if (@event is InputEventMouseButton mouseEvent && 
            mouseEvent.ButtonIndex == MouseButton.Right && 
            mouseEvent.Pressed)
        {
            _contextMenu.Position = (Vector2I)GetGlobalMousePosition();
            _contextMenu.Show();
        }
    }

    private void AddContextMenuItem()
    {
        _contextMenu.AddItem("添加起始节点");
        _contextMenu.AddItem("添加动作节点");
        _contextMenu.AddItem("删除选中节点");
        _contextMenu.AddItem("为选中节点添加槽位");
        _contextMenu.AddItem("为选中节点删除槽位");
    }

    private static Dictionary<string, int> prefixIdGenerate = new Dictionary<string, int>();
    /// <summary>
    /// 生成带有指定前缀的名字
    /// </summary>
    /// <param name="prefix"></param>
    private string GeneratePrefixName(string prefix)
    {
        prefixIdGenerate.TryAdd(prefix, 0);
        return prefix + "_" + prefixIdGenerate[prefix]++;//_作为分隔符
    }
    
    private void OnContextMenuItemSelected(long id)
    {
        switch (id)
        {
            case 0: // 添加起始节点
                AddChild(CreateTask(GeneratePrefixName("StartTask"), GetLocalMousePosition()));
                break;
            case 1: // 添加动作节点
                AddChild(CreateTask(GeneratePrefixName("ActionTask"), GetLocalMousePosition()));
                break;
            case 2:
                RemoveTask(_currentGraphNode);
                break;
            case 3:
                _currentGraphNode.AddInputSlot();
                _currentGraphNode.AddOutputSlot();
                break;
            case 4:
                _currentGraphNode.RemoveInputSlot();
                _currentGraphNode.RemoveOutputSlot();
                break;
        }
    }

    /// <summary>
    /// 移除task
    /// </summary>
    /// <param name="task"></param>
    private void RemoveTask(BaseGraphTask task)
    {
        RemoveChild(task);
        SaveTargetData.NodeEditDataDictionary.Remove(task.Name);
        //删除相关line
        var deleteList = new List<NodeLineEditData>();
        foreach (var line in SaveTargetData.NodeLineEditDataSet)
        {
            if (line.FromTask == task.Name || line.ToTask == task.Name)
            {
                deleteList.Add(line);
            }
        }

        foreach (var line in deleteList)
        {
            SaveTargetData.NodeLineEditDataSet.Remove(line);
        }
    }

    private GraphNode CreateTask(string title, Vector2 position, int slotCount = 0)
    {
        var Task = BaseTaskScene.Instantiate<BaseGraphTask>();
        Task.Title = title;
        Task.Name = title;//todo 后续可以把titile拿出来让用户编辑
        Task.PositionOffset = position;

        // 根据类型添加端口
        switch (title)
        {
            case "StartTask":
                Task.SetSlot(0, false, 0, Colors.Red, true, 0, Colors.Green);
                break;
            case "ActionTask":
                Task.SetSlot(0, true, 0, Colors.Red, true, 0, Colors.Green);
                break;
        }
        SaveTargetData.NodeEditDataDictionary.TryAdd(title, new NodeEditData()
        {
            Name = title,
            Pos = position,
        });

        return Task;
    }

    private void OnConnectionRequested(StringName fromTask, long fromPort, StringName toTask, long toPort)
    {
        // 允许连接
        ConnectNode(fromTask, (int)fromPort, toTask, (int)toPort);
        SaveTargetData.NodeLineEditDataSet.Add(new NodeLineEditData(fromTask, (int)fromPort, toTask, (int)toPort));
    }

    private void OnDisconnectionRequested(StringName fromTask, long fromPort, StringName toTask, long toPort)
    {
        // 断开连接
        DisconnectNode(fromTask, (int)fromPort, toTask, (int)toPort);
        SaveTargetData.NodeLineEditDataSet.Remove(new NodeLineEditData(fromTask, (int)fromPort, toTask, (int)toPort));
    }
}