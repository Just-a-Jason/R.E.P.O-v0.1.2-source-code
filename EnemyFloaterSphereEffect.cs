using System;
using UnityEngine;

// Token: 0x0200004B RID: 75
public class EnemyFloaterSphereEffect : MonoBehaviour
{
	// Token: 0x06000259 RID: 601 RVA: 0x00017EA4 File Offset: 0x000160A4
	private void Start()
	{
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.lightSphere = base.GetComponentInChildren<Light>();
		this.floaterAttack = base.GetComponentInParent<FloaterAttackLogic>();
		this.originalScale = base.transform.localScale.x;
		this.myChildNumber = base.transform.GetSiblingIndex();
		this.originalMaterialColor = this.meshRenderer.material.color;
		if (this.lightSphere)
		{
			this.originalLightColor = this.lightSphere.color;
			this.originalLightIntensity = this.lightSphere.intensity;
			this.originalLightRange = this.lightSphere.range;
		}
	}

	// Token: 0x0600025A RID: 602 RVA: 0x00017F54 File Offset: 0x00016154
	private void StateMachine()
	{
		switch (this.state)
		{
		case EnemyFloaterSphereEffect.FloaterSphereEffectState.levitate:
			this.StateLevitate();
			return;
		case EnemyFloaterSphereEffect.FloaterSphereEffectState.stop:
			this.StateStop();
			return;
		case EnemyFloaterSphereEffect.FloaterSphereEffectState.smash:
			this.StateSmash();
			return;
		default:
			return;
		}
	}

	// Token: 0x0600025B RID: 603 RVA: 0x00017F90 File Offset: 0x00016190
	private void StateLevitate()
	{
		if (this.stateStart)
		{
			base.transform.localScale = new Vector3(this.originalScale, this.originalScale, this.originalScale);
			this.meshRenderer.material.color = this.originalMaterialColor;
			if (this.lightSphere)
			{
				this.lightSphere.color = this.originalLightColor;
				this.lightSphere.intensity = this.originalLightIntensity;
				this.lightSphere.range = this.originalLightRange;
			}
			this.stateStart = false;
		}
		this.PulseEffect();
	}

	// Token: 0x0600025C RID: 604 RVA: 0x0001802A File Offset: 0x0001622A
	private void StateStop()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
		this.StopEffect();
	}

	// Token: 0x0600025D RID: 605 RVA: 0x00018041 File Offset: 0x00016241
	private void StateSmash()
	{
		if (this.stateStart)
		{
			this.stateStart = false;
		}
	}

	// Token: 0x0600025E RID: 606 RVA: 0x00018054 File Offset: 0x00016254
	private void Update()
	{
		this.StateMachine();
		if (this.floaterAttack.state == FloaterAttackLogic.FloaterAttackState.levitate || this.floaterAttack.state == FloaterAttackLogic.FloaterAttackState.start)
		{
			this.StateSet(EnemyFloaterSphereEffect.FloaterSphereEffectState.levitate);
		}
		if (this.floaterAttack.state == FloaterAttackLogic.FloaterAttackState.stop)
		{
			this.StateSet(EnemyFloaterSphereEffect.FloaterSphereEffectState.stop);
		}
		if (this.floaterAttack.state == FloaterAttackLogic.FloaterAttackState.smash)
		{
			this.StateSet(EnemyFloaterSphereEffect.FloaterSphereEffectState.smash);
		}
	}

	// Token: 0x0600025F RID: 607 RVA: 0x000180B3 File Offset: 0x000162B3
	private void StateSet(EnemyFloaterSphereEffect.FloaterSphereEffectState _state)
	{
		if (this.state != _state)
		{
			this.state = _state;
			this.stateStart = true;
		}
	}

	// Token: 0x06000260 RID: 608 RVA: 0x000180CC File Offset: 0x000162CC
	private void PulseEffect()
	{
		if (base.transform.parent.transform.localScale == Vector3.zero)
		{
			return;
		}
		base.transform.localScale += new Vector3(1f, 1f, 1f) * Time.deltaTime * 2f;
		Color color = this.meshRenderer.material.color;
		if (base.transform.localScale.magnitude > 10f)
		{
			color.a -= 1f * Time.deltaTime;
			if (this.lightSphere)
			{
				this.lightSphere.intensity = 4f * color.a;
			}
		}
		this.meshRenderer.material.color = color;
		if (this.lightSphere)
		{
			this.lightSphere.range = base.transform.localScale.x * 2.8f;
		}
		this.meshRenderer.material.mainTextureOffset += new Vector2(0.1f, 0.1f) * Time.deltaTime;
		if (color.a <= 0f)
		{
			if (this.lightSphere)
			{
				this.lightSphere.intensity = 4f;
			}
			if (this.lightSphere)
			{
				this.lightSphere.range = 0f;
			}
			base.transform.localScale = Vector3.zero;
			color.a = 1f;
			this.meshRenderer.material.color = color;
		}
	}

	// Token: 0x06000261 RID: 609 RVA: 0x00018288 File Offset: 0x00016488
	private void StopEffect()
	{
		if (this.lightSphere)
		{
			Color red = Color.red;
			float b = 8f;
			float b2 = 15f;
			this.lightSphere.color = Color.Lerp(this.lightSphere.color, red, Time.deltaTime * 10f);
			this.lightSphere.intensity = Mathf.Lerp(this.lightSphere.intensity, b, Time.deltaTime * 10f);
			this.lightSphere.range = Mathf.Lerp(this.lightSphere.range, b2, Time.deltaTime * 10f);
		}
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, Vector3.one, Time.deltaTime * 10f);
		base.transform.localScale += new Vector3(0.4f, 0.4f, 0.4f) * Mathf.Sin((Time.time + (float)(this.myChildNumber * 10)) * (float)this.myChildNumber * 20f) * (0.1f + (float)this.myChildNumber / 10f);
		this.meshRenderer.material.color = Color.Lerp(this.meshRenderer.material.color, Color.red, Time.deltaTime * 10f);
	}

	// Token: 0x04000461 RID: 1121
	private MeshRenderer meshRenderer;

	// Token: 0x04000462 RID: 1122
	private Light lightSphere;

	// Token: 0x04000463 RID: 1123
	private FloaterAttackLogic floaterAttack;

	// Token: 0x04000464 RID: 1124
	private bool stateStart = true;

	// Token: 0x04000465 RID: 1125
	private float originalScale;

	// Token: 0x04000466 RID: 1126
	private Color originalLightColor;

	// Token: 0x04000467 RID: 1127
	private float originalLightIntensity;

	// Token: 0x04000468 RID: 1128
	private float originalLightRange;

	// Token: 0x04000469 RID: 1129
	private int myChildNumber;

	// Token: 0x0400046A RID: 1130
	private Color originalMaterialColor;

	// Token: 0x0400046B RID: 1131
	internal EnemyFloaterSphereEffect.FloaterSphereEffectState state;

	// Token: 0x020002D0 RID: 720
	public enum FloaterSphereEffectState
	{
		// Token: 0x04002410 RID: 9232
		levitate,
		// Token: 0x04002411 RID: 9233
		stop,
		// Token: 0x04002412 RID: 9234
		smash
	}
}
