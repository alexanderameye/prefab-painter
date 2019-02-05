using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PrefabPainter
{
    [CreateAssetMenu(fileName = "prefabpainter_palette", menuName = "Prefab Painter/Create Palette", order = 1)]
    public class PaintPalette : ScriptableObject
    {
        public List<PaintObject> palette = new List<PaintObject>();
    }
}