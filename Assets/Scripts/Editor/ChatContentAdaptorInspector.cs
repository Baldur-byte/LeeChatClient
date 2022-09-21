using UnityEditor;

[CustomEditor(typeof(ChatContentAdaptor), true)]
public class ChatContentAdaptorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        OnUpdateGUI();
    }
    void OnUpdateGUI()
    {
        var la = (ChatContentAdaptor)target;
        la.RefreshRect();
    }
}
