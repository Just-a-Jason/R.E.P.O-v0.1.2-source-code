using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000077 RID: 119
public class EnemySlowMouthParticlePukeCollision : MonoBehaviour
{
	// Token: 0x0600047F RID: 1151 RVA: 0x0002CEA2 File Offset: 0x0002B0A2
	private void Start()
	{
		this.pukeParticles = base.GetComponent<ParticleSystem>();
		this.parentTransform = base.transform.parent;
		this.startPosition = base.transform.localPosition;
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x0002CED4 File Offset: 0x0002B0D4
	private void Update()
	{
		if (this.pukeParticles.isPlaying)
		{
			if (!this.pukeLight.enabled)
			{
				this.pukeLight.enabled = true;
				this.pukeLight.intensity = 0f;
			}
			this.pukeLight.intensity = Mathf.Lerp(this.pukeLight.intensity, 0.6f, Time.deltaTime * 5f);
			this.pukeLight.intensity += Mathf.Sin(Time.time * 40f) * 0.07f;
		}
		else if (this.pukeLight.enabled)
		{
			this.pukeLight.intensity = Mathf.Lerp(this.pukeLight.intensity, 0f, Time.deltaTime * 1f);
			this.pukeLight.intensity += Mathf.Sin(Time.time * 30f) * 0.01f;
			if (this.pukeLight.intensity < 0.01f)
			{
				this.pukeLight.enabled = false;
			}
		}
		if (this.hurtColliderTimer > 0f)
		{
			this.hurtColliderTimer -= Time.deltaTime;
			if (this.hurtColliderTimer <= 0f)
			{
				this.hurtCollider.SetActive(false);
			}
		}
		if (SemiFunc.FPSImpulse15())
		{
			base.transform.localPosition = this.startPosition;
			float num = Vector3.Distance(this.parentTransform.position, base.transform.position);
			RaycastHit raycastHit;
			if (Physics.Raycast(this.parentTransform.position, num * this.parentTransform.forward, out raycastHit, num, SemiFunc.LayerMaskGetVisionObstruct()))
			{
				Vector3 point = raycastHit.point;
				if (Vector3.Distance(this.parentTransform.position, point) < num)
				{
					Vector3 vector = this.parentTransform.InverseTransformPoint(point);
					base.transform.localPosition = new Vector3(0f, 0f, vector.z);
				}
			}
		}
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x0002D0D4 File Offset: 0x0002B2D4
	private void ActivateHurtCollider(Vector3 _direction, Vector3 _position)
	{
		this.pukeSmokeParticles.transform.position = _position;
		this.pukeSmokeParticles.transform.rotation = Quaternion.LookRotation(_direction);
		this.pukeSmokeParticles.Emit(1);
		this.hurtCollider.SetActive(true);
		this.pukeBubbleParticles.transform.position = _position;
		this.pukeBubbleParticles.transform.rotation = Quaternion.LookRotation(_direction);
		this.pukeBubbleParticles.Emit(2);
		this.pukeSplashParticles.transform.position = _position;
		this.pukeSplashParticles.Emit(3);
		this.hurtCollider.transform.rotation = Quaternion.LookRotation(_direction);
		this.hurtCollider.transform.position = _position;
		this.hurtColliderTimer = 0.2f;
		_direction.y += 180f;
		this.pukeSplashParticles.transform.rotation = Quaternion.LookRotation(_direction);
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x0002D1C8 File Offset: 0x0002B3C8
	private void OnParticleCollision(GameObject other)
	{
		List<ParticleCollisionEvent> list = new List<ParticleCollisionEvent>();
		int collisionEvents = this.pukeParticles.GetCollisionEvents(other, list);
		for (int i = 0; i < collisionEvents; i++)
		{
			ParticleCollisionEvent particleCollisionEvent = list[i];
			Vector3 intersection = particleCollisionEvent.intersection;
			Vector3 velocity = particleCollisionEvent.velocity;
			Vector3 normalized = velocity.normalized;
			if (velocity.magnitude > 3f)
			{
				this.ActivateHurtCollider(normalized, intersection);
			}
		}
	}

	// Token: 0x0400075B RID: 1883
	public Light pukeLight;

	// Token: 0x0400075C RID: 1884
	private ParticleSystem pukeParticles;

	// Token: 0x0400075D RID: 1885
	public GameObject hurtCollider;

	// Token: 0x0400075E RID: 1886
	private float hurtColliderTimer;

	// Token: 0x0400075F RID: 1887
	private Transform parentTransform;

	// Token: 0x04000760 RID: 1888
	private Vector3 startPosition;

	// Token: 0x04000761 RID: 1889
	public ParticleSystem pukeBubbleParticles;

	// Token: 0x04000762 RID: 1890
	public ParticleSystem pukeSplashParticles;

	// Token: 0x04000763 RID: 1891
	public ParticleSystem pukeSmokeParticles;
}
