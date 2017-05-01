using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace TexDrawLib
{
	public partial class TexFormulaParser
	{

        private static readonly string[] commands = new string[]
        {
			// List of commands at a glance
            "not", "nnot", "hnot", "dnot", "unot", "onot", "vnot", "vnnot",	// Not family
			"frac", "nfrac", "lfrac", "rfrac", "nlfrac", "nrfrac", 			// Fraction family
			"trs", "mtrs", "ltrs", "root", "link", "ulink", "under", "over",// Misc purposes
	        "table", "vtable", "ltable", "rtable", "vmatrix", "matrix",		// Matrix family
			"hold", "vhold", "thold", "bhold", "lhold", "rhold", "meta",	// Meta purposes
 			"color", "mclr", "clr", "size", "text", "math", 				// Font stylings
			 
            // Uncomment here if you want full control on fraction alignments
            /*   
				"llfrac", "rrfrac", "nllfrac", "nrrfrac", "lrfrac", "rlfrac", "nlrfrac", "nrlfrac", 
                "clfrac", "lcfrac", "crfrac", "clfrac", "nclfrac", "nlcfrac", "ncrfrac", "nclfrac",
            */ 
			
			// Uncomment here if you want full control on table alignments
            /*   
				"llltable", "llctable", "llrtable", "lcltable", "lcctable", "lcrtable", "lrltable", "lrctable", "lrrtable", 
            	"clltable", "clctable", "clrtable", "ccltable", "ccctable", "ccrtable", "crltable", "crctable", "crrtable", 
            	"rlltable", "rlctable", "rlrtable", "rcltable", "rcctable", "rcrtable", "rrltable", "rrctable", "rrrtable", 
            	
				"vllltable", "vllctable", "vllrtable", "vlcltable", "vlcctable", "vlcrtable", "vlrltable", "vlrctable", "vlrrtable", 
            	"vclltable", "vclctable", "vclrtable", "vccltable", "vccctable", "vccrtable", "vcrltable", "vcrctable", "vcrrtable", 
            	"vrlltable", "vrlctable", "vrlrtable", "vrcltable", "vrcctable", "vrcrtable", "vrrltable", "vrrctable", "vrrrtable", 
            */ 
        };
		
		// HashSet have huge performance benefit when using Contains() for repeated times
		public static readonly HashSet<string> commandsKey = new HashSet<string>(commands);
	
		private Atom ProcessCommand(TexFormula formula, string value, ref int position, string command)
		{
			SkipWhiteSpace(value, ref position);
			if (position == value.Length)
				return null;
			
			switch (command) {
			case "meta":
				// Command is meta
				var metaRule = formula.AttachedMetaRenderer;
				if (metaRule == null)
					metaRule = formula.AttachedMetaRenderer = ObjPool<TexMetaRenderer>.Get ();
				else
					metaRule.Reset();
					
				string metaPar = null;
				if (value[position] == leftBracketChar) {
					metaPar = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
					SkipWhiteSpace(value, ref position);
					metaRule.ParseString(metaPar);
 				}
			
			return null;
			case "root":
                // Command is radical.
				
				TexFormula degreeFormula = null;
				if (value[position] == leftBracketChar) {
                	degreeFormula = Parse(ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar));
					SkipWhiteSpace(value, ref position);
				}
				return Radical.Get(Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar))
					.GetRoot, degreeFormula == null ? null : degreeFormula.GetRoot);
			case "vmatrix":
			case "matrix":
                //Command is Matrix
				MatrixAtom matrixAtom = MatrixAtom.Get();
				List<List<Atom>> childs = matrixAtom.Elements;
				
				Atom parsedChild = (Parse(ReadGroup(
					formula, value, ref position, leftGroupChar, rightGroupChar)).GetRoot);
				childs.Add(ListPool<Atom>.Get());
				if (parsedChild == null)
					MatrixAtom.Last(childs).Add(SpaceAtom.Get());
				if (parsedChild is RowAtom) {
					List<Atom> el = ((RowAtom)parsedChild).Elements;
					if (command == "matrix")
						MatrixAtom.ParseMatrix(el, childs);
					else
						MatrixAtom.ParseMatrixVertical(el, childs);
					el.Clear();
					ObjPool<RowAtom>.Release((RowAtom)parsedChild);
				} else
					MatrixAtom.Last(childs).Add(parsedChild);
				matrixAtom.Elements = childs;
				return matrixAtom;
				
			case "math":
			case "text":
                    int idx = TexUtility.RenderFont == -1 ? TEXPreference.main.defaultTypefaces[TexCharKind.Text] : TexUtility.RenderFont;
				if (value[position] == leftBracketChar) {
					int.TryParse(ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar), out idx);
					SkipWhiteSpace(value, ref position);
				} else if (command == "math")
					idx = -1;
				
				var oldType = TexUtility.RenderFont;
				TexUtility.RenderFont = idx;
				var parsed = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar)).GetRoot;
				TexUtility.RenderFont = oldType;
				return parsed;
			case "clr":
			case "mclr":
			case "color":
				// Command is color
				string clr = null;
				if (value[position] == leftBracketChar) {
					clr = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
					SkipWhiteSpace(value, ref position);
				}
				
				if (position == value.Length)
					return null;
				AttrColorAtom endColor;
				var startColor = AttrColorAtom.Get(clr, command == "color" ? 1 : (command == "clr" ? 0 : 2), out endColor);
				return InsertAttribute(Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar)).GetRoot, startColor, endColor);
			case "size":
				// Command is size
				string sz = null;
				if (value[position] == leftBracketChar) {
					sz = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
					SkipWhiteSpace(value, ref position);
				}
				
				if (position == value.Length)
					return null;
				
				return AttrSizeAtom.Get(Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar))
					.GetRoot, sz);
			case "link":
			case "ulink":
				// Command is Link
				string meta = null;
				if (value[position] == leftBracketChar) {
					meta = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
					SkipWhiteSpace(value, ref position);
				}
				
				if (position == value.Length)
					return null;
				
				string groupBrack = ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar);
				if (meta == null)
					meta = groupBrack;
				return AttrLinkAtom.Get(Parse(groupBrack).GetRoot, meta, command == "ulink");
			case "under":
				command = "unot";
				break;
			case "over":
				command = "onot";
				break;
			}
			if (command.Length > 2 && command.Substring(command.Length - 3) == "not") {
				int NotMode = 0;
				string prefix = command.Substring(0, command.Length - 3);
				if (prefix.Length > 0) {
					switch (prefix[0]) {
					case 'n':
						NotMode = 1;
						break;
					case 'h':
						NotMode = 2;
						break;
					case 'd':
						NotMode = 3;
						break;
					case 'u':
						NotMode = 4;
						break;
					case 'o':
						NotMode = 5;
						break;
					case 'v':
						if (prefix.Length > 1 && prefix[1] == 'n')
							NotMode = 7;
						else
							NotMode = 6;
						break;
					}
				}
				if (position == value.Length)
					return null;
				
				string sz = null;
				if (value[position] == leftBracketChar) {
					sz = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
					SkipWhiteSpace(value, ref position);
				}
				
				return NegateAtom.Get(Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar))
					.GetRoot, NotMode, sz);
				
			}
			if (command.Length > 2 && command.Substring(command.Length - 3) == "trs") {
				int PivotMode = 0;
				string prefix = command.Substring(0, command.Length - 3);
				if (prefix.Length > 0) {
					switch (prefix[0]) {
					case 'm':
						PivotMode = 1;
						break;
					case 'l':
						PivotMode = 2;
						break;
					}
				}
				if (position == value.Length)
					return null;
				
				string trs = null;
				if (value[position] == leftBracketChar) {
					trs = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
					SkipWhiteSpace(value, ref position);
				}
				
				AttrTransformationAtom endTRS;
				var startTRS = AttrTransformationAtom.Get(trs, PivotMode, out endTRS);
				return InsertAttribute(Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar)).GetRoot, startTRS, endTRS);
			}
			if (command.Length > 3 && command.Substring(command.Length - 4) == "frac") {
				
				int FracAlignT = 0, FracAlignB = 0;
				bool FracAlignN = true; 
				string prefix = command.Substring(0, command.Length - 4);
				if (prefix.Length > 0) {
					if (prefix[0] == 'n') {
						FracAlignN = false;
						prefix = prefix.Substring(1);
					}
					if (prefix.Length == 1) {
						FracAlignT = fracP(prefix[0]);
						FracAlignB = FracAlignT;
					} else if (prefix.Length == 2) {
						FracAlignT = fracP(prefix[0]);
						FracAlignB = fracP(prefix[1]);
					}
				}
				if (position == value.Length)
					return null;
				Atom numeratorFormula = null, denominatorFormula = null;
				numeratorFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar)).GetRoot;
				SkipWhiteSpace(value, ref position);
				if (position != value.Length)
					denominatorFormula = Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar)).GetRoot;
				
				return FractionAtom.Get(numeratorFormula, denominatorFormula, FracAlignN,
				(TexAlignment)FracAlignT, (TexAlignment)FracAlignB);
			}
			if (command.Length > 3 && command.Substring(command.Length - 4) == "hold") {
				TexAlignment align = TexAlignment.Center;
				bool isVertical = false;
				string prefix = command.Substring(0, command.Length - 3);
				if (prefix.Length > 0) {
					switch (prefix[0]) {
					case 'v':
						isVertical = true;
						break;
					case 'l':
						align = TexAlignment.Left;
						break;
					case 'r':
						align = TexAlignment.Right;
						break;
					case 'b':
						align = TexAlignment.Bottom;
						goto case 'v';
					case 't':
						align = TexAlignment.Top;
						goto case 'v';
					}
				}

				float sz = 0;
				if (position < value.Length && value[position] == leftBracketChar) {
					float.TryParse (ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar),out sz);
					SkipWhiteSpace(value, ref position);
				}
				if (position < value.Length && value[position] == leftGroupChar)
				return HolderAtom.Get(Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar))
					.GetRoot, sz, isVertical, align);
				else
					return HolderAtom.Get(null, sz, isVertical, align);
			}
			if (command.Length > 4 && command.Substring(command.Length - 5) == "table") {
				bool vertical = false;
				int align = 1 + 8 + 64;
				string prefix = command.Substring(0, command.Length - 5);
				if (prefix.Length > 0) {
					if (prefix[0] == 'v') {
						vertical = true;
						prefix = prefix.Substring(1);
					}
					if (prefix.Length == 1) {
						var pref = fracP(prefix[0]);
						align = Math.Max(1, pref * 2) + Math.Max(8, pref * 16) + Math.Max(64, pref * 128);
					} else if (prefix.Length == 3) {
						var pref0 = fracP(prefix[0]);
						var pref1 = fracP(prefix[1]);
						var pref2 = fracP(prefix[2]);
						align = Math.Max(1, pref0 * 2) + Math.Max(8, pref1 * 16) + Math.Max(64, pref2 * 128);
					}
				}
				
				int lineStyleH = 0, lineStyleV = 0;
				if (value[position] == leftBracketChar) {
					string lineOpt;
					int lineP = 0;
					lineOpt = ReadGroup(formula, value, ref position, leftBracketChar, rightBracketChar);
					for (int i = 0; i < lineOpt.Length; i++) {
						if (!int.TryParse(lineOpt[i].ToString(), out	lineP))
							continue;
						if (i >= 6)
							break;
						switch (i) {
						case 0:
							lineStyleH += lineP >= 2 ? 17 : lineP;
							break;
						case 1:
							lineStyleH += lineP >= 2 ? 10 : (lineP == 1 ? 2 : 0);
							break;
						case 2:
							lineStyleH += lineP >= 1 ? 4 : 0;
							break;
						case 3:
							lineStyleV += lineP >= 2 ? 17 : lineP;
							break;
						case 4:
							lineStyleV += lineP >= 2 ? 10 : (lineP == 1 ? 2 : 0);
							break;
						case 5:
							lineStyleV += lineP >= 1 ? 4 : 0;
							break;				            
						}
					}
					SkipWhiteSpace(value, ref position);
				} else {
					lineStyleH = 7;
					lineStyleV = 7;
				}
				
				
				List<List<Atom>> childs = new List<List<Atom>>();
				MatrixAtom matrixAtom = ObjPool<MatrixAtom>.Get();
				matrixAtom.horizontalAlign = align;
				matrixAtom.horizontalLine = lineStyleH;
				matrixAtom.verticalLine = lineStyleV;
				
				Atom parsedChild = (Parse(ReadGroup(
					formula, value, ref position, leftGroupChar, rightGroupChar)).GetRoot);
				childs.Add(ListPool<Atom>.Get());
				if (parsedChild == null)
					MatrixAtom.Last(childs).Add(SpaceAtom.Get());
				if (parsedChild is RowAtom) {
					List<Atom> el = ((RowAtom)parsedChild).Elements;
					if (!vertical)
						MatrixAtom.ParseMatrix(el, childs);
					else
						MatrixAtom.ParseMatrixVertical(el, childs);
					el.Clear();
					ObjPool<RowAtom>.Release((RowAtom)parsedChild);
				} else
					MatrixAtom.Last(childs).Add(parsedChild);
				matrixAtom.Elements = childs;
				return matrixAtom;
				
			}
			throw new TexParseException("Invalid command.");
		}
		
		static int fracP(char c)
		{
			switch (c) {
			case 'l':
				return 1;
			case 'c':
				return 0;
			case 'r':
				return 2;
			default:
				return 0;
			}
		}

        static public bool isCommandRegistered(string str)
        {
			return commandsKey.Contains(str);
	        /*for (int i = commands.Length; i --> 0;) {
                if(commands[i].Equals (str))
                    return true;
            }
            return false;*/
        }
		
	}
}