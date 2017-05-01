using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TexDrawLib
{
    [AddComponentMenu("TEXDraw/Supplemets/TEXSup Vertex Gradient", 16), ExecuteInEditMode]
    [TEXSupHelpTip("Blend vertex colors on each vertex corner")]
	public class TEXSupVertexGradient : TEXDrawMeshEffectBase
    {
      
        public Color topLeft = Color.white;
        public Color topRight = Color.white;
        public Color bottomRight = Color.white;
        public Color bottomLeft = Color.white;
        public override void ModifyMesh(Mesh m)
        {
           var colors = m.colors32;
            for (int i = 0; i < colors.Length;)
            {
                colors[i++] *= bottomLeft;
                colors[i++] *= bottomRight;
                colors[i++] *= topRight;
                colors[i++] *= topLeft;
            }
            m.colors32 = colors;
        }
    }
}