using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

namespace TexDrawLib
{
	[AddComponentMenu("TEXDraw/Supplemets/TEXSup Auto Link URL")]
	[TEXSupHelpTip("Auto detect URL and email links. Requires TEXLink")]
	public class TEXSupLinkURL : TEXDrawSupplementBase
    {
    public string commands = @"\ulink";
        const string f = @"(https?:\/\/)?(mailto:)?([@\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?";
        const string t = @"$&}";

        public override string ReplaceString(string original)
        {
            string dest = "{" + commands + " $&}";
            //This will giving \link any detected URL (and email)
            return Regex.Replace(original, f, dest,RegexOptions.IgnoreCase);
        }
    }
}