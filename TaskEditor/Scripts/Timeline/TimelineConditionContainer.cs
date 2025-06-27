using Godot;
using System;
using System.Collections.Generic;

namespace BbxCommon
{
	public partial class TimelineConditionContainer : BbxControl
	{
		[Export]
		public BbxButton CreateButton;
		[Export]
		public Label Label;
		[Export]
		public VBoxContainer ConditionList;
		[Export]
		public PackedScene ConditionPrefab;

		private EConditionType m_ConditionType;

        public void SetConditionType(EConditionType conditionType)
		{
			m_ConditionType = conditionType;
			RefreshLabel();
        }

		public void AddCreateButtonCallback(Action callback)
		{
            CreateButton.Pressed += callback;
		}

		public void RefreshConditionList(List<TaskEditData> conditions)
		{
			ConditionList.RemoveChildren();
			foreach (var condition in conditions)
			{
				var conditionNode = ConditionPrefab.Instantiate<TimelineCondition>();
				conditionNode.BindTask(condition);
				conditionNode.SetConditionType(m_ConditionType);
				ConditionList.AddChild(conditionNode);
			}
			this.CustomMinimumSize = this.GetSizeIncludeChildren();
			RefreshLabel();
		}

		private void RefreshLabel()
		{
			string text = null;
            switch (m_ConditionType)
            {
                case EConditionType.EnterCondition:
                    text = "Enter Conditions";
                    break;
                case EConditionType.Condition:
                    text = "Conditions";
                    break;
                case EConditionType.ExitCondition:
                    text = "Exit Conditions";
                    break;
            }
			text = text + "(" + ConditionList.GetChildCount() + ")";
			Label.Text = text;
        }
    }
}
