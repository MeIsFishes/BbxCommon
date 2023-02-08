using UnityEngine;
using CardGame.Animation;
using CardGame.Input;
using System;
using System.Collections.Generic;

namespace CardGame.Layouts {
    public enum LayoutType {
        Deck,
        Table
    }

    public abstract class Layout : MonoBehaviour {
        #region serialized fields
#pragma warning disable CS0649
        /// <summary>
        /// layout are selectables, need to register / unregister to inputActions.
        /// </summary>
        [SerializeField] protected InputActions inputActions;
        /// <summary>
        /// row layout capacity. could not hold more than this number.
        /// </summary>
        [SerializeField] protected int capacity = 9;
        /// <summary>
        /// Default facing of the layout.
        /// </summary>
        [SerializeField] protected float zRotation = 180;
        /// <summary>
        /// Default angle of the layout
        /// </summary>
        [SerializeField] protected float xRotation = 0;

        [SerializeField] private float boundSizeZ = 2;
        [SerializeField] private float boundSizeY = 1;

        [SerializeField] protected LayoutAnimation defaultAnimation;
        [SerializeField] protected LayoutAnimation fancyAnimation;

#pragma warning restore CS0649
        #endregion

        public virtual LayoutType LayoutType => LayoutType.Deck;

        [Range (0, 1)]
        /// <summary>
        /// Simulation speed of the layout.
        /// </summary>
        public float SimulationSpeed = 1;

        protected Bounds bounds;
        public Card[] cards;
        protected bool updated = false;
        public AnimationQuery currentQuery;

        public abstract void StopInteraction();

        private void OnDrawGizmos() {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawCube(bounds.center, bounds.size);
        }

        /// <summary>
        /// Returns the capacity of the layout;
        /// </summary>
        public int Capacity => capacity;

        /// <summary>
        /// How many cards are not null in this layout?
        /// </summary>
        public int ValidCardCount {
            protected set;
            get;
        }

        protected void CountValidCards() {
            ValidCardCount = 0;
            for (int i = 0; i < capacity; i++) {
                if (cards[i] != null) {
                    ValidCardCount++;
                }
            }
        }

        private void Awake() {
            cards = new Card[capacity];

            // create bounds.
            float sizeX = GetOffset() * capacity;
            float sizeZ = boundSizeZ;
            Vector3 center = transform.position;
            Vector3 size = new Vector3(sizeX, boundSizeY, sizeZ);
            bounds.center = center;
            bounds.size = size;
        }

        /// <summary>
        /// Finds the index number of given card. -1 if the card is not in this layout.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public int FindIndex (Card card) {
            for (int i=0; i<capacity; i++) {
                if (cards[i] == card) {
                    return i;
                }
            }

            return -1;
        }

        public Vector3 GetCenter () {
            return bounds.center;
        }

        public void Close () {
            zRotation = 0;
        }

        public void Open () {
            zRotation = 180;
            ForceRefresh();
        }

        /// <summary>
        /// Reorder the cards.
        /// </summary>
        public void ReOrder () {
            CountValidCards();

            var found = new Card[capacity];
            int foundCount = 0;
            for (int i=0; i<capacity; i++) {
                if (cards[i] != null) {
                    found[foundCount++] = cards[i];
                }
            }

            if (foundCount == capacity) {
                return; // no reorder needed.
            }

            var startIndex = capacity/2 + foundCount / 2;

            // clear first.
            Clear();

            // assign reorder.
            for (int i = 0; i<foundCount; i++) {
                cards[startIndex - i] = found[foundCount - i - 1];
            }
        }

