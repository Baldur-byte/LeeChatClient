using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;

[CustomEditor(typeof(ChatScrollRectAdaptor), true)]
public class ChatScrollRectAdaptorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        OnUpdateGUI();
    }
    void OnUpdateGUI()
    {
        var la = (ChatScrollRectAdaptor)target;
        la.AutoUpdateUI();
    }
}
