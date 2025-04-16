using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PopupHandler : MonoBehaviour
{
    
    //Website w/ char to glyph map for keyboard font https://www.fontspace.com/212-keyboard-font-f34592#action=charmap&id=lmRZ
    
    [SerializeField] GameObject container;
    
    [SerializeField] private float popupSpeed;

    [SerializeField] private InputActionAsset inputActions;

    private InputAction ExitInputAction;

    private PopupScriptableObject followupPopup;
    
    private bool popupOpen = false;

    private TMP_FontAsset messageFont;
    private TMP_FontAsset titleFont;

    public enum Location
    {
        LEFT, RIGHT, TOP, TOP_LEFT, TOP_RIGHT
    }

    public enum Recipient
    {
        NONE, HEART, MIND
    }

    private void Update()
    {
        if (ExitInputAction != null && ExitInputAction.triggered)
        {
            ExitInputAction = null;
            HidePopup();
        }
    }

    public Recipient recipient;

    public void setPopup(PopupScriptableObject popupData)
    {
        SetRecipient((int)popupData.recipient);
        titleFont = popupData.titleFont;
        messageFont = popupData.messageFont;
        SetTitle(popupData.title);
        SetMessage(popupData.message);
        SetInputAction(popupData.exitAction);
        StoreFollowupPopup(popupData.followupPopup);
        SetLocation((int)popupData.location);
    }

    public void StoreFollowupPopup(PopupScriptableObject followupData)
    {
        followupPopup = followupData;
    }

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

    public void SetLocation(int local)
    {
        Vector2 position;
        switch ((Location)local)
        {
            case Location.LEFT:
                position = new Vector2(680, 0);
                break;
            case Location.RIGHT:
                position = new Vector2(-680, 0);
                break;
            case Location.TOP:
                position = new Vector2(0, 330);
                break;
            case Location.TOP_LEFT:
                position = new Vector2(680, 330);
                break;
            case Location.TOP_RIGHT:
                position = new Vector2(-680, 330);
                break;
            default:
                position = new Vector2(680, 0);
                break;
        }
        container.GetComponent<RectTransform>().anchoredPosition = position;
        originalPosition = container.GetComponent<RectTransform>().anchoredPosition;
        
    }

    public void SetInputAction(string input_name)
    {
        ExitInputAction = inputActions.FindAction(input_name);
    }

    public void SetPosition(Vector2 position)
    {
        container.GetComponent<RectTransform>().anchoredPosition = position;
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
        if (popupOpen) HidePopup();
        resetPosition();
        StartCoroutine(MovePopup());
        StartCoroutine(FadePopupIn());
    }

    public void HidePopup()
    {
        StopAllCoroutines(); //avoids the edge case of a sequencial call of show then  where both coroutines are coexisting and fighting against eachother 
        if(!popupOpen) return;
        if (followupPopup != null) //if we have a followup popup stored
        {
            popupOpen = false; //avoid loop
            ShowPopup(); //resets position, starts playing animation to show popup (must do this first before setting values)
            setPopup(followupPopup); //set our values to that temporary stored popup
        }
        else //otherwise do normal behavior
        {
            StartCoroutine(MovePopup());
            StartCoroutine(FadePopupOut());
        }
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
