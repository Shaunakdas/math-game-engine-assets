using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

namespace TexDrawLib
{
	[AddComponentMenu("TEXDraw/Supplemets/TEXSup New Line")]
	[TEXSupHelpTip("Detect \\n for new line")]
	public class TEXSupNewLine : TEXDrawSupplementBase
    {
        const string f = @"\\n[(?=\W)|\s]";
        const string t = "\n";

        public override string ReplaceString(string original)
        {
            //This will recognize \n as new line
            return Regex.Replace(original, f, t);
        }
    }
}