using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PaintObject
{
    public GameObject prefab = null;
    public Vector2 scale = Vector2.one;
    public bool randomRotationX = false;
    public bool randomRotationY = false;
    public bool randomRotationZ = false;
    public Editor gameObjectEditor;

    public static void Display(PaintObject obj)
    {
        if (obj == null) return;

        Texture2D background = new Texture2D(128, 128);

        for (int y = 0; y < 128; y++)
        {
            for (int x = 0; x < 128; x++)
            {
                background.SetPixel(x, y, Color.grey);
            }
        }
        background.Apply();

        EditorGUILayout.BeginVertical(PrefabPainter.BoxStyle);
        GUILayout.Space(3);

        EditorGUI.BeginChangeCheck();
        GameObject gameObject = obj.prefab;
        if (EditorGUI.EndChangeCheck())
        {
            if (obj.gameObjectEditor != null) Object.DestroyImmediate(obj.gameObjectEditor);
        }

        GUIStyle bgColor = new GUIStyle();
        bgColor.normal.background = background;

        if (gameObject != null)
        {
            if (obj.gameObjectEditor == null)
                obj.gameObjectEditor = Editor.CreateEditor(gameObject);
            obj.gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(50, 50), bgColor);
        }

        EditorGUI.BeginChangeCheck();
        gameObject = (GameObject)EditorGUILayout.ObjectField("", obj.prefab, typeof(GameObject), true);
        obj.prefab = gameObject;
        if (EditorGUI.EndChangeCheck())
        {
            if (obj.gameObjectEditor != null) Object.DestroyImmediate(obj.gameObjectEditor);
        }

        if (obj.prefab != null)
        {
            obj.scale.x = EditorGUILayout.FloatField("Min Size", obj.scale.x);
            obj.scale.y = EditorGUILayout.FloatField("Max Size", obj.scale.y);
            GUILayout.Label("Random Rotation :");
            EditorGUILayout.BeginHorizontal();
            obj.randomRotationX = GUILayout.Toggle(obj.randomRotationX, "X");
            obj.randomRotationY = GUILayout.Toggle(obj.randomRotationY, "Y");
            obj.randomRotationZ = GUILayout.Toggle(obj.randomRotationZ, "Z");
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.Space(3);
        EditorGUILayout.EndVertical();
        GUILayout.Space(0);
    }
}

public class PrefabPainter : EditorWindow
{
    // UI
    Vector2 scrollPos;
    int toolBar;
    Rect windowBounds = new Rect(0, 0, 0, 0);

    // Brush Gizmo
    Color activeColor = Color.blue;
    Color passiveColor = Color.green;
    public float SizeInterval = 0.10f;
    float gizmoNormalLength = 1;

    // Brush Settings
    public bool showGizmoInfo = true;
    public float brushSize = 0.1f;
    public int brushDensity = 2;
    public LayerMask paintMask = 1;
    public float maxYPosition = 400;

    // Mouse Info
    Vector3 currentMousePos = Vector3.zero;
    Vector3 lastPaintPos = Vector3.zero;
    RaycastHit mouseHitPoint;
    Event currentEvent;

    // Hierarchy
    string paintGroupName = "Paint";
    string TEMPORARY_OBJECT_NAME = "Gizmo Location";
    Transform gizmoLocation;

    // Paint Objects
    public int listSize = 1;
    public List<PaintObject> objects;
    GameObject paintGroup = null;
    bool isPainting = false;
    List<string> layerNames;

    [MenuItem("Tools/Prefab Brush")]
    public static void Init()
    {
        PrefabPainter myWindow = (PrefabPainter)GetWindow(typeof(PrefabPainter));
        myWindow.Show();
        myWindow.Focus();
        myWindow.Repaint();

    }


    void OnEnable()
    {
        toolBar = 1;
        listSize = 1;

        SceneView.onSceneGUIDelegate += SceneGUI;

        if (objects == null) objects = new List<PaintObject>();

        layerNames = new List<string>();
        for (int i = 0; i <= 10; i++) layerNames.Add(LayerMask.LayerToName(i));

        EditorApplication.hierarchyWindowChanged += HierarchyChanged;
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= SceneGUI;
        if (gizmoLocation) DestroyImmediate(gizmoLocation.gameObject);
        if (GameObject.Find(TEMPORARY_OBJECT_NAME) != null) DestroyImmediate(GameObject.Find(TEMPORARY_OBJECT_NAME));
        EditorApplication.hierarchyWindowChanged -= HierarchyChanged;
    }

