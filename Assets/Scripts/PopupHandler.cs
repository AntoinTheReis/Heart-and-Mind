using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PopupHandler : MonoBehaviour
{
    
    //Website w/ char to glyph map for keyboard font https://www.fontspace.com/212-keyboard-font-f34592#action=charmap&id=lmRZ
    
    [SerializeField] GameObject container;
    
    [SerializeField] private float popupSpeed;
    
    private bool popupOpen = false;

    private TMP_FontAsset messageFont;
    private TMP_FontAsset titleFont;

    public enum Recipient
    {
        NONE, HEART, MIND
    }
    
    public Recipient recipient;
    
    [Header("Colors\n")] 
    public Color heartBackgroundColor;
    public Color heartTitleColor;
    public Color mindBackgroundColor;
    public Color mindTitleColor;

    
    [Header("\nText Fields")] 
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    [Header("\nThings that will be colorful")] 
    //public Image[] outlines;
    public Image background;

    Vector2 originalPosition;
    private void Start()
    {
        originalPosition = container.GetComponent<RectTransform>().anchoredPosition;
        messageFont = messageText.font;
        titleFont = titleText.font;
        resetPosition();
    }

    private void resetPosition()
    {
        //halt all current routines
        StopAllCoroutines();
        popupOpen = false;
        //hide canvas container and get ready to show
        container.GetComponent<CanvasGroup>().alpha = 0;
        container.GetComponent<RectTransform>().anchoredPosition = originalPosition;
        container.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -120f);
        
    }
    public void ShowPopup()
    {
        if (popupOpen) return;
        resetPosition();
        StartCoroutine(MovePopup());
        StartCoroutine(FadePopupIn());
    }

    public void HidePopup()
    {
        if(!popupOpen) return;
        StartCoroutine(MovePopup());
        StartCoroutine(FadePopupOut());
    }

    IEnumerator MovePopup()
    {
        Vector2 targetPosition = new Vector2(container.GetComponent<RectTransform>().anchoredPosition.x,
            container.GetComponent<RectTransform>().anchoredPosition.y + 120);
        float t = 0f;
        
        Debug.Log("Moving popup!");
        while (container.GetComponent<RectTransform>().anchoredPosition.y < targetPosition.y)
        {
            container.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(container.GetComponent<RectTransform>().anchoredPosition, targetPosition, t);
            t += Time.deltaTime * popupSpeed;
            yield return null;
        }
        container.GetComponent<RectTransform>().anchoredPosition = targetPosition;
        StopCoroutine(MovePopup());
    }

    IEnumerator FadePopupIn()
    {
        popupOpen = true;
        container.GetComponent<CanvasGroup>().alpha = 0;
        while (container.GetComponent<CanvasGroup>().alpha < 1)
        {
            container.GetComponent<CanvasGroup>().alpha += Time.deltaTime;
            yield return null;
        }
        container.GetComponent<CanvasGroup>().alpha = 1;
        StopCoroutine(FadePopupIn());
    }

    IEnumerator FadePopupOut()
    {
        popupOpen = false;
        container.GetComponent<CanvasGroup>().alpha = 1;
        while (container.GetComponent<CanvasGroup>().alpha > 0)
        {
            container.GetComponent<CanvasGroup>().alpha -= Time.deltaTime;
            yield return null;
        }
        container.GetComponent<CanvasGroup>().alpha = 0;
        //Set fonts to default
        messageText.font = messageFont;
        titleText.font = titleFont;
        resetPosition();
        StopCoroutine(FadePopupOut());
    }
    
    public void SetTitle(string title)
    {
        titleText.text = title;
    }
    public void SetMessage(string message)
    {
        messageText.text = message;
    }
    [Tooltip("0 = NONE, 1 = HEART, 2 = MIND")]
    public void SetRecipient(int _recipient)
    {
        Recipient rec = (Recipient)_recipient;
        this.recipient = rec;
        // foreach (Image image in outlines)
        // {
        //     image.color = rec == Recipient.HEART ? heartOutlineColor : rec == Recipient.MIND ? mindOutlineColor : Color.black;
        // }
        background.color = background.color = rec == Recipient.HEART ? heartBackgroundColor : rec == Recipient.MIND ? mindBackgroundColor : Color.black;
        titleText.color = rec == Recipient.HEART ? heartTitleColor : rec == Recipient.MIND ? mindTitleColor : Color.white;
    }

    public void SetTitleFont(TMP_FontAsset font)
    {
        titleText.font = font;
    }

    public void SetMessageFont(TMP_FontAsset font)
    {
        messageText.font = font;
    }

    public void HidePopupAfterSeconds(float seconds)
    {
        StartCoroutine(WaitToHidePopup(seconds));
    }

    private IEnumerator WaitToHidePopup(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        HidePopup();
    }
    
}
