using System;
using UnityEngine;

// Token: 0x020001CF RID: 463
public class Fire : MonoBehaviour
{
	// Token: 0x06000F7A RID: 3962 RVA: 0x0008E049 File Offset: 0x0008C249
	private void Update()
	{
		if (this.propLight.turnedOff)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000F7B RID: 3963 RVA: 0x0008E063 File Offset: 0x0008C263
	public void OnHit()
	{
		this.soundHit.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x04001A4B RID: 6731
	public PropLight propLight;

	// Token: 0x04001A4C RID: 6732
	[Space]
	public Sound soundHit;
}
