using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class PixelEffect : MonoBehaviour {
 	[SerializeField][Range(1,100)] private int pixels;
	[SerializeField] private Material material;
 
	void Awake ()
	{
		//material = new Material( Shader.Find("PixelShader") );
		//material.SetVector("_ScreenRes", new Vector2(Screen.currentResolution.width, Screen.currentResolution.height));
	}
	
	// Postprocess the image
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		//print(Screen.currentResolution.width);
		if(pixels == 1)
		{
			Graphics.Blit (source, destination);
			return;
		}
		material.SetVector("_ScreenRes", new Vector2(Screen.currentResolution.width, Screen.currentResolution.height));
		material.SetInt("_Pixels", pixels);
		Graphics.Blit (source, destination, material);
	}
}
