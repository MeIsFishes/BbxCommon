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
            switch (m_ConditionType)
			{
				case EConditionType.EnterCondition:
                    Label.Text = "Enter Conditions";
                    break;
                case EConditionType.Condition:
                    Label.Text = "Conditions";
                    break;
                case EConditionType.ExitCondition:
					Label.Text = "Exit Conditions";
                    break;
            }
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
			}
		}
    }
}
