using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000041 RID: 65
public class SemiLaser : MonoBehaviour
{
	// Token: 0x06000182 RID: 386 RVA: 0x0000F5FC File Offset: 0x0000D7FC
	private void Start()
	{
		this.enableLaser.SetActive(true);
		this.lineRenderers = base.GetComponentsInChildren<LineRenderer>().ToList<LineRenderer>();
		this.startPosition = base.transform.position;
		this.endPosition = base.transform.position + base.transform.forward * 10f;
		this.pointLights = base.GetComponentsInChildren<Light>().ToList<Light>();
		this.pointLights.RemoveAll((Light light) => light.type != LightType.Point);
		this.pointLights.RemoveAll((Light light) => light.shadows > LightShadows.None);
		this.hitMeshRenderers = this.hitTransform.GetComponentsInChildren<MeshRenderer>().ToList<MeshRenderer>();
		this.hitTransform.localScale = Vector3.one * 0.1f;
		this.hitTransform.gameObject.SetActive(false);
		this.hitParticles = this.HitParticlesTransform.GetComponentsInChildren<ParticleSystem>().ToList<ParticleSystem>();
		this.shootParticles = this.shootTransform.GetComponentsInChildren<ParticleSystem>().ToList<ParticleSystem>();
		this.hurtCollider = base.GetComponentInChildren<HurtCollider>();
		this.hitLight = this.hitTransform.GetComponentInChildren<Light>();
		this.hitLightOriginalIntensity = this.hitLight.intensity;
		this.graceParticles = this.graceTransform.GetComponentsInChildren<ParticleSystem>().ToList<ParticleSystem>();
		this.graceTransform.localScale = Vector3.one * this.beamThickness;
		this.shootTransform.localScale = Vector3.one * this.beamThickness;
		this.hitTransform.localScale = Vector3.one * this.beamHitSize * this.beamThickness;
		this.enableLaser.SetActive(false);
		this.beamThicknessOriginal = this.beamThickness;
		this.originalHitLightRange = this.hitLight.range;
		this.audioSourceTransform = this.audioSource.transform;
		this.audioSourceHitTransform = this.audioSourceHit.transform;
	}

	// Token: 0x06000183 RID: 387 RVA: 0x0000F821 File Offset: 0x0000DA21
	public void LaserActive(Vector3 _startPosition, Vector3 _endPosition, bool _isHitting)
	{
		if (!this.enableLaser.activeSelf)
		{
			this.enableLaser.SetActive(true);
		}
		this.startPosition = _startPosition;
		this.endPosition = _endPosition;
		this.isHitting = _isHitting;
		this.isActiveTimer = 0.1f;
	}

	// Token: 0x06000184 RID: 388 RVA: 0x0000F85C File Offset: 0x0000DA5C
	private void ActiveTimer()
	{
		if (this.isActiveTimer <= 0f)
		{
			this.isActive = false;
			this.HitParticles(false);
			this.ShootParticles(false);
		}
		if (this.isActiveTimer > 0f)
		{
			this.isActive = true;
			this.isActiveTimer -= Time.fixedDeltaTime;
		}
	}

	// Token: 0x06000185 RID: 389 RVA: 0x0000F8B1 File Offset: 0x0000DAB1
	private void FixedUpdate()
	{
		this.ActiveTimer();
	}

