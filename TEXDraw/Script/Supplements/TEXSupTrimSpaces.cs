using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System;

namespace TexDrawLib
{
	[AddComponentMenu("TEXDraw/Supplemets/TEXSup Trim Spaces")]
	[TEXSupHelpTip("Trim Empty spaces between lines", true)]
	public class TEXSupTrimSpaces : TEXDrawSupplementBase
    {
      
        public override string ReplaceString(string original)
        {
            //Not the best way to go (GC Hugger)
            var arr = original.Split(new char[] { '\n' });
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = arr[i].Trim();
            }
            return String.Join("\n", arr);
        }
    }
}