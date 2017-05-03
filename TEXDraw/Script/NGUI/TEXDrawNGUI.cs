
using UnityEngine;
using System.Collections.Generic;
using System;
using TexDrawLib;
using UnityEngine.UI;

[ExecuteInEditMode]
[AddComponentMenu("TEXDraw/TEXDraw NGUI", 3)]
public class TEXDrawNGUI : UIWidget, ITEXDraw
{
    public TEXPreference pref;
    
    public TEXPreference preference { get{ return pref; }}
    
    [SerializeField]    string          m_Text = "TEXDraw";
    [NonSerialized]     bool            m_TextDirty = true;
    [SerializeField]    int             m_FontIndex = -1;
    [SerializeField]    Fitting         m_AutoFit = Fitting.DownScale;
    [SerializeField]    Wrapping        m_AutoWrap = 0;
    [SerializeField]    Filling         m_AutoFill = 0;
  [Range(1, 200), SerializeField] float m_Size = 40f;
    [SerializeField]    Material        m_Material;
    [SerializeField, Range(0, 2)] float m_SpaceSize = 0.2f;
    [SerializeField]    Vector2         m_Align = new Vector2(0.5f, 0.5f);



    public virtual string text
    {
        get { return m_Text; }
        set {
            if (String.IsNullOrEmpty(value)) {
                if (String.IsNullOrEmpty(m_Text))
                    return;
                m_Text = "";
                m_TextDirty = true;
                MarkAsChanged();
            } else if (m_Text != value) {
                m_Text = value;
                m_TextDirty = true;
                MarkAsChanged();
            }
        }
    }

    public virtual int fontIndex
    {
        get { return m_FontIndex; }
        set {
            if (m_FontIndex != value) {
                m_FontIndex = Mathf.Clamp(value, -1, 31);
                MarkAsChanged();
            }
        }
    }

    public virtual Fitting autoFit
    {
        get { return m_AutoFit; }
        set {
            if (m_AutoFit != value) {
                m_AutoFit = value;
                MarkAsChanged();
            }
        }
    }

    public virtual Wrapping autoWrap
    {
        get { return m_AutoWrap; }
        set {
            if (m_AutoWrap != value) {
                m_AutoWrap = value;
                MarkAsChanged();
            }
        }
    }

    public virtual Filling autoFill
    {
        get { return m_AutoFill; }
        set {
            if (m_AutoFill != value) {
                m_AutoFill = value;
                MarkAsChanged();
            }
        }
    }

    public virtual float size
    {
        get { return m_Size; }
        set {
            if (m_Size != value) {
                m_Size = Mathf.Max(value, 1);
                MarkAsChanged();
            }
        } 
    }

    public override Material material
    {
        get { return Repaint(); }
        set {
            if (m_Material != value) {
                if (value == null || value == pref.defaultMaterial)
                    m_Material = null;
                else
                    m_Material = value;
                MarkAsChanged();
            }
        }
    }

    public virtual float spaceSize
    {
        get { return m_SpaceSize; }
        set {
            if (m_SpaceSize != value) {
                m_SpaceSize = value;
                Redraw();
            }
        }
    }

    public virtual Vector2 alignment
    {
        get { return m_Align; }
        set {
            if (m_Align != value) {
                m_Align = value;
                Redraw();
            }
        }
    }

    public string debugReport = String.Empty;

    #if UNITY_EDITOR
    void Reset()
    {
        pref = TEXPreference.main;
    }

    [ContextMenu("Repick Preference Asset")]
    public void PickPreferenceAsset()
    {
        pref = TEXPreference.main;
    }

    [ContextMenu("Open Preference")]
    public void OpenPreference()
    {
        UnityEditor.Selection.activeObject = pref;   
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!pref) {
            pref = TEXPreference.main;
            if (!pref)
                Debug.LogWarning("A TEXDraw NGUI Component hasn't the preference yet");
        }
    
