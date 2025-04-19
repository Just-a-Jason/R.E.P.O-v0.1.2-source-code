using System;
using UnityEngine;

// Token: 0x020001A2 RID: 418
public class PlayerArmCollision : MonoBehaviour
{
	// Token: 0x06000E02 RID: 3586 RVA: 0x0007E6CD File Offset: 0x0007C8CD
	private void OnCollisionStay(Collision other)
	{
		this.Blocked = true;
		this.BlockedTimer = 0.25f;
	}

	// Token: 0x06000E03 RID: 3587 RVA: 0x0007E6E1 File Offset: 0x0007C8E1
	private void Update()
	{
		if (this.BlockedTimer <= 0f)
		{
			this.Blocked = false;
			return;
		}
		this.BlockedTimer -= Time.deltaTime;
	}

	// Token: 0x040016EC RID: 5868
	public bool Blocked;

	// Token: 0x040016ED RID: 5869
	public float BlockDistance;

	// Token: 0x040016EE RID: 5870
	private float BlockedTimer;
}
