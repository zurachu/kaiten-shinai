using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class ButtonEffectFunction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GetComponent<Button>().interactable)
        {
            transform.DOScale(0.95f, 0.25f).SetLink(gameObject);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(1f, 0.25f).SetLink(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(1f, 0.25f).SetLink(gameObject);
    }
}
