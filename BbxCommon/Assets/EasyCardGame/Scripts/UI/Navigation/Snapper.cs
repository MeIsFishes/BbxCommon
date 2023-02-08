using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

[RequireComponent (typeof (ScrollRect))]
public class Snapper : MonoBehaviour
{
    public enum SnapAxis {
        Y,
        X
    }

    [SerializeField] private SnapAxis snapAxis = default;
    [SerializeField] private Vector2 offset = default;
    [SerializeField] private float snapSpeed = default;

    private bool Snap = false;
    private Vector2 _target;
    private float snapper = 0;

    private ScrollRect _Rect;
    private void OnValidate () {
        _Rect = GetComponent<ScrollRect>();
    }

    private void Update() {
        if (Snap) {
            snapper = Mathf.Min(snapper + Time.deltaTime * snapSpeed, 1);
            if (snapper >= 1)
                Snap = false;

            _Rect.content.anchoredPosition = Vector2.Lerp(_Rect.content.anchoredPosition, _target, snapper);
        }
    }

    public void SnapToIndex(int index) {
        if (_Rect == null)
            return;

        Canvas.ForceUpdateCanvases();

        _target = (Vector2)_Rect.transform.InverseTransformPoint(_Rect.content.position)
        - (Vector2)_Rect.transform.InverseTransformPoint(_Rect.content.GetChild(index).position);

        if (snapAxis == SnapAxis.X)
            _target.y = _Rect.content.anchoredPosition.y;
        else _target.x = _Rect.content.anchoredPosition.x;

        _target += offset;

        Snap = true;
        snapper = 0;
    }
}
