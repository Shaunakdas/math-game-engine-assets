using System;
using System.Collections.Generic;

using System.Text;
using UnityEngine;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace TexDrawLib
{
	public static class TexUtility
	{
		// Few const for simple adjusting ------------------------------------------------------------------------------
		public const float FloatPrecision = 0.001f;
        //The reason why it's 31 textures: because index 32 preserved for this block font!
        public const int blockFontIndex = 31;
        public static readonly Color white = Color.white; //Cached for speed
		public const FontStyle FontStyleDefault = (FontStyle)(-1);
		
		// Preserved Dynamic Configurations ----------------------------------------------------------------------------
		// These processed inside boxing process... all except RenderColor (that's one is used in final rendering )
		public static float 	RenderSizeFactor = 1;
		public static Color32 	RenderColor;
        public static int 		RenderFont = -1;
        public static int 		RenderTextureSize = 0;
		public static FontStyle RenderFontStyle = FontStyleDefault;
		public static float		AdditionalGlueSpace = 0;
		// RenderFont in parsing, is default -2. Which makes some problem. So sometimes we need to give a hint
        public static int 		RawRenderFont = -1;
		
            
		public static float spaceWidth
		{
			get	{ return TEXPreference.main.preferences["SpaceWidth"]; }
		}
        public static float spaceHeight
        {
            get { return TEXPreference.main.preferences["LineHeight"]; }
        }
        public static float glueRatio
		{
			get	{ return TEXPreference.main.preferences["GlueRatio"]; }
		}
		public static float lineThickness
		{
			get	{ return TEXPreference.main.preferences["LineThickness"]; }
		}

		// TexStyle manipulations (for different style, etc.) ----------------------------------------------------------
		public static float SizeFactor (TexStyle style)
		{
			if (style < TexStyle.Script)
				return RenderSizeFactor;
			else if (style < TexStyle.ScriptScript)
                return TEXPreference.main.preferences["ScriptFactor"] * RenderSizeFactor;
			else
                return TEXPreference.main.preferences["NestedScriptFactor"] * RenderSizeFactor;
		}

		public static TexStyle GetCrampedStyle (TexStyle Style)
		{
			return (int)Style % 2 == 1 ? Style : Style + 1;
		}

		public static TexStyle GetNumeratorStyle (TexStyle Style)
		{
			return Style + 2 - 2 * ((int)Style / 6);
		}

		public static TexStyle GetDenominatorStyle (TexStyle Style)
		{
			return (TexStyle)(2 * ((int)Style / 2) + 1 + 2 - 2 * ((int)Style / 6));
		}

		public static TexStyle GetRootStyle ()
		{
			return TexStyle.Script;
		}

		public static TexStyle GetSubscriptStyle (TexStyle Style)
		{
			return (TexStyle)(2 * ((int)Style / 4) + 4 + 1);
		}

		public static TexStyle GetSuperscriptStyle (TexStyle Style)
		{
			return (TexStyle)(2 * ((int)Style / 4) + 4 + ((int)Style % 2));
		}

        public static void CentreBox(Box box, TexStyle style)
        {
            float axis = TEXPreference.main.GetPreference("AxisHeight", style);
            box.shift = (box.height - box.depth) / 2 - axis;
        }

        public static Box GetBox (Atom atom, TexStyle style)
        {
            var box = atom.CreateBox(style);
            atom.Flush();
            return box;
        }

        public static Color MultiplyColor(Color a, Color b)
        {
        	if(a == white)
        		return b;
        	return new Color (a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
        }

        public static Color32 MultiplyAlphaOnly(Color32 c, float a)
        {
            c.a = (byte)(c.a * a);
            return c;
        }


        static public List<T> GetRangePool<T> (this List<T> source, int index, int count)
        {
            List<T> list = ListPool<T>.Get();
			//Method 1: Not working (Still Produces GC)
            /*
            T[] a = new T[count];
            source.CopyTo(index, a, 0, count);
            list.AddRange(a);
            */
            //Method 2: Too Slow
            /*
            for (int i = 0; i < count; i++)
            {
                list.Add(source[i + index]);
            }
             */
			//Method 3
            //CHAMPAGNE! It's works ;-D
			list.AddRange(source);
			if(index > 0)
				list.RemoveRange(0, index);
			if(list.Count > count)
				list.RemoveRange(count, list.Count - count);
			return list;

			//The only known thing that Produces GC, 
			//however, it currently the fastest thing to process
			//So I leave as it is,
            //return source.GetRange(index, count);
        }

		 public static FontStyle TexStyle2FontStyle (FontStyle style) {
            var s = (int)style;
            switch (Mathf.Min(s, s & 3))
            {
                case -1:    case 0:    return FontStyle.Normal;
                case 1:                 return FontStyle.Bold;
                case 2:                 return FontStyle.Italic;
                case 3:                 return FontStyle.BoldAndItalic;
                default:                return FontStyle.Normal;
            }
        }
		
    }
	
	[Serializable]
	public struct ScaleOffset
	{
        public float scale;
        public float offset;

		public ScaleOffset (float Scale, float Offset) {
            scale = Scale;
            offset = Offset;
        }
		
		public static ScaleOffset identity {
			get {
                return new ScaleOffset(1, 0);
            }
		}
        
		public float Evaluate(float v) {
            return v * scale + offset;
        }
		
		public Vector3 Evaluate (Vector3 v) {
		    return v * scale + Vector3.one * offset;
        }
    }
}