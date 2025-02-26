#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ObjectBrushTool2D : EditorWindow
{
    [SerializeField] private List<GameObject> prefabs = new();
    [SerializeField] private float radius = 2f;
    [SerializeField] private int minCount = 1;
    [SerializeField] private int maxCount = 5;
    
    private SerializedObject _serializedObject;
    private SerializedProperty _prefabsList;
    private SerializedProperty _brushRadius;
    private SerializedProperty _minObjects;
    private SerializedProperty _maxObjects;

    private Vector3 _brushPosition;
    private bool _isBrushActive;

    [MenuItem("Tools/2D Object Brush Tool")]
    public static void ShowWindow()
    {
        GetWindow<ObjectBrushTool2D>("2D Object Brush");
    }

    private void OnEnable()
    {
        _serializedObject = new SerializedObject(this);
        _prefabsList = _serializedObject.FindProperty("prefabs");
        _brushRadius = _serializedObject.FindProperty("radius");
        _minObjects = _serializedObject.FindProperty("minCount");
        _maxObjects = _serializedObject.FindProperty("maxCount");
        
        SceneView.duringSceneGui += OnSceneGUI;
        Tools.current = Tool.None;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        Tools.current = Tool.Custom;
    }

    private void OnGUI()
    {
        _serializedObject.Update();
        
        EditorGUILayout.PropertyField(_prefabsList, new GUIContent("Prefabs"), true);
        EditorGUILayout.Slider(_brushRadius, 0.1f, 10f, "Brush Radius");
        EditorGUILayout.IntSlider(_minObjects, 1, _maxObjects.intValue, "Min Objects");
        EditorGUILayout.IntSlider(_maxObjects, _minObjects.intValue, 50, "Max Objects");
        
        _serializedObject.ApplyModifiedProperties();
        
        EditorGUILayout.HelpBox("Left Click to paint objects\nHold Shift + Left Click to erase objects", MessageType.Info);
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        
        Event e = Event.current;
        
        Vector2 mousePosition = e.mousePosition;
        mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y * EditorGUIUtility.pixelsPerPoint;
        mousePosition.x *= EditorGUIUtility.pixelsPerPoint;
        
        _brushPosition = sceneView.camera.ScreenToWorldPoint(mousePosition);
        _brushPosition.z = 0;
        
        Handles.color = _isBrushActive ? Color.red : Color.green;
        Handles.DrawWireDisc(_brushPosition, Vector3.forward, radius);

        sceneView.Repaint();

        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            if (prefabs.Count == 0 && !e.shift)
            {
                Debug.LogWarning("No prefabs assigned in 2D Object Brush Tool!");
                return;
            }

            _isBrushActive = true;
            PaintObjects();
            e.Use();
        }
        else if (e.type == EventType.MouseUp && e.button == 0)
        {
            _isBrushActive = false;
        }
    }

    private void PaintObjects()
    {
        for (int i = 0; i < Random.Range(minCount, maxCount + 1); i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * radius;
            Vector3 spawnPos = _brushPosition + new Vector3(randomOffset.x, randomOffset.y, 0);

            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
            
            // Maybe throw an exception on null or check if any object is null.
            if (prefab == null)
            {
                continue;
            }

            GameObject spawnObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            Undo.RegisterCreatedObjectUndo(spawnObject, "Paint Object");
            spawnObject.transform.position = spawnPos;
            spawnObject.transform.rotation = Quaternion.identity;
        }
    }
}
#endif