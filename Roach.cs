using System;
using UnityEngine;

// Token: 0x020000BB RID: 187
[RequireComponent(typeof(Transform))]
public class Roach : MonoBehaviour
{
	// Token: 0x060006EE RID: 1774 RVA: 0x000415EC File Offset: 0x0003F7EC
	private void Start()
	{
		this.origin = base.transform.position;
		this.roachSpeedTarget = Random.Range(this.minRoachSpeed, this.maxRoachSpeed);
		this.targetPosition = this.GetOrbitPoint(this.angle);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
		base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		base.transform.rotation = Quaternion.identity;
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x00041684 File Offset: 0x0003F884
	private void Update()
	{
		this.currentOrbitDistance = Mathf.Lerp(this.minOrbitDistance, this.maxOrbitDistance, (Mathf.Sin(Time.time * this.orbitWiggleFrequency) + 1f) * 0.5f);
		this.currentOrbitSpeed = Mathf.Lerp(this.minOrbitSpeed, this.maxOrbitSpeed, (Mathf.Sin(Time.time * this.speedWiggleFrequency) + 1f) * 0.5f);
		this.angle += Time.deltaTime * this.currentOrbitSpeed;
		Vector3 orbitPoint = this.GetOrbitPoint(this.angle);
		if (Vector3.Distance(base.transform.position, this.targetPosition) < 0.1f)
		{
			Vector3 normalized = (orbitPoint - this.targetPosition).normalized;
			this.targetPosition = orbitPoint + normalized * this.overshootMultiplier;
		}
		this.currentRoachSpeed = Mathf.Lerp(this.currentRoachSpeed, this.roachSpeedTarget, Time.deltaTime * this.roachSpeedFluctuationFrequency);
		if (Mathf.Abs(this.currentRoachSpeed - this.roachSpeedTarget) < 0.1f)
		{
			this.roachSpeedTarget = Random.Range(this.minRoachSpeed, this.maxRoachSpeed);
		}
		Vector3 a = ((this.targetPosition - base.transform.position).normalized * this.currentRoachSpeed - this.velocity) * this.turnMultiplier;
		this.velocity += a * Time.deltaTime;
		base.transform.position += this.velocity * Time.deltaTime;
		if (this.velocity != Vector3.zero)
		{
			base.transform.rotation = Quaternion.LookRotation(this.velocity, Vector3.up);
		}
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x0004186B File Offset: 0x0003FA6B
	private Vector3 GetOrbitPoint(float angle)
	{
		return this.origin + new Vector3(Mathf.Sin(angle) * this.currentOrbitDistance, 0f, Mathf.Cos(angle) * this.currentOrbitDistance);
	}

	// Token: 0x04000BAA RID: 2986
	[Space]
	[Header("Orbit Parameters")]
	public float minOrbitDistance = 1f;

	// Token: 0x04000BAB RID: 2987
	public float maxOrbitDistance = 5f;

	// Token: 0x04000BAC RID: 2988
	public float minOrbitSpeed = 1f;

	// Token: 0x04000BAD RID: 2989
	public float maxOrbitSpeed = 5f;

	// Token: 0x04000BAE RID: 2990
	public float orbitWiggleFrequency = 1f;

	// Token: 0x04000BAF RID: 2991
	public float speedWiggleFrequency = 1f;

	// Token: 0x04000BB0 RID: 2992
	[Space]
	[Header("Roach Parameters")]
	public float minRoachSpeed = 1f;

	// Token: 0x04000BB1 RID: 2993
	public float maxRoachSpeed = 3f;

	// Token: 0x04000BB2 RID: 2994
	public float roachSpeedFluctuationFrequency = 0.5f;

	// Token: 0x04000BB3 RID: 2995
	public float overshootMultiplier = 1.5f;

	// Token: 0x04000BB4 RID: 2996
	public float turnMultiplier = 0.5f;

	// Token: 0x04000BB5 RID: 2997
	[Space]
	[Header("Roach Smash")]
	public GameObject roachSmashPrefab;

	// Token: 0x04000BB6 RID: 2998
	private Vector3 origin;

	// Token: 0x04000BB7 RID: 2999
	private float currentOrbitDistance;

	// Token: 0x04000BB8 RID: 3000
	private float currentOrbitSpeed;

	// Token: 0x04000BB9 RID: 3001
	private float angle;

	// Token: 0x04000BBA RID: 3002
	private float roachSpeedTarget;

	// Token: 0x04000BBB RID: 3003
	private float currentRoachSpeed;

	// Token: 0x04000BBC RID: 3004
	private Vector3 targetPosition;

	// Token: 0x04000BBD RID: 3005
	private Vector3 velocity;
}
