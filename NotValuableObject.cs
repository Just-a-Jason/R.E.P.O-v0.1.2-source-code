using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200018D RID: 397
public class NotValuableObject : MonoBehaviour
{
	// Token: 0x06000CFD RID: 3325 RVA: 0x00071C78 File Offset: 0x0006FE78
	private void Start()
	{
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.navMeshObstacle = base.GetComponent<NavMeshObstacle>();
		if (this.navMeshObstacle)
		{
			Debug.LogError(base.gameObject.name + " has a NavMeshObstacle component. Please remove it.", base.gameObject);
		}
		base.StartCoroutine(this.EnableRigidbody());
		this.rb = base.GetComponent<Rigidbody>();
		if (this.rb)
		{
			this.rb.mass = this.physAttributePreset.mass;
		}
		if (this.physGrabObject)
		{
			this.physGrabObject.massOriginal = this.physAttributePreset.mass;
		}
		if (this.hasHealth)
		{
			this.healthCurrent = this.healthMax;
		}
	}

	// Token: 0x06000CFE RID: 3326 RVA: 0x00071D40 File Offset: 0x0006FF40
	public void Impact(PhysGrabObjectImpactDetector.ImpactState _impactState)
	{
		switch (_impactState)
		{
		case PhysGrabObjectImpactDetector.ImpactState.Light:
			this.healthCurrent -= this.healthLossOnBreakLight;
			break;
		case PhysGrabObjectImpactDetector.ImpactState.Medium:
			this.healthCurrent -= this.healthLossOnBreakMedium;
			break;
		case PhysGrabObjectImpactDetector.ImpactState.Heavy:
			this.healthCurrent -= this.healthLossOnBreakHeavy;
			break;
		}
		if (this.healthCurrent <= 0)
		{
			this.physGrabObject.impactDetector.DestroyObject(true);
		}
	}

	// Token: 0x06000CFF RID: 3327 RVA: 0x00071DBA File Offset: 0x0006FFBA
	private IEnumerator EnableRigidbody()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		PhysGrabObject component = base.GetComponent<PhysGrabObject>();
		if (!component)
		{
			yield return new WaitForSeconds(0.5f);
			yield return new WaitForFixedUpdate();
		}
		else
		{
			component.spawned = true;
		}
		yield break;
	}

	// Token: 0x040014DA RID: 5338
	public PhysAttribute physAttributePreset;

	// Token: 0x040014DB RID: 5339
	public PhysAudio audioPreset;

	// Token: 0x040014DC RID: 5340
	public Durability durabilityPreset;

	// Token: 0x040014DD RID: 5341
	public Gradient particleColors;

	// Token: 0x040014DE RID: 5342
	[Range(0.5f, 3f)]
	public float audioPresetPitch = 1f;

	// Token: 0x040014DF RID: 5343
	private NavMeshObstacle navMeshObstacle;

	// Token: 0x040014E0 RID: 5344
	private PhysGrabObject physGrabObject;

	// Token: 0x040014E1 RID: 5345
	private Rigidbody rb;

	// Token: 0x040014E2 RID: 5346
	[Space]
	public bool hasHealth;

	// Token: 0x040014E3 RID: 5347
	private int healthCurrent;

	// Token: 0x040014E4 RID: 5348
	public int healthMax;

	// Token: 0x040014E5 RID: 5349
	public int healthLossOnBreakLight;

	// Token: 0x040014E6 RID: 5350
	public int healthLossOnBreakMedium;

	// Token: 0x040014E7 RID: 5351
	public int healthLossOnBreakHeavy;
}
