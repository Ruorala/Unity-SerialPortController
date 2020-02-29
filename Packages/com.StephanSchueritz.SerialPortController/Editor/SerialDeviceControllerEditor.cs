using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SerialDeviceController))]
public class SerialDeviceControllerEditor : Editor
{
    private SerialDeviceController t;
    private string firstReadMessage = "";
    private string firstWriteMessage = "";

    private void OnEnable()
    {
        t = target as SerialDeviceController;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        if(t.GetFirstReadQueueMessage(out string firstReadMessage))
            this.firstReadMessage = firstReadMessage;

        if(t.GetFirstWriteQueueMessage(out string firstWriteMessage))
            this.firstWriteMessage = firstWriteMessage;

        EditorGUILayout.LabelField(string.Format("Threading ({0} '{1}')", t.Thread != null, t.Thread != null ? t.Thread.Name : "None"), EditorStyles.boldLabel);
        EditorGUILayout.LabelField(string.Format("Read Queue Size: {0} ({1})", t.SerialPortReadQueueSize, this.firstReadMessage));
        EditorGUILayout.LabelField(string.Format("Write Queue Size: {0} ({1})", t.SerialPortWriteQueueSize, this.firstWriteMessage));

        EditorGUILayout.LabelField(string.Format("Value Listeners: {0}", t.ValueListeners.Length), EditorStyles.boldLabel);
        for(int i = 0; i < t.ValueListeners.Length; i++)
            EditorGUILayout.LabelField(string.Format("{0}: {1}", t.ValueListeners[i].label, t.ValueListeners[i].value));

        EditorGUILayout.EndVertical();

        Repaint();
    }
}
