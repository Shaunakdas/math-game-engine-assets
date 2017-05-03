#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TexDrawLib
{
    public partial class TEXPreference : ScriptableObject
    {
#if UNITY_EDITOR

        [ContextMenu("Rebuild Font Data")]
        public void Reload()
	    {
            if (!EditorUtility.DisplayDialog("Confirm Reload", 
                "Are you sure to rebuild the font data?", 
                "OK", "No"))
                return;	
		    FirstInitialize(MainFolderPath); 
		 }
        
        [ContextMenu("Wipe All Data")]
        public void ResetWholeData () {
            var respond = EditorUtility.DisplayDialogComplex("Confirm Reset", 
                "Do you really want to reset all symbol setups from beginning?\n(WARNING: will erase anything in TexFontMetadata anda can't be undone)",
                "YES", "Yes, don't pick up from XML", "Cancel");
            if (respond == 2)
                return;
            foreach (var f in fontData)
            {
                DestroyImmediate(f, true);
            }
            FirstInitialize(MainFolderPath); 
            if (respond == 1)
                return; 
            ResetWholeDataConfirmed();
        }
        
        void ResetWholeDataConfirmed () {
            var user = AssetDatabase.LoadAssetAtPath<TextAsset>(MainFolderPath + "/XMLs/TexSymbolDefinitions.xml").text;
            TexImporterUtility.ReadLegacyXMLSymbols(this, false, user);
            var math = AssetDatabase.LoadAssetAtPath<TextAsset>(MainFolderPath + "/XMLs/TexMathDefinitions.xml").text;
            TexImporterUtility.ReadLegacyXMLSymbols(this, true, math);
            var config = AssetDatabase.LoadAssetAtPath<TextAsset>(MainFolderPath + "/XMLs/TexConfigurations.xml").text;
            var preset = AssetDatabase.LoadAssetAtPath<TextAsset>(MainFolderPath + "/XMLs/TexFontDefinitions.xml").text;
            TexImporterUtility.ReadLegacyPreferences(this, preset, config);            
        }
        
        void Reset()
        {
            string dataPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(this));
            if(string.IsNullOrEmpty(dataPath))
                return;
            m_main = AssetDatabase.LoadAssetAtPath<TEXPreference>(dataPath);
            FirstInitialize (dataPath);
            if( EditorUtility.DisplayDialog("Some suggestions", 
                "You just reset the preference. Do you want to pickup the original settings from XML (RECOMMENDED)",
                "YES", "No"))
                ResetWholeDataConfirmed();

        }
        
        [ContextMenu("Transfer from Legacy XML Data")]
        public void ReloadLegacy()
        {
            var respond = EditorUtility.DisplayDialogComplex("Confirm Transfer", 
                "Are you sure you want to transfer symbols from XML data? Use this for maintaining projects which using TEXDraw prior to 3.0.\nDo you also need to overwrite math symbols?",
                "Yes", "Cancel", "No, just User Fonts");
            if (respond == 1)
                return;
            var user = AssetDatabase.LoadAssetAtPath<TextAsset>(MainFolderPath + "/XMLs/TexSymbolDefinitions.xml").text;
            TexImporterUtility.ReadLegacyXMLSymbols(this, false, user);
            if (respond == 2)
                return;
            var math = AssetDatabase.LoadAssetAtPath<TextAsset>(MainFolderPath + "/XMLs/TexMathDefinitions.xml").text;
            TexImporterUtility.ReadLegacyXMLSymbols(this, true, math);
        }


        public void FirstInitialize(string mainPath)
        {
            try
            {
                editorReloading = true;
                EditorUtility.DisplayProgressBar("Reloading", "Reading XML Files...", 0f);
	            
                ClearDictionary();
                
                MainFolderPath = mainPath;

                TexImporterUtility.ReadFromResources(this);
                EditorUtility.DisplayProgressBar("Reloading", "Reading Configurations...", .95f);

                EditorUtility.DisplayProgressBar("Reloading", "Refreshing Instances...", .95f);

                PaintFontList();
                RebuildMaterial();
                PushToDictionaries();
                editorReloading = false;
                CallRedraw();
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
                AssetDatabase.SaveAssets();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                Debug.LogError("Failed to Rebuilding TEXDraw's Font Data");
            }
            EditorUtility.ClearProgressBar();
        }

        

        public bool PushToDictionaries()
        {
            return PushToDictionaries(false);
        }

        public bool PushToDictionaries(bool onlyPref)
        {
            int lastHash = 0;
            try
            {
                if (!onlyPref)
                {
                    ClearDictionary();
                    for (int i = 0; i < fontData.Length; i++)
                    {
                        var count = fontData[i].chars.Length;
                        for (int j = 0; j < count; j++)
                        {
                            TexChar c = GetChar(i, j);
                            c.CheckValidity();
                            lastHash = c.ToHash();
                            if (!string.IsNullOrEmpty(c.symbolName))
                                symbolData.Add(c.symbolName, lastHash);
                            if (!string.IsNullOrEmpty(c.symbolAlt))
                                symbolData.Add(c.symbolAlt, lastHash);
                            if (c.characterMap > 0)
                                charMapData.Add(TexChar.possibleCharMaps[c.characterMap], lastHash);
                        }
                    }
                }
                //else
                    preferences.Clear();
                for (int i = 0; i < configs.Length; i++)                
                    preferences.Add(configs[i].name, configs[i].value);
                
            }
            catch (System.Exception ex)
            {
                if (ex is System.ArgumentException)
                {
                    int pair;
                    try   { pair = GetChar(GetChar(lastHash).symbolName).ToHash(); }
                    catch { return false; }
                    Debug.LogErrorFormat("Duplicate Definitions Exist at {0:X3} and {1:X3}, Please fix it now.\nError: {2}", lastHash, pair, ex.Message);
                }
                else
                {
                    Debug.LogErrorFormat("Unknown Error at {0:X3}: {1}\n{2}\n\nStackTrace:\n{3}", lastHash, ex.GetType().Name , ex.Message, ex.StackTrace);
                }
                return false;
            }
            return true;
        }

        /*public bool TryPushToDictionaries()
        {
            int lastHash = 0;
            try
            {
                ClearDictionary();
                for (int i = 0; i < fontData.Length; i++)
                {
                    for (int j = 0; j < 128; j++)
                    {
                        TexChar c = GetChar(i, j);
                        c.CheckValidity();
                        lastHash = c.ToHash();
                        if (!string.IsNullOrEmpty(c.symbolName))
                            symbolData.Add(c.symbolName, lastHash);
                        if (!string.IsNullOrEmpty(c.symbolAlt))
                            symbolData.Add(c.symbolAlt, lastHash);
                        if (c.characterMap > 0)
                            charMapData.Add(TexChar.possibleCharMaps[c.characterMap], lastHash);
                    }
                }
                for (int i = 0; i < configs.Length; i++)
                {
                    preferences.Add(configs[i].name, configs[i].value);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                if (ex is System.ArgumentException)
                {
                    Debug.LogError(string.Format("Failed Exporting to XML Files: Duplicate Definitions Exist at {0:X}, Please fix it. Message:\n{1}", lastHash, ex.Message));
                }
                else
                {
                    Debug.LogError("Failed Exporting to XML Files: " + ex.GetType().Name + " at " + lastHash.ToString("0:X") + ", Message:\n" + ex.Message);
                }
                return false;
            }
        }

        public void RevalidateDictionary()
        {
            int lastHash = 0;
            ClearDictionary();
            for (int i = 0; i < fontData.Length; i++)
            {
                var count = fontData[i].chars.Length;
                for (int j = 0; j < count; j++)
                {
                    TexChar c = GetChar(i, j);
                    c.CheckValidity();
                    lastHash = c.ToHash();
                    if (!string.IsNullOrEmpty(c.symbolName))
                        symbolData[c.symbolName] = lastHash;
                    if (!string.IsNullOrEmpty(c.symbolAlt))
                        symbolData[c.symbolAlt] = lastHash;
                    if (c.characterMap > 0)
                        charMapData.Add(TexChar.possibleCharMaps[c.characterMap], lastHash);
                }
            }
        }*/

        void ClearDictionary()
        {
            if (symbolData != null)
                symbolData.Clear();
            else
                symbolData = new TexSymbolDictionary();
            if (charMapData != null)
                charMapData.Clear();
            else
                charMapData = new TexCharMapDictionary();
            if (preferences != null)
                preferences.Clear();
            else
                preferences = new TexPrefsDictionary();
            //glueTable = new int[100];

        }

        public void CallRedraw()
        {
            Component[] tex = Object.FindObjectsOfType<Component>();
            for (int i = 0; i < tex.Length; i++) {
                if(tex[i] is ITEXDraw) {
                    ((ITEXDraw)tex[i]).SetTextDirty(true);
                    EditorUtility.SetDirty(tex[i]);                    
                }
            }
            SceneView.RepaintAll();
        }

        public string[] ConfigIDs;
        public string[] FontIDs;
        public GUIContent[] FontIDsGUI;
        public int[] FontIndexs;

        public void PaintFontList()
        {
            List<string> s = new List<string>();
            List<string> t = new List<string>();
            List<int> n = new List<int>();
            n.Add(-1);
            t.Add("-1 (Use Math Typefaces)");
            for (int i = 0; i < fontData.Length; i++)
            {
                t.Add(string.Format("{0} - {1}.ttf", i , fontData[i].id));
                n.Add(i);
                s.Add(fontData[i].id);
            }
            ConfigIDs = s.ToArray();
            FontIDs = t.ToArray();
            FontIndexs = n.ToArray();
            FontIDsGUI = t.ConvertAll<GUIContent>(x => new GUIContent(x)).ToArray();
        }

        public Material[] watchedMaterial;

        public void RebuildMaterial()
        {
            EditorUtility.DisplayProgressBar("Updating...", "Updating TEXDraw materials", 0);
            if (!defaultMaterial)
            {
                defaultMaterial = AssetDatabase.LoadAssetAtPath<Material>(MainFolderPath + "/TEX-Default.mat");
                if (!defaultMaterial)
                    Debug.LogError("TEXDraw default Material Asset didn't found! please assign it manually.");
            }

            if(!watchedMaterial.Contains(defaultMaterial))
                ArrayUtility.Insert(ref watchedMaterial, 0, defaultMaterial);
                
            // Validate our lists first (no null and duplicates)
            watchedMaterial = watchedMaterial.Distinct().Where(x => x).ToArray();
            //var shaderLists = System.Array.ConvertAll<string, Shader>(AssetDatabase.FindAssets("t:Shader"), x => AssetDatabase.LoadAssetAtPath<Shader>(AssetDatabase.GUIDToAssetPath(x)));
            //var shaderNames = System.Array.ConvertAll<Shader, string>(shaderLists, x => x.name);
            var texCount = fontData.Length;
            for (int i = 0; i < watchedMaterial.Length; i++)
            {
                // Find best match sampler count in 'samples' shaders
                var mat = watchedMaterial[i];
                var type = mat.GetTag("TexMaterialType", false, "Null");
                var alts = mat.GetTag("TexMaterialAlts", false, "");
                if (type == "Null") {
                    Debug.LogWarningFormat("Material {0} is not using TEXDraw shaders. Removing it from Material Stacks", mat);
                    ArrayUtility.RemoveAt(ref watchedMaterial, i--);
                    continue;
                }  
                if (alts.Length > 1 && alts[0] == '@') {
                   // if (texCount > int.Parse(Regex.Match(mat.shader.name, @"([A-F0-9]+)(?= Samples)").Value, System.Globalization.NumberStyles.HexNumber)) {
                        mat.shader = Shader.Find(alts.Substring(1)) ?? Shader.Find(alts.Substring(1) + "/Full");
                        alts = mat.GetTag("TexMaterialAlts", false, "");
                   // }
                }
                if (alts.Length >= 1 && alts[0] != '@') {
                    var variants = TexCharPresets.charsFromString(alts);
                    if (variants.Any(x => x >= texCount)) {
                        var target = (int)(variants.Where(x => x >= texCount).Min());
                        var name = mat.shader.name;
///                        Debug.LogFormat("{0} : {1}", shaderTarget, shaderTarget.LastIndexOf("/Full"));
                        if (name.LastIndexOf("/Full") == name.Length - 5)
                            name = name.Substring(0, name.Length - 5);
                        var shaderTarget =string.Format("{0}/x{1:X} Samples", name, target);
                        var newShader = Shader.Find(shaderTarget);
                        if (newShader)
                            mat.shader = newShader;
                        else
                            Debug.LogWarningFormat("Shader {0} wasn't found, have you double check TexMaterialAlts?", shaderTarget);
                    }
                }
                // Now plug it on each data textures
                for (int j = 0; j < fontData.Length; j++)
                {
                    var propName = string.Format("_Font{0:X}", j);
                    var font = fontData[j];
                    if(!mat.HasProperty(propName))
                    {
                        Debug.LogWarningFormat("The shader {0} doesn't plug {1} or not declaring full fallback on TexMaterialAlts", mat.shader.name, propName);
                        break; 
                    }
                    if(font.type == TexFontType.Font) 
                        mat.SetTexture(propName, font.Font_Asset ? font.Font_Asset.material.mainTexture : null);
                    else
                        mat.SetTexture(propName, font.Sprite_Asset);
                }
            }
            EditorUtility.ClearProgressBar();
        }
#endif

    }
}