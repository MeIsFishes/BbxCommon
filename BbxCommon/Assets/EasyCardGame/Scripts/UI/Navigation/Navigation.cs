using UnityEngine;
using CardGame.Input;
using CardGame.Sounds;
using UnityEngine.Events;

namespace CardGame.UI {
    public class Navigation : MonoBehaviour {
#pragma warning disable CS0649
        [SerializeField] private InputActions inputActions;
        [SerializeField] private float cursorSelectionThreshold = 0.5f;
        [SerializeField] private float cursorSelectionTimeOffset = 0.5f;
        [SerializeField] private Vector2 navigationAxis = Vector2.right;
        [SerializeField] protected NavigationMember[] targetRects;
        [SerializeField] private bool resetOnEnable = true;
        [Tooltip ("Mostly used for gamepads. When user nagivate to a member, Select () & Interact () functions will be called same time.")]
        [SerializeField] private bool interactOnSelection = false;

        [SerializeField] private SoundClip navigationSoundClip;
        [SerializeField] private SoundClip selectSoundClip;

        [SerializeField] private UnityEvent onInteractedMember;

        [SerializeField] private Canvas canvas3D;

        [SerializeField] private bool useMouseRelease;

        [SerializeField] private Snapper snapper;
#pragma warning restore CS0649

        private int currentSelected = -1;
        private float cursorSelectedTime;
        private NavigationMember lastSelected;

        public int CurrentSelected => currentSelected;

        public void Interact (int index) {
            currentSelected = index;
            Select(0);
            lastSelected.Interact();
        }

        public void SetTargetRects (NavigationMember[] members) {
            targetRects = members;
        }

        private void OnEnable() {
            inputActions.OnMousePositionUpdate += MousePositionUpdate;
            inputActions.OnGamepadDirectionUpdate += CursorPositionUpdate;

            if (useMouseRelease)
                inputActions.OnSelectUp += InteractMember;
            else inputActions.OnSelect += InteractMember;

            if (resetOnEnable) {
                currentSelected = 0;
                Select(0);
            }
        }

        private void OnDisable() {
            inputActions.OnMousePositionUpdate -= MousePositionUpdate;
            inputActions.OnGamepadDirectionUpdate -= CursorPositionUpdate;

            if (useMouseRelease)
                inputActions.OnSelectUp -= InteractMember;
            else inputActions.OnSelect -= InteractMember;
        }

        private void CursorPositionUpdate (Vector2 direction) {
            if ((direction * navigationAxis).magnitude > cursorSelectionThreshold) {
                float time = Time.time;
                if (cursorSelectedTime < time) {
                    cursorSelectedTime = time + cursorSelectionTimeOffset;

                    Vector2 vec = direction * navigationAxis;
                    float val = Mathf.Abs(vec.x) > Mathf.Abs(vec.y) ? vec.x : vec.y;
                    Select(val < 0 ? -1 : 1);

                    if (interactOnSelection) {
                        InteractMember();
                    }
                }
            }
        }

        private void MousePositionUpdate (Vector2 position) {
            int length = targetRects.Length;

            Vector3 pos3d = position;
            pos3d.z = canvas3D.planeDistance;
            pos3d = Camera.main.ScreenToWorldPoint(pos3d);

            for (int i=0; i<length; i++) {
                var pos = targetRects[i].Rect.InverseTransformPoint(pos3d);
                pos.z = 0;

                if (targetRects[i].Rect.rect.Contains (pos)) {
                    currentSelected = i;
                    Select(0);

                    if (InputListener.isTouching) {
                        InteractMember();
                    }

                    return;
                }
            }

            currentSelected = -1;
            Select(0);
        }

        private void Select (int change) {
            if (targetRects.Length == 0)
                return;

            currentSelected += change;
            int length = targetRects.Length;
            
            if (change != 0) {
                if (currentSelected >= length)
                    currentSelected = 0;
                if (currentSelected < 0)
                    currentSelected = length-1;
            }

            if (currentSelected != -1 && lastSelected == targetRects[currentSelected])
                return;

            if (lastSelected != null) {
                lastSelected.DeSelect();
                lastSelected = null;
            }

            if (currentSelected == -1)
                return;

            navigationSoundClip.Play();

            lastSelected = targetRects[currentSelected];
            lastSelected.Select();
        }

        private void InteractMember () {
            if (lastSelected != null) {
                if (snapper != null) {
                    snapper.SnapToIndex(currentSelected);
                }

                lastSelected.Interact();
                onInteractedMember?.Invoke();
                selectSoundClip.Play();
            }
        }
    }
}

