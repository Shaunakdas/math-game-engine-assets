using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace TexDrawLib
{
    public partial class TexFormulaParser
    {
        private Atom AttachScripts(TexFormula formula, string value, ref int position, Atom atom)
        {
            if (position == value.Length)
                return atom;
            if (value[position] == superScriptChar || value[position] == subScriptChar) {
                if (position == value.Length - 1) {
                    position++;
                    return atom;
                }
            } else 
                return atom;
			
            TexFormula superscriptFormula = null;
            TexFormula subscriptFormula = null;
			
            bool? markAsBig = null;
            //True: we are in ^ ;False: We are in _ ;Null: In Beginning
            bool? lastIsSuper = null;
			
            while (position < value.Length) {
                var ch = value[position];
//                Debug.Log(ch);
                if (ch == superScriptChar || ch == subScriptChar) {
                    if (markAsBig == false)
                        markAsBig = true;
                    else if (markAsBig == null)
                        markAsBig = false;
                    bool v = ch == superScriptChar;
                    if ((v ? superscriptFormula : subscriptFormula) == null)
                        lastIsSuper = v;
                    position++;
                    continue;
                } else if (ch == rightGroupChar || (value[position-1] != '^' && value[position-1] != '_'))
                    break;
                if (lastIsSuper == true) {
                    if (superscriptFormula == null)
                        superscriptFormula = ReadScript(formula, value, ref position);
                    else {
                        position--;
                        superscriptFormula.RootAtom = AttachScripts(formula, value, ref position, superscriptFormula.RootAtom);
                    }
                } else if (lastIsSuper == false) {
                    if (subscriptFormula == null)
                        subscriptFormula = ReadScript(formula, value, ref position);
                    else {
                        position--;
                        subscriptFormula.RootAtom = AttachScripts(formula, value, ref position, subscriptFormula.RootAtom);
                    }
                } else
                    break;
                if (markAsBig != true)
                    markAsBig = null;
            }
            /*if (ch == superScriptChar) {
                // Attahch superscript.
				position++;
				if (value[position] == superScriptChar) {
					markAsBig = true;
					position++;
				}
				superscriptFormula = ReadScript(formula, value, ref position);
				
				if (position < value.Length && value[position] == subScriptChar) {
                    // Attach subscript also.
					position++;
					if (value[position] == subScriptChar) {
						markAsBig = true;
						position++;
					}
					if (position < value.Length)
						subscriptFormula = ReadScript(formula, value, ref position);
				}
				
				if (position < value.Length && (value[position] == superScriptChar || value[position] == subScriptChar)) 
					superscriptFormula.RootAtom = AttachScripts(formula, value, ref position, superscriptFormula.RootAtom);
			} else if (ch == subScriptChar) {
                // Add subscript.
				position++;
				if (value[position] == subScriptChar) {
					markAsBig = true;
					position++;
				}
				subscriptFormula = ReadScript(formula, value, ref position);
				
				if (position < value.Length && value[position] == superScriptChar) {
                    // Attach superscript also.
					position++;
					if (value[position] == superScriptChar) {
						markAsBig = true;
						position++;
					}
					if (position < value.Length)
						superscriptFormula = ReadScript(formula, value, ref position);
				} 
				
				if (position < value.Length && (value[position] == superScriptChar || value[position] == subScriptChar)) 
					subscriptFormula.RootAtom = AttachScripts(formula, value, ref position, subscriptFormula.RootAtom);
			}
			*/
            if (superscriptFormula == null && subscriptFormula == null)
                return atom;
			
			
            // Check whether to return Big Operator or Scripts.
            if (atom != null && (atom.GetRightType() == CharType.BigOperator || markAsBig == true))
                return BigOperatorAtom.Get(atom, subscriptFormula == null ? null : subscriptFormula.GetRoot,
                    superscriptFormula == null ? null : superscriptFormula.GetRoot);
            else
                return ScriptsAtom.Get(atom, subscriptFormula == null ? null : subscriptFormula.GetRoot,
                    superscriptFormula == null ? null : superscriptFormula.GetRoot);
        }

		
        static readonly string scriptCloseChars =  " +-*/=()[]<>|.,;:`~\'\"?!@#$%&{}\\_^";

        private string ReadScriptGroup(TexFormula formula, string value, ref int position)
        {
            if (position == value.Length)
                return string.Empty;
			
            var startPosition = position;
            var group = 0;
            position++;
            while (position < value.Length && !(group == 0 && isScriptCloseChar(value[position]))) {
                if (value[position] == escapeChar) {
                    //result.Append(value[position]);
                    position++;
                    if (position == value.Length) {
                        // Reached end of formula but group has not been closed.
                        return value.Substring(startPosition);
                    }
                } else if (value[position] == leftGroupChar)
                    group++;
                else if (isScriptCloseChar(value[position]))
                    group--;
                // result.Append(value[position]);
                position++;
            }
            return value.Substring(startPosition, position - startPosition);
        }

        private TexFormula ReadScript(TexFormula formula, string value, ref int position)
        {
            if (position == value.Length)
                //throw new TexParseException("illegal end, missing script!");
				return formula;
			
            SkipWhiteSpace(value, ref position);
            var ch = value[position];
            if (ch == leftGroupChar) {
                return Parse(ReadGroup(formula, value, ref position, leftGroupChar, rightGroupChar));
            } else {
                return Parse(ReadScriptGroup(formula, value, ref position));
            }
        }

        static bool isScriptCloseChar(char c)
        {
            for (int i = 0; i < scriptCloseChars.Length; i++) {
                if (scriptCloseChars[i].Equals(c))
                    return true;
            }
            return false;
        }

		
		
    }
	
	
	
}
