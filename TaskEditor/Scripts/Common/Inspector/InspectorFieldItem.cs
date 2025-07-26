using BbxCommon.Internal;
using Godot;
using System;
using System.Collections.Generic;
using System.Text;

namespace BbxCommon
{
    /// <summary>
    /// There are two field types: normal field and special field.
    /// 
    /// Normal field represents the fields in Task that developer claimed.
    /// 
    /// Special field is those ones claimed in base classes, for example, start time and duration in TaskTimeline.
    /// And maybe there will be some fields only used in editor.
    /// However, special field means it should be processed differently from normal ones.
    /// 
    /// Take care of these functions:
    /// RebindField: Rebind normal fields.
    /// RebindSpecialField: Rebind special fields.
    /// ExportCurField: Inspector will call this function to export field info to the task.
    /// </summary>
    public partial class InspectorFieldItem : BbxControl
	{
        #region Lifecycle
        [Export]
		public Label FieldNameLabel;
		[Export]
		public OptionButton ValueSourceOption;
		[Export]
		public LineEdit CustomValueEdit;
		[Export]
		public OptionButton PresetValueOption;
		[Export]
		public PackedScene CollectionItemPrefab;
		[Export]
		public Container CollectionItemRoot;

		public List<InspectorFieldItemCollectionItem> CollectionItems = new();

		public enum ESpecialField
		{
			None,
			TimelineStartTime,
			TimelineDuration,
		}

		private TaskNode m_TaskNode;
		private ESpecialField m_SpecialField;
		private TaskEditField m_EditFieldInfo;
		private float m_OriginalMinY;

        protected override void OnUiOpen()
        {
			m_OriginalMinY = CustomMinimumSize.Y;
			OnUiInitFieldNameLabel();
			OnUiInitValueSourceOption();
			OnUiInitCustomValueEdit();
			OnUiInitPresetValueOption();
            OnUiInitCollectionItemRoot();
        }

		private void OnUiInitFieldNameLabel()
		{
            FieldNameLabel.MouseEntered += () =>
			{
				if (m_EditFieldInfo != null)
				{
					EditorModel.ShowTipsText(FieldNameLabel, TaskUtils.GenerateTaskFieldTypeString(m_EditFieldInfo.TypeInfo));
				}
			};
            FieldNameLabel.MouseExited += () =>
            {
                EditorModel.HideTipsText(FieldNameLabel);
            };
        }

        private void OnUiInitValueSourceOption()
        {
            ValueSourceOption.AddItem("Value", 0);
            ValueSourceOption.AddItem("Context", 1);
            ValueSourceOption.AddItem("Blackboard", 2);
            ValueSourceOption.ItemSelected += OnValueSourceChanged;
        }

		private void OnUiInitCustomValueEdit()
		{
			CustomValueEdit.TextChanged += (text) => { ExportCurField(m_TaskNode); };
		}

		private void OnUiInitPresetValueOption()
		{
			PresetValueOption.ItemSelected += (index) => { ExportCurField(m_TaskNode); };
		}

        private void OnUiInitCollectionItemRoot()
        {
            CollectionItemRoot.SortChildren += RefreshCustomMinimumSizeY;
        }
        #endregion

        #region Bind Field
        /// <summary>
        /// Bind normal fields.
        /// </summary>
        public void RebindField(TaskEditField editField, TaskNode nodeBelongs)
        {
			ValueSourceOption.Visible = true;
			m_TaskNode = nodeBelongs;
			m_SpecialField = ESpecialField.None;
			m_EditFieldInfo = editField;

			FieldNameLabel.Text = editField.FieldName;
			switch (editField.ValueSource)
			{
				case ETaskFieldValueSource.Value:
                    ValueSourceOption.Select(0);
                    OnValueSourceChanged(0);
					if (editField.TypeInfo.IsEnum())
					{
						PresetValueOption.Select(editField.Value);
					}
					else if (editField.TypeInfo.IsList())
					{
						ClearCollectionItems();
						var values = editField.Value.Split(TaskExportCrossVariable.ListElementSplit, StringSplitOptions.RemoveEmptyEntries);
						if (values.Length == 0)
						{
							AddListItem("");
                        }
						else
						{
							for (int i = 0; i < values.Length; i++)
							{
								AddListItem(values[i]);
							}
						}
					}
					else
					{
						CustomValueEdit.Text = editField.Value;
					}
                    break;
				case ETaskFieldValueSource.Context:
                    ValueSourceOption.Select(1);
                    OnValueSourceChanged(1);
                    PresetValueOption.Select(editField.Value);
                    break;
				case ETaskFieldValueSource.Blackboard:
                    ValueSourceOption.Select(2);
                    OnValueSourceChanged(2);
                    CustomValueEdit.Text = editField.Value;
                    break;
            }
        }

