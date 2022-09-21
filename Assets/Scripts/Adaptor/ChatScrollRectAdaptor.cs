using JetBrains.Annotations;
using UnityEngine;

public class ChatScrollRectAdaptor : MonoBehaviour
{
    public void AutoUpdateUI()
    {
        RectTransform[] rects = this.transform.GetComponentsInChildren<RectTransform>(true);
        float height = 0;
        int count = this.transform.childCount;
        for(int i = 0; i < count; i++)
        {
            height += this.transform.GetChild(i).GetComponent<RectTransform>().rect.height;
        }
        this.GetComponent<RectTransform>().sizeDelta =
            new Vector2(this.GetComponent<RectTransform>().rect.width, height);
        Transform transform = this.GetComponent<Transform>();
        this.GetComponent<RectTransform>().localPosition  = new Vector3(transform.localPosition.x, 425 + ((height - 850) > 0 ? (height - 850) : 0), 0);
        //this.GetComponent<RectTransform>().localPosition = new Vector3(0, -425, 0);
    }
}