        public int FindAPlace () {
            for (int i=0; i<capacity; i++) {
                if (cards[i] == null)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Get all not null cards in range.
        /// </summary>
        /// <param name="card">Origin of point</param>
        /// <param name="range">Distance of the range</param>
        /// <param name="except">Don't include those, even if they are inside the list.</param>
        /// <returns></returns>
        public Card[] GetCardsInRange (Card card, int range, List<Card> except = null) {
            int length = range * 2;
            Card[] m_cards = new Card[length+1];
            var startIndex = FindIndex(card);

            Debug.Assert(startIndex != -1, "Given card is not in the layout.", this);

            for (int i=startIndex - range, c = 0; i<startIndex + range + 1; c++, i++) {
                if (i >= 0 && i < capacity && (except == null || !except.Contains (cards[i]))) {
                    m_cards[c] = cards[i];
                }
            }

            return m_cards;
        }

        public Card[] GetCardsInRange (int startIndex, int range, List<Card> except = null) {
            int length = range * 2;
            Card[] m_cards = new Card[length + 1];

            for (int i = startIndex - range, c = 0; i < startIndex + range + 1; c++, i++) {
                if (i >= 0 && i < capacity && (except == null || !except.Contains(cards[i]))) {
                    m_cards[c] = cards[i];
                }
            }

            return m_cards;
        }

        private void ClaimCard(Card card) {
            if (card.CurrentLayout != null) {
                card.CurrentLayout.Remove(card);
            }

            card.CurrentLayout = this;
        }

        /// <summary>
        /// Clears the disabled members.
        /// </summary>
        protected void ClearNulls() {
            while (true) {
                bool redo = false;
                for (int i = 0; i < capacity; i++) {
                    if (cards[i] != null && !cards[i].gameObject.activeSelf) {
                        cards[i] = null;
                        updated = true;
                        redo = true;
                        break;
                    }
                }

                if (!redo)
                    break;
            }
        }

        protected virtual float GetOffset() { return 0; } 

        /// <summary>
        /// Returns the card from the given index.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Card Get (int i) {
            return cards[i];
        }

        /// <summary>
        /// Adds a card to the list.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public bool Add(Card card) {
            int place = FindAPlace();
            if (place == -1) {
                return false;
            }

            ClaimCard(card);

            cards[place] = card;

            ReOrder();

            updated = true;

            return true;
        }

        /// <summary>
        /// Inserts a card to given index.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="targetIndex"></param>
        /// <param name="reOrderAfterInsert"></param>
        /// <returns></returns>
        public bool Insert(Card card, int targetIndex, bool reOrderAfterInsert = true) {
            Debug.Log("Insert card at " + targetIndex);

            var anyAvailable = FindAPlace();
            if (anyAvailable == -1) {
                Debug.Log("[Layout] No space available.");
                return false;
            }

            // target index is not null. split.
            if (cards[targetIndex] != null) {
                
                // check right side.
                bool rightSideIsOk = false;
                bool leftSideIsOk = false;

                if (cards[capacity -1] == null) {
                    rightSideIsOk = true;
                } 
                
                if (cards[0] == null) {
                    leftSideIsOk = true;
                }

                Debug.Assert(leftSideIsOk || rightSideIsOk, "[Layout] Both sides are not available, it seems layout is full.", this);

                void moveToRight() {
                    for (int i = capacity - 1; i > targetIndex; i--) {
                        cards[i] = cards[i - 1];
                    }

                    Debug.Log("move to right.");
                }

                void moveToLeft () {
                    for (int i = 0; i < targetIndex; i++) {
                        cards[i] = cards[i + 1];
                    }

                    Debug.Log("move to left.");
                }

                if (isUniqueNumber()) {
                    if (leftSideIsOk) {
                        moveToLeft();
                    } else {
                        moveToRight();
                    }
                }

                else {
                    if (rightSideIsOk) {
                        moveToRight();
                    } else {
                        moveToLeft();
                    }
                }
            }

            if (card != null) {
                ClaimCard(card);
            }

            cards[targetIndex] = card;

            if (reOrderAfterInsert) {
                ReOrder();
            }

            updated = true;
            return true;
        }

        public void Remove(Card card, bool reorder = true) {
            for (int i = 0; i < capacity; i++) {
                if (cards[i] == card) {
                    if (cards[i].IsDummy) {
                        cards[i].SetPosition(Vector3.zero);
                    }

                    cards[i] = null;
                    if (reorder) {
                        ReOrder();
                    }
                    updated = true;
                    break;
                }
            }
        }

        public void RemoveAt (int index) {
            if (cards[index] == null)
                return;

            updated = true;
            Debug.Log("[Layout] Remove " + cards[index]);
            cards[index] = null;

            ReOrder();
        }

