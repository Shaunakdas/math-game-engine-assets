using UnityEngine;
using System.Collections.Generic;


/*
	TEXDrawNGUIPanelManager is a component that facilitates an extra (but unofficial)
	features that makes uploading UV2 is possible within it's internal engine.
	You don't and shouldn't attaching it manually, because it is done automatically.
	
	If there are two diferent TEXDraw material used, then this component 
	is replicating into two separate component, automatically too...
 */

[RequireComponent(typeof(UIPanel))]
[ExecuteInEditMode]
public class TEXDrawNGUIPanelManager : MonoBehaviour
{
    public Material panelMaterial;
    public BetterList<UIDrawCall> drawCalls;
    public BetterList<TEXDrawNGUIDrawCall> texDrawCalls;
    public BetterList<Vector2[]> pendingUV2;
    public BetterList<Vector4[]> pendingTan;
    public BetterList<TEXDrawNGUI> pendingTex;

    void UpdateNGUIStacks()
    {
        UpdateNGUIStacks(false);
    }

    void UpdateNGUIStacks(bool alsoRefill)
    {
        drawCalls.Clear();
        texDrawCalls.Clear();
        var dc = GetComponent<UIPanel>().drawCalls;
        var dn = GetComponents<TEXDrawNGUIDrawCall>();
        for (int i = 0; i < dc.Count; i++) {
            var dnc = i >= dn.Length ? gameObject.AddComponent<TEXDrawNGUIDrawCall>() : dn[i];
            dnc.index = i;

            drawCalls.Add(dc[i]);
            texDrawCalls.Add(dnc);
            dnc.drawCallQueue = dc[i].renderQueue;
            dnc.material = dc[i].baseMaterial;
            dnc.drawCall = dc[i];
            dnc.Widgets.Clear();
            dnc.enabled = true;
        }

        for (int i = dc.Count; i < dn.Length; i++) {
            dn[i].enabled = false;
        }

        var w = GetComponent<UIPanel>().widgets;
        for (int i = 0; i < w.Count; i++) {
            if (w[i] is TEXDrawNGUI && w[i].isActiveAndEnabled) {
                for (int j = 0; j < drawCalls.size; j++) {
                    if (drawCalls[j] == w[i].drawCall) {
                        var t = (TEXDrawNGUI)w[i];
                        texDrawCalls[j].Widgets.Add(t);
                        if (alsoRefill)
                            t.FillUV2();
                        break;
                    }
                }
            }
        }
    }
    //Called by TEXDraw NGUI if there is OnFill called,
    public void UploadToDrawCalls(TEXDrawNGUI texDraw, Vector2[] uv2, Vector4[] tan)
    {
        pendingTex.Add(texDraw);
        pendingUV2.Add(uv2);
        pendingTan.Add(tan);
    }

    void LateUpdate()
    {
        if (pendingTex.size > 0) {
            //Also refill since there's a probability
            //that UV Stack willn't match their widget
            UpdateNGUIStacks(true);
            for (int i = 0; i < pendingTex.size; i++) {
                var texDraw = pendingTex[i];
                var uv2 = pendingUV2[i];
                var tan = pendingTan[i];
                var dc = texDraw.drawCall;
                int idx = -1;
                if (dc == null) {
                    //We don't know which one, so warn every draw call
                    for (int j = 0; j < texDrawCalls.size; j++) {
                        if (texDrawCalls[j].ReportEmptyWidget(texDraw))
                            break;     
                    }
                    continue;
                }
                for (int j = 0; j < drawCalls.size; j++) {
                    if (drawCalls[j] == dc) {
                        idx = j;
                        break;
                    }
                }
                if (idx == -1) {   
                    Debug.LogWarning("IM HIT");
                    return;
                }
                texDrawCalls[idx].Upload(texDraw, uv2, tan);     
            }
            pendingTex.Clear();
            pendingUV2.Clear();
            pendingTan.Clear();
        }
    }

    void OnEnable()
    {
        if (panelMaterial == null)
            panelMaterial = TexDrawLib.TEXPreference.main.defaultMaterial;
        drawCalls = new BetterList<UIDrawCall>();
        texDrawCalls = new BetterList<TEXDrawNGUIDrawCall>();  
        pendingTex = new BetterList<TEXDrawNGUI>();
        pendingUV2 = new BetterList<Vector2[]>(); 
        pendingTan = new BetterList<Vector4[]>(); 
        UpdateNGUIStacks();
    }

    public void ForceDirty()
    {
        if (drawCalls == null)
            return;
        UpdateNGUIStacks(true);
    }

    public void RelocateDrawCall(TEXDrawNGUIDrawCall dn)
    {
        if (drawCalls.size > 0) {
            var dc = GetComponent<UIPanel>().drawCalls;
            for (int i = 0; i < dc.Count; i++) {
                if (dc[i].renderQueue == dn.drawCallQueue && dc[i].baseMaterial == dn.material) {
                    dn.drawCall = dc[i];
                    return;
                }
            }
        }
        UpdateNGUIStacks(true);
    }

    [ContextMenu("Cleanup Unused Draw Calls")]
    public void Cleanup()
    {
	    var dn = texDrawCalls;
        bool allOfThemAreDisabled = true;
	    for (int i = 0; i < dn.size; i++) {
            if (!dn[i].enabled) {
                if (Application.isPlaying)
                    Destroy(dn[i]);
                else
                    DestroyImmediate(dn[i]);
            } else
                allOfThemAreDisabled = false;
        }
        if (allOfThemAreDisabled) {
            if (Application.isPlaying)
                Destroy(this);
            else
                DestroyImmediate(this);
        }      
    }
	
	public bool CanCleanup()
	{
		var dn = texDrawCalls;
		if(dn.size == 0)
			return true;
		for (int i = 0; i < dn.size; i++) {
			if(!dn[i].enabled)
				return true;
		}
		return false;
	}
}

