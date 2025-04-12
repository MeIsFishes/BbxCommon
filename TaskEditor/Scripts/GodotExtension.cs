using Godot;
using System.Collections.Generic;

namespace BbxCommon
{
	public static class GodotExtension
	{
        #region Node
        public static void AddChild<T>(this Node node) where T : Node, new()
		{
			var child = new T();
			child.Name = typeof(T).Name;
			node.AddChild(child);
		}

		public static void AddChild<T>(this Node node, string name) where T : Node, new()
		{
			var child = new T();
			child.Name = name;
			node.AddChild(child);
		}

		public static void AddSibling<T>(this Node node) where T : Node, new()
		{
            var sibling = new T();
            sibling.Name = typeof(T).Name;
            node.AddSibling(sibling);
        }

		public static void AddSibling<T>(this Node node, string name) where T : Node, new()
		{
			var sibling = new T();
			sibling.Name = name;
			node.AddSibling(sibling);
		}

		public static T GetChild<T>(this Node node) where T : Node
		{
			var children = node.GetChildren();
			foreach (var child in children)
			{
				if (child is T t)
					return t;
			}
			return null;
		}

		public static T GetChild<T>(this Node node, string name) where T : Node
		{

            var children = node.GetChildren();
            foreach (var child in children)
            {
                if (child.Name == name && child is T)
                    return (T)child;
            }
            return null;
        }

        public static List<T> GetChildren<T>(this Node node) where T : Node
        {
            var children = node.GetChildren();
			var res = new List<T>();
            foreach (var child in children)
            {
				if (child is T t)
					res.Add(t);
            }
            return res;
        }

        public static T GetSibling<T>(this Node node) where T : Node
		{
			var siblings = node.GetParent().GetChildren();
			foreach (var sibling in siblings)
			{
				if (sibling is T t)
					return t;
			}
			return null;
		}

        public static T GetSibling<T>(this Node node, string name) where T : Node
        {
            var siblings = node.GetParent().GetChildren();
            foreach (var sibling in siblings)
            {
                if (sibling.Name == name && sibling is T t)
                    return t;
            }
            return null;
        }

        public static List<T> GetSiblings<T>(this Node node) where T : Node
        {
            var siblings = node.GetParent().GetChildren();
            var res = new List<T>();
            foreach (var sibling in siblings)
            {
                if (sibling is T t)
                    res.Add(t);
            }
            return res;
        }

        public static void RemoveChild<T>(this Node node) where T : Node
		{
            var children = node.GetChildren();
            foreach (var child in children)
            {
                if (child is T)
				{
					node.RemoveChild(child);
					return;
				}
            }
        }

        public static void RemoveChild(this Node node, string name)
        {
            var children = node.GetChildren();
            foreach (var child in children)
            {
                if (child.Name == name)
                {
                    node.RemoveChild(child);
                    return;
                }
            }
        }

        /// <summary>
        /// Remove all children.
        /// </summary>
        public static void RemoveChildren(this Node node)
        {
            var children = node.GetChildren();
            foreach (var child in children)
            {
                node.RemoveChild(child);
            }
        }

        public static void RemoveChildren<T>(this Node node) where T : Node
		{
            var children = node.GetChildren();
            foreach (var child in children)
            {
                if (child is T)
                    node.RemoveChild(child);
            }
        }

		public static void RemoveSibling<T>(this Node node) where T : Node
		{
			var siblings = node.GetParent().GetChildren();
			foreach (var sibling in siblings)
			{
				if (sibling is T)
				{
					node.GetParent().RemoveChild(sibling);
					return;
				}
			}
		}

        public static void RemoveSibling(this Node node, string name)
        {
            var siblings = node.GetParent().GetChildren();
            foreach (var sibling in siblings)
            {
                if (sibling.Name == name)
                {
                    node.GetParent().RemoveChild(sibling);
                    return;
                }
            }
        }

        public static void RemoveSiblings<T>(this Node node) where T : Node
        {
            var siblings = node.GetParent().GetChildren();
            foreach (var sibling in siblings)
            {
                if (sibling is T)
                {
                    node.GetParent().RemoveChild(sibling);
                }
            }
        }
        #endregion

        #region OptionButton
        public static int GetItemIndex(this OptionButton option, string label)
        {
            for (int i = 0; i < option.ItemCount; i++)
            {
                if (option.GetItemText(i) == label)
                    return i;
            }
            return 0;
        }

        public static void Select(this OptionButton option, string label)
        {
            var contextIndex = option.GetItemIndex(label);
            option.Select(contextIndex);
        }
        #endregion
    }
}
