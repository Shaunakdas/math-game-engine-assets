using UnityEngine;
using System.Collections;

/*
	TEXDrawNGUIDrawCall is a component that running besides TEXDrawNGUIPanelManager.
	it's actual job is nothing but uploads what given in TEXDrawNGUIPanelManager to
	one NGUI's Draw Call. If there's two, then it'll replicate into two separate component.
	If referenced draw call is inactive, then this component will disabled, and vice-versa.
	
	DO NOT ENABLE/DISABLE THIS COMPONENT MANUALLY
 */
 
[RequireComponent(typeof(TEXDrawNGUIPanelManager)), ExecuteInEditMode]
public class TEXDrawNGUIDrawCall : MonoBehaviour
{
	//Referenced TEXDraw Panel
    TEXDrawNGUIPanelManager panel;

    //An unique index, for given panel
	public int index = 0;
	//Referenced NGUI Panel
	public UIDrawCall drawCall;
	
	//Internal stuff
	public Material material;
    public int drawCallQueue;
	bool changed;
	public BetterList<TEXDrawNGUI> Widgets = new BetterList<TEXDrawNGUI>();
    BetterList<Vector2[]> UVStack = new BetterList<Vector2[]>();
    BetterList<Vector2> cachedUV = new BetterList<Vector2>();
    BetterList<Vector4[]> TanStack = new BetterList<Vector4[]>();
    BetterList<Vector4> cachedTan = new BetterList<Vector4>();

    void OnEnable()
    {
        panel = GetComponent<TEXDrawNGUIPanelManager>();
    }

    public bool ReportEmptyWidget(TEXDrawNGUI tex)
    {
        for (int i = 0; i < Widgets.size; i++) {
            if (Widgets[i] == tex) {
                if (!changed) {
                    changed = true;
                }
                UVStack[i] = new Vector2[]{ };
                TanStack[i] = new Vector4[]{ };
                return true;
            }
        }
        return false;
    }

    public void Upload(TEXDrawNGUI tex, Vector2[] uv2, Vector4[] tan)
    {   
        if (!changed) {
            changed = true;
        }
        //find the index
        int index = -1;
        for (int i = 0; i < Widgets.size; i++) {
            if (tex == Widgets[i]) {
                index = i;
                break;
            }
        }
        if (index == -1) {
            Debug.LogWarning("I'M HIT!");
            return; //This should never happen
        }
        UVStack.ForceAllocation(Widgets.size);
        UVStack[index] = uv2;
        TanStack.ForceAllocation(Widgets.size);
        TanStack[index] = tan;
        #if UNITY_EDITOR
        if (!Application.isPlaying)
            LateUpdate();
        #endif
    }

    bool DrawCallValid()
    {
        if (!drawCall)
	        return false;
	    #if UNITY_EDITOR
        if (!drawCall.isActive)
	        return false;
	    #endif
        if (drawCall.renderQueue != drawCallQueue)
            return false;
        if (drawCall.baseMaterial != material)
            return false;
        return true;
    }

    void LateUpdate()
    {
        if (!DrawCallValid()) {
            panel.RelocateDrawCall(this);
            if (!DrawCallValid())
                return;
        }

        //Update material properties too. Assume we didn't change it at runtime to keep it efficient ;)
        if (!Application.isPlaying)
            drawCall.dynamicMaterial.CopyPropertiesFromMaterial(material);

        Mesh m = drawCall.GetComponent<MeshFilter>().sharedMesh;
        if (!m)
            return;
        if (changed || m.uv2.Length != cachedUV.size) {
            changed = false;
            cachedUV.Clear();
            cachedTan.Clear();
            for (int i = 0; i < UVStack.size; i++) {
                if (Widgets[i].drawCall == null)
                    continue;
                cachedUV.AddRange(UVStack[i]);
            }
            for (int i = 0; i < TanStack.size; i++) {
                if (Widgets[i].drawCall == null)
                    continue;
                cachedTan.AddRange(TanStack[i]);
            }
            if (cachedUV.size == 0 && cachedTan.size == 0)
                return;

            int vert = m.vertices.Length;

            if (vert != cachedUV.buffer.Length) {
                cachedUV.ToArray();
                cachedUV.ForceAllocation(vert);
            }

            if (vert != cachedTan.buffer.Length) {
                cachedTan.ToArray();
                cachedTan.ForceAllocation(vert);
            }

            m.uv2 = cachedUV.buffer;
            m.tangents = cachedTan.buffer;
        }
    }
}

