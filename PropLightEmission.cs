using System;
using UnityEngine;

// Token: 0x020000FA RID: 250
public class PropLightEmission : MonoBehaviour
{
	// Token: 0x060008CA RID: 2250 RVA: 0x0005432A File Offset: 0x0005252A
	private void Awake()
	{
		this.meshRenderer = base.GetComponent<Renderer>();
		this.material = this.meshRenderer.material;
		this.originalEmission = this.material.GetColor("_EmissionColor");
	}

	// Token: 0x04000FF3 RID: 4083
	public bool levelLight = true;

	// Token: 0x04000FF4 RID: 4084
	internal bool turnedOff;

	// Token: 0x04000FF5 RID: 4085
	internal Renderer meshRenderer;

	// Token: 0x04000FF6 RID: 4086
	internal Color originalEmission;

	// Token: 0x04000FF7 RID: 4087
	internal Material material;
}
