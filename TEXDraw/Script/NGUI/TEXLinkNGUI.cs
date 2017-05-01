using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TEXDrawNGUI))]
[AddComponentMenu("TEXDraw/TEXLink NGUI", 6)]
public class TEXLinkNGUI : TEXLinkBase
{
    protected override int SamplePointerStatus(int linkIdx)
    {
        float o;
        if (linkIdx >= m_DrawingContext.linkBoxRect.Count)
            return 0;
        var plane = new Plane(-target.transform.forward, target.transform.position);
        for (int i = 0; i < input_PressPos.Count; i++) {
            var screenPos = input_PressPos[i];

            var ray = triggerCamera.ScreenPointToRay(screenPos);
            if (plane.Raycast(ray, out o)) {
                if (m_DrawingContext.linkBoxRect[linkIdx].Contains(target.transform.InverseTransformPoint(ray.GetPoint(o))))
                    return 2;
            }
        }

        if (!Input.mousePresent)
            return 0;
        var r = triggerCamera.ScreenPointToRay(input_HoverPos);
        if (plane.Raycast(r, out o)) {
            var pos = target.transform.InverseTransformPoint(r.GetPoint(o));
            if (m_DrawingContext.linkBoxRect[linkIdx].Contains(pos))
                return 1;
           // Debug.Log(pos.ToString("0.0") + "     " + m_DrawingContext.linkBoxRect[linkIdx].ToString("0.0"));
        }

        return 0;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        target = GetComponent<TEXDrawNGUI>();
        var tex = (TEXDrawNGUI)target;
        triggerCamera = tex.anchorCamera;
    }

}

