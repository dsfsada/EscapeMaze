using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TopDownCamera))]
public class TopDownCameraSceneEditor : Editor
{
    #region Variables
    private TopDownCamera targetCamera;
    #endregion Variables

    public override void OnInspectorGUI()
    {
        targetCamera = (TopDownCamera)target;
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        if (!targetCamera || !targetCamera.target) return;

        Transform cameraTarget = targetCamera.target;
        Vector3 targetPosition = cameraTarget.position;
        targetPosition.y += targetCamera.lookAtHeight;

        // Draw distance circle
        Handles.color = new Color(1f, 0f, 0f, .15f);
        Handles.DrawSolidDisc(targetPosition, Vector3.up, targetCamera.distance);

        Handles.color = new Color(0f, 1f, 0f, .75f);
        Handles.DrawWireDisc(targetPosition, Vector3.up, targetCamera.distance);

        // Create slider handles to adjust camera properties
        Handles.color = new Color(1f, 0f, 0f, .5f);
        targetCamera.distance = Handles.ScaleSlider(targetCamera.distance, targetPosition, -cameraTarget.forward, Quaternion.identity, targetCamera.distance, .1f);
        targetCamera.distance = Mathf.Clamp(targetCamera.distance, 2f, float.MaxValue);

        Handles.color = new Color(0f, 0f, 1f, .5f);
        targetCamera.height = Handles.ScaleSlider(targetCamera.height, targetPosition, Vector3.up, Quaternion.identity, targetCamera.height, .1f);
        targetCamera.height = Mathf.Clamp(targetCamera.height, 2f, float.MaxValue);

        // Create Lables
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 15;
        labelStyle.normal.textColor = Color.white;
        labelStyle.alignment = TextAnchor.UpperCenter;

        Handles.Label(targetPosition + (-cameraTarget.forward * targetCamera.distance), "Distance", labelStyle);

        labelStyle.alignment = TextAnchor.MiddleRight;
        Handles.Label(targetPosition + (Vector3.up * targetCamera.height), "Height", labelStyle);

        targetCamera.HandleCamera();
    }
}
