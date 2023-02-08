using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace CardGame.Input
{
    /// <summary>
    /// Input listener of the whole game.
    /// </summary>
    public class InputListener : MonoBehaviour {
        public static bool isTouching { private set; get; }
        public static bool isMousePressing { private set; get; }
        public static bool isDragging { private set; get; }

#pragma warning disable CS0649
        [SerializeField] private InputActions inputActions;
        [Min (1)]
        [SerializeField] private float minDragMagnitude = 20f;

        [Tooltip ("Minimum time offset for next touch")]
        [SerializeField] private float touchButtonPressOffset = 0.2f;
#pragma warning restore CS0649

        private Vector2 lastMousePosition;
        private Vector2 lastTouchPosition;
        
        private Vector2 pressedPosition;

        private bool needToCancelDrag;

        private float nextTouchTime;

        private void Start() {
            inputActions.OnCancel += () => {
                if (isDragging) {
                    needToCancelDrag = true;
                }
            };

            
        }

        private void Update() {
            var time = Time.time;

            var keyboard = Keyboard.current;
            var gamepad = Gamepad.current;

            var touch = Touchscreen.current;
            Mouse mouse = Mouse.current;

            if (Game.Current.isGameStarted && touch != null && touch.press.isPressed) {
                if (EventSystem.current.IsPointerOverGameObject ()) {
                    return;
                }
            }

            if (mouse != null) {
                if (Mouse.current.leftButton.wasPressedThisFrame) {
                    isMousePressing = true;

                    if (!needToCancelDrag) {
                        pressedPosition = mouse.position.ReadValue();
                        inputActions.OnSelect?.Invoke();
                    }
                }
                
                if (Mouse.current.leftButton.wasReleasedThisFrame) {
                    if (needToCancelDrag) {
                        needToCancelDrag = false;
                    } else if (!isDragging) {
                        inputActions.OnSelectUp?.Invoke();
                        inputActions.OnReturnUp?.Invoke();
                    }

                    isMousePressing = false;

                    if (CheckDragEnd()) {
                        Debug.Log("[InputListener] [Mouse] Drag ended by check drag end.");
                    }
                }

                if (!needToCancelDrag) {
                    MousePosition(mouse);
                }
            }

            if (keyboard != null) {
                if (keyboard.escapeKey.wasPressedThisFrame) {
                    inputActions.OnCancel?.Invoke();
                } else if (keyboard.escapeKey.wasReleasedThisFrame) {
                    inputActions.OnCancelUp?.Invoke();
                }

                if (keyboard.enterKey.wasPressedThisFrame) {
                    inputActions.OnReturn?.Invoke();
                } else if (keyboard.enterKey.wasReleasedThisFrame) {
                    inputActions.OnReturnUp?.Invoke();
                }
            }

            if (needToCancelDrag) {
                return;
            }

            if (gamepad != null) {
                if (gamepad.bButton.wasPressedThisFrame) {
                    inputActions.OnCancel?.Invoke();
                } else if (gamepad.bButton.wasReleasedThisFrame) {
                    inputActions.OnCancelUp?.Invoke();
                }

                if (gamepad.xButton.wasPressedThisFrame) {
                    inputActions.OnSelect?.Invoke();
                } else if (gamepad.xButton.wasReleasedThisFrame) {
                    inputActions.OnSelectUp?.Invoke();
                }

                if (gamepad.startButton.wasPressedThisFrame) {
                    inputActions.OnReturn?.Invoke();
                } else if (gamepad.startButton.wasReleasedThisFrame) {
                    inputActions.OnReturnUp?.Invoke();
                }

                GamepadDirection(gamepad);
            }

            if (touch != null) {
                if (touch.press.wasPressedThisFrame) {
                    if (nextTouchTime < time) {
                        nextTouchTime = time + touchButtonPressOffset;
                        isTouching = true;
                    }
                } else if (touch.press.wasReleasedThisFrame && isTouching) {
                    isTouching = false;

                    if (!isDragging) {
                        if (nextTouchTime > time) {
                            inputActions.OnSelectUp?.Invoke();
                            inputActions.OnReturnUp?.Invoke();
                        }
                    }
                }

                TouchPosition(touch);
            } else {
                isTouching = false;
            }
        }

        private void MousePosition(Mouse mouse) {
            var pos = mouse.position.ReadValue();
            if (Vector2.Distance (lastMousePosition,pos) >= 1) {
                lastMousePosition = pos;
                lastTouchPosition = pos;
                inputActions.OnMousePositionUpdate?.Invoke(pos);

                if (isMousePressing && !needToCancelDrag) {
                    CheckDragBegin(pos);
                }
            }
        }

        private void GamepadDirection (Gamepad gamepad) {
            var pos = Gamepad.current.leftStick.ReadValue();

            if (pos.magnitude > 0) {
                inputActions.OnGamepadDirectionUpdate?.Invoke(pos);
            }

            if (gamepad.leftShoulder.isPressed) {
                inputActions.OnGamepadDirectionUpdate?.Invoke(new Vector2(-1, 0));
            }

            if (gamepad.rightShoulder.isPressed) {
                inputActions.OnGamepadDirectionUpdate?.Invoke(new Vector2(1, 0));
            }
        }

        private void TouchPosition (Touchscreen touch) {
            if (isTouching) {
                var pos = touch.position.ReadValue();

                if (touch.press.wasPressedThisFrame) {
                    pressedPosition = pos;
                }

                if (lastTouchPosition != pos) {
                    lastTouchPosition = pos;
                    lastMousePosition = pos;
                    inputActions.OnMousePositionUpdate?.Invoke(pos);

                    CheckDragBegin(pos);
                }
            } else {
                if (touch.press.wasReleasedThisFrame) {
                    if (CheckDragEnd()) {
                        Debug.Log("[InputListener] [Touch] Drag ended by check drag end.");
                    }
                }
            }
        }

        private void CheckDragBegin (Vector2 pos) {
            if (needToCancelDrag) {
                return;
            }

            if (isDragging) {
                return;
            }

            if (Vector2.Distance(pressedPosition, pos) >= minDragMagnitude) {
                isDragging = true;
                inputActions.OnDragStarted?.Invoke();

                Debug.Log("[InputListener] Dragging started.");
            }
        }

        private bool CheckDragEnd () {
            if (isDragging) {
                isDragging = false;

                Debug.Log("[InputListener] Dragging end.");

                if (!needToCancelDrag) {
                    inputActions.OnDragEnd?.Invoke();
                }

                return true;
            }

            return false;
        }
    }
}
