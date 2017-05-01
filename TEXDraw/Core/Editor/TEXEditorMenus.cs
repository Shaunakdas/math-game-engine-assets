using UnityEngine;
using UnityEditor;
using TexDrawLib;
using System.Collections.Generic;

[InitializeOnLoadAttribute()]
public static class TEXEditorMenus
{

    [MenuItem("Tools/TEXDraw/Show Preference",false,5)]
    public static void OpenPreference()
    {
        Selection.activeObject = TEXPreference.main;
    }
    
    [MenuItem("Tools/TEXDraw/Rebuild Font Data",false,6)]
    public static void ImportPreference()
    {
        TEXPreference.main.Reload();
    }
     
    [MenuItem("Tools/TEXDraw/Rebuild Material",false,7)]
    public static void ImportMaterial()
    {
        TEXPreference.main.RebuildMaterial();
    }
    
    [MenuItem("Tools/TEXDraw/Rebuild TEXDraw on Scene %&R",false,7)]
    public static void RepaintTEXDraws()
    {
        TEXPreference.main.CallRedraw();
    }
    
    [MenuItem("Tools/TEXDraw/Select Default Material",false,7)]
    public static void SelectDefaultMaterial()
    {
         Selection.activeObject = TEXPreference.main.defaultMaterial;
    }
    
    [MenuItem("Tools/TEXDraw/Pool Checks", false, 20)]
    public static void ShowPoolCheck()
    {
        var w = ScriptableObject.CreateInstance<EditorObjectPool>();
        w.Show();
    }
    
    [MenuItem("Tools/TEXDraw/Font Swapper Tool", false, 20)]
    public static void ShowFontSwapper()
    {
        var w = ScriptableObject.CreateInstance<EditorTEXFontSwapper>();
        w.Show();
    }
    
    [MenuItem("Tools/TEXDraw/Quick Editor Tool", false, 20)]
    public static void ShowQuickEditor()
    {
        var w = ScriptableObject.CreateInstance<EditorTEXUIQuickEditors>();
        w.Show();
    }
    
    const string lorem1K = @"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Maecenas porttitor congue massa. Fusce posuere, magna sed pulvinar ultricies, purus lectus malesuada libero. 
Sit amet commodo magna eros quis urna. Nunc viverra imperdiet enim. Fusce est. Vivamus a tellus. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. 
Proin pharetra nonummy pede. Mauris et orci. Aenean nec lorem. In porttitor. Donec laoreet nonummy augue. Suspendisse dui purus, scelerisque at, vulputate vitae, pretium mattis, nunc. 
Mauris eget neque at sem venenatis eleifend. Ut nonummy. Fusce aliquet pede non pede. Suspendisse dapibus lorem pellentesque magna. Integer nulla. Donec blandit feugiat ligula. Donec hendrerit. 
Felis et imperdiet euismod, purus ipsum pretium metus, in lacinia nulla nisl eget sapien. Donec ut est in lectus consequat consequat. Etiam eget dui. Aliquam erat volutpat. Sed at lorem in nunc porta tristique.";

    [MenuItem("Tools/TEXDraw/Fill with Lorem Ipsum",false, 60)]
    public static void FillLoremIpsum()
    {
        var sels = Selection.gameObjects;
        foreach (var obj in sels)
        {
            var tex = GetTexDraw(obj);
            if (tex != null) {
                Undo.RecordObject((Object)tex, "Change TexDraw texts");
                tex.text = lorem1K;
                if (tex.autoWrap == Wrapping.NoWrap)
                    tex.autoWrap = Wrapping.WordWrap;
                tex.autoFit = Fitting.BestFit;
                EditorUtility.SetDirty((Object)tex);
            }
        }
    }
    
    [MenuItem("Tools/TEXDraw/Benchmark for 15 Seconds",false, 60)]
    public static void Benchmark ()
    {
        var tex = GetTexDraw(Selection.activeGameObject);
        if (tex == null)
            return;
        try{
        // Init
        var title = "Benchmarking " + Selection.activeGameObject.name;
        var parsingTxt = "Measuring Parsing Performance ..";
        var boxingTxt = "Measuring Boxing Performance ..";
        var renderingTxt = "Measuring Rendering Performance ..";
        
        int parseCount = 0, boxCount = 0, renderCount = 0;
        var parser = tex.drawingContext;
        var text = tex.text;
        for (int i = 0; i++ < 10;)
        {
            var now = EditorApplication.timeSinceStartup;
            do
            {
                parser.Parse(text);
                parseCount++;
            } while ((EditorApplication.timeSinceStartup - now) < 0.5);
            EditorUtility.DisplayProgressBar(title, parsingTxt, i / 30f);
        }
        Debug.LogFormat("Parsing {0} times in 5 seconds, average {1:F2} ms, normally makeup {2:P2} (of 60 FPS) CPU Performance", parseCount, 5000.0 / parseCount,  5.0 * 60.0 / parseCount);
        
        var mesh = new Mesh();
        var param = tex.drawingParams;
        
        for (int i = 0; i++ < 10;)
        {
            var now = EditorApplication.timeSinceStartup;
            do
            {
                param.formulas = DrawingContext.ToRenderers(parser.parsed, param);
                boxCount++;
            } while ((EditorApplication.timeSinceStartup - now) < 0.5);
            EditorUtility.DisplayProgressBar(title, boxingTxt, i / 30f + 0.333f);
        }
        Debug.LogFormat("Boxing {0} times in 5 seconds, average {1:F2} ms, normally makeup {2:P2} (of 60 FPS) CPU Performance", boxCount, 5000.0 / boxCount,  5.0 * 60.0 / boxCount);

        for (int i = 0; i++ < 10;)
        {
            var now = EditorApplication.timeSinceStartup;
            do
            {
                parser.Render(mesh, param);
                renderCount++;
            } while ((EditorApplication.timeSinceStartup - now) < 0.5);
            EditorUtility.DisplayProgressBar(title, renderingTxt, i / 30f + 0.667f);
        }
        Debug.LogFormat("Rendering {0} times in 5 seconds, average {1:F2} ms, normally makeup {2:P2} (of 60 FPS) CPU Performance", renderCount, 5000.0 / renderCount, 5.0 * 60.0 / renderCount);
        Object.DestroyImmediate(mesh);
        
        var totalTime = 5.0 / parseCount + 5.0 / boxCount + 5.0 / renderCount; // in seconds
        if (totalTime > 1 / 60.0)
            Debug.LogFormat("Complete build time takes <b>{0:F2}</b> ms, makeup <b>{1:P2}</b> of CPU Time at 60 FPS. <color={2}><b><i>Potentially Breakup Game Performance</i></b></color>", totalTime * 1000.0, totalTime * 60.0, EditorGUIUtility.isProSkin ? "yellow" : "brown");
        else
            Debug.LogFormat("Complete build time takes <b>{0:F2}</b> ms, makeup <b>{1:P2}</b> of CPU Time at 60 FPS, or up to <b>{2}</b> build times at 60 FPS", totalTime * 1000.0, totalTime * 60.0, (int)(1.0 / (totalTime * 60.0)));
        
        } catch {}
        
        
        EditorUtility.ClearProgressBar();
    }
    
