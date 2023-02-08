using UnityEngine;
using CardGame.Animation;
using CardGame.GameEvents;
using System;

namespace CardGame.Layouts {
    public class TableLayout : Layout {
#pragma warning disable CS0649
        /// <summary>
        /// Placement offset.
        /// </summary>
        [SerializeField] private float offset = 1;
        [SerializeField] private Cursor cursor;
        [SerializeField] private GameObject selectionGuideEffect;
        [SerializeField] private GameObject errorGuideEffect;
        [SerializeField] private GameObject interactionEffect;
#pragma warning restore CS0649
        
        /// <summary>
        /// Current interacted card on this table layout, by mouse / touch / gamepad.
        /// </summary>
        public Card CurrentInteracted;

        public bool SelectionGuide {
            set {
                selectionGuideEffect.SetActive(value);
            }
        }

        public bool ErrorGuide {
            set {
                errorGuideEffect.SetActive(value);
            }
        }

        public bool Interacted {
            set {
                interactionEffect.SetActive(value);
            }
        }

        public override LayoutType LayoutType => LayoutType.Deck;

        protected override float GetOffset() {
            return offset;
        }

        private void Start() {
            EventCursor.Triggered += (active, position, visual) => {
                enabled = !active;

                if (active) {
                    cursor.OnPositionUpdate += CursorPoint;
                } else {
                    cursor.OnPositionUpdate -= CursorPoint;

                    // disable interaction.
                    Interact(null);
                }
            };
        }

        #region interaction
        private void OnEnable() {
            inputActions.OnMousePositionUpdate += MousePositionUpdate;
        }

        private void OnDisable() {
            inputActions.OnMousePositionUpdate -= MousePositionUpdate;
        }

        private void MousePositionUpdate(Vector2 position) {
            Ray ray = Camera.main.ScreenPointToRay(position);

            float distance;
            if (bounds.IntersectRay(ray, out distance)) {
                Vector3 pointOnDeck = ray.origin + ray.direction * distance;

                int index = FindIndexOnLayoutByPosition(pointOnDeck);

                if (index == -1 || index >= capacity) {
                    return;
                }

                Interact(cards[index]);
            } else {
                Interact(null);
            }
        }

        private void CursorPoint (Vector3 position) {
            if (bounds.Contains(position)) {
                int index = FindIndexOnLayoutByPosition(position);
                if (index >= 0 && index < capacity) {
                    Interact(cards[index]);
                }
            } else {
                Interact(null);
            }
        }

        private void Interact (Card card) {
            if (card == CurrentInteracted)
                return;

            if (CurrentInteracted != null) {
                CurrentInteracted.Interested(false);
                CurrentInteracted = null;
            }

            if (card != null) {
                CurrentInteracted = card;
                CurrentInteracted.Interested(true);
            }
        }
        #endregion

        public override void StopInteraction() {
            Interact(null);
        }

        /// <summary>
        /// FindIndexOnLayoutByPosition + bound check for intruder requests.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public int PositionToLayoutIndex(Vector3 position) {
            if (bounds.Contains(position)) {
                return FindIndexOnLayoutByPosition(position);
            }

            return -1;
        }

        /// <summary>
        /// Adjust all movables.
        /// </summary>
        public override void Refresh(Action onCompleted = null, bool finishCurrentAnimation = true, bool useFancy = false) {
            base.Refresh();

            if (!updated)
                return; // no need for refresh.

            Debug.Log("[TableLayout] Refreshing " + finishCurrentAnimation);

            updated = false;

            if (currentQuery != null) {
                if (finishCurrentAnimation) {
                    currentQuery.StopWithInstantFinish();
                } else {
                    currentQuery.Stop();
                }

                currentQuery = null;
            }

            DefineAnimationQuery(useFancy);

            float length = cards.Length;
            float totalSize = offset * length - offset;

            for (int i=0; i<length; i++) {
                if (cards[i] == null)
                    continue;

                Vector3 position;
                Quaternion rotation;
                GetPositionAtIndex(i, ref totalSize, out position, out rotation);

                if (cards[i].IsDummy) {
                    cards[i].SetPosition(position);
                    cards[i].SetRotation(rotation);
                    cards[i].SetScale(Vector3.one);
                } else {
                    AnimateCard(useFancy, cards[i], position, rotation, Vector3.one);
                }
            }
            
            currentQuery.Start(this, () => {
                Debug.Log("[TableLayout] Completed.");
                onCompleted?.Invoke();
                currentQuery = null;
            });
        }

        private void GetPositionAtIndex(int index, ref float totalSize, out Vector3 position, out Quaternion rotation) {
            position = new Vector3(-totalSize / 2f + index * offset - (isUniqueNumber() ? offset / 2 : 0), 0, 0);
            position = transform.TransformPoint(position);
            rotation = transform.rotation * Quaternion.Euler(xRotation, 0, zRotation);
        }

        public void SimulatePlacement () {
            CountValidCards();
            ValidCardCount++;
        }

        public override void FindTransformByIndex(int index, out Vector3 position, out Quaternion rotation) {
            float length = cards.Length;
            float totalSize = offset * length - offset;
            GetPositionAtIndex(index, ref totalSize, out position, out rotation);
        }
    }
}

