using ScriatableObjectBuilderFromExcel;
using UnityEditor;
using UnityEngine;

public class IMGUIExample : EditorWindow {
    private string textFieldValue = "Hello, IMGUI!";
    private float sliderValue = 0.5f;
    private bool toggleValue = false;
    private TextAsset textAsset;

    [MenuItem("Tools/IMGUI Example")]
    public static void ShowWindow() {
        EditorWindow.GetWindow<IMGUIExample>("IMGUI Example");
    }

    private void OnGUI() {
        // ���� ǥ��
        GUILayout.Label("IMGUI Example", EditorStyles.boldLabel);

        // �ؽ�Ʈ �Է� �ʵ�
        textFieldValue = EditorGUILayout.TextField("Text Field:", textFieldValue);

        // ��� ��ư
        toggleValue = EditorGUILayout.Toggle("Toggle:", toggleValue);

        // �����̴�
        sliderValue = EditorGUILayout.Slider("Slider:", sliderValue, 0f, 1f);

        // ��ư
        if (GUILayout.Button("Click Me"))
        {
            ExcelParser.Parse(textFieldValue);
        }

        // ���� ���
        EditorGUILayout.Space();
        GUILayout.Label("Current Status:");
        GUILayout.Label($"Text: {textFieldValue}");
        GUILayout.Label($"Toggle: {toggleValue}");
        GUILayout.Label($"Slider: {sliderValue:F2}");
    }
}