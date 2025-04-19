using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B2 RID: 178
[RequireComponent(typeof(Renderer))]
public class CanvasAssigner : MonoBehaviour
{
	// Token: 0x060006CA RID: 1738 RVA: 0x00040C4E File Offset: 0x0003EE4E
	private void Awake()
	{
		this.AssignRandomTexture();
	}

	// Token: 0x060006CB RID: 1739 RVA: 0x00040C58 File Offset: 0x0003EE58
	private void AssignRandomTexture()
	{
		if (CanvasAssigner.AvailableTextures.Count == 0)
		{
			CanvasList.PopulateAvailableTextures();
		}
		int index = Random.Range(0, CanvasAssigner.AvailableTextures.Count);
		Texture mainTexture = CanvasAssigner.AvailableTextures[index];
		CanvasAssigner.AvailableTextures.RemoveAt(index);
		Renderer component = base.GetComponent<Renderer>();
		if (component && component.material)
		{
			component.material.mainTexture = mainTexture;
		}
	}

	// Token: 0x04000B77 RID: 2935
	public static List<Texture> AvailableTextures = new List<Texture>();
}