    void SceneGUI(SceneView sceneView)
    {
        windowBounds.width = Screen.width;
        windowBounds.height = Screen.height;

        if (toolBar == 0)
        {
            currentEvent = Event.current;
            UpdateMousePos(sceneView);
            DrawGizmo();
            SceneInput();
            if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDrag) SceneView.RepaintAll();
        }
    }

    public static Rect ClampRect(Rect rect, Rect bounds)
    {
        if (rect.x + rect.width > bounds.x + bounds.width)
            rect.x = (bounds.x + bounds.width) - rect.width;
        else if (rect.x < bounds.x)
            rect.x = bounds.x;

        if (rect.y + rect.height > bounds.y + bounds.height)
            rect.y = (bounds.y + bounds.height) - rect.height;
        else if (rect.y < bounds.y)
            rect.y = bounds.y;

        return rect;
    }

    public static bool GizmoFold
    {
        get { return EditorPrefs.GetBool("GizmoFold", false); }
        set { EditorPrefs.SetBool("GizmoFold", value); }
    }

    public static bool BrushSettingsFold
    {
        get { return EditorPrefs.GetBool("BrushSettingsFold", false); }
        set { EditorPrefs.SetBool("BrushSettingsFold", value); }
    }

    public static bool PrefabsFold
    {
        get { return EditorPrefs.GetBool("PrefabsFold", false); }
        set { EditorPrefs.SetBool("PrefabsFold", value); }
    }

    static GUIStyle _foldoutStyle;
    static GUIStyle FoldoutStyle
    {
        get
        {
            if (_foldoutStyle == null)
            {
                _foldoutStyle = new GUIStyle(EditorStyles.foldout)
                {
                    font = EditorStyles.boldFont
                };
            }
            return _foldoutStyle;
        }
    }

    static GUIStyle _boxStyle;
    public static GUIStyle BoxStyle
    {
        get
        {
            if (_boxStyle == null)
            {
                _boxStyle = new GUIStyle(EditorStyles.helpBox);
            }
            return _boxStyle;
        }
    }

    void OnGUI()
    {
        string[] menuOptions = new string[2];
        menuOptions[0] = "Prefab Painter";
        menuOptions[1] = "Settings";

        EditorGUILayout.Space();
        toolBar = GUILayout.Toolbar(toolBar, menuOptions);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

        switch (toolBar)
        {
            case 0:
                float tempSize = brushSize;
                int tempDensity = brushDensity;

                // Brush Settings
                BrushSettingsFold = BeginFold("Brush Settings", BrushSettingsFold);
                if (BrushSettingsFold)
                {
                    paintMask = EditorGUILayout.MaskField(new GUIContent("Paint Layer", "On which layer the tool will paint."), paintMask, layerNames.ToArray());
                    brushSize = EditorGUILayout.FloatField("Brush Size", brushSize);
                    brushDensity = EditorGUILayout.IntField("Brush Density", brushDensity);
                    paintGroupName = EditorGUILayout.TextField("Paint Group Name", paintGroupName);
                }
                EndFold();

                // Prefabs
                listSize = Mathf.Max(0, listSize);

                for (int i = 0; i < objects.Count; i++) PaintObject.Display(objects[i]);

                GUI.color = Color.green;
                if (GUILayout.Button("Add Prefab")) listSize++;
                GUI.color = Color.red;
                if (GUILayout.Button("Remove Prefab") && listSize != 0) listSize--;

                CheckForChanges(tempSize, tempDensity);

                break;

            case 1:
                GizmoFold = BeginFold("Placement Tool", GizmoFold);
                if (GizmoFold)
                {
                    showGizmoInfo = EditorGUILayout.Toggle("Display Brush Info", showGizmoInfo);
                    activeColor = EditorGUILayout.ColorField("Active Color", activeColor);
                    passiveColor = EditorGUILayout.ColorField("Passive Color", passiveColor);
                    gizmoNormalLength = EditorGUILayout.FloatField("Arrow Length", gizmoNormalLength);
                    SizeInterval = EditorGUILayout.FloatField("Size Interval", SizeInterval);
                    maxYPosition = EditorGUILayout.FloatField("Maximum Y Position", maxYPosition);
                }
                EndFold();
                break;
            default: break;
        }
        EditorGUILayout.EndScrollView();
    }

    void CheckForChanges(float tempSize, int tempDensity)
    {
        if (tempSize != brushSize)
        {
            brushSize = Mathf.Max(brushSize, 1);
            SceneView.RepaintAll();
        }

        else if (brushDensity != tempDensity)
        {
            brushDensity = Mathf.Max(brushDensity, 1);
            SceneView.RepaintAll();
        }

        else if (objects != null && listSize != objects.Count)
        {
            List<PaintObject> tempObj = new List<PaintObject>(listSize);
            for (int i = 0; i < listSize; i++)
            {
                if (objects.Count > i) tempObj.Add(objects[i]);
                else tempObj.Add(new PaintObject());
            }
            objects = new List<PaintObject>(tempObj);
        }
    }

    void DrawGizmo()
    {
        if (isPainting) Handles.color = activeColor;
        else Handles.color = passiveColor;

        if (toolBar == 0)
        {
            if (mouseHitPoint.transform)
            {
                if (GameObject.Find(TEMPORARY_OBJECT_NAME) == null) gizmoLocation = new GameObject(TEMPORARY_OBJECT_NAME).transform;
                else gizmoLocation = GameObject.Find(TEMPORARY_OBJECT_NAME).transform;
                gizmoLocation.rotation = mouseHitPoint.transform.rotation;
                gizmoLocation.forward = mouseHitPoint.normal;
                Handles.ArrowHandleCap(3, mouseHitPoint.point, gizmoLocation.rotation, gizmoNormalLength * brushSize, EventType.Repaint);
                Handles.CircleHandleCap(2, currentMousePos, gizmoLocation.rotation, brushSize, EventType.Repaint);
                gizmoLocation.up = mouseHitPoint.normal;
            }
        }

        Handles.BeginGUI();
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.black;
        GUILayout.BeginArea(new Rect(currentEvent.mousePosition.x + 10, currentEvent.mousePosition.y + 10, 250, 100));
        if (showGizmoInfo && toolBar == 0)
        {
            GUILayout.TextField("Size: " + System.Math.Round(brushSize, 2), style);
            GUILayout.TextField("Density: " + System.Math.Round((double)brushDensity, 2), style);
            GUILayout.TextField("Height: " + System.Math.Round(currentMousePos.y, 2), style);
            GUILayout.TextField("Surface Name: " + (mouseHitPoint.collider ? mouseHitPoint.collider.name : "none"), style);
            GUILayout.TextField("Position: " + currentMousePos.ToString(), style);
        }

        GUILayout.EndArea();
        Handles.EndGUI();
    }

    void UpdateMousePos(SceneView sceneView)
    {
        if (currentEvent.control) HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        RaycastHit hit;
        Ray ray = sceneView.camera.ScreenPointToRay(new Vector2(currentEvent.mousePosition.x, sceneView.camera.pixelHeight - currentEvent.mousePosition.y));
        if (Physics.Raycast(ray, out hit, 1000, paintMask))
        {
            currentMousePos = hit.point;
            mouseHitPoint = hit;
        }
        else mouseHitPoint = new RaycastHit();
    }

    public bool PreventCustomUserHotkey(EventType type, EventModifiers codeModifier, KeyCode hotkey)
    {
        Event currentevent = Event.current;
        if (currentevent.type == type && currentevent.modifiers == codeModifier && currentevent.keyCode == hotkey)
        {
            currentevent.Use();
            return true;
        }
        return false;
    }

    void SceneInput()
    {
        if (PreventCustomUserHotkey(EventType.ScrollWheel, EventModifiers.Control, KeyCode.None))
        {
            if (currentEvent.delta.y > 0) brushSize = brushSize + SizeInterval;
            else
            {
                brushSize = brushSize - SizeInterval;
                brushSize = Mathf.Max(SizeInterval, brushSize);
            }
            Repaint();
        }

        else if (PreventCustomUserHotkey(EventType.ScrollWheel, EventModifiers.Alt, KeyCode.None))
        {
            if (toolBar == 0)
            {
                if (currentEvent.delta.y > 0) brushDensity++;
                else
                {
                    brushDensity--;
                    brushDensity = Mathf.Max(1, brushDensity);
                }
                Repaint();
            }
        }

        else if (currentEvent.control && (currentEvent.button == 0 && currentEvent.type == EventType.MouseDown))
        {
            if (toolBar == 0)
            {
                isPainting = true;
                Painting();
            }
        }

        else if (isPainting && !currentEvent.control || (currentEvent.button != 0 || currentEvent.type == EventType.MouseUp))
        {
            if (toolBar == 0)
            {
                lastPaintPos = Vector3.zero;
                isPainting = false;
            }
        }

        else if (isPainting && (currentEvent.type == EventType.MouseDrag)) Painting();
    }

    void Painting()
    {
        if (objects != null && objects.Count > 0)
        {
            if (Vector3.Distance(lastPaintPos, currentMousePos) > brushSize)
            {
                lastPaintPos = currentMousePos;
                DrawPaint();
            }
        }
        else Debug.LogWarning("Prefab list is empty!");
    }

    void DrawPaint()
    {
        if (paintGroup == null)
        {
            if (GameObject.Find(paintGroupName)) paintGroup = GameObject.Find(paintGroupName);
            else paintGroup = new GameObject(paintGroupName);
        }

        int localDensity = brushDensity;
        Vector3 dir = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * Vector3.right;
        Vector3[] spawnPoint = new Vector3[localDensity];

        for (int i = 0; i < localDensity; i++)
        {
            dir = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up) * Vector3.right;
            Vector3 spawnPos = (dir * brushSize * Random.Range(0.1f, 1.1f)) + currentMousePos;

            if (spawnPos != Vector3.zero)
            {
                spawnPoint[i] = spawnPos;
                SpawnObject(spawnPoint[i]);
            }
        }
    }

    GameObject SpawnObject(Vector3 pos)
    {
        int rndIndex = Random.Range(0, objects.Count);
        GameObject prefabObj = objects[rndIndex].prefab;
        GameObject go = null;
        if (prefabObj != null)
        {
            go = (GameObject)PrefabUtility.InstantiatePrefab(prefabObj);

            if (gizmoLocation)
            {
                go.transform.rotation = gizmoLocation.rotation;
                go.transform.up = gizmoLocation.up;
            }

            else go.transform.rotation = Quaternion.identity;

            bool randomRotationX = objects[rndIndex].randomRotationX;
            bool randomRotationY = objects[rndIndex].randomRotationY;
            bool randomRotationZ = objects[rndIndex].randomRotationZ;

            if (randomRotationX) go.transform.Rotate(Vector3.right, Random.Range(0, 360));
            if (randomRotationY) go.transform.Rotate(Vector3.up, Random.Range(0, 360));
            if (randomRotationZ) go.transform.Rotate(Vector3.forward, Random.Range(0, 360));

            Vector2 scale = objects[rndIndex].scale;
            if (scale != Vector2.one && scale != Vector2.zero) go.transform.localScale *= Random.Range(scale.x, scale.y);
            go.transform.position = pos;
            DoubleRayCast(go, rndIndex);
            if (go) AddObjectToGroup(go, rndIndex);
        }
        return go;
    }

    void AddObjectToGroup(GameObject obj, int index)
    {
        Transform parent = GameObject.Find(paintGroupName).transform;
        if (parent == null) parent = new GameObject(paintGroupName).transform;
        obj.transform.SetParent(parent);
    }

    public bool LayerContain(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }

    void DoubleRayCast(GameObject obj, int index)
    {
        Vector3 position = obj.transform.position + obj.transform.up * maxYPosition;
        obj.transform.position = position;
        obj.SetActive(false);
        RaycastHit groundHit;

        if (Physics.Raycast(position, -obj.transform.up, out groundHit))
        {
            RaycastHit objectHit;
            if (LayerContain(paintMask, groundHit.collider.gameObject.layer))
            {
                obj.SetActive(true);
                if (Physics.Raycast(groundHit.point, obj.transform.up, out objectHit) && obj.layer == objectHit.collider.gameObject.layer)
                {
                    Vector3 newPos;
                    float differencialDistance = Vector3.Distance(objectHit.point, obj.transform.position);
                    newPos = groundHit.point + (obj.transform.up * differencialDistance);
                    obj.transform.position = newPos;
                    return;
                }
            }
        }
        DestroyImmediate(obj);
    }

    void Update()
    {
        SceneView.RepaintAll();
    }

    private void HierarchyChanged()
    {
        Repaint();
    }

    public static bool BeginFold(string foldName, bool foldState)
    {
        EditorGUILayout.BeginVertical(BoxStyle);
        GUILayout.Space(3);
        foldState = EditorGUI.Foldout(EditorGUILayout.GetControlRect(),
        foldState, foldName, true, FoldoutStyle);
        if (foldState) GUILayout.Space(3);
        return foldState;
    }

    public static void EndFold()
    {
        GUILayout.Space(3);
        EditorGUILayout.EndVertical();
        GUILayout.Space(0);
    }
}

