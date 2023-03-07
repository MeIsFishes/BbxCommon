using UnityEngine;
using System.Collections.Generic;

namespace BbxCommon.Container
{
    internal class KdTree2DNode : PooledObject
    {
        internal enum EChildrenDirection
        {
            Horizontal,
            Vertical,
        }

        internal bool IsLeaf { get; private set; }
        internal EChildrenDirection ChildrenDirection;
        internal float DivideLine;
        internal KdTree2D KdTreeInstance;
        internal KdTree2DNode Parent;
        // Based on ChildrenDirection:
        // Horizontal: ChildLow at left, ChildHigh at right.
        // Vertical: ChildLow at bottom, ChildHigh at top.
        // The child which represents the lower position value is ChildLow.
        internal KdTree2DNode ChildLow;
        internal KdTree2DNode ChildHigh;

        private HashSet<ColliderItem2D> m_ColliderItemSet = new HashSet<ColliderItem2D>();

        private const int MAX_ITEM_NUMBER = 16; // Leaf nodes with too many items will divide itself into subtrees.
        private const int MIN_ITEM_NUMBER = 2;  // Leaf nodes with too few items will be merged into its parent node.

        // ------------ Building tree related ------------

        internal void InitLeaf(KdTree2D instance, KdTree2DNode parent)
        {
            KdTreeInstance = instance;
            Parent = parent;
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
                if (BelongsToLow(item))
                    ChildLow.AddColliderItem(item);
                if (BelongsToHigh(item))
                    ChildHigh.AddColliderItem(item);
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
                if (m_ColliderItemSet.Count < MIN_ITEM_NUMBER && Parent != null)
                {
                    Parent.DestroyChild(this);
                }
            }
            else
            {
                // For child nodes and current node may be destroyed during removing item, saving them is neccessary.
                var childLow = ChildLow;
                var childHigh = ChildHigh;
                childLow.RemoveColliderItem(item);
                childHigh.RemoveColliderItem(item);
            }
        }

        internal void DestroyTree()
        {
            if (IsLeaf)
            {
                m_ColliderItemSet.Clear();
                CollectToPool();
            }
            else
            {
                ChildLow.DestroyTree();
                ChildHigh.DestroyTree();
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
                bool originBelongsToLow = BelongsToLow(item);
                bool originBelongsToHigh = BelongsToHigh(item);
                item.SetPosition(newPosition);
                bool newBelongsToLow = BelongsToLow(item);
                bool newBelongsToHigh = BelongsToHigh(item);
                item.SetPosition(originPosition);
                if (originBelongsToLow != newBelongsToLow ||
                    originBelongsToHigh != newBelongsToHigh)
                {
                    return true;
                }
                else
                {
                    bool lowChanged = originBelongsToLow ? ChildLow.NodeBelongsToChanged(item, newPosition) : false;
                    bool highChanged = originBelongsToHigh ? ChildHigh.NodeBelongsToChanged(item, newPosition) : false;
                    return lowChanged || highChanged;
                }
            }
        }

        private void DivideIntoSubtrees()
        {
            IsLeaf = false;
            // Set ChildrenDirection.
            if (Parent == null)
            {
                ChildrenDirection = EChildrenDirection.Horizontal;
            }
            else
            {
                switch (Parent.ChildrenDirection)
                {
                    case EChildrenDirection.Horizontal:
                        ChildrenDirection = EChildrenDirection.Vertical;
                        break;
                    case EChildrenDirection.Vertical:
                        ChildrenDirection = EChildrenDirection.Horizontal;
                        break;
                }
            }
            // Calculate average position value as the dividing line.
            float posValue = 0;
            switch (ChildrenDirection)
            {
                case EChildrenDirection.Horizontal:
                    foreach (var item in m_ColliderItemSet)
                    {
                        posValue += item.GetPosition().x;
                    }
                    break;
                case EChildrenDirection.Vertical:
                    foreach (var item in m_ColliderItemSet)
                    {
                        posValue += item.GetPosition().y;
                    }
                    break;
            }
            DivideLine = posValue / m_ColliderItemSet.Count;
            // Create and initialize children, and then add all collider items stored in current node into child nodes.
            ChildLow = ObjectPool<KdTree2DNode>.Alloc();
            ChildLow.InitLeaf(KdTreeInstance, this);
            ChildHigh = ObjectPool<KdTree2DNode>.Alloc();
            ChildHigh.InitLeaf(KdTreeInstance, this);
            // Add all items to child nodes, and clear self's item set. Only leaf nodes store collider item.
            foreach (var item in m_ColliderItemSet)
            {
                AddColliderItem(item);
            }
            m_ColliderItemSet.Clear();
        }