        /// <summary>
        /// Clears the row.
        /// </summary>
        public void Clear(bool releaseCards = false) {
            for (int i = 0; i < capacity; i++) {
                if (cards[i] != null) {
                    if (releaseCards) {
                        cards[i].CurrentLayout = null;
                    }

                    cards[i] = null;
                }
            }

            updated = true;
        }

        /// <summary>
        /// Adjust all movables, if there is any change on the order.
        /// </summary>
        public virtual void Refresh(Action onCompleted = null, bool finishCurrentAnimation = true, bool useFancy = false) {
            ClearNulls();
        }

        /// <summary>
        /// Refresh, force on it.
        /// </summary>
        public virtual void ForceRefresh (Action onCompleted = null) {
            updated = true;
        }
        
        protected void DefineAnimationQuery (bool useFancy) {
            LayoutAnimation animator = useFancy ? fancyAnimation : defaultAnimation;

            if (currentQuery == null) {
                // convert progress clips to progress events.
                var length = animator.ProgressClips.Length;
                var progressEvents = new AnimationQuery.ProgressEvent[length];
                for (int i = 0; i < length; i++) {
                    int tmp = i;
                    progressEvents[i] = new AnimationQuery.ProgressEvent(
                        animator.ProgressClips[i].Time, 
                        ()=> { 
                            animator.ProgressClips[tmp].Clip.Play(); 
                        });
                }
                //

                currentQuery = new AnimationQuery(progressEvents);
            }
        }

        protected void AnimateCard (bool useFancy, Card card, Vector3 position, Quaternion rotation, Vector3 scale, float speedMod = 1) {
            LayoutAnimation animator = useFancy ? fancyAnimation : defaultAnimation;

            currentQuery.AddToQuery(new MovementAction(card,
                position,
                animator.MovementSpeed * speedMod * SimulationSpeed,
                animator.MovementCurve, animator.HeightCurve, false));

            currentQuery.AddToQuery(new RotateAction(card,
                rotation,
                animator.RotateSpeed * speedMod * SimulationSpeed,
                animator.RotateCurve));

            currentQuery.AddToQuery(new ScaleAction(card,
                scale,
                animator.ScaleSpeed * speedMod * SimulationSpeed,
                animator.ScaleCurve));
        }

        protected bool isUniqueNumber () {
            bool isUniqueNumber = false;
            if (capacity % 2 == 0) {
                if (ValidCardCount == 0) {
                    isUniqueNumber = true;
                } else if (ValidCardCount % 2 == 1) {
                    isUniqueNumber = true;
                }
            } else {
                if (ValidCardCount == 0) {
                    isUniqueNumber = false;
                } else if (ValidCardCount % 2 == 0) {
                    isUniqueNumber = true;
                }
            }

            return isUniqueNumber;
        }

        /// <summary>
        /// Find the index on deck bounds.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        protected virtual int FindIndexOnLayoutByPosition (Vector3 position) {
            var boundStartPoint = bounds.center - (bounds.size / 2);

            var distanceToOrigin = position.x - boundStartPoint.x;

            var off = bounds.size.x / capacity;

            int index;
            if (isUniqueNumber()) {
                index = Mathf.RoundToInt (distanceToOrigin / off);
            } else {
                index = (int)(distanceToOrigin / off);
            }

            index = Mathf.Clamp(index, 0, capacity-1);

            return index;
        }

        public virtual void FindTransformByIndex(int index, out Vector3 position, out Quaternion rotation) {
            position = Vector3.zero;
            rotation = Quaternion.identity;
        }

        /// <summary>
        /// Check if there is anycard else inside.
        /// </summary>
        /// <returns></returns>
        public bool IsAnyCardElseInside () {
            return IsAnyCardElseInside (null);
        }

        /// <summary>
        /// Check if there is anycard else inside except the given card.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public bool IsAnyCardElseInside (Card card) {
            for (int i=0; i<capacity; i++) {
                if (cards[i] != null && cards[i] != card) {
                    return true;
                }
            }

            return false;
        }
    }
}

