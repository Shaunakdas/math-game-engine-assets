using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

namespace TexDrawLib
{
	[AddComponentMenu("TEXDraw/Supplemets/TEXSup Transform Follow")]
	[TEXSupHelpTip("Give signal to repaint if transform is changed")]
	public class TEXSupTransformFollow : TEXDrawSupplementBase
    {
        public override string ReplaceString(string original)
        {
            return original;
        }
         
        void Update ()
        {
            if (transform.hasChanged && tex != null) {
                tex.SetTextDirty(true);      
            }
        }
    }
}