		/// <summary>
		/// If there are more special fields in future, write in this function.
		/// </summary>
		public void RebindSpecialField(ESpecialField field, TaskNode taskNode)
		{
			m_EditFieldInfo = null;
			m_TaskNode = taskNode;
			switch (field)
			{
				case ESpecialField.TimelineStartTime:
					ValueSourceOption.Visible = false;
                    CustomValueEdit.Visible = true;
                    PresetValueOption.Visible = false;
                    m_SpecialField = ESpecialField.TimelineStartTime;
					FieldNameLabel.Text = "StartTime";
					if (taskNode.TaskEditData is TimelineItemEditData timelineData1)
					{
						CustomValueEdit.Text = timelineData1.StartTime.ToString();
					}
					break;
				case ESpecialField.TimelineDuration:
                    ValueSourceOption.Visible = false;
                    CustomValueEdit.Visible = true;
                    PresetValueOption.Visible = false;
                    m_SpecialField = ESpecialField.TimelineDuration;
                    FieldNameLabel.Text = "Duration";
                    if (taskNode.TaskEditData is TimelineItemEditData timelineData2)
                    {
                        CustomValueEdit.Text = timelineData2.Duration.ToString();
                    }
                    break;
			}
		}

		private void OnValueSourceChanged(long index)
		{
            ClearCollectionItems();
            switch (index)
			{
				case 0: // value
					if (m_EditFieldInfo.TypeInfo.IsEnum())
					{
                        CustomValueEdit.Visible = false;
                        PresetValueOption.Visible = true;
						var enumInfo = TaskUtils.GetEnumInfo(m_EditFieldInfo.TypeInfo);
						RefreshEnumFields(enumInfo);
                    }
					else if (m_EditFieldInfo.TypeInfo.IsList())
					{
						CustomValueEdit.Visible = false;
						PresetValueOption.Visible = false;
						AddListItem("");
					}
					else
					{
						CustomValueEdit.Visible = true;
						PresetValueOption.Visible = false;
					}
					break;
				case 1: // context
					CustomValueEdit.Visible = false;
					PresetValueOption.Visible = true;
					RefreshContextFields();
					break;
				case 2: // blackboard
					CustomValueEdit.Visible = true;
					PresetValueOption.Visible = false;
					break;
			}
		}
        #endregion

        #region Preset Values
        private void RefreshEnumFields(TaskEnumExportInfo enumInfo)
		{
			TaskUtils.SetEnumPresetValues(PresetValueOption, enumInfo);
		}

        private void RefreshContextFields()
		{
			PresetValueOption.Clear();
			var contextInfo = EditorModel.CurSaveTarget.BindingContextInfo;
			for (int i = 0; i < contextInfo.FieldInfos.Count; i++)
			{
				PresetValueOption.AddItem(contextInfo.FieldInfos[i].FieldName);
			}
		}
        #endregion

        #region Collection Items
		private void ClearCollectionItems()
		{
			CollectionItems.Clear();
			CollectionItemRoot.RemoveChildren();
            CustomMinimumSize = new Vector2(CustomMinimumSize.X, m_OriginalMinY);
        }

		public void InsertCollectionItem(int index, string value1, string value2)
		{
			if (m_EditFieldInfo.TypeInfo.IsList())
			{
				InsertListItem(index, value2);
			}
        }

