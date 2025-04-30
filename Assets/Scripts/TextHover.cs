using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class TextHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("No Animator found on " + gameObject.name);
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse entered ");
        anim.SetBool("Hover", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.SetBool("Hover", false);
    }
}
