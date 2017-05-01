using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

namespace TexDrawLib
{
	//[AddComponentMenu("TEXDraw/Supplemets/TEXSup Auto Link URL")]
	/*public class TEXSupRTLSupport : TEXDrawSupplementBase
    {
	    public string commands = @"\ulink";
        const string f = @"(https?:\/\/)?(mailto:)?([@\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?";
        const string t = @"$&}";

        public override string ReplaceString(string original)
        {
        //var s= new  System.Globalization.TextInfo("");
           ///s.
            string dest = "{" + commands + " $&}";
            //This will recognize \n as new line
            return Regex.Replace(original, f, dest,RegexOptions.IgnoreCase);
        }
    }*/
}