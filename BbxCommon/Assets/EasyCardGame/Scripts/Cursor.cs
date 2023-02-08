using UnityEngine;

using CardGame.Layouts;
using CardGame.Input;
using CardGame.GameEvents;

using System;
using System.Linq;
using System.Collections.Generic;

namespace CardGame {
    public enum TargetingTypes {
        Placement,
        Target
    }

    [RequireComponent(typeof(LineRenderer))]
    public class Cursor : MonoBehaviour {
        public delegate void TargetingFeedback (int targetLayoutIndex, int targetLayoutMemberIndex);
#pragma warning disable CS0649
        [SerializeField] private InputActions inputActions;
        [SerializeField] public Card dummyCard;
        [SerializeField] private LineRenderer lineRenderer;

        [SerializeField]
        private LayerMask groundLayer;

        /// <summary>
        /// How many points will be used for height curve?
        /// </summary>
        [Range (3, 32)]
        [SerializeField] private ushort lineRendererThickness = 16;

        [SerializeField] private AnimationCurve heightCurve;

        [SerializeField] private float gamePadSensivity = 0.01f;

        /// <summary>
        /// Target layouts. Some will be valid, some will be not. That decision will be assigned by Game.
        /// </summary>
        [SerializeField] private TableLayout[] targetLayouts;

        /// <summary>
        /// arrow at the last point with direction.
        /// </summary>
        [SerializeField] private Transform apex;
#pragma warning restore CS0649

        private Vector3[] points;
        private TableLayout[] validLayouts;
        private int validLayoutCount;

        private Vector2 currentScreenPosition;
        private Vector3 startingPosition;

        private bool isFromDeck;

        private TableLayout targetLayout;
        private int targetPointIndex;
        private int targetLayoutIndex;
        private int cardLayoutIndex;

        private Card selectedCard;

        private bool noVisual;

        private int effectRange;
        private int rangedEffectRange;
        private bool isOffensive;

        /// <summary>
        /// Except card from targeting.
        /// </summary>
        private Card exceptThis;

        private bool fixedCursor = false;
        private Vector3 fixedCursorPoint;

        private TargetingFeedback targetingFeedback;
        private TargetingTypes targetingType;

        /// <summary>
        /// Cursor end point update.
        /// </summary>
        public Action<Vector3> OnPositionUpdate;

