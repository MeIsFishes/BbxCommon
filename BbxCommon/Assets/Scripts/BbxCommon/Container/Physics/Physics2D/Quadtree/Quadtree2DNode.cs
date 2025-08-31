using UnityEngine;
using System.Collections.Generic;

namespace BbxCommon.Container
{
    internal class Quadtree2DNode : PooledObject
    {
        internal bool IsLeaf;
        internal Vector2 CenterPoint;
        internal float SideLength; // Consider each leaf's region as a square.
        internal Quadtree2D QuadtreeInstance;
        // Children stored in a quadtree node is sort clockwise:
        // Children[0]: top left child.
        // Children[1]: top right child.
        // Children[2]: bottom right child.
        // Children[3]: bottom left child.
        internal Quadtree2DNode[] Children = new Quadtree2DNode[4];
        internal Quadtree2DNode Parent;

        private HashSet<ColliderItem2D> m_ColliderItemSet = new HashSet<ColliderItem2D>();

        private const int MAX_ITEM_NUMBER = 16; // Nodes with too many items will divide itself into subtrees.
        private const int MIN_ITEM_NUMBER = 4;  // Nodes with too few items will be merged into its parent node.

        // ------------ Building tree related ------------

        internal void InitLeaf(Quadtree2D instance, Quadtree2DNode parent, Vector2 centerPoint, float sideLength)
        {
            IsLeaf = true;
            QuadtreeInstance = instance;
            Parent = parent;
            CenterPoint = centerPoint;
            SideLength = sideLength;
        }

        internal void AddColliderItem(ColliderItem2D item)
        {
            if (IsLeaf)
            {
                if (m_ColliderItemSet.Contains(item))
                {
                    return;
                }
                m_ColliderItemSet.Add(item);
                if (m_ColliderItemSet.Count > MAX_ITEM_NUMBER)
                {
                    DivideIntoSubtrees();
                }
            }
            else
            {
                var childrenBelongs = ChildrenBelongsTo(item);
                for (int i = 0; i < 4; i++)
                {
                    if (IsBoolTrue(childrenBelongs, i))
                    {
                        Children[i].AddColliderItem(item);
                    }
                }
            }
        }

        internal void RemoveColliderItem(ColliderItem2D item)
        {
            if (IsLeaf)
            {
                if (m_ColliderItemSet.Contains(item) == false)
                {
                    return;
                }
                m_ColliderItemSet.Remove(item);
                if (Parent.GetItemCount() < MIN_ITEM_NUMBER)
                {
                    Parent.DestroyChildren();
                }
            }
            else
            {
                var childrenBelongs = ChildrenBelongsTo(item);
                for (int i = 0; i < 4; i++)
                {
                    if (IsBoolTrue(childrenBelongs, i))
                    {
                        Children[i].RemoveColliderItem(item);
                    }
                }
            }
        }

        internal void DestroyTree()
        {
            if (IsLeaf)
            {
                CollectToPool();
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    Children[i].DestroyTree();
                }
                CollectToPool();
            }
        }

