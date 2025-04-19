using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000101 RID: 257
public class ToiletFun : MonoBehaviour
{
	// Token: 0x060008E4 RID: 2276 RVA: 0x00054FF2 File Offset: 0x000531F2
	private void Start()
	{
		this.sphereChecker = base.GetComponentInChildren<SphereCollider>().transform;
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x00055014 File Offset: 0x00053214
	private void FixedUpdate()
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			bool flag = false;
			foreach (PhysGrabObject physGrabObject in this.physGrabObjects)
			{
				if (physGrabObject)
				{
					if (this.toiletActive)
					{
						Rigidbody component = physGrabObject.GetComponent<Rigidbody>();
						if (component)
						{
							component.AddTorque(Vector3.up * this.toiletCharge, ForceMode.Impulse);
							if (this.randomForceTimer <= 0f)
							{
								Vector3 a = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
								component.AddForce(a * this.toiletCharge, ForceMode.Impulse);
							}
						}
					}
					float num = Vector3.Distance(physGrabObject.midPoint, this.sphereChecker.position);
					if ((physGrabObject.impactHappenedTimer > 0f || physGrabObject.impactLightTimer > 0f || physGrabObject.impactHeavyTimer > 0f || physGrabObject.impactMediumTimer > 0f) && num < this.sphereChecker.localScale.x)
					{
						flag = true;
					}
				}
			}
			foreach (PlayerAvatar playerAvatar in this.playerAvatars)
			{
				if (playerAvatar && this.toiletActive)
				{
					playerAvatar.tumble.TumbleRequest(true, false);
					playerAvatar.tumble.TumbleOverrideTime(10f);
				}
			}
			if (this.playerController && this.toiletActive)
			{
				this.playerController.playerAvatarScript.tumble.TumbleRequest(true, false);
				this.playerController.playerAvatarScript.tumble.TumbleOverrideTime(10f);
			}
			if (this.splashTimer <= 0f && flag)
			{
				if (SemiFunc.IsMultiplayer())
				{
					this.photonView.RPC("SplashRPC", RpcTarget.All, Array.Empty<object>());
				}
				else
				{
					this.Splash();
				}
				this.splashTimer = 0.5f;
			}
			if (this.toiletActive)
			{
				if (this.hingeRattlingTimer <= 0f)
				{
					if (this.hingeRigidBody && !this.hingeRigidBody.GetComponent<PhysGrabHinge>().broken)
					{
						this.hingeRigidBody.AddForce(-Vector3.up * 2f * this.toiletCharge * 0.5f, ForceMode.Impulse);
					}
					this.hingeRattlingTimer = 0.1f;
					return;
				}
				this.hingeRattlingTimer -= Time.deltaTime;
			}
		}
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x000552F8 File Offset: 0x000534F8
	private void Update()
	{
		float loopPitch = 1f + this.toiletCharge / 8f * 2f;
		this.soundLoop.LoopPitch = loopPitch;
		this.soundLoop.PlayLoop(this.toiletActive, 1f, 1f, 1f);
		if (this.toiletActive)
		{
			float num = 1f + this.toiletCharge * 20f;
			GameDirector.instance.CameraShake.ShakeDistance(num / 30f, 0f, 3f, base.transform.position, 0.5f);
		}
		if ((!SemiFunc.IsMultiplayer() && this.explosionTimer <= 0f) || (SemiFunc.IsMasterClientOrSingleplayer() && this.explosionTimer <= 0f))
		{
			this.checkTimer += Time.deltaTime;
			if (this.checkTimer > 1f)
			{
				this.physGrabObjects.Clear();
				this.playerAvatars.Clear();
				this.playerController = null;
				foreach (Collider collider in Physics.OverlapSphere(this.sphereChecker.position, this.sphereChecker.localScale.x * 0.5f, SemiFunc.LayerMaskGetPlayersAndPhysObjects(), QueryTriggerInteraction.Ignore))
				{
					if (collider)
					{
						PhysGrabObject componentInParent = collider.GetComponentInParent<PhysGrabObject>();
						if (componentInParent)
						{
							this.physGrabObjects.Add(componentInParent);
							break;
						}
						if (collider.GetComponentInParent<PlayerAvatar>())
						{
							this.playerAvatars.Add(collider.GetComponentInParent<PlayerAvatar>());
							break;
						}
						if (collider.GetComponentInParent<PlayerController>())
						{
							this.playerController = collider.GetComponentInParent<PlayerController>();
							break;
						}
					}
				}
				this.checkTimer = 0f;
			}
		}
		if (this.splashTimer > 0f)
		{
			this.splashTimer -= Time.deltaTime;
		}
		if (this.explosionTimer > 0f)
		{
			this.explosionTimer -= Time.deltaTime;
			if (this.explosionTimer <= 0f && this.explosion)
			{
				this.explosion.gameObject.SetActive(false);
			}
		}
		if (this.hurtColliderTimer > 0f)
		{
			this.hurtColliderTimer -= Time.deltaTime;
			if (this.hurtColliderTimer <= 0f)
			{
				if (this.hurtCollider)
				{
					this.hurtCollider.SetActive(false);
				}
			}
			else if (this.hurtCollider)
			{
				this.hurtCollider.SetActive(true);
			}
		}
		if (this.toiletActive)
		{
			if (this.randomForceTimer > 0f)
			{
				this.randomForceTimer -= Time.deltaTime;
			}
			else
			{
				this.randomForceTimer = Random.Range(0.5f, 2f);
			}
			if (!this.smallParticles.isPlaying)
			{
				this.smallParticles.Play();
			}
			if (!this.bigParticles.isPlaying)
			{
				this.bigParticles.Play();
			}
			this.toiletCharge += Time.deltaTime * 2f;
			this.toiletCharge = Mathf.Clamp(this.toiletCharge, 0f, 8f);
			if (this.toiletCharge > 7.5f)
			{
				this.safetyTimer += Time.deltaTime;
				if (this.safetyTimer > 3f)
				{
					this.EndCook();
				}
				if (SemiFunc.IsMasterClientOrSingleplayer())
				{
					this.Explosion();
					return;
				}
			}
		}
		else
		{
			this.toiletCharge -= Time.deltaTime * 20f;
			this.toiletCharge = Mathf.Clamp(this.toiletCharge, 0f, 8f);
			this.smallParticles.Stop();
			this.bigParticles.Stop();
		}
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x000556BD File Offset: 0x000538BD
	private void Explosion()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("ExplosionRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.ExplosionRPC();
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x000556E3 File Offset: 0x000538E3
	[PunRPC]
	public void EndCookRPC()
	{
		this.EndCook();
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x000556EB File Offset: 0x000538EB
	private void EndCook()
	{
		this.toiletActive = false;
		this.safetyTimer = 0f;
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x00055700 File Offset: 0x00053900
	[PunRPC]
	public void ExplosionRPC()
	{
		GameDirector.instance.CameraShake.ShakeDistance(8f, 3f, 8f, base.transform.position, 0.1f);
		GameDirector.instance.CameraImpact.ShakeDistance(20f, 3f, 8f, base.transform.position, 0.1f);
		this.soundExplosion.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.explosion.gameObject.SetActive(true);
		this.hurtCollider.gameObject.SetActive(true);
		this.explosionTimer = 3f;
		this.hurtColliderTimer = 0.5f;
		this.EndCook();
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x000557D2 File Offset: 0x000539D2
	private void FlushStart()
	{
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("FlushStartRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.FlushStartRPC();
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x000557F8 File Offset: 0x000539F8
	[PunRPC]
	public void FlushStartRPC()
	{
		this.soundFlush.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.toiletActive = true;
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x0005582C File Offset: 0x00053A2C
	public void Flush()
	{
		if (!this.toiletActive && SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.FlushStart();
		}
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x00055843 File Offset: 0x00053A43
	[PunRPC]
	public void SplashRPC()
	{
		this.Splash();
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x0005584C File Offset: 0x00053A4C
	private void Splash()
	{
		this.splashBigParticles.Play();
		this.splashSmallParticles.Play();
		this.soundSplash.Play(base.transform.position, 1f, 1f, 1f, 1f);
		this.splashTimer = 1f;
	}

	// Token: 0x04001035 RID: 4149
	internal Transform sphereChecker;

	// Token: 0x04001036 RID: 4150
	internal List<PhysGrabObject> physGrabObjects = new List<PhysGrabObject>();

	// Token: 0x04001037 RID: 4151
	internal List<PlayerAvatar> playerAvatars = new List<PlayerAvatar>();

	// Token: 0x04001038 RID: 4152
	private PlayerController playerController;

	// Token: 0x04001039 RID: 4153
	internal float checkTimer;

	// Token: 0x0400103A RID: 4154
	private PhotonView photonView;

	// Token: 0x0400103B RID: 4155
	private float toiletCharge;

	// Token: 0x0400103C RID: 4156
	private float explosionTimer;

	// Token: 0x0400103D RID: 4157
	private float hurtColliderTimer;

	// Token: 0x0400103E RID: 4158
	private bool toiletActive;

	// Token: 0x0400103F RID: 4159
	public GameObject hurtCollider;

	// Token: 0x04001040 RID: 4160
	public Transform explosion;

	// Token: 0x04001041 RID: 4161
	public Sound soundLoop;

	// Token: 0x04001042 RID: 4162
	public Sound soundExplosion;

	// Token: 0x04001043 RID: 4163
	public Sound soundFlush;

	// Token: 0x04001044 RID: 4164
	public Sound soundSplash;

	// Token: 0x04001045 RID: 4165
	private float safetyTimer;

	// Token: 0x04001046 RID: 4166
	public ParticleSystem smallParticles;

	// Token: 0x04001047 RID: 4167
	public ParticleSystem bigParticles;

	// Token: 0x04001048 RID: 4168
	public ParticleSystem splashBigParticles;

	// Token: 0x04001049 RID: 4169
	public ParticleSystem splashSmallParticles;

	// Token: 0x0400104A RID: 4170
	private float splashTimer;

	// Token: 0x0400104B RID: 4171
	private float randomForceTimer;

	// Token: 0x0400104C RID: 4172
	public Rigidbody hingeRigidBody;

	// Token: 0x0400104D RID: 4173
	private float hingeRattlingTimer;
}
