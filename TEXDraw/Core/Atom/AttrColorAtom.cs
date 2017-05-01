using UnityEngine;
using System.Collections;

namespace TexDrawLib
{
    public class AttrColorAtom : Atom
    {
		      
        public static AttrColorAtom Get(string colorStr, int mix, out AttrColorAtom endBlock)
        {
            var atom = ObjPool<AttrColorAtom>.Get();
            endBlock = ObjPool<AttrColorAtom>.Get();
            atom.EndAtom = endBlock;
            atom.mix = mix;
            endBlock.mix = mix;
            if (colorStr == null)
                return atom;
            if (colorStr.Length == 1)
                colorStr = ModifiedTerminalColor(colorStr[0]);
            if (!ColorUtility.TryParseHtmlString(colorStr, out atom.color))
            if (!ColorUtility.TryParseHtmlString("#" + colorStr, out atom.color))
                atom.color = Color.white;
            endBlock.color = atom.color;
            return atom;
        }

        public AttrColorAtom EndAtom;

        public AttrColorBox generatedBox;

        public Color color = Color.white;
        public int mix;

        public override Box CreateBox(TexStyle style)
        {
            if (generatedBox != null) {
                return generatedBox;
            }

            generatedBox = AttrColorBox.Get(this, 
                EndAtom == null ? null : (AttrColorBox)EndAtom.CreateBox(style));
            return generatedBox;
            
        }

        public override void Flush()
        {
            EndAtom = null;
            color = Color.clear;
            generatedBox = null;
            ObjPool<AttrColorAtom>.Release(this);
        }
	    
        // Real CMD/Terminal color switch index
        public static string terminalColor(char code)
        {
            switch (code) {
                case '0':
                    return "#000"; // Black
                case '1':
                    return "#008"; // Blue
                case '2':
                    return "#080"; // Green
                case '3':
                    return "#088"; // Aqua
                case '4':
                    return "#800"; // Red
                case '5':
                    return "#808"; // Purple
                case '6':
                    return "#880"; // Yellow
                case '7':
                    return "#ccc"; // White
                case '8':
                    return "#888"; // Gray
                case '9':
                    return "#00f"; // Light Blue
                case 'a':
                case 'A':
                    return "#0f0"; // Light Green
                case 'b':
                case 'B':
                    return "#0ff"; // Light Aqua
                case 'c':
                case 'C':
                    return "#f00"; // Light Red
                case 'd':
                case 'D':
                    return "#f0f"; // Light Purple
                case 'e':
                case 'E':
                    return "#ff0"; // Light Yellow
                case 'f':
                case 'F':
                    return "#fff"; // Bright White
                default:
                    return "#fff";
            }
        }
	    
        // Modifier version, from 0 (darkest), to f (lightest)
        // Sorted according to our eye spectrum : Blue, Red, then Green
        public static string ModifiedTerminalColor(char code)
        {
            switch (code) {
                case '0':
                    return "#000"; // Black
                case '1':
                    return "#008"; // Blue
                case '2':
                    return "#800"; // Red
                case '3':
                    return "#080"; // Green
                case '4':
                    return "#808"; // Purple
                case '5':
                    return "#088"; // Aqua
                case '6':
                    return "#880"; // Yellow
                case '7':
                    return "#888"; // Gray
                case '8':
                    return "#ccc"; // White
                case '9':
                    return "#00f"; // Light Blue
                case 'a':
                case 'A':
                    return "#f00"; // Light Red
                case 'b':
                case 'B':
                    return "#0f0"; // Light Green
                case 'c':
                case 'C':
                    return "#f0f"; // Light Purple
                case 'd':
                case 'D':
                    return "#0ff"; // Light Aqua
                case 'e':
                case 'E':
                    return "#ff0"; // Light Yellow
                case 'f':
                case 'F':
                    return "#fff"; // Bright White
                default:
                    return "#fff";
            }
        }
	    
    }
}
