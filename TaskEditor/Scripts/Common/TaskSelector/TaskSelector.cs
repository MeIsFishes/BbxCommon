using BbxCommon.Internal;
using Godot;
using System;
using System.Collections.Generic;

namespace BbxCommon
{
	public interface ITaskSelectorTarget
	{
		public void SelectTask(TaskExportInfo taskInfo);
	}

	public partial class TaskSelector : Control
	{
        [Export]
        public PackedScene TaskItemPrefab;
        [Export]
		public LineEdit SearchEdit;
		[Export]
		public BbxButton BackButton;
		[Export]
		public Control ItemContainer;
		[Export]
		public int ItemLimit;
		[Export]
		public Label PageLabel;
		[Export]
		public BbxButton LastPageButton;
		[Export]
		public BbxButton NextPageButton;

		public ITaskSelectorTarget m_Target;
		private List<TaskExportInfo> m_TaskInfos;
		private List<TaskSelectorItem> m_Items = new();
		private List<TaskExportInfo> m_SearchedTaskInfos = new();
		private int m_CurPage = 1;

        public override void _Ready()
		{
			m_TaskInfos = EditorDataStore.GetTaskInfoList();
			// init items
			for (int i = m_Items.Count; i < ItemLimit; i++)
			{
				var item = TaskItemPrefab.Instantiate<TaskSelectorItem>();
				item.Visible = false;
				item.SetCallback(() =>
				{
					m_Target.SelectTask(item.TaskInfo);
					this.Visible = false;
				});
				ItemContainer.AddChild(item);
				m_Items.Add(item);
			}
			// add callbacks
			BackButton.Pressed += OnBackButtonClick;
			LastPageButton.Pressed += OnLastPageButtonClick;
			NextPageButton.Pressed += OnNextPageButtonClick;
			SearchEdit.TextChanged += RefreshTasks;
			VisibilityChanged += OnShow;
			// init display
			SearchEdit.Text = string.Empty;
			RefreshTasks(string.Empty);
			SetPage(1);
		}

		public void SetTarget(ITaskSelectorTarget target)
		{
			m_Target = target;
		}

		private void OnBackButtonClick()
		{
			Visible = false;
			SetProcessInput(false);
			SetProcessUnhandledInput(false);
		}

		private void OnLastPageButtonClick()
		{
			if (m_CurPage != 1)
			{
				SetPage(m_CurPage - 1);
			}
		}

		private void OnNextPageButtonClick()
		{
			if (m_CurPage * ItemLimit < m_SearchedTaskInfos.Count)
			{
				SetPage(m_CurPage + 1);
			}
		}

		private void OnShow()
		{
			if (Visible == true)
			{
				SearchEdit.Text = string.Empty;
				SetPage(1);
				SetProcessInput(true);
                SetProcessUnhandledInput(true);
            }
		}

		private void RefreshTasks(string searchStr)
		{
			m_SearchedTaskInfos.Clear();
			// check search
			if (searchStr.IsNullOrEmpty())
			{
				m_SearchedTaskInfos.AddList(m_TaskInfos);
			}
			else
			{
				for (int i = 0; i < m_TaskInfos.Count; i++)
				{
					var item = m_TaskInfos[i];
					if (item.TaskTypeName.Find(searchStr, caseSensitive: false) >= 0)
						m_SearchedTaskInfos.Add(item);
				}
			}
			// refresh page
			SetPage(1);
		}

		private void SetPage(int page)
		{
			m_CurPage = page;
			PageLabel.Text = page.ToString();
			for (int i = 0; i < m_Items.Count; i++)
			{
				m_Items[i].Visible = false;
			}
			int startIndex = (page - 1) * ItemLimit;
			for (int itemIndex = 0; itemIndex < ItemLimit; itemIndex++)
			{
				int infoIndex = startIndex + itemIndex;
				if (infoIndex >= m_SearchedTaskInfos.Count)
					break;
				m_Items[itemIndex].Visible = true;
				m_Items[itemIndex].SetTaskInfo(m_SearchedTaskInfos[infoIndex]);
			}
		}
	}
}