        private void OnValidate() {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Awake() {
            Debug.Assert(lineRenderer, "Line renderer of animated cursor is null. What's wrong?", this);

            lineRenderer.useWorldSpace = true;

            lineRenderer.positionCount = lineRendererThickness;
            points = new Vector3[lineRendererThickness];

            EventGameOver.OnTriggered += (isAborted) => {
                DisableTargeting();
                OnPositionUpdate = null;
                targetingFeedback = null;
            };
        }

        private bool IsLayoutValidTarget (TableLayout layout) {
            for (int i=0; i<validLayoutCount; i++) {
                if (validLayouts[i] != null) {
                    if (validLayouts[i] == layout) {
                        return true;
                    }
                }
            }

            return false;
        }

        public void EnableTargeting (bool noCancellation, Vector3 startingPosition, TableLayout[] validLayouts, TargetingTypes targetingType, TargetingFeedback targetingFeedback) {
            this.validLayouts = validLayouts;
            this.startingPosition = startingPosition;
            this.targetingFeedback = targetingFeedback;
            this.targetingType = targetingType;

            lineRenderer.positionCount = 0;

            if (targetingType == TargetingTypes.Placement) {
                foreach (var layout in validLayouts) {
                    layout.enabled = false; // disable interaction for placement.
                }
            }

            exceptThis = null;

            fixedCursor = false;
            fixedCursorPoint = Vector3.zero;
            targetLayoutIndex = -1;
            targetLayout = null;
            effectRange = 0;

            isFromDeck = targetingType == TargetingTypes.Placement;
            
            // find closest one in valid layouts.
            var isAssigned = false;
            var breaker = false;
            validLayoutCount = validLayouts.Length;

            if (!InputListener.isDragging) {
                for (int i = 0; i < validLayoutCount; i++) {
                    if (validLayouts[i].CurrentInteracted != null) {
                        // targeted already.
                        currentScreenPosition = Camera.main.WorldToScreenPoint(validLayouts[i].CurrentInteracted.CursorFocusPoint);
                        break;
                    } else {
                        for (int c = 0; c < validLayouts[i].Capacity; c++) {
                            bool isAnyCardInside = validLayouts[i].IsAnyCardElseInside();
                            if (isAnyCardInside || !isAssigned) {
                                var tCard = validLayouts[i].cards[validLayouts[i].Capacity / 2];
                                if (tCard != null) {
                                    currentScreenPosition = Camera.main.WorldToScreenPoint(validLayouts[i].cards[validLayouts[i].Capacity / 2].CursorFocusPoint);
                                } else {
                                    currentScreenPosition = Camera.main.WorldToScreenPoint(validLayouts[i].GetCenter());
                                }

                                if (isAnyCardInside) {
                                    breaker = true;
                                    break;
                                }

                                isAssigned = true;
                            }
                        }

                        if (breaker) {
                            break;
                        }
                    }
                }
            } else {
                currentScreenPosition = Camera.main.WorldToScreenPoint(startingPosition);
            }
            
            apex.gameObject.SetActive(true);
            lineRenderer.positionCount = lineRendererThickness;

            inputActions.OnMousePositionUpdate += MousePositionUpdate;
            inputActions.OnGamepadDirectionUpdate += GamepadDirectionUpdate;

            if (!noCancellation) {
                inputActions.OnCancel += DisableTargeting;
                inputActions.OnCancel += Restore;
            }

            if (!noVisual) {
                EventCursor.Triggered?.Invoke(true, startingPosition, !noCancellation);
            }

            inputActions.OnSelect += Done;

            foreach (var layout in validLayouts) {
                layout.SelectionGuide = true;
            }

            UpdateTargeting();
        }

        private void EnableTargeting (bool noCancellation, Vector3 startingPosition, TableLayout[] validLayouts, TargetingTypes targetingType, TargetingFeedback targetingFeedback, Card exceptThis) {
            EnableTargeting(noCancellation, startingPosition, validLayouts, targetingType, targetingFeedback);
            this.exceptThis = exceptThis;
        }

        /// <summary>
        /// For touch & drag purposes. There is no cursor, and card follows mouse.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="validLayouts"></param>
        /// <param name="targetingType"></param>
        /// <param name="targetingFeedback"></param>
        public void EnableTargeting (Vector3 startingPosition, TableLayout[] validLayouts, TargetingFeedback targetingFeedback) {
            noVisual = true;
            EnableTargeting(false, startingPosition, validLayouts, TargetingTypes.Placement, targetingFeedback);
            inputActions.OnDragEnd += Done;
            apex.gameObject.SetActive(false);

            UpdateTargeting();
        }

        /// <summary>
        /// Enable targeting by giving effect range.
        /// This is used for targeting to card only. Not for placement.
        /// </summary>
        /// <param name="startingPosition"></param>
        /// <param name="validLayouts"></param>
        /// <param name="effectRange"></param>
        /// <param name="targetingFeedback"></param>
        /// <param name="exceptThis"></param>
        public void EnableTargetingWithMark (bool isFromDeck, int cardLayoutIndex, Card selectedCard, Vector3 startingPosition, TableLayout[] validLayouts, int effectRange, int rangedEffectRange, bool isOffensive, TargetingFeedback targetingFeedback, Card exceptThis) {
            EnableTargeting(!isFromDeck, startingPosition, validLayouts, TargetingTypes.Target, targetingFeedback, exceptThis);
            this.effectRange = effectRange;
            this.rangedEffectRange = rangedEffectRange;
            this.isOffensive = isOffensive;
            this.selectedCard = selectedCard;
            this.cardLayoutIndex = cardLayoutIndex;
            this.isFromDeck = isFromDeck;

            UpdateTargeting();
        }

        public void DisableTargeting() {
            inputActions.OnMousePositionUpdate -= MousePositionUpdate;
            inputActions.OnGamepadDirectionUpdate -= GamepadDirectionUpdate;
            inputActions.OnCancel -= DisableTargeting;
            inputActions.OnCancel -= Restore;
            inputActions.OnSelect -= Done;
            inputActions.OnDragEnd -= Done;

            selectedCard = null;

            noVisual = false;

            EventCursor.Triggered?.Invoke(false, Vector3.zero, false);

            Debug.Log("DisableTargeting ()");

            foreach (var layout in targetLayouts) {
                layout.SelectionGuide = false;
                layout.ErrorGuide = false;
                layout.Interacted = false;
                layout.enabled = true; // enable interaction.
            }

            dummyCard.SetPosition(Vector3.zero);

            targetingFeedback = null;

            validLayoutCount = 0;
            apex.gameObject.SetActive(false);
            lineRenderer.positionCount = 0;
        }

        private void Restore () {
            if (targetLayout != null) {
                targetLayout.Interacted = false;

                if (targetingType == TargetingTypes.Placement) {
                    EventCursorTarget.Triggered?.Invoke(false);
                    ThrowDummy();
                    targetLayout.ReOrder();
                    targetLayout.Refresh(null, false);
                }
            }

            targetLayoutIndex = -1;
            targetPointIndex = -1;

            if (targetingType == TargetingTypes.Target) {
                RestoreFixedTarget();
            } else {
                targetLayout = null;
            }
        }

        private void MousePositionUpdate (Vector2 mousePos) {
            currentScreenPosition = mousePos;

            UpdateTargeting();
            EventCursor.Moved?.Invoke();
        }

        private void GamepadDirectionUpdate (Vector2 direction) {
            currentScreenPosition += direction * gamePadSensivity;
            UpdateTargeting();
            EventCursor.Moved?.Invoke();
        }

        private void UpdateTargeting () {
            if (targetingType == TargetingTypes.Target && selectedCard == null) {
                return;
            }

            // clamp current screenPosition.
            currentScreenPosition.x = Mathf.Clamp(currentScreenPosition.x, 0, Screen.width);
            currentScreenPosition.y = Mathf.Clamp(currentScreenPosition.y, 0, Screen.height);

            var ray = Camera.main.ScreenPointToRay(currentScreenPosition);

            RaycastHit hit;
            if (Physics.Raycast (ray, out hit, Mathf.Infinity, groundLayer)) {
                Vector3 hitPosition = hit.point;
                Set(hitPosition);

                OnPositionUpdate?.Invoke(fixedCursor ? fixedCursorPoint : hitPosition);

                // check for target layouts to see if there is a hit.
                for (int i =0; i<4; i++) {
                    int pointOnLayout = targetLayouts[i].PositionToLayoutIndex(hitPosition);

                    if (pointOnLayout >= 0 && pointOnLayout < targetLayouts[i].Capacity) {
                        if (!IsLayoutValidTarget (targetLayouts[i])) {
                            // wrong target.
                            targetLayouts[i].ErrorGuide = true;
                            RestoreFixedTarget();
                            ReOrderSimulation();
                        } else {
                            if (targetLayout == targetLayouts[i] && targetPointIndex == pointOnLayout) {
                                continue;
                            }

                            if (targetingType == TargetingTypes.Placement) {
                                if (targetLayouts[i].FindAPlace () == -1) {
                                    continue;
                                }

                                // valid target found for placement.
                                ReOrderSimulation();
                                // insert a null card without reordering.
                                if (targetLayouts[i].Insert(dummyCard, pointOnLayout, true)) {
                                    targetLayouts[i].Refresh(null, false);
                                    // assign new selection.
                                    targetLayout = targetLayouts[i];
                                    targetLayout.Interacted = true;

                                    EventCursorTarget.Triggered?.Invoke(true);
                                }
                            } else {
                                // targeting.
                                var targetCard = targetLayouts[i].Get(pointOnLayout);
                                if (targetCard != null && (exceptThis == null || exceptThis != targetCard)) {
                                    RestoreFixedTarget();

                                    fixedCursor = true;
                                    fixedCursorPoint = targetCard.CursorFocusPoint;
                                    Set(fixedCursorPoint);
                                    targetLayout = targetLayouts[i];
                                    targetLayout.Interacted = true;

                                    EventCursorTarget.Triggered?.Invoke(true);

                                    targetPointIndex = pointOnLayout;

                                    Scores.Instance.ClearSimulation();

                                    bool isRanged = Mathf.Abs(cardLayoutIndex - i) >= 1;

                                    var targetMarks = targetLayouts[i].GetCardsInRange(targetCard, isRanged ? rangedEffectRange : effectRange, new List<Card>() { selectedCard }).ToList ();

                                    void doFor(List<Card> tCards, Layout tLayout) {
                                        foreach (var tCard in tLayout.cards) {
                                            if (tCard != null) {
                                                if (tCards.Contains(tCard)) {
                                                    int amount = 0;
                                                    if (selectedCard.IsHealer) {
                                                        amount = selectedCard.healerCard.Heal;
                                                    } else {
                                                        amount = selectedCard.CalculateAttack(tCard, isRanged, out bool isKilled);

                                                        Scores.Instance.ScoreSimulationHit(amount);
                                                        if (isKilled) {
                                                            Scores.Instance.ScoreSimulationDeath(tCard.vulnerableCard.Points);
                                                        }
                                                    }

                                                    tCard.MarkAsTarget(true, amount, isOffensive);
                                                } else {
                                                    tCard.MarkAsTarget(false);
                                                }
                                            }
                                        }
                                    }

                                    doFor(targetMarks, targetLayout);


                                    // add more if both rows.
                                    var rangedEffect = isRanged && selectedCard.organicAttackerCard != null && selectedCard.organicAttackerCard.AttackType == GameData.Cards.AttackTypes.Both;

                                    if ((rangedEffect && selectedCard.organicAttackerCard.RangedEffectBothRows) ||
                                        selectedCard.baseCard.EffectBothRows) {

                                        Layout otherTable;
                                        // including other row.
                                        if (targetLayout == validLayouts [0]) {
                                            otherTable = validLayouts[1];
                                        } else {
                                            otherTable = validLayouts[0]; 
                                        }

                                        var otherTargets = otherTable.GetCardsInRange(targetPointIndex, isRanged ? rangedEffectRange : effectRange, new List<Card>() { selectedCard });

                                        doFor(otherTargets.ToList (), otherTable);
                                    }

                                    if (!selectedCard.IsHealer) {
                                        Scores.Instance.ScoreSimulationDone();
                                    }
                                    //
                                }
                                else {
                                    if (targetLayout != null) {
                                        RestoreFixedTarget();
                                        targetLayout.Interacted = false;
                                    }

                                    targetLayout = null;
                                }
                            }

                            if (targetLayoutIndex != i) {
                                targetLayoutIndex = i;
                                Debug.LogFormat("[Cursor] New layout => {0}", targetLayoutIndex);
                            }

                            targetPointIndex = pointOnLayout;
                        }
                    } else {
                        targetLayouts[i].ErrorGuide = false;

                        if (targetLayout == targetLayouts[i]) {
                            targetLayoutIndex = -1;
                            targetPointIndex = -1;
                            Restore();
                        }
                    }
                }
            } else {
                //reset
                currentScreenPosition = Camera.main.WorldToScreenPoint(startingPosition);
            }
        }

        private void RestoreFixedTarget () {
            Scores.Instance.ClearSimulation();

            fixedCursor = false;
            EventCursorTarget.Triggered?.Invoke(false);

            foreach (var layout in validLayouts) {
                foreach (var tCard in layout.cards) {
                    if (tCard != null) {
                        tCard.MarkAsTarget(false);
                    }
                }
            }
        }

        private void ReOrderSimulation () {
            if (targetLayout != null) {
                if (targetingType == TargetingTypes.Placement) {
                    ThrowDummy();
                }

                targetLayout.ReOrder();
                targetLayout = null;
            }
        }

        private void ThrowDummy () {
            targetLayout.Remove(dummyCard, false);
        }

        /// <summary>
        /// Interaction is done. Call the feedback and unregister from input actions.
        /// </summary>
        private void Done () {
            if (targetLayout == null) {
                targetLayoutIndex = -1;
            }

            if (targetingType == TargetingTypes.Placement && targetLayout != null) {
                ThrowDummy();
            }

            if (targetLayout != null || isFromDeck) {
                if (targetingType == TargetingTypes.Target) {
                    if (!fixedCursor) {
                        targetLayoutIndex = -1;

                        if (!isFromDeck) {
                            Debug.Log("[Cursor] Cursor needs a fixed target.");
                            return;
                        }
                    }
                }

                Debug.LogFormat ("[Cursor] Done with layout: {0}, point {1}", targetLayoutIndex, targetPointIndex);

                targetingFeedback?.Invoke(targetLayoutIndex, targetPointIndex);

                inputActions.OnSelect -= Done;
                inputActions.OnDragEnd -= Done;

                DisableTargeting();

                if (targetLayout != null) {
                    targetLayout.Interacted = false;
                }

                RestoreFixedTarget();
            }
        }

        private void Set (Vector3 endPosition) {
            if (noVisual) {
                if (lineRenderer.positionCount != 0) {
                    lineRenderer.positionCount = 0;
                }

                return;
            }

            if (lineRenderer.positionCount != lineRendererThickness) {
                lineRenderer.positionCount = lineRendererThickness;
            }

            if (fixedCursor)
                endPosition = fixedCursorPoint;

            points[0] = startingPosition;
            points[lineRendererThickness - 1] = endPosition;

            float length = lineRendererThickness - 1;
            float distance = Vector3.Distance(startingPosition, endPosition);
            float step = distance / length;
            Vector3 dir = (endPosition - startingPosition).normalized * step;

            // set positions
            for (int i = 1; i < length; i++) {
                points[i] = points[i - 1] + dir;
            }
            
            // set height curve.
            for (int i=0, splitter = lineRendererThickness - 1; i<lineRendererThickness; i++) {
                points[i].y += heightCurve.Evaluate((float)i / splitter);
            }

            lineRenderer.SetPositions(points);

            endPosition = points[lineRendererThickness - 1];

            // put apex to the end.
            apex.position = endPosition;
            apex.rotation = Quaternion.LookRotation(endPosition - points[lineRendererThickness - 2]);
        }
    }
}

