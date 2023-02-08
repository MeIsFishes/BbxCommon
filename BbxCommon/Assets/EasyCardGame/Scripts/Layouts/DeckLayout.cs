using UnityEngine;
using CardGame.Animation;
using System;

namespace CardGame.Layouts {
    /// <summary>
    /// Todo dont manage if its in interaction.
    /// </summary>
    public class DeckLayout : Layout {
        [HideInInspector] 
        [SerializeField] 
        private DeckInteraction deckInteraction;

#pragma warning disable CS0649
        [SerializeField] private float fDistance;
        [SerializeField] private float fAngle;
        [SerializeField] private float frontFix = 0.1f;
        [SerializeField] private float gamePadNavigationTimeThreshold = 0.1f;
        [SerializeField] private float gamePadNavigationAxisThreshold = 0.1f;
#pragma warning restore CS0649

        private float lastGamePadNavigationTime;
        private int lastGamePadInteractionIndex;
        private bool isRefreshing;

        private bool _gamePadNavigation;
        /// <summary>
        /// Turns on/off the game pad navigation. True means on
        /// </summary>
        public bool GamePadNavigation {
            get {
                return _gamePadNavigation;
            }

            set {
                _gamePadNavigation = value;
                deckInteraction.Interact(null);
            }
        }

        public override LayoutType LayoutType => LayoutType.Deck;

        #region actions
        /// <summary>
        /// Fired when user selected a card.
        /// </summary>
        public Action<Card> OnCardSelected;
        #endregion

        private void OnValidate() {
            deckInteraction = GetComponent<DeckInteraction>();
        }

        #region interaction
        protected void OnEnable() {
            inputActions.OnGamepadDirectionUpdate += GamePadDirectionUpdate;
            inputActions.OnMousePositionUpdate += MousePositionUpdate;
            inputActions.OnSelectUp += Select;
            inputActions.OnDragStarted += Select;

            GamePadNavigation = true;
        }

        protected void OnDisable() {
            inputActions.OnGamepadDirectionUpdate -= GamePadDirectionUpdate;
            inputActions.OnMousePositionUpdate -= MousePositionUpdate;
            inputActions.OnSelectUp -= Select;
            inputActions.OnDragStarted -= Select;
        }

        private void GamePadDirectionUpdate(Vector2 direction) {
            if (isRefreshing) {
                return;
            }

            if (!GamePadNavigation) {
                return;
            }

            if (direction.x > gamePadNavigationAxisThreshold) {
                NavigateBy(1);
            } else if (direction.x < -gamePadNavigationAxisThreshold) {
                NavigateBy(-1);
            }
        }

        private void NavigateBy (int val) {
            int searcher = lastGamePadInteractionIndex + val;
            for (int i=0; i<capacity; i++) {
                if (searcher >= capacity) {
                    searcher = 0;
                } else if (searcher < 0) {
                    searcher = capacity - 1;
                }

                var card = Get(searcher);
                if (card != null) {
                    Navigate(searcher);
                    return;
                }

                searcher += val;
            }

            Debug.LogError("No card found to navigate.");
        }

        private void Navigate (int i) {
            Debug.Log("[DeckLayout] " + i);
            float time = Time.time;

            if (time < lastGamePadNavigationTime + gamePadNavigationTimeThreshold) {
                return; // not yet.
            }

            lastGamePadInteractionIndex = i;
            lastGamePadNavigationTime = time;

            deckInteraction.Interact(cards[i]);
        }

        public override void StopInteraction () {
            if (deckInteraction != null) {
                deckInteraction.Interact(null, true);
            }
        }

        private void MousePositionUpdate(Vector2 position) {
            if (isRefreshing) {
                return;
            }

            if (!GamePadNavigation) {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(position);

            float distance;
            if (bounds.IntersectRay(ray, out distance)) {
                Vector3 pointOnDeck = ray.origin + ray.direction * distance;

                int index = FindIndexOnLayoutByPosition(pointOnDeck);
                deckInteraction.Interact(index == -1 ? null : cards[index]);
            } else deckInteraction.Interact(null);
        }

        private void Select () {
            if (deckInteraction.CurrentInteracted != null) {
                OnCardSelected?.Invoke(deckInteraction.CurrentInteracted);
            }
        }
        #endregion

        protected override float GetOffset() {
            Vector2[] points = new Vector2[2];
            int[] indexes = new int[2] { 0, capacity - 1 };
            for (int i=0; i<2; i++) {
                float val = fAngle * indexes[i];
                var rotation = Quaternion.Euler(xRotation, fAngle * indexes[i], zRotation);
                points[i] = rotation * -transform.right * fDistance * val;
            }

            return Mathf.Abs ( (points[0].x - points[1].x) / (capacity-1) );
        }

        /// <summary>
        /// Adjust all movables.
        /// </summary>
        public override void Refresh (Action onCompleted = null, bool finishCurrentAnimation = true, bool useFancy = false) {
            base.Refresh();

            if (!updated)
                return; // no need for refresh.

            isRefreshing = true;

            Debug.Log("[DeckLayout] Refreshing...");

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

            float fOffsetStart;
            float length = cards.Length;

            fOffsetStart = -((length + 0.5f) * fAngle) / 2f;

            Vector3 thisPosition = transform.position;
            Quaternion thisRotation = transform.rotation;

            for (int i = 0; i < length; i++) {
                if (cards[i] == null)
                    continue;

                Quaternion rotation;
                Vector3 position;
                GetPositionAtIndex(i, ref thisPosition, ref thisRotation, ref fOffsetStart, out position, out rotation);

                AnimateCard(useFancy, cards[i], position, rotation, Vector3.one, useFancy ? (1f / i) : 1);
            }
            
            currentQuery.Start(this, () => {
                Debug.Log("[DeckLayout] Completed.");
                currentQuery = null;
                onCompleted?.Invoke();
                isRefreshing = false;
            });
        }
        private void GetPositionAtIndex(int index, ref Vector3 thisPosition, ref Quaternion thisRotation, ref float offsetStart, out Vector3 position, out Quaternion rotation) {
            float val = offsetStart + fAngle * index;
            rotation = thisRotation * Quaternion.Euler(xRotation, val, zRotation);
            position = thisPosition + rotation * -transform.right * fDistance * val;
            position += transform.up * frontFix * index;
        }

        protected override int FindIndexOnLayoutByPosition (Vector3 position) {
            float fOffsetStart;
            float length = cards.Length;

            fOffsetStart = -((length + 0.5f) * fAngle) / 2f;

            Vector3 thisPosition = transform.position;
            Quaternion thisRotation = transform.rotation;

            for (int i = 0; i < length; i++) {
                Quaternion rot;
                Vector3 pos;
                GetPositionAtIndex(i, ref thisPosition, ref thisRotation, ref fOffsetStart, out pos, out rot);

                float val = fOffsetStart + fAngle * (i + 1);
                var nextRot = thisRotation * Quaternion.Euler(xRotation, val, zRotation);
                var nextPos = thisPosition + nextRot * -transform.right * fDistance * val;

                var size = (nextPos.x - pos.x) / 2;
  
                if (position.x < pos.x + size) {
                    return i;
                }
            }

            return -1;
        }

        public override void ForceRefresh(Action onCompleted) {
            base.ForceRefresh();
            
            Refresh(onCompleted);
        }
    }
}

