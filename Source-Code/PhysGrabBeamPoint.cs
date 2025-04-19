using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000194 RID: 404
public class PhysGrabBeamPoint : MonoBehaviour
{
	// Token: 0x06000D79 RID: 3449 RVA: 0x000789D5 File Offset: 0x00076BD5
	private void Start()
	{
		this.originalScale = base.transform.localScale;
		this.originalMaterial = base.GetComponent<Renderer>().material;
	}

	// Token: 0x06000D7A RID: 3450 RVA: 0x000789FC File Offset: 0x00076BFC
	private void OnEnable()
	{
		if (VideoGreenScreen.instance)
		{
			base.GetComponent<Renderer>().material = this.greenScreenMaterial;
			using (IEnumerator enumerator = base.transform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					((Transform)obj).GetComponent<Renderer>().material = this.greenScreenMaterial;
				}
				return;
			}
		}
		base.GetComponent<Renderer>().material = this.originalMaterial;
		foreach (object obj2 in base.transform)
		{
			((Transform)obj2).GetComponent<Renderer>().material = this.originalMaterial;
		}
	}

	// Token: 0x06000D7B RID: 3451 RVA: 0x00078ADC File Offset: 0x00076CDC
	private void Update()
	{
		float num = Time.time * this.tileSpeedX;
		float num2 = Time.time * this.tileSpeedY;
		base.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(num, num2);
		float num3 = Mathf.Sin(Time.time * this.textureJitterSpeed) * 0.1f;
		base.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1f + num3, 1f + num3);
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			transform.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(-num, -num2);
			transform.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1f - num3, 1f - num3);
		}
		float num4 = Mathf.Sin(Time.time * this.sphereJitterSpeed) * (this.originalScale.x * 0.3f);
		base.transform.localScale = (this.originalScale + new Vector3(num4, num4, num4)) * 0.5f;
	}

	// Token: 0x0400160F RID: 5647
	public float tileSpeedX = 0.5f;

	// Token: 0x04001610 RID: 5648
	public float tileSpeedY = 0.5f;

	// Token: 0x04001611 RID: 5649
	public float textureJitterSpeed = 10f;

	// Token: 0x04001612 RID: 5650
	public float sphereJitterSpeed = 10f;

	// Token: 0x04001613 RID: 5651
	private Vector3 originalScale;

	// Token: 0x04001614 RID: 5652
	public Material originalMaterial;

	// Token: 0x04001615 RID: 5653
	public Material greenScreenMaterial;
}
