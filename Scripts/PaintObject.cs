using UnityEngine;
using UnityEditor;

namespace PrefabPainter
{
    [System.Serializable]
    public class PaintObject
    {
        public GameObject prefab = null;
        public Vector2 scale = Vector2.one;
        public bool randomRotationX = false;
        public bool randomRotationY = false;
        public bool randomRotationZ = false;

        [HideInInspector] public Editor gameObjectEditor;

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
            gameObject = (GameObject) EditorGUILayout.ObjectField("", obj.prefab, typeof(GameObject), true);
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
}