	// Token: 0x06000186 RID: 390 RVA: 0x0000F8BC File Offset: 0x0000DABC
	private void LaserActiveIntroOutro()
	{
		if (!this.isActive)
		{
			this.beamThickness = Mathf.Lerp(this.beamThickness, 0f, Time.deltaTime * 10f);
			this.hurtCollider.gameObject.SetActive(false);
			if (this.beamThickness < 0.01f)
			{
				this.enableLaser.SetActive(false);
				this.beamThickness = 0f;
			}
			if (!this.laserEnd)
			{
				this.soundLaserEnd.Play(this.audioSourceTransform.position, 1f, 1f, 1f, 1f);
				this.soundLaserEndGlobal.Play(this.audioSourceTransform.position, 1f, 1f, 1f, 1f);
				this.laserEnd = true;
			}
			this.laserStart = false;
			return;
		}
		this.laserEnd = false;
		if (!this.laserStart)
		{
			this.soundLaserStart.Play(this.audioSourceTransform.position, 1f, 1f, 1f, 1f);
			this.soundLaserStartGlobal.Play(this.audioSourceTransform.position, 1f, 1f, 1f, 1f);
			this.laserStart = true;
		}
		this.beamThickness = Mathf.Lerp(this.beamThickness, 1f, Time.deltaTime * 10f);
		if (this.beamThickness > 0.95f)
		{
			this.hurtCollider.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000187 RID: 391 RVA: 0x0000FA44 File Offset: 0x0000DC44
	private void LaserPositioning()
	{
		base.transform.position = this.startPosition;
		this.hitTransform.LookAt(this.startPosition);
		this.shootTransform.LookAt(this.endPosition);
		this.graceTransform.LookAt(this.endPosition);
		this.shootTransform.position = this.startPosition;
		this.hitTransform.position = this.endPosition + this.hitTransform.forward * 0.3f;
		this.audioSourceHitTransform.position = this.hitTransform.position;
		this.HitParticlesTransform.position = this.hitTransform.position;
		this.HitParticlesTransform.LookAt(this.startPosition);
		this.hurtCollider.transform.localScale = new Vector3(this.hurtColliderBeamThickness, this.hurtColliderBeamThickness, Vector3.Distance(this.startPosition, this.endPosition));
		this.hurtCollider.transform.localPosition = new Vector3(-this.hurtCollider.transform.localScale.x / 2f, -this.hurtCollider.transform.localScale.y / 2f, 0f);
		this.hurtColliderRotation.transform.LookAt(this.endPosition);
		this.laserSpotLight.transform.LookAt(this.endPosition);
		this.laserSpotLight.range = Vector3.Distance(this.startPosition, this.endPosition) * 1.5f;
		this.laserSpotLight.intensity = this.laserSpotLightOriginalIntensity * this.beamThickness;
	}

	// Token: 0x06000188 RID: 392 RVA: 0x0000FBF8 File Offset: 0x0000DDF8
	private void Update()
	{
		this.soundLaserLoop.PlayLoop(this.enableLaser.activeSelf, 2f, 2f, 1f);
		this.soundLaserHitLoop.PlayLoop(this.isHitting && this.enableLaser.activeSelf, 2f, 2f, 1f);
		if (!this.enableLaser.activeSelf)
		{
			return;
		}
		this.LaserPositioning();
		this.AudioSourcePositioning();
		this.LaserEffectGrace();
		this.LaserEffectLine();
		this.LaserEffectIsHitting();
		this.LaserActiveIntroOutro();
	}

	// Token: 0x06000189 RID: 393 RVA: 0x0000FC8C File Offset: 0x0000DE8C
	private void LaserEffectGrace()
	{
		if (this.graceSoundTimer > 0f)
		{
			this.graceSoundTimer -= Time.deltaTime;
		}
		if (SemiFunc.FPSImpulse15())
		{
			foreach (RaycastHit raycastHit in Physics.SphereCastAll(this.startPosition, this.hurtColliderBeamThickness, this.shootTransform.forward, Vector3.Distance(this.startPosition, this.endPosition), SemiFunc.LayerMaskGetVisionObstruct()))
			{
				foreach (ParticleSystem particleSystem in this.graceParticles)
				{
					if (raycastHit.point != Vector3.zero && this.isActive)
					{
						particleSystem.transform.position = raycastHit.point;
						particleSystem.Emit(3);
						float num = Vector3.Distance(this.graceSoundPosition, raycastHit.point);
						if (this.graceSoundTimer <= 0f || num > 1f)
						{
							this.soundLaserGrace.Play(raycastHit.point, 1f, 1f, 1f, 1f);
							this.graceSoundPosition = raycastHit.point;
							this.graceSoundTimer = 0.15f;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600018A RID: 394 RVA: 0x0000FE04 File Offset: 0x0000E004
	private void AudioSourcePositioning()
	{
		Transform transform = AudioListenerFollow.instance.transform;
		Vector3 vector = this.endPosition - this.startPosition;
		float num = Vector3.Dot(transform.position - this.startPosition, vector) / vector.sqrMagnitude;
		num = Mathf.Clamp01(num);
		Vector3 position = this.startPosition + vector * num;
		this.audioSourceTransform.position = position;
	}

	// Token: 0x0600018B RID: 395 RVA: 0x0000FE74 File Offset: 0x0000E074
	private void LaserEffectLine()
	{
		float d = 0.035f * this.wobbleAmount + (this.beamThicknessOriginal - this.beamThickness) * 0.02f;
		int num = Mathf.CeilToInt(Vector3.Distance(this.startPosition, this.endPosition) * 2f);
		foreach (LineRenderer lineRenderer in this.lineRenderers)
		{
			Vector3[] array = new Vector3[num];
			for (int i = 0; i < num; i++)
			{
				float t = (float)i / (float)num;
				array[i] = Vector3.Lerp(this.startPosition, this.endPosition, t);
				array[i] += Vector3.right * Mathf.Sin(Time.time * 60f + (float)i) * d;
				array[i] += Vector3.up * Mathf.Cos(Time.time * 60f + (float)i) * d;
			}
			lineRenderer.material.mainTextureOffset = new Vector2(-Time.time * 30f, 0f);
			lineRenderer.widthMultiplier = (Mathf.PingPong(Time.time * 60f, 0.4f) + 0.8f) * this.beamThickness;
			lineRenderer.positionCount = num;
			lineRenderer.SetPositions(array);
			if (this.isHitting)
			{
				lineRenderer.endWidth = 0.4f * this.beamThickness;
			}
			else
			{
				lineRenderer.endWidth = 0f;
			}
		}
		float num2 = 4f;
		float b = 4f;
		int count = this.pointLights.Count;
		float num3 = Vector3.Distance(this.startPosition, this.endPosition);
		Vector3 normalized = (this.endPosition - this.startPosition).normalized;
		int num4 = Mathf.Min(count, Mathf.CeilToInt(num3 / 2f));
		for (int j = 0; j < count; j++)
		{
			if (j < num4)
			{
				int num5 = num4 - 1;
				if (num5 <= 0)
				{
					num5 = 1;
				}
				float t2 = (float)j / (float)num5;
				Vector3 vector = Vector3.Lerp(this.startPosition, this.endPosition, t2);
				this.pointLights[j].transform.position = Vector3.Lerp(this.pointLights[j].transform.position, vector, Time.deltaTime * 20f);
				if (!this.pointLights[j].enabled)
				{
					this.pointLights[j].transform.position = vector;
					this.pointLights[j].enabled = true;
					this.pointLights[j].range = 0f;
				}
				this.pointLights[j].range = Mathf.Lerp(this.pointLights[j].range, b, Time.deltaTime * 10f) * this.beamThickness;
				this.pointLights[j].intensity = Mathf.PingPong(Time.time * 20f, 2f) + num2;
			}
			else
			{
				this.pointLights[j].range = Mathf.Lerp(this.pointLights[j].range, 0f, Time.deltaTime * 8f);
				if (this.pointLights[j].range < 0.05f)
				{
					this.pointLights[j].enabled = false;
				}
			}
		}
	}

	// Token: 0x0600018C RID: 396 RVA: 0x00010260 File Offset: 0x0000E460
	private void HitParticles(bool _play)
	{
		if (!this.isActive)
		{
			_play = false;
		}
		foreach (ParticleSystem particleSystem in this.hitParticles)
		{
			if (_play)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x0600018D RID: 397 RVA: 0x000102C8 File Offset: 0x0000E4C8
	private void ShootParticles(bool _play)
	{
		if (!this.isActive)
		{
			_play = false;
		}
		foreach (ParticleSystem particleSystem in this.shootParticles)
		{
			if (_play)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
	}

	// Token: 0x0600018E RID: 398 RVA: 0x00010330 File Offset: 0x0000E530
	private void LaserEffectIsHitting()
	{
		if (this.isHitting)
		{
			this.HitParticles(true);
			this.ShootParticles(true);
			if (!this.hitTransform.gameObject.activeSelf)
			{
				this.hitTransform.gameObject.SetActive(true);
				GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, this.hitTransform.position, 0.1f);
				GameDirector.instance.CameraImpact.ShakeDistance(12f, 3f, 8f, this.hitTransform.position, 0.1f);
				this.hitTransform.localScale = Vector3.zero;
				this.hitLight.intensity = 0f;
				this.soundLaserHitStart.Play(this.hitTransform.position, 1f, 1f, 1f, 1f);
				this.hitEnd = false;
			}
			GameDirector.instance.CameraShake.ShakeDistance(8f, 0f, 6f, this.hitTransform.position, 0.1f);
			this.hitTransform.localScale = Vector3.Lerp(this.hitTransform.localScale, Vector3.one, Time.deltaTime * 40f) * this.beamHitSize * this.beamThickness;
			this.hitLight.intensity = Mathf.Lerp(this.hitLight.intensity, this.hitLightOriginalIntensity, Time.deltaTime * 40f) * this.beamThickness;
			this.hitLight.intensity += Mathf.Sin(Time.time * 40f) * 0.5f * this.beamThickness;
			this.hitLight.range = this.originalHitLightRange * this.beamThickness;
			int num = 0;
			float num2 = 0.15f;
			float num3 = 60f;
			using (List<MeshRenderer>.Enumerator enumerator = this.hitMeshRenderers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MeshRenderer meshRenderer = enumerator.Current;
					float num4;
					if (num == 0)
					{
						num4 = 0.85f;
					}
					else
					{
						num4 = 1.55f;
					}
					Vector3 vector = new Vector3(Mathf.Sin(Time.time * num3) * num2 + 1f, Mathf.Cos(Time.time * num3) * num2 + 1f, Mathf.Sin(Time.time * num3) * num2 + 1f);
					meshRenderer.transform.localScale = new Vector3(vector.x * num4, vector.y * num4, vector.z * num4);
					meshRenderer.material.mainTextureOffset = new Vector2(Time.time * 10f, 0f);
					meshRenderer.material.mainTextureScale = new Vector2(Mathf.Sin(Time.time * 20f) * 0.4f + 1f, Mathf.Cos(Time.time * 10f) * 0.4f + 1f);
					num++;
				}
				return;
			}
		}
		if (!this.hitEnd)
		{
			this.soundLaserHitEnd.Play(this.hitTransform.position, 1f, 1f, 1f, 1f);
			this.hitEnd = true;
		}
		this.hitTransform.localScale = Vector3.Lerp(this.hitTransform.localScale, Vector3.zero, Time.deltaTime * 40f) * this.beamHitSize * this.beamThickness;
		this.hitLight.intensity = Mathf.Lerp(this.hitLight.intensity, 0f, Time.deltaTime * 40f) / this.beamThickness;
		if (this.hitTransform.localScale.x < 0.01f)
		{
			this.HitParticles(false);
			this.ShootParticles(false);
			this.hitTransform.gameObject.SetActive(false);
		}
	}

	// Token: 0x04000338 RID: 824
	public float hurtColliderBeamThickness = 1f;

	// Token: 0x04000339 RID: 825
	public float beamThickness = 1f;

	// Token: 0x0400033A RID: 826
	public float beamHitSize = 1f;

	// Token: 0x0400033B RID: 827
	public float wobbleAmount = 1f;

	// Token: 0x0400033C RID: 828
	public GameObject enableLaser;

	// Token: 0x0400033D RID: 829
	public Transform hitTransform;

	// Token: 0x0400033E RID: 830
	public Transform shootTransform;

	// Token: 0x0400033F RID: 831
	public Transform graceTransform;

	// Token: 0x04000340 RID: 832
	public Light laserSpotLight;

	// Token: 0x04000341 RID: 833
	public Transform hurtColliderRotation;

	// Token: 0x04000342 RID: 834
	public Transform HitParticlesTransform;

	// Token: 0x04000343 RID: 835
	public AudioSource audioSource;

	// Token: 0x04000344 RID: 836
	public AudioSource audioSourceHit;

	// Token: 0x04000345 RID: 837
	public Sound soundLaserStart;

	// Token: 0x04000346 RID: 838
	public Sound soundLaserStartGlobal;

	// Token: 0x04000347 RID: 839
	public Sound soundLaserEnd;

	// Token: 0x04000348 RID: 840
	public Sound soundLaserEndGlobal;

	// Token: 0x04000349 RID: 841
	public Sound soundLaserLoop;

	// Token: 0x0400034A RID: 842
	public Sound soundLaserHitStart;

	// Token: 0x0400034B RID: 843
	public Sound soundLaserHitEnd;

	// Token: 0x0400034C RID: 844
	public Sound soundLaserHitLoop;

	// Token: 0x0400034D RID: 845
	public Sound soundLaserGrace;

	// Token: 0x0400034E RID: 846
	internal SemiLaser.LaserState state;

	// Token: 0x0400034F RID: 847
	private List<LineRenderer> lineRenderers = new List<LineRenderer>();

	// Token: 0x04000350 RID: 848
	private List<Light> pointLights = new List<Light>();

	// Token: 0x04000351 RID: 849
	private Vector3 startPosition;

	// Token: 0x04000352 RID: 850
	private Vector3 endPosition;

	// Token: 0x04000353 RID: 851
	private List<MeshRenderer> hitMeshRenderers = new List<MeshRenderer>();

	// Token: 0x04000354 RID: 852
	private bool isHitting = true;

	// Token: 0x04000355 RID: 853
	private List<ParticleSystem> hitParticles = new List<ParticleSystem>();

	// Token: 0x04000356 RID: 854
	private List<ParticleSystem> shootParticles = new List<ParticleSystem>();

	// Token: 0x04000357 RID: 855
	private List<ParticleSystem> graceParticles = new List<ParticleSystem>();

	// Token: 0x04000358 RID: 856
	internal HurtCollider hurtCollider;

	// Token: 0x04000359 RID: 857
	private Light hitLight;

	// Token: 0x0400035A RID: 858
	private float hitLightOriginalIntensity;

	// Token: 0x0400035B RID: 859
	private bool isActive;

	// Token: 0x0400035C RID: 860
	private float isActiveTimer;

	// Token: 0x0400035D RID: 861
	private float beamThicknessOriginal;

	// Token: 0x0400035E RID: 862
	private float originalHitLightRange;

	// Token: 0x0400035F RID: 863
	private float laserSpotLightOriginalIntensity;

	// Token: 0x04000360 RID: 864
	private bool hitEnd;

	// Token: 0x04000361 RID: 865
	private bool laserEnd;

	// Token: 0x04000362 RID: 866
	private bool laserStart;

	// Token: 0x04000363 RID: 867
	private Transform audioSourceTransform;

	// Token: 0x04000364 RID: 868
	private Transform audioSourceHitTransform;

	// Token: 0x04000365 RID: 869
	private float graceSoundTimer;

	// Token: 0x04000366 RID: 870
	private Vector3 graceSoundPosition;

	// Token: 0x020002CA RID: 714
	public enum LaserState
	{
		// Token: 0x040023D4 RID: 9172
		Intro,
		// Token: 0x040023D5 RID: 9173
		Active,
		// Token: 0x040023D6 RID: 9174
		Outro
	}
}