    [MenuItem("Tools/TEXDraw/Benchmark for 15 Seconds", true, 20)]
    [MenuItem("Tools/TEXDraw/Fill with Lorem Ipsum", true, 20)]
    public static bool CanBenchmark ()
    {
        return GetTexDraw(Selection.activeGameObject) != null;
    }

    public static ITEXDraw GetTexDraw(GameObject obj)
    {
        if (!obj)
            return null;
        ITEXDraw target = null;
        List<MonoBehaviour> l = ListPool<MonoBehaviour>.Get();
        obj.GetComponents<MonoBehaviour>(l);
        for (int i = 0; i < l.Count; i++)
        {
            if (l[i] is ITEXDraw)
            {
                target = (ITEXDraw)l[i];
                break;
            }
        }
       ListPool<MonoBehaviour>.ReleaseNoFlush(l);
       return target;
    }
    
    [MenuItem("Tools/TEXDraw/Show Info on Supplements", false, 60)]
    public static void ToggleAdditionalInfo()
    {
        var v = TEXSupplementEditor.isHelpShown = !TEXSupplementEditor.isHelpShown;
        EditorPrefs.SetBool("TEXDraw_ShowTipOnSupplement", v);
        if (Selection.activeGameObject)
            EditorUtility.SetDirty(Selection.activeGameObject);
    }

    [MenuItem("Tools/TEXDraw/Show Info on Supplements", true, 60)]
    public static bool InvalidateToggleAdditionalInfo()
    {
        Menu.SetChecked("Tools/TEXDraw/Show Info on Supplements", TEXSupplementEditor.isHelpShown);
        return true;
    }

    static TEXEditorMenus () {
        TEXSupplementEditor.isHelpShown = EditorPrefs.GetBool("TEXDraw_ShowTipOnSupplement", true);
    }
    
    [MenuItem("Tools/TEXDraw/Set Selected as Template", false, 80)]
    public static void SetDefaultTemplate () {
        var obj = Selection.activeGameObject;
        var tex = GetTexDraw(obj);
        var name = (tex.GetType().Name);
        var respond = EditorUtility.DisplayDialogComplex("Confirm set as Template",
            string.Format("Do you want to set selected {0} as a template every time you create a new {0} object?" +
        "\n(This includes its components and values expect the text itself)\n(This template will only affect project-wide environment)", name)
        , "Yes", "Yes, but keep the text", "Cancel");
        if (respond == 2)
            return;
        var path = TEXPreference.main.MainFolderPath + "/Template-" + name + ".prefab";
        obj = GameObject.Instantiate(obj);
        if (respond == 0)
            GetTexDraw(obj).text = "TEXDraw";
        PrefabUtility.CreatePrefab(path, obj, ReplacePrefabOptions.Default);
        GameObject.DestroyImmediate(obj, false);
    }

    [MenuItem("Tools/TEXDraw/Set Selected as Template", true, 80)]
    public static bool ValidateDefaultTemplate()
    {
        return GetTexDraw(Selection.activeGameObject) != null;
    }
    
    [MenuItem("Tools/TEXDraw/Clear Template", false, 80)]
    public static void ClearDefaultTemplates () {
        if (!EditorUtility.DisplayDialog("Confirm clear Template",
        "This action will clear every TEXDraw template you made in this project.\n(You actually can do this manually via the root of TEXDraw folder)\n(Can't be undone)", "OK", "Cancel"))
            return;
        var files = AssetDatabase.FindAssets("Template-", new string[]{ TEXPreference.main.MainFolderPath});
        foreach (var guid in files)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));
        }
    }
    
    [MenuItem("Tools/TEXDraw/Clear Template", true, 80)]
    public static bool ValidateTemplateExist()
    {
       var files = AssetDatabase.FindAssets("Template-", new string[]{ TEXPreference.main.MainFolderPath});
        return files.Length > 0;
    }


}