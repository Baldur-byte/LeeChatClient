using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ChatContentAdaptor : MonoBehaviour
{
    RectTransform content;

    RectTransform rectTransform;

    public void RefreshRect()
    {
        rectTransform = GetComponent<RectTransform>();
        Transform transform = this.transform.Find("Content(Text)");
        if (transform == null)
        {
            transform = this.transform.Find("Content(Image)");
            content = transform.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, content.rect.height + 28);
        }
        else
        {
            content = transform.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(content.rect.width + 30, content.rect.height + 28);
        }
        
    }
}
