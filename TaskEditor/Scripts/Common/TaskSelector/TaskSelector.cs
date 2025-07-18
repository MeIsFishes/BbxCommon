using BbxCommon.Internal;
using Godot;
using System;
using System.Collections.Generic;

namespace BbxCommon
{
	public partial class TaskSelector : BbxControl
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

		private Action<TaskExportInfo> m_OnSelectTask;
		private List<TaskExportInfo> m_TaskInfos = new();
		private List<TaskSelectorItem> m_Items = new();
		private int m_CurPage = 1;

		private List<string> m_SearchTaskTags = new();
		private List<string> m_SearchTaskWithoutTags = new();
        private List<TaskExportInfo> m_SearchedTaskInfos = new();

        protected override void OnUiInit()
		{
			EditorModel.TaskSelector = this;
			Visible = false;
			// init items
			for (int i = m_Items.Count; i < ItemLimit; i++)
			{
				var item = TaskItemPrefab.Instantiate<TaskSelectorItem>();
				item.Visible = false;
				item.SetCallback(() =>
				{
					m_OnSelectTask(item.TaskInfo);
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
		}

		protected override void OnUiHide()
		{
			m_SearchTaskTags.Clear();
			m_SearchTaskWithoutTags.Clear();
		}

        /// <summary>
        /// Open the selector, and all tasks will has at least one of the given tags.
        /// </summary>
        public void OpenWithTags(Action<TaskExportInfo> onSelectTask, params string[] tags)
		{
			m_SearchTaskTags.AddRange(tags);
			Open(onSelectTask);
		}

        /// <summary>
        /// Open the selector, and all tasks will not has any one of the given tags.
        /// </summary>
        public void OpenWithoutTags(Action<TaskExportInfo> onSelectTask, params string[] tags)
		{
			m_SearchTaskWithoutTags.AddRange(tags);
			Open(onSelectTask);
		}

        /// <summary>
        /// Open the selector, and all tasks will has at least one of tag in withTags, with no tag in withoutTags.
        /// </summary>
        public void Open(Action<TaskExportInfo> onSelectTask, List<string> withTags, List<string> withoutTags)
		{
			m_SearchTaskTags.AddRange(withTags);
			m_SearchTaskWithoutTags.AddRange(withoutTags);
			Open(onSelectTask);
		}

        /// <summary>
        /// Open the selector with all tasks.
        /// </summary>
        public void Open(Action<TaskExportInfo> onSelectTask)
		{
			m_TaskInfos.Clear();
			// check if task's tags fit
			foreach (var info in EditorDataStore.GetTaskInfoList())
			{
				bool valid = true;
				if (m_SearchTaskTags.Count > 0)
				{
					bool hasTag = false;
					for (int i = 0; i < m_SearchTaskTags.Count; i++)
					{
						if (info.Tags.Contains(m_SearchTaskTags[i]))
						{
							hasTag = true;
							break;
						}
					}
					valid = hasTag;
				}
                if (valid && m_SearchTaskWithoutTags.Count > 0)
                {
                    for (int i = 0; i < m_SearchTaskWithoutTags.Count; i++)
					{
						if (info.Tags.Contains(m_SearchTaskWithoutTags[i]))
						{
							valid = false;
							break;
						}
					}
                }
                if (valid)
                {
					m_TaskInfos.Add(info);
                }
            }
			// init display
			Visible = true;
            SearchEdit.Text = string.Empty;
            SetPage(1);
            SetProcessInput(true);
            SetProcessUnhandledInput(true);
            RefreshTasks(SearchEdit.Text);
			m_OnSelectTask = onSelectTask;
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