        Font.textureRebuilt += OnFontRebuild;
        if (!mesh) {
            mesh = new Mesh();
            mesh.hideFlags = HideFlags.DontSave;
            mesh.name = "TEXDraw NGUI";
        }
        if (manager)
            manager.ForceDirty();
    }
    #else
	protected override void OnEnable()
	{
    	base.OnEnable();
    	if(!TEXPreference.main)
        	TEXPreference.main = pref; //Assign the Preference to main stack
    	else if(!pref)
        	pref = TEXPreference.main; //This component may added runtimely
       	Font.textureRebuilt += OnFontRebuild;
	    if(!mesh)
	    {
	        mesh=new Mesh();
	    }
        manager.ForceDirty();
	}
	#endif

    #region Engine

    DrawingContext m_cachedDrawing;

    public DrawingContext drawingContext
    {
        get {
            if (m_cachedDrawing == null)
                m_cachedDrawing = new DrawingContext(this);
            return m_cachedDrawing;
        }
    }

    Mesh mesh;

    public void Redraw()
    {
        if (isActiveAndEnabled) {
            if (pref == null)
                OnEnable();
            #if UNITY_EDITOR
            if (pref.editorReloading)
                return;
            #endif
            GenerateParam();
            drawingContext.Render(mesh, cacheParam);
            PerformPostEffects(mesh);
        }
    }

    void CheckTextDirty()
    {
        if (m_TextDirty) {
            m_TextDirty = false;
            drawingContext.Parse(PerformSupplements(m_Text), out debugReport);   
         }
        cacheParam.formulas = DrawingContext.ToRenderers(drawingContext.parsed, cacheParam);
    }

    public void SetTextDirty()
    {
        m_TextDirty = true;
    }

    public void SetTextDirty(bool forceRedraw)
    {
        m_TextDirty = true;
        if (forceRedraw) {
            Invalidate(false);
        }
    }

    public Material Repaint()
    {
        if (!m_Material)
            return pref.defaultMaterial;
        else
            return m_Material;
    }

