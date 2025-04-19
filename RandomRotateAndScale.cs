using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000BA RID: 186
public class RandomRotateAndScale : MonoBehaviour
{
	// Token: 0x060006E7 RID: 1767 RVA: 0x000413F8 File Offset: 0x0003F5F8
	private void Start()
	{
		this.RotateObjectAndChildren();
		base.StartCoroutine(this.ScaleAnimation(this.spawnScaleCurve, this.spawnAnimationLength, delegate
		{
			base.StartCoroutine(this.WaitAndDespawn(this.durationBeforeDespawn));
		}));
		base.transform.position += Vector3.up * 0.02f;
		float maxDistance = 0.1f;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(base.transform.position, -Vector3.up), out raycastHit, maxDistance))
		{
			base.transform.position = raycastHit.point + Vector3.up * 0.0001f;
		}
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x000414A8 File Offset: 0x0003F6A8
	private void RotateObjectAndChildren()
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		base.transform.localRotation = Quaternion.Euler(localEulerAngles.x + 90f, localEulerAngles.y, (float)Random.Range(0, 360));
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			Vector3 localEulerAngles2 = transform.localEulerAngles;
			transform.localRotation = Quaternion.Euler(localEulerAngles2.x, localEulerAngles2.y, (float)Random.Range(0, 360));
		}
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x0004155C File Offset: 0x0003F75C
	private IEnumerator ScaleAnimation(AnimationCurve curve, float animationLength, Action onComplete)
	{
		float elapsedTime = 0f;
		while (elapsedTime < animationLength)
		{
			elapsedTime += Time.deltaTime;
			float time = elapsedTime / animationLength;
			float num = curve.Evaluate(time) * this.scaleMultiplier;
			base.transform.localScale = new Vector3(num, num, num);
			yield return null;
		}
		if (onComplete != null)
		{
			onComplete();
		}
		yield break;
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x00041580 File Offset: 0x0003F780
	private IEnumerator WaitAndDespawn(float duration)
	{
		yield return new WaitForSeconds(duration);
		base.StartCoroutine(this.ScaleAnimation(this.despawnScaleCurve, this.despawnAnimationLength, delegate
		{
			Object.Destroy(base.gameObject);
		}));
		yield break;
	}

	// Token: 0x04000BA4 RID: 2980
	[Space]
	[Header("Spawn")]
	public AnimationCurve spawnScaleCurve;

	// Token: 0x04000BA5 RID: 2981
	public float spawnAnimationLength = 1f;

	// Token: 0x04000BA6 RID: 2982
	[Space]
	[Header("Time before despawn")]
	public float durationBeforeDespawn = 5f;

	// Token: 0x04000BA7 RID: 2983
	[Space]
	[Header("Despawn")]
	public AnimationCurve despawnScaleCurve;

	// Token: 0x04000BA8 RID: 2984
	public float despawnAnimationLength = 1f;

	// Token: 0x04000BA9 RID: 2985
	[Space]
	[Header("Scale")]
	public float scaleMultiplier = 1f;
}
