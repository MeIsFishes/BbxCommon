using UnityEngine;
using CardGame.Animation;
using CardGame.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CanvasGroup))]
public class UIAnimatedPanel : MonoBehaviour {
#pragma warning disable CS0649
    [SerializeField] private Animator _uiAnimator;
    [SerializeField] private float _hideInSeconds;
#pragma warning restore CS0649

    private bool _activeSelf;
    private bool activeSelf {
        set {
            _activeSelf = value;

            var navigations = GetComponentsInChildren<Navigation>();
            foreach (var nav in navigations) {
                nav.enabled = value;
            }
        }

        get {
            return _activeSelf;
        }
    }
    private AnimationQuery waitForSeconds;

    private void OnValidate() {
        _uiAnimator = GetComponent<Animator>(); 
    }

    public void SetPanel(bool Value) {
        if (activeSelf == Value) {
            return; // same.
        }

        activeSelf = Value;

        if (Value) {
            gameObject.SetActive(true);
        }

        _uiAnimator.SetBool("IsOpened", Value);

        if (Value) {
            if (_hideInSeconds > 0) {
                if (waitForSeconds != null) {
                    waitForSeconds.Stop();
                }

                waitForSeconds = new AnimationQuery();
                waitForSeconds.AddToQuery(new TimerAction(_hideInSeconds));
                waitForSeconds.Start(this, () => {
                    SetPanel(false);
                    waitForSeconds = null;
                });
            }
        }
    }

    public void Close() {
        SetPanel(false);
    }

    public void Open() {
        SetPanel(true);
    }

    /// <summary>
    /// We are calling this from animation event when panel is closed.
    /// </summary>
    public void OnClosed () {
        if (activeSelf) {
            return;
        }

        gameObject.SetActive(false);
    }

    /// <summary>
    /// We are calling this from animation event when panel is opened.
    /// </summary>
    public void OnOpened () {
        if (!activeSelf) {
            return;
        }

        gameObject.SetActive(true);
    }
}