        public void RemoveCollectionItem(InspectorFieldItemCollectionItem node)
        {
            CollectionItemRoot.RemoveChild(node);
            CollectionItems.Remove(node);
        }

		private void RefreshCustomMinimumSizeY()
		{
			if (CollectionItems.Count == 0)
			{
				CustomMinimumSize = new Vector2(CustomMinimumSize.X, m_OriginalMinY);
			}
			else
			{
				var lastItem = CollectionItems[CollectionItems.Count - 1];
				CustomMinimumSize = new Vector2(CustomMinimumSize.X, m_OriginalMinY + lastItem.Position.Y + lastItem.CustomMinimumSize.Y);
			}
		}

        private void AddListItem(string value)
		{
            var collectionItem = CollectionItemPrefab.Instantiate<InspectorFieldItemCollectionItem>();
            CollectionItemRoot.AddChild(collectionItem);
			CollectionItems.Add(collectionItem);
            collectionItem.InitList(this, m_EditFieldInfo.TypeInfo, value);
        }

		private void InsertListItem(int index, string value)
		{
            var collectionItem = CollectionItemPrefab.Instantiate<InspectorFieldItemCollectionItem>();
            CollectionItemRoot.InsertChild(index, collectionItem);
            CollectionItems.Insert(index, collectionItem);
            collectionItem.InitList(this, m_EditFieldInfo.TypeInfo, value);
        }
        #endregion

        #region Export Value
        public void ExportCurField(TaskNode node)
		{
			if (node == null)
				return;
			var editData = node.TaskEditData;
			if (m_SpecialField == ESpecialField.TimelineStartTime)
			{
				if (editData is TimelineItemEditData timelineData)
				{
					float.TryParse(CustomValueEdit.Text, out var startTime);
					timelineData.StartTime = startTime;
                }
				EventBus.DispatchEvent(EEvent.TimelineNodeStartTimeOrDurationChanged);
            }
			else if (m_SpecialField == ESpecialField.TimelineDuration)
			{
                if (editData is TimelineItemEditData timelineData)
                {
                    float.TryParse(CustomValueEdit.Text, out var duration);
                    timelineData.Duration = duration;
                }
				var durationField = editData.GetEditField("Duration");
				if (durationField != null)
				{
					durationField.ValueSource = ETaskFieldValueSource.Value;
					durationField.Value = CustomValueEdit.Text;
				}
                EventBus.DispatchEvent(EEvent.TimelineNodeStartTimeOrDurationChanged);
            }
			else if (m_SpecialField == ESpecialField.None)
			{
				if (m_EditFieldInfo == null)
					return;
				switch (ValueSourceOption.Selected)
				{
					case 0:	// value
                        m_EditFieldInfo.ValueSource = ETaskFieldValueSource.Value;
						if (m_EditFieldInfo.TypeInfo.IsEnum())
						{
							m_EditFieldInfo.Value = PresetValueOption.GetItemText(PresetValueOption.Selected);
						}
						else if (m_EditFieldInfo.TypeInfo.IsList())
						{
							var sb = new StringBuilder();
							foreach (var collectionItem in CollectionItems)
							{
								sb.Append(collectionItem.GetValue2());
								sb.Append(TaskExportCrossVariable.ListElementSplit);
							}
							m_EditFieldInfo.Value = sb.ToString();
						}
						else
						{
							m_EditFieldInfo.Value = CustomValueEdit.Text;
						}
						break;
					case 1:	// context
                        m_EditFieldInfo.ValueSource = ETaskFieldValueSource.Context;
                        m_EditFieldInfo.Value = PresetValueOption.GetItemText(PresetValueOption.Selected);
						break;
					case 2:	// blackboard
                        m_EditFieldInfo.ValueSource = ETaskFieldValueSource.Blackboard;
                        m_EditFieldInfo.Value = CustomValueEdit.Text;
						break;
				}
			}
		}
        #endregion
    }
}