        internal bool NodeBelongsToChanged(ColliderItem2D item, Vector2 newPosition)
        {
            if (IsLeaf)
            {
                return false;
            }
            else
            {
                // Check from root to leaves, until find it belongs to different leaves.
                var originPosition = item.GetPosition();
                byte originBelongsTo = ChildrenBelongsTo(item);
                item.SetPosition(newPosition);
                byte newBelongsTo = ChildrenBelongsTo(item);
                item.SetPosition(originPosition);
                if (originBelongsTo != newBelongsTo)
                {
                    return true;
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (IsBoolTrue(originBelongsTo, i) && Children[i].NodeBelongsToChanged(item, newPosition))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
        }

        private void DivideIntoSubtrees()
        {
            IsLeaf = false;
            // Calculate subtrees' center points and side lengths.
            float halfLength = SideLength * 0.5f;
            float quarterLength = SideLength * 0.5f;
            Children[0] = ObjectPool<Quadtree2DNode>.Alloc();
            Children[0].InitLeaf(QuadtreeInstance, this, CenterPoint + new Vector2(-quarterLength, quarterLength), halfLength);
            Children[1] = ObjectPool<Quadtree2DNode>.Alloc();
            Children[1].InitLeaf(QuadtreeInstance, this, CenterPoint + new Vector2(quarterLength, quarterLength), halfLength);
            Children[2] = ObjectPool<Quadtree2DNode>.Alloc();
            Children[2].InitLeaf(QuadtreeInstance, this, CenterPoint + new Vector2(quarterLength, -quarterLength), halfLength);
            Children[3] = ObjectPool<Quadtree2DNode>.Alloc();
            Children[3].InitLeaf(QuadtreeInstance, this, CenterPoint + new Vector2(-quarterLength, -quarterLength), halfLength);
            // Add all items to child nodes, and clear self's item set. Only leaf nodes store collider item.
            foreach (var item in m_ColliderItemSet)
            {
                AddColliderItem(item);
            }
            m_ColliderItemSet.Clear();
        }

        private void DestroyChildren()
        {
            if (IsLeaf == false)
            {
                IsLeaf = true;
                for (int i = 0; i < 4; i++)
                {
                    Children[i].MergeTo(this);
                    Children[i].CollectToPool();
                }
            }
        }

        private void MergeTo(Quadtree2DNode targetNode)
        {
            foreach (var item in m_ColliderItemSet)
            {
                targetNode.AddColliderItem(item);
            }
            m_ColliderItemSet.Clear();
        }

        // ------------ Collision check related ------------

        internal bool CheckCollision(ColliderItem2D targetItem)
        {
            if (IsLeaf)
            {
                foreach (var item in m_ColliderItemSet)
                {
                    if (item.CheckCollision(targetItem))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                byte childrenBelongsTo = ChildrenBelongsTo(targetItem);
                for (int i = 0; i < 4; i++)
                {
                    if (IsBoolTrue(childrenBelongsTo, i))
                    {
                        if (Children[i].CheckCollision(targetItem) == true)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        internal bool CheckCollisionCapsule(CapsuleBaselineData2D capsule)
        {
            if (IsLeaf)
            {
                foreach (var item in m_ColliderItemSet)
                {
                    if (item.CheckCollisionCapsule(capsule))
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                byte childrenBelongsTo = ChildrenBelongsToCapsule(capsule);
                for (int i = 0; i < 4; i++)
                {
                    if (IsBoolTrue(childrenBelongsTo, i))
                    {
                        if (Children[i].CheckCollisionCapsule(capsule) == true)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private byte ChildrenBelongsToCircle(CircleData2D circle)
        {
            if (IsLeaf)
            {
                return 0;
            }
            else
            {
                bool belongsToBottom = circle.Position.y - circle.Radius < CenterPoint.y;
                bool belongsToTop = circle.Position.y + circle.Radius > CenterPoint.y;
                bool belongsToLeft = circle.Position.x - circle.Radius < CenterPoint.x;
                bool belongsToRight = circle.Position.x + circle.Radius > CenterPoint.x;
                byte res = 0;
                if (belongsToTop && belongsToLeft)
                    res |= 1 << 0;
                if (belongsToTop && belongsToRight)
                    res |= 1 << 1;
                if (belongsToBottom && belongsToRight)
                    res |= 1 << 2;
                if (belongsToBottom && belongsToLeft)
                    res |= 1 << 3;
                return res;
            }
        }

        private byte ChildrenBelongsToCapsule(CapsuleBaselineData2D capsule)
        {
            if (IsLeaf)
            {
                return 0;
            }
            else
            {
                bool belongsToBottom = capsule.BaselineStartPos.y - capsule.Radius < CenterPoint.y ||
                    capsule.BaselineEndPos.y - capsule.Radius < CenterPoint.y;
                bool belongsToTop = capsule.BaselineStartPos.y + capsule.Radius > CenterPoint.y ||
                    capsule.BaselineEndPos.y + capsule.Radius > CenterPoint.y;
                bool belongsToLeft = capsule.BaselineStartPos.x - capsule.Radius < CenterPoint.x ||
                    capsule.BaselineEndPos.x - capsule.Radius < CenterPoint.x;
                bool belongsToRight = capsule.BaselineStartPos.x + capsule.Radius > CenterPoint.x ||
                    capsule.BaselineEndPos.x + capsule.Radius > CenterPoint.x;
                byte res = 0;
                if (belongsToTop && belongsToLeft)
                    res |= 1 << 0;
                if (belongsToTop && belongsToRight)
                    res |= 1 << 1;
                if (belongsToBottom && belongsToRight)
                    res |= 1 << 2;
                if (belongsToBottom && belongsToLeft)
                    res |= 1 << 3;
                return res;
            }
        }

        private byte ChildrenBelongsToLine(Vector2 startPos, Vector2 endPos)
        {
            return ChildrenBelongsToCapsule(new CapsuleBaselineData2D(startPos, endPos, 0));
        }

        private byte ChildrenBelongsTo(ColliderItem2D item)
        {
            switch (item)
            {
                case ColliderItem2DCircle circle:
                    return ChildrenBelongsToCircle(circle.CircleData);
                case ColliderItem2DCapsule capsule:
                    return ChildrenBelongsToCapsule(capsule.CapsuleData.ConvertToBaselineData());
            }
            return 0;
        }

        // ------------ Common functions ------------

        /// <summary>
        /// Return the item reference count belongs to the current node. Duplicate references will be discarded.
        /// </summary>
        internal int GetItemCount()
        {
            if (IsLeaf)
            {
                return m_ColliderItemSet.Count;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    Children[i].GetItemSet(m_ColliderItemSet);
                }
                int res = m_ColliderItemSet.Count;
                m_ColliderItemSet.Clear();
                return res;
            }
        }

        private void GetItemSet(HashSet<ColliderItem2D> resultSet)
        {
            if (IsLeaf)
            {
                resultSet.IntersectWith(m_ColliderItemSet);
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    Children[i].GetItemSet(resultSet);
                }
            }
        }

        /// <summary>
        /// Consider byte to be an 8-slots bool array. However, there are only 4 slots valid in quadtree.
        /// </summary>
        private bool IsBoolTrue(byte array, int order)
        {
            if (order >= 4)
                return false;
            return (array | (1 << order)) != 0;
        }

        protected override void OnCollect()
        {
            base.OnCollect();
            m_ColliderItemSet.Clear();
            Parent = null;
            if (IsLeaf == false)
            {
                for (int i = 0; i < 4; i++)
                {
                    Children[i] = null;
                }
            }
        }
    }
}
