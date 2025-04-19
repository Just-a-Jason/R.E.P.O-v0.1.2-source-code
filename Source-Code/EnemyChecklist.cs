using System;
using UnityEngine;

// Token: 0x0200008A RID: 138
public class EnemyChecklist : MonoBehaviour
{
	// Token: 0x06000588 RID: 1416 RVA: 0x00036804 File Offset: 0x00034A04
	private void ResetChecklist()
	{
		this.difficulty = false;
		this.type = false;
		this.center = false;
		this.killLookAt = false;
		this.sightingStinger = false;
		this.enemyNearMusic = false;
		this.healthMax = false;
		this.healthMeshParent = false;
		this.healthOnHurt = false;
		this.healthOnDeath = false;
		this.healthImpact = false;
		this.healthObject = false;
		this.rigidbodyPhysAttribute = false;
		this.rigidbodyAudioPreset = false;
		this.rigidbodyColliders = false;
		this.rigidbodyFollow = false;
		this.rigidbodyCustomGravity = false;
		this.rigidbodyGrab = false;
		this.rigidbodyPositionFollow = false;
		this.rigidbodyRotationFollow = false;
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x000368A0 File Offset: 0x00034AA0
	private void SetAllChecklist()
	{
		this.difficulty = true;
		this.type = true;
		this.center = true;
		this.killLookAt = true;
		this.sightingStinger = true;
		this.enemyNearMusic = true;
		this.healthMax = true;
		this.healthMeshParent = true;
		this.healthOnHurt = true;
		this.healthOnDeath = true;
		this.healthImpact = true;
		this.healthObject = true;
		this.rigidbodyPhysAttribute = true;
		this.rigidbodyAudioPreset = true;
		this.rigidbodyColliders = true;
		this.rigidbodyFollow = true;
		this.rigidbodyCustomGravity = true;
		this.rigidbodyGrab = true;
		this.rigidbodyPositionFollow = true;
		this.rigidbodyRotationFollow = true;
	}

	// Token: 0x040008CE RID: 2254
	private Color colorPositive = Color.green;

	// Token: 0x040008CF RID: 2255
	private Color colorNegative = new Color(1f, 0.74f, 0.61f);

	// Token: 0x040008D0 RID: 2256
	[Space]
	public bool hasRigidbody;

	// Token: 0x040008D1 RID: 2257
	public new bool name;

	// Token: 0x040008D2 RID: 2258
	public bool difficulty;

	// Token: 0x040008D3 RID: 2259
	public bool type;

	// Token: 0x040008D4 RID: 2260
	public bool center;

	// Token: 0x040008D5 RID: 2261
	public bool killLookAt;

	// Token: 0x040008D6 RID: 2262
	public bool sightingStinger;

	// Token: 0x040008D7 RID: 2263
	public bool enemyNearMusic;

	// Token: 0x040008D8 RID: 2264
	public bool healthMax;

	// Token: 0x040008D9 RID: 2265
	public bool healthMeshParent;

	// Token: 0x040008DA RID: 2266
	public bool healthOnHurt;

	// Token: 0x040008DB RID: 2267
	public bool healthOnDeath;

	// Token: 0x040008DC RID: 2268
	public bool healthImpact;

	// Token: 0x040008DD RID: 2269
	public bool healthObject;

	// Token: 0x040008DE RID: 2270
	public bool rigidbodyPhysAttribute;

	// Token: 0x040008DF RID: 2271
	public bool rigidbodyAudioPreset;

	// Token: 0x040008E0 RID: 2272
	public bool rigidbodyColliders;

	// Token: 0x040008E1 RID: 2273
	public bool rigidbodyFollow;

	// Token: 0x040008E2 RID: 2274
	public bool rigidbodyCustomGravity;

	// Token: 0x040008E3 RID: 2275
	public bool rigidbodyGrab;

	// Token: 0x040008E4 RID: 2276
	public bool rigidbodyPositionFollow;

	// Token: 0x040008E5 RID: 2277
	public bool rigidbodyRotationFollow;
}
