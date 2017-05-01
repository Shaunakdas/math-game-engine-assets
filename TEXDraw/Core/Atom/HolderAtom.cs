using UnityEngine;
using System.Collections;

namespace TexDrawLib
{
    public class HolderAtom : Atom
    {
	    public static HolderAtom Get(Atom baseAtom, float Width, bool IsVertical, TexAlignment Alignment)
        {
            var atom = ObjPool<HolderAtom>.Get();
            atom.BaseAtom = baseAtom;
            atom.isVertical = IsVertical;
	        atom.length = Width;
	        atom.align = Alignment;
	       
	        atom.Type = baseAtom != null ? baseAtom.Type : CharType.Ordinary;
            return atom;
        }


        public Atom BaseAtom;

	    public float length = 0;
	    public bool isVertical = false;
        public TexAlignment align;

        public override Box CreateBox(TexStyle style)
	    {
		    var width = isVertical ? 0 : length;
		    var height = isVertical ? length : 0;

			
		    if (BaseAtom == null || length < 0)
                return StrutBox.Get(width, height, 0, 0);
            else if (BaseAtom is SpaceAtom)
                return StrutBox.Get(width, height, 0, 0);
            else
            {
            	Box result;
                if (width == 0 && BaseAtom is SymbolAtom)
                    result = VerticalBox.Get(DelimiterFactory.CreateBox(((SymbolAtom)BaseAtom).Name, height, style), height, align);
                else if (height == 0 && BaseAtom is SymbolAtom)
                    result = HorizontalBox.Get(DelimiterFactory.CreateBoxHorizontal(((SymbolAtom)BaseAtom).Name, width, style), width, align);
                else if (width == 0)
	                result = VerticalBox.Get(BaseAtom.CreateBox(style), height, align);
                else
	                result = HorizontalBox.Get(BaseAtom.CreateBox(style), width, align);
	            TexUtility.CentreBox(result, style);
	            return result;
            }
        }

        public override void Flush()
        {
            if (BaseAtom != null)
            {
                BaseAtom.Flush();
                BaseAtom = null;
            }
            ObjPool<HolderAtom>.Release(this);
        }


    }
}
