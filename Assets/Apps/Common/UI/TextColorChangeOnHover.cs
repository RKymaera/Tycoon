using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Apps.Common.UI
{
    /// <summary>
    /// Changes the color of a TextMeshProUGUI component when hovered over.
    /// </summary>
[RequireComponent(typeof(TextMeshPro))]
public class TextColorChangeOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Tooltip("The text component to change the color of.")]
    [SerializeField] private TextMeshPro _text;

    [Tooltip("The color to change the text to when hovered.")]
    [SerializeField] private Color _hoverColor;
    [Tooltip("The duration of the color change tween.")]
    [SerializeField] private float _tweenDuration = 0.1f;

    private Color defaultColor;

    protected void OnValidate()
    {
        if (_text == null)
        {
            _text = GetComponent<TextMeshPro>();
        }
    }

    protected void Start()
    {
        defaultColor = _text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _text.color = _hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _text.color = defaultColor;
    }
}
}