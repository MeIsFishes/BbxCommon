using Godot;
using System;
using System.Collections.Generic;
using BbxCommon;

public class TaskSaveDataBase
{
	public class SlotData
	{
		public string SlotName;
		public int Index;
		public bool EnableRight;
		public int RightType;
		public bool EnableLeft;
		public int LeftType;
		public Color RightColor;
		public Color LeftColor;
		public string LeftIcon;
		public string RightIcon;
		public bool DrawStyleBox;//	启用或禁用每个插槽的背景样式框的绘制
	}
	[Export] public Vector2 UiPosition;
	[Export] public List<TaskSaveDataBase> InputSlotList;
	[Export] public List<TaskSaveDataBase> OutputSlotList;
}
public partial class BaseGraphTask : GraphNode
{
	public override void _Ready()
	{
		// 允许拖拽
		Draggable = true;
	}

	public void AddInputSlot()
	{
		var curInputSlotCount = GetInputPortCount();
		SetSlotEnabledLeft(curInputSlotCount, true);
		SetSlotTypeLeft(curInputSlotCount, 0);//todo 后面设置槽位槽位连接 可以通过槽位类型限制 这里先统一0
	}
	
	public void RemoveInputSlot()
	{
		var curInputSlotCount = GetInputPortCount();
		if (curInputSlotCount > 0)
		{
			SetSlotEnabledLeft(curInputSlotCount - 1, false);
			SetSlotTypeLeft(curInputSlotCount - 1, 0);//todo 后面设置槽位槽位连接 可以通过槽位类型限制 这里先统一0
		}
	}
	
	public void AddOutputSlot()
	{
		var curInputSlotCount = GetOutputPortCount();
		SetSlotEnabledRight(curInputSlotCount, true);
		SetSlotTypeLeft(curInputSlotCount, 0);//todo 后面设置槽位槽位连接 可以通过槽位类型限制 这里先统一0
	}
	
	public void RemoveOutputSlot()
	{
		var curInputSlotCount = GetOutputPortCount();
		if (curInputSlotCount > 0)
		{
			SetSlotEnabledRight(curInputSlotCount - 1, false);
			SetSlotTypeRight(curInputSlotCount - 1, 0);//todo 后面设置槽位槽位连接 可以通过槽位类型限制 这里先统一0
		}
	}

	public void ClearInputSlot()
	{
		for (int i = 0; i < GetInputPortCount(); i++)
		{
			SetSlotEnabledLeft(i, false);
			SetSlotTypeLeft(i, 0);//todo 后面设置槽位槽位连接 可以通过槽位类型限制 这里先统一0
		}
	}

	public void ClearOutputSlot()
	{
		for (int i = 0; i < GetOutputPortCount(); i++)
		{
			SetSlotEnabledRight(i, false);
			SetSlotTypeRight(i, 0);//todo 后面设置槽位槽位连接 可以通过槽位类型限制 这里先统一0
		}
	}
	
	public void ClearAllSlot()
	{
		ClearAllSlots();
	}
}
