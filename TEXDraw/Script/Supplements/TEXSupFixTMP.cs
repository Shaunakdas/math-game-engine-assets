using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace TexDrawLib
{
    [AddComponentMenu("TEXDraw/Supplemets/TEXSup Fix TMP", 16), ExecuteInEditMode]
    [TEXSupHelpTip("Special modifier to fix UV2 behavior when using TMP")]
	public class TEXSupFixTMP : TEXDrawMeshEffectBase
    {
        [Range(0.001f, 5f)]
        public float sharpnessRatio = 1;
        public bool uniformSharpness = false;
        public override void ModifyMesh(Mesh m)
        {
#if TEXDRAW_TMP
            var xScale =  transform.lossyScale.y * tex.drawingParams.factor * sharpnessRatio;
            
            var tans = m.tangents;
            if (!uniformSharpness) {
                for (int i = 0; i < tans.Length; i++)
                {
                       // uv2[i].x = PackUV(uv2[i].x, uv2[i].y); 
                        tans[i].z = xScale ;
                }
            } else {
                var verts = m.vertices;
                xScale /= tex.size;
                for (int i = 0; i < tans.Length;)
                {
                        var min = verts[i];
                        var max = verts[i+2];
                        var xScale2 = xScale * (max.y - min.y);
                        for (int j = 0; j < 4; j++)
                        {
                        //    uv2[i].x = PackUV(uv2[i].x, uv2[i].y); 
                            tans[i++].z = xScale2 ;
                        }
                }
            }
            m.tangents = tans;
#endif
        }        
    }
}
