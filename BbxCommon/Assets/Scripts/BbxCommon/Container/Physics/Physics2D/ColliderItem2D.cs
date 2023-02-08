using UnityEngine;

namespace BbxCommon.Container
{
    public enum EColliderShape2D
    {
        Circle,
        Capsule,
    }

    // Data like position is saved as UnityEngine.Vector2, if you are using a 2D physics container
    // to store a projected 3D world, convert Vector3 to Vector2 yourself.
    public struct CircleData2D
    {
        public Vector2 Position;
        public float Radius;

        public CircleData2D(Vector2 position, float radius)
        {
            Position = position;
            Radius = radius;
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
        }
    }

    public struct CapsuleBaselineData2D
    {
        public Vector2 BaselineStartPos;
        public Vector2 BaselineEndPos;
        public float Radius;

        public CapsuleBaselineData2D(Vector2 baselineStartPos, Vector2 baselineEndPos, float radius)
        {
            BaselineStartPos = baselineStartPos;
            BaselineEndPos = baselineEndPos;
            Radius = radius;
        }
    }

    public struct CapsuleData2D
    {
        public Vector2 Position;
        public float RotationAngle; // Clockwise rotation.
        public float Height;
        public float Radius;

        /// <param name="rotationAngle"> Clockwise rotation. </param>
        public CapsuleData2D(Vector2 position, float rotationAngle, float height, float radius)
        {
            Position = position;
            RotationAngle = rotationAngle;
            Height = height;
            Radius = radius;
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public CapsuleBaselineData2D ConvertToBaselineData()
        {
            var direction = Vector2.up;
            if (RotationAngle != 0)
            {
                direction = Quaternion.AngleAxis(RotationAngle, new Vector3(0, 0, 1)) * direction;
            }
            return new CapsuleBaselineData2D(Position - direction * Height * 0.5f,
                Position + direction * Height * 0.5f, Radius);
        }
    }

    public abstract class ColliderItem2D : PooledObject
    {
        public ExtraCheckItem ExtraCheckItem;
        public float RotationAngle; // Clockwise rotation.
        internal EColliderShape2D Shape;

        public ColliderItem2D()
        {
            SetShape();
        }

        protected abstract void SetShape();
        public abstract Vector2 GetPosition();
        public abstract void SetPosition(Vector2 position);
        public abstract bool CheckCollisionCircle(CircleData2D circleData);
        public abstract bool CheckCollisionCapsule(CapsuleBaselineData2D capsuleData);

        public bool CheckCollisionLine(Vector3 startPos, Vector3 endPos)
        {
            return CheckCollisionCapsule(new CapsuleBaselineData2D(startPos, endPos, 0));
        }

        public bool CheckCollision(ColliderItem2D targetItem)
        {
            switch (targetItem)
            {
                case ColliderItem2DCircle circle:
                    return CheckCollisionCircle(circle.CircleData);
                case ColliderItem2DCapsule capsule:
                    return CheckCollisionCapsule(capsule.CapsuleData.ConvertToBaselineData());
            }
            return false;
        }
    }

    public class ColliderItem2DCircle : ColliderItem2D
    {
        public CircleData2D CircleData;

        public void Init(Vector2 position, float radius)
        {
            CircleData = new CircleData2D(position, radius);
        }

        protected override void SetShape()
        {
            Shape = EColliderShape2D.Circle;
        }

        public override Vector2 GetPosition()
        {
            return CircleData.Position;
        }

        public override void SetPosition(Vector2 position)
        {
            CircleData.SetPosition(position);
        }

        public override bool CheckCollisionCircle(CircleData2D circleData)
        {
            if (ExtraCheckItem != null && !ExtraCheckItem.IsTrue())
                return false;
            return (circleData.Position - CircleData.Position).magnitude < circleData.Radius + CircleData.Radius;
        }

        public override bool CheckCollisionCapsule(CapsuleBaselineData2D capsuleBaselineData)
        {
            if (ExtraCheckItem != null && !ExtraCheckItem.IsTrue())
                return false;
            Vector2 v1 = capsuleBaselineData.BaselineEndPos - capsuleBaselineData.BaselineStartPos;
            Vector2 v2 = CircleData.Position - capsuleBaselineData.BaselineStartPos;
            Vector2 v3 = CircleData.Position - capsuleBaselineData.BaselineEndPos;
            if (Vector2.Dot(v1, v2) < 0)
                return v2.magnitude < CircleData.Radius + capsuleBaselineData.Radius;
            else if (Vector2.Dot(v1, v3) > 0)
                return v3.magnitude < CircleData.Radius + capsuleBaselineData.Radius;
            else
                return Vector3.Cross(v1, v2).magnitude / v1.magnitude < CircleData.Radius + capsuleBaselineData.Radius;
        }
    }

    public class ColliderItem2DCapsule : ColliderItem2D
    {
        public CapsuleData2D CapsuleData;

        public void Init(Vector2 position, float rotationAngle, float height, float radius)
        {
            CapsuleData = new CapsuleData2D(position, rotationAngle, height, radius);
        }

        protected override void SetShape()
        {
            Shape = EColliderShape2D.Capsule;
        }

        public override Vector2 GetPosition()
        {
            return CapsuleData.Position;
        }

        public override void SetPosition(Vector2 position)
        {
            CapsuleData.SetPosition(position);
        }

        public override bool CheckCollisionCircle(CircleData2D circleData)
        {
            if (ExtraCheckItem != null && !ExtraCheckItem.IsTrue())
                return false;
            var capsuleBaselineData = CapsuleData.ConvertToBaselineData();
            Vector2 v1 = capsuleBaselineData.BaselineEndPos - capsuleBaselineData.BaselineStartPos;
            Vector2 v2 = circleData.Position - capsuleBaselineData.BaselineStartPos;
            Vector2 v3 = circleData.Position - capsuleBaselineData.BaselineEndPos;
            if (Vector2.Dot(v1, v2) < 0)
                return v2.magnitude < circleData.Radius + capsuleBaselineData.Radius;
            else if (Vector2.Dot(v1, v3) > 0)
                return v3.magnitude < circleData.Radius + capsuleBaselineData.Radius;
            else
                return Vector3.Cross(v1, v2).magnitude / v1.magnitude < circleData.Radius + capsuleBaselineData.Radius;
        }

        public override bool CheckCollisionCapsule(CapsuleBaselineData2D capsuleData)
        {
            if (ExtraCheckItem != null && !ExtraCheckItem.IsTrue())
                return false;
            return true; // TODO
        }
    }
}
