using System;
using UnityEngine;

// Token: 0x02000122 RID: 290
public class TextureScroller : MonoBehaviour
{
	// Token: 0x0600096D RID: 2413 RVA: 0x00057930 File Offset: 0x00055B30
	private void Start()
	{
		this.rend = base.GetComponent<Renderer>();
		this.savedOffset = this.rend.material.mainTextureOffset;
		this.truck = base.GetComponentInParent<TruckLandscapeScroller>();
		if (this.truck != null)
		{
			this.scrollSpeed *= this.truck.truckSpeed;
		}
	}

	// Token: 0x0600096E RID: 2414 RVA: 0x00057994 File Offset: 0x00055B94
	private void Update()
	{
		float x = Mathf.Repeat(Time.time * this.scrollSpeed, 1f);
		Vector2 mainTextureOffset = new Vector2(x, this.savedOffset.y);
		this.rend.material.mainTextureOffset = mainTextureOffset;
	}

	// Token: 0x0600096F RID: 2415 RVA: 0x000579DC File Offset: 0x00055BDC
	private void OnDisable()
	{
		this.rend.material.mainTextureOffset = this.savedOffset;
	}

	// Token: 0x040010BF RID: 4287
	public float scrollSpeed = 0.5f;

	// Token: 0x040010C0 RID: 4288
	private Renderer rend;

	// Token: 0x040010C1 RID: 4289
	private Vector2 savedOffset;

	// Token: 0x040010C2 RID: 4290
	private TruckLandscapeScroller truck;
}
