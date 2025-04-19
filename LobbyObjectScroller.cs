using System;
using UnityEngine;

// Token: 0x02000121 RID: 289
public class LobbyObjectScroller : MonoBehaviour
{
	// Token: 0x0600096A RID: 2410 RVA: 0x00057836 File Offset: 0x00055A36
	private void Start()
	{
		this.truck = base.GetComponentInParent<TruckLandscapeScroller>();
		if (this.truck != null)
		{
			this.scrollSpeed *= this.truck.truckSpeed;
		}
	}

	// Token: 0x0600096B RID: 2411 RVA: 0x0005786C File Offset: 0x00055A6C
	private void Update()
	{
		base.transform.position += Vector3.right * this.scrollSpeed * Time.deltaTime;
		if (base.transform.position.x > this.maxDistanceX + this.offsetX)
		{
			base.transform.position = new Vector3(-this.maxDistanceX + this.offsetX, base.transform.position.y, base.transform.position.z);
		}
	}

	// Token: 0x040010BB RID: 4283
	public float scrollSpeed = 12f;

	// Token: 0x040010BC RID: 4284
	public float maxDistanceX = 80f;

	// Token: 0x040010BD RID: 4285
	private float offsetX = -22f;

	// Token: 0x040010BE RID: 4286
	private TruckLandscapeScroller truck;
}
