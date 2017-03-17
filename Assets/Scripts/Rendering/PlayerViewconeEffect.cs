using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
[AddComponentMenu ("Image Effects/Other/PlayerViewCone")]
public class PlayerViewconeEffect : PostEffectsBase
{

	[SerializeField]
    [Range(0.0f, 2.0f)]
	private float m_visibleSaturation = 1.0f;
	[SerializeField]
	[Range(0.0f, 2.0f)]
	private float m_visibleLuminance = 1.0f;

	[SerializeField]
	[Range(0.0f, 2.0f)]
	private float m_invisibleSaturation = 0.0f;
	[SerializeField]
	[Range(0.0f, 2.0f)]
	private float m_invisibleLuminance = 0.4f;

	[SerializeField]
	private Color m_invisibleTint = new Color (1, 1, 1, 1);

	[SerializeField]
	private Color m_fullscreenTint = new Color (1, 1, 1, 0);

	[SerializeField]
	private Shader m_shader = null;

	[SerializeField]
	private GameObject m_viewConeCamera;

    private Material m_material = null;


	public Color FullscreenTint {
		get {return m_fullscreenTint;}
		set { m_fullscreenTint = value; }
	}

    public override bool CheckResources ()
	{
        CheckSupport (false);

		m_material = CheckShaderAndCreateMaterial (m_shader, m_material);

        if (!isSupported)
            ReportAutoDisable ();
        return isSupported;
    }

    void OnDisable ()
	{
		if (m_material)
			DestroyImmediate (m_material);
    }

    void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (CheckResources() == false || m_viewConeCamera==null || m_viewConeCamera.GetComponent<Camera>().targetTexture==null)
		{
            Graphics.Blit (source, destination);
            return;
        }

		m_material.SetTexture ("_VisibilityTexture", m_viewConeCamera.GetComponent<Camera> ().targetTexture);

		m_material.SetVector ("_Parameter", new Vector4(m_invisibleSaturation, m_visibleSaturation, m_invisibleLuminance, m_visibleLuminance));
		m_material.SetVector ("_InvisibleTint", m_invisibleTint);
		m_material.SetVector ("_FullscreenTint", m_fullscreenTint);
		Graphics.Blit (source, destination, m_material, 0);
    }
}
