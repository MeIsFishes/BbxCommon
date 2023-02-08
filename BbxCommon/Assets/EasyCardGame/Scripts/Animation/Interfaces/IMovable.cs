using UnityEngine;

namespace CardGame.Animation
{
    public interface IMovable {
        Vector3 GetPosition();
        void SetPosition(Vector3 targetPosition);
        Quaternion GetRotation();
        void SetRotation(Quaternion targetRotation);
    }
}
