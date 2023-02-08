using UnityEngine;

namespace CardGame.Animation {
    public class TimerAction : BaseAction {
        private readonly float speed;

        public TimerAction (float time) {
            speed = 1/time;
        }

        public override bool Update(in float deltaTime) {
            progress = Mathf.Min(progress + deltaTime * speed, 1);
            return progress == 1;
        }
    }
}
