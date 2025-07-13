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

        public static void InsertChild(this Node node, int index, Node child)
        {
            var children = node.GetChildren();
            var temp = new List<Node>();
            for (int i = index; i < children.Count; i++)
            {
                temp.Add(children[i]);
            }
            for (int i = 0; i < temp.Count; i++)
            {
                node.RemoveChild(temp[i]);
            }
            node.AddChild(child);
            for (int i = 0; i < temp.Count; i++)
            {
                node.AddChild(temp[i]);
            }
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

        public static void GetChildrenRecursively<T>(this Node node, List<T> result, bool ignoreInvisible = true, bool includeSelf = true)
        {
            if (includeSelf && node is T t)
            {
                if (node is CanvasItem canvasItem)
                {
                    if (ignoreInvisible && !canvasItem.Visible)
                        return; // Skip invisible nodes
                }
                result.Add(t);
            }
            foreach (Node child in node.GetChildren())
            {
                child.GetChildrenRecursively(result, ignoreInvisible, true);
            }
        }

        public static List<Node> GetChildrenRecursively(this Node node, bool ignoreInvisible = true, bool includeSelf = true)
        {
            var result = new List<Node>();
            node.GetChildrenRecursively(result, ignoreInvisible, includeSelf);
            return result;
        }

        public static List<T> GetChildrenRecursively<T>(this Node node, bool ignoreInvisible = true, bool includeSelf = true)
        {
            var result = new List<T>();
            node.GetChildrenRecursively(result, ignoreInvisible, includeSelf);
            return result;
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

        public static void RemoveFromParent(this Node node)
        {
            node.GetParent().RemoveChild(node);
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
            return -1;
        }

        public static void Select(this OptionButton option, string label)
        {
            var contextIndex = option.GetItemIndex(label);
            option.Select(contextIndex);
        }
        #endregion

        #region Control
        /// <summary>
        /// Find MinX, MinY, MaxX, MaxY of all children under the control, and calculate its size.
        /// The function doesn't consider rotation.
        /// </summary>
        public static Vector2 GetSizeIncludeChildren(this Control control)
        {
            var controls = control.GetChildrenRecursively<Control>(true, false);
            float minX, minY, maxX, maxY;
            if (controls.Count == 0)
            {
                return new Vector2();
            }
            else
            {
                minX = controls[0].GlobalPosition.X;
                minY = controls[0].GlobalPosition.Y;
                maxX = controls[0].GlobalPosition.X + controls[0].Size.X;
                maxY = controls[0].GlobalPosition.Y + controls[0].Size.Y;
            }
            for (int i = 1; i < controls.Count; i++)
            {
                var newMinX = controls[i].GlobalPosition.X;
                var newMinY = controls[i].GlobalPosition.Y;
                var newMaxX = controls[i].GlobalPosition.X + controls[i].Size.X;
                var newMaxY = controls[i].GlobalPosition.Y + controls[i].Size.Y;
                minX = Mathf.Min(minX, newMinX);
                minY = Mathf.Min(minY, newMinY);
                maxX = Mathf.Max(maxX, newMaxX);
                maxY = Mathf.Max(maxY, newMaxY);
            }
            return new Vector2(maxX - minX, maxY - minY);
        }
        #endregion
    }
}