#if LEGACY_NGUI
    public override void OnFill(BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
    {
#else
    public override void OnFill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
    {
        #endif
		
        Redraw();
        verts.AddRange(mesh.vertices);
        uvs.AddRange(mesh.uv);
#if LEGACY_NGUI
        cols.AddRange(mesh.colors32);
#else
        cols.AddRange(mesh.colors);
#endif

        //UV2? let the manager fills itself.
        CheckManager();
        manager.UploadToDrawCalls(this, mesh.uv2, mesh.tangents);
    }

    public void FillUV2()
    {
        CheckManager();
        manager.UploadToDrawCalls(this, mesh.uv2, mesh.tangents);
    }

    public int managerQ;
    TEXDrawNGUIPanelManager cachedManager;

    TEXDrawNGUIPanelManager manager
    {
        get {
            CheckManager();
            return cachedManager;
        }
        set {
            if (value != null)
                cachedManager = value;
        }
    }

    void CheckManager()
    {
        if (!cachedManager) {
	        var panel = GetComponentInParent<UIPanel>();
	        if (!panel)
			//   Debug.LogError("You didn't put this TEXDraw NGUI under a UIPanel, do you?", this);
		        return;
            cachedManager = panel.GetComponent<TEXDrawNGUIPanelManager>();
            if (!cachedManager)
                cachedManager = panel.gameObject.AddComponent<TEXDrawNGUIPanelManager>();
        }
    }


    public Vector2 RectSize
    {
        get {
            if (cacheParam == null)
                return Vector2.zero;
            switch (autoFit) {
                case Fitting.RectSize:
                    return cacheParam.layoutSize;
                case Fitting.HeightOnly:
                    return new Vector2(mWidth, cacheParam.layoutSize.y);
                default:
                    return new Vector2(mWidth, mHeight);
            }
        }
    }

    void OnTransformParentChanged()
    {
        cachedManager = null;
        mParentFound = false;
    }

    DrawingParams cacheParam;
    
    public DrawingParams drawingParams { get{return cacheParam;} }

    /// Where all layout and rendering setup happens
    DrawingParams GenerateParam()
    {
        if (cacheParam == null)
            cacheParam = new DrawingParams();

        Vector2 pv = pivotOffset;
        Vector2 rc = RectSize;
        cacheParam.alignment = pv;
        cacheParam.color = color;
        cacheParam.color.a *= finalAlpha;
        cacheParam.scale = m_Size;
        cacheParam.fontSize = (int)m_Size;
        cacheParam.fontIndex = m_FontIndex;
        cacheParam.spaceSize = m_SpaceSize;
        cacheParam.hasRect = true;
        cacheParam.autoFit = m_AutoFit;
        cacheParam.autoWrap = m_AutoFit == Fitting.RectSize ? Wrapping.NoWrap : m_AutoWrap;
        cacheParam.autoFill = m_AutoFill;
        if (m_AutoFit == Fitting.RectSize) {
            rc.x = 0;
            rc.y = 0;
        } else if (m_AutoFit == Fitting.HeightOnly)
            rc.y = 0;
        cacheParam.rectArea = new Rect(Vector2.zero, rc);
        //Every Params filled, now calculate everything, internally
        CheckTextDirty();
        rc = RectSize;
        cacheParam.rectArea.position = -Vector2.Scale(rc, pv);
        if (m_AutoFit == Fitting.RectSize) {
            mWidth = (int)rc.x;
            mHeight = (int)rc.y;
        } else if (m_AutoFit == Fitting.HeightOnly)
            mHeight = (int)rc.y;
        return cacheParam;
    }

    void OnDestroy()
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying) {
            DestroyImmediate(mesh);
        } else {
            #endif
            Destroy(mesh);
            #if UNITY_EDITOR
        }
        #endif
    }

    #if LEGACY_NGUI
    public override bool canResize
    {
        get
        {
            return m_AutoFit != Fitting.RectSize;
        }
    }
    #endif

    protected override void OnDisable()
    {
        base.OnDisable();
        Font.textureRebuilt -= OnFontRebuild;
        manager.ForceDirty();
    }

    void OnFontRebuild(Font f)
    {
        MarkAsChanged();
    }

    #endregion

    #region Update Behaviour

    int lastDepth;

    protected override void OnUpdate()
    {
        base.OnUpdate();
        //Guessing this: OnFill willn't triggered if depth changed
        //While it's not important, the panel manager need to be alerted
        //Since last mesh UV2 upload is no longer valid
        if (lastDepth != depth) {
            lastDepth = depth;
            manager.ForceDirty();
        }
   
    }

    void LateUpdate()
    {
        //THIS IS JUST ESTIMATION: just like above, checking whether our UV2 still valid
        if (drawCall && (drawCall.renderQueue != managerQ || drawCall.isDirty)) {
            lastDepth = depth;
            manager.ForceDirty();
            managerQ = drawCall.renderQueue;
        }
    
    }

    #endregion

    #region Supplements

    List<BaseMeshEffect> postEffects = new List<BaseMeshEffect>();
    List<TEXDrawSupplementBase> supplements = new List<TEXDrawSupplementBase>();

    public void SetSupplementDirty()
    {
        UpdateSupplements();
        SetTextDirty(true);
    }

    void UpdateSupplements()
    {
        GetComponents<TEXDrawSupplementBase>(supplements);
        GetComponents<BaseMeshEffect>(postEffects);
    }

    string PerformSupplements(string original)
    {
        if (supplements == null)
            return original;
        TEXDrawSupplementBase s;
        for (int i = 0; i < supplements.Count; i++) 
            if ((s = supplements[i]) && s.enabled)
                original = s.ReplaceString(original);
        
        return original;
    }
    
    void PerformPostEffects(Mesh m)
    {
        if (postEffects == null)
            return;
        BaseMeshEffect p;
        for (int i = 0; i < postEffects.Count; i++) 
            if ((p = postEffects[i]) && p.enabled)
                p.ModifyMesh(m);
        
    }

    #endregion

}