        // Destroying request should be sent by a child node self.
        private void DestroyChild(KdTree2DNode childNode)
        {
            var anotherNode = AnotherChild(childNode);
            if (anotherNode == null)
            {
                Debug.LogError("Unexpected child Node!");
                return;
            }
            // Merge the child node to the another one, and set the another one to parent's child in place of
            // the current node.
            childNode.MergeTo(anotherNode);
            if (Parent == null)
            {
                KdTreeInstance.Root = anotherNode;
                CollectToPool();
            }
            else
            {
                if (Parent.ChildLow == this)
                {
                    Parent.ChildLow = anotherNode;
                    CollectToPool();
                }
                else
                {
                    Parent.ChildHigh = anotherNode;
                    CollectToPool();
                }
            }
        }

        private void MergeTo(KdTree2DNode targetNode)
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
                bool collidesLow = false, collidesHigh = false;
                if (BelongsToLow(targetItem))
                {
                    collidesLow = ChildLow.CheckCollision(targetItem);
                }
                if (BelongsToHigh(targetItem))
                {
                    collidesHigh = ChildHigh.CheckCollision(targetItem);
                }
                return collidesLow || collidesHigh;
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
                bool collidesLow = false, collidesHigh = false;
                if (BelongsToLowCapsule(capsule))
                {
                    collidesLow = ChildLow.CheckCollisionCapsule(capsule);
                }
                if (BelongsToHighCapsule(capsule))
                {
                    collidesHigh = ChildHigh.CheckCollisionCapsule(capsule);
                }
                return collidesLow || collidesHigh;
            }
        }

        private bool BelongsToLowCircle(CircleData2D circleData)
        {
            float posValue = 0;
            switch (ChildrenDirection)
            {
                case EChildrenDirection.Horizontal:
                    posValue = circleData.Position.x;
                    break;
                case EChildrenDirection.Vertical:
                    posValue = circleData.Position.y;
                    break;
            }
            return posValue - circleData.Radius < DivideLine;
        }

        private bool BelongsToHighCircle(CircleData2D circleData)
        {
            float posValue = 0;
            switch (ChildrenDirection)
            {
                case EChildrenDirection.Horizontal:
                    posValue = circleData.Position.x;
                    break;
                case EChildrenDirection.Vertical:
                    posValue = circleData.Position.y;
                    break;
            }
            return posValue + circleData.Radius > DivideLine;
        }

        private bool BelongsToLowCapsule(CapsuleBaselineData2D capsuleData)
        {
            float startPosValue = 0, endPosValue = 0;
            switch (ChildrenDirection)
            {
                case EChildrenDirection.Horizontal:
                    startPosValue = capsuleData.BaselineStartPos.x;
                    endPosValue = capsuleData.BaselineEndPos.x;
                    break;
                case EChildrenDirection.Vertical:
                    startPosValue = capsuleData.BaselineStartPos.y;
                    endPosValue = capsuleData.BaselineEndPos.y;
                    break;
            }
            return startPosValue - capsuleData.Radius < DivideLine ||
                endPosValue - capsuleData.Radius < DivideLine;
        }

        private bool BelongsToHighCapsule(CapsuleBaselineData2D capsuleData)
        {
            float startPosValue = 0, endPosValue = 0;
            switch (ChildrenDirection)
            {
                case EChildrenDirection.Horizontal:
                    startPosValue = capsuleData.BaselineStartPos.x;
                    endPosValue = capsuleData.BaselineEndPos.x;
                    break;
                case EChildrenDirection.Vertical:
                    startPosValue = capsuleData.BaselineStartPos.y;
                    endPosValue = capsuleData.BaselineEndPos.y;
                    break;
            }
            return startPosValue + capsuleData.Radius > DivideLine ||
                endPosValue + capsuleData.Radius > DivideLine;
        }

        private bool BelongsToLowLine(Vector3 startPos, Vector3 endPos)
        {
            return BelongsToLowCapsule(new CapsuleBaselineData2D(startPos, endPos, 0));
        }

        private bool BelongsToHighLine(Vector3 startPos, Vector3 endPos)
        {
            return BelongsToHighCapsule(new CapsuleBaselineData2D(startPos, endPos, 0));
        }

        private bool BelongsToLow(ColliderItem2D item)
        {
            switch (item)
            {
                case ColliderItem2DCircle circle:
                    return BelongsToLowCircle(circle.CircleData);
                case ColliderItem2DCapsule capsule:
                    return BelongsToLowCapsule(capsule.CapsuleData.ConvertToBaselineData());
            }
            return false;
        }

        private bool BelongsToHigh(ColliderItem2D item)
        {
            switch (item)
            {
                case ColliderItem2DCircle circle:
                    return BelongsToHighCircle(circle.CircleData);
                case ColliderItem2DCapsule capsule:
                    return BelongsToHighCapsule(capsule.CapsuleData.ConvertToBaselineData());
            }
            return false;
        }

        // ------------ Common functions ------------

        private KdTree2DNode AnotherChild(KdTree2DNode currentNode)
        {
            if (currentNode == ChildLow)
                return ChildHigh;
            if (currentNode == ChildHigh)
                return ChildLow;
            return null;
        }

        public override void OnCollect()
        {
            base.OnCollect();
            Parent = null;
            ChildLow = null;
            ChildHigh = null;
            m_ColliderItemSet.Clear();
        }
    }
}
