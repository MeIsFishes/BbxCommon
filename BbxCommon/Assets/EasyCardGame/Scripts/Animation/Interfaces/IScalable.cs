using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Animation {
    public interface IScalable {
        Vector3 GetScale();
        void SetScale(Vector3 scale);
    }
}

