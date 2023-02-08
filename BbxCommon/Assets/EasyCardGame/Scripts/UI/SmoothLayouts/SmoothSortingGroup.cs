using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using CardGame.Animation;

namespace CardGame.UI {
    public class SmoothSortingGroup : MonoBehaviour {
#pragma warning disable CS0649
        [Tooltip ("0, 1 means vertical. 1,0 means horizontal.")]
        [SerializeField] private Vector2 stepModifier = new Vector2 (0, 1);
        [SerializeField] private Vector2 spacing;
        [SerializeField] private float speed;
        [SerializeField] private AnimationCurve curve;
#pragma warning restore CS0649

        private List<AnimationQuery> animations = new List<AnimationQuery>();
        private List<SmoothSortingGroupMember> members = new List<SmoothSortingGroupMember>();
        private int childCount;

        // Update is called once per frame
        void Update() {
            var currentMembers = transform.GetComponentsInChildren<SmoothSortingGroupMember>();
            int currentChildCount = currentMembers.Length;
            
            if (currentChildCount != childCount) {
                // reorder.
                var membersList = members.ToList();
                var currentMemberList = currentMembers.ToList();
                var newMembers = currentMemberList.FindAll(x => !members.Contains(x));
                var oldMembers = members.FindAll(x => !currentMemberList.Contains(x));
                
                foreach (var oldMember in oldMembers) {
                    members.Remove(oldMember);
                }

                foreach (var newMember in newMembers) {
                    members.Add(newMember);
                }
                //

                childCount = members.Count;
                
                UpdateLayout();
            }
        }

        private void UpdateLayout () {
            foreach (var anim in animations)
                anim.Stop();

            animations.Clear();

            Canvas.ForceUpdateCanvases();

            Vector2 currentPoint = Vector2.zero;
            for (int i= childCount-1; i>=0; i--) {
                currentPoint += spacing;

                var newAnimation = new AnimationQuery();
                newAnimation.AddToQuery(new MovementAction(members[i], currentPoint, speed, curve));

                currentPoint += members[i].rect.sizeDelta * stepModifier;

                animations.Add(newAnimation);
                newAnimation.Start(this, () => {
                    animations.Remove(newAnimation);
                });
            }
        }
    }
}
