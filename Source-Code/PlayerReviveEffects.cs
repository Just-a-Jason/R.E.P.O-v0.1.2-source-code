using System;
using UnityEngine;

// Token: 0x020001BE RID: 446
public class PlayerReviveEffects : MonoBehaviour
{
	// Token: 0x06000F04 RID: 3844 RVA: 0x00088F02 File Offset: 0x00087102
	private void Start()
	{
		this.reviveLightIntensityDefault = this.reviveLight.intensity;
	}

	// Token: 0x06000F05 RID: 3845 RVA: 0x00088F18 File Offset: 0x00087118
	private void Update()
	{
		if (this.triggered)
		{
			this.reviveLight.intensity = Mathf.Lerp(this.reviveLight.intensity, 0f, Time.deltaTime * 1f);
			if (this.impactParticle.isStopped && this.swirlParticle.isStopped && this.reviveLight.intensity < 0.01f)
			{
				this.triggered = false;
				this.reviveLight.intensity = this.reviveLightIntensityDefault;
				this.enableTransform.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000F06 RID: 3846 RVA: 0x00088FB0 File Offset: 0x000871B0
	public void Trigger()
	{
		base.transform.position = this.PlayerAvatar.playerDeathHead.physGrabObject.centerPoint;
		if (SemiFunc.RunIsTutorial())
		{
			base.transform.position = PlayerAvatar.instance.transform.position;
		}
		GameDirector.instance.CameraShake.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.5f);
		GameDirector.instance.CameraImpact.ShakeDistance(5f, 3f, 8f, base.transform.position, 0.1f);
		this.triggered = true;
		this.enableTransform.gameObject.SetActive(true);
		this.reviveSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
	}

	// Token: 0x0400193F RID: 6463
	private bool triggered;

	// Token: 0x04001940 RID: 6464
	public PlayerAvatar PlayerAvatar;

	// Token: 0x04001941 RID: 6465
	public Transform enableTransform;

	// Token: 0x04001942 RID: 6466
	[Space]
	public Light reviveLight;

	// Token: 0x04001943 RID: 6467
	private float reviveLightIntensityDefault;

	// Token: 0x04001944 RID: 6468
	public ParticleSystem impactParticle;

	// Token: 0x04001945 RID: 6469
	public ParticleSystem swirlParticle;

	// Token: 0x04001946 RID: 6470
	[Space]
	public Sound reviveSound;
}
