using UnityEngine;
using UnityEditor;

namespace PrefabPainter
{
    [System.Serializable]
    public class PaintObject
    {
        private GameObject go;
        private Vector2 size = Vector2.one;
        private bool randomRotationX = false;
        private bool randomRotationY = false;
        private bool randomRotationZ = false;
        private string prefabName;

        private bool settingsToggled;

        [HideInInspector] public Editor gameObjectEditor;

        public PaintObject(GameObject go)
        {
            this.go = go;
        }

        public void displaySettings()
        {
            EditorGUILayout.BeginVertical(PrefabPainter.BoxStyle);
            GUILayout.Space(3);

            if (prefabName == "") GUILayout.Label("Prefab Settings", EditorStyles.boldLabel);
            else GUILayout.Label("Prefab Settings - " + prefabName, EditorStyles.boldLabel);

            size.x = EditorGUILayout.FloatField("Min Size", size.x);
            size.y = EditorGUILayout.FloatField("Max Size", size.y);
            GUILayout.Label("Random Rotation :");
            EditorGUILayout.BeginHorizontal();
            randomRotationX = GUILayout.Toggle(randomRotationX, "X");
            randomRotationY = GUILayout.Toggle(randomRotationY, "Y");
            randomRotationZ = GUILayout.Toggle(randomRotationZ, "Z");
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(3);
            EditorGUILayout.EndVertical();
            GUILayout.Space(0);
        }

        public bool getRandomRotationX()
        {
            return randomRotationX;
        }

        public bool getRandomRotationY()
        {
            return randomRotationY;
        }

        public bool getRandomRotationZ()
        {
            return randomRotationZ;
        }

        public Vector2 getSize()
        {
            return size;
        }

        public GameObject GetGameObject()
        {
            return go;
        }

        public void setName(string name)
        {
            prefabName = name;
        }

        public string getName()
        {
            return prefabName;
        }
    }
}