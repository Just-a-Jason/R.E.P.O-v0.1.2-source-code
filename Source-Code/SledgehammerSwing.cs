using System;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class SledgehammerSwing : MonoBehaviour
{
	// Token: 0x06000718 RID: 1816 RVA: 0x000433E5 File Offset: 0x000415E5
	public void Swing()
	{
		if (!this.Swinging)
		{
			this.Swinging = true;
			this.SwingSound = true;
		}
	}

	// Token: 0x06000719 RID: 1817 RVA: 0x000433FD File Offset: 0x000415FD
	public void HitOutro()
	{
		this.MeshTransform.gameObject.SetActive(false);
		this.DisableTimer = 0.1f;
		this.SwingingOutro = true;
		this.LerpAmount = 0.5f;
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x00043430 File Offset: 0x00041630
	private void Update()
	{
		this.CanHit = false;
		if (this.Swinging && this.DisableTimer <= 0f)
		{
			if (!this.SwingingOutro)
			{
				if (this.LerpAmount == 0f)
				{
					PlayerController.instance.MoveForce(PlayerController.instance.transform.forward, -5f, 0.25f);
					GameDirector.instance.CameraShake.Shake(5f, 0f);
					this.Controller.SoundMoveShort.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
				if (this.SwingSound && (double)this.LerpAmount >= 0.2)
				{
					this.Controller.SoundSwing.Play(base.transform.position, 1f, 1f, 1f, 1f);
					this.SwingSound = false;
				}
				if ((double)this.LerpAmount >= 0.25 && (double)this.LerpAmount <= 0.3)
				{
					PlayerController.instance.MoveForce(PlayerController.instance.transform.forward, 20f, 0.01f);
				}
				if ((double)this.LerpAmount >= 0.2 && (double)this.LerpAmount <= 0.5)
				{
					this.CanHit = true;
				}
				this.LerpAmount += this.SwingSpeed * Time.deltaTime;
				if (this.LerpAmount >= 1f)
				{
					this.SwingingOutro = true;
					this.LerpAmount = 0f;
				}
			}
			else
			{
				if (this.LerpAmount == 0f)
				{
					GameDirector.instance.CameraShake.Shake(2f, 0.5f);
					this.Controller.SoundMoveLong.Play(base.transform.position, 1f, 1f, 1f, 1f);
				}
				if ((double)this.LerpAmount >= 0.25 && (double)this.LerpAmount <= 0.3)
				{
					PlayerController.instance.MoveForce(PlayerController.instance.transform.forward, -5f, 0.25f);
				}
				this.LerpAmount += this.OutroSpeed * Time.deltaTime;
				if (this.LerpAmount >= 1f)
				{
					if (!this.DebugSwing)
					{
						this.Swinging = false;
					}
					this.SwingingOutro = false;
					this.LerpAmount = 0f;
				}
			}
		}
		if (!this.SwingingOutro)
		{
			this.LerpResult = this.SwingCurve.Evaluate(this.LerpAmount);
		}
		else
		{
			this.LerpResult = this.OutroCurve.Evaluate(this.LerpAmount);
		}
		base.transform.localRotation = Quaternion.LerpUnclamped(Quaternion.identity, Quaternion.Euler(this.SwingRotation.x, this.SwingRotation.y, this.SwingRotation.z), this.LerpResult);
		base.transform.localPosition = Vector3.LerpUnclamped(Vector3.zero, this.SwingPosition, this.LerpResult);
		if (this.DisableTimer > 0f)
		{
			this.DisableTimer -= Time.deltaTime;
			if (this.DisableTimer <= 0f)
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x0004379C File Offset: 0x0004199C
	private void OnDrawGizmosSelected()
	{
		if (this.DebugMeshActive)
		{
			Gizmos.color = new Color(0.75f, 0f, 0f, 0.75f);
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawMesh(this.DebugMesh, 0, Vector3.zero + this.SwingPosition, Quaternion.Euler(this.SwingRotation.x, this.SwingRotation.y, this.SwingRotation.z), Vector3.one);
		}
	}

	// Token: 0x04000C3C RID: 3132
	[Header("Debug")]
	public bool DebugSwing;

	// Token: 0x04000C3D RID: 3133
	public bool DebugMeshActive;

	// Token: 0x04000C3E RID: 3134
	public Mesh DebugMesh;

	// Token: 0x04000C3F RID: 3135
	[Space]
	[Header("Other")]
	public SledgehammerController Controller;

	// Token: 0x04000C40 RID: 3136
	[Space]
	[Header("Swinging")]
	public Transform MeshTransform;

	// Token: 0x04000C41 RID: 3137
	[Space]
	[Header("State")]
	public bool Swinging;

	// Token: 0x04000C42 RID: 3138
	public bool CanHit;

	// Token: 0x04000C43 RID: 3139
	private bool SwingingOutro;

	// Token: 0x04000C44 RID: 3140
	[Space]
	[Header("Swinging")]
	public AnimationCurve SwingCurve;

	// Token: 0x04000C45 RID: 3141
	public float SwingSpeed = 1f;

	// Token: 0x04000C46 RID: 3142
	public Vector3 SwingRotation;

	// Token: 0x04000C47 RID: 3143
	public Vector3 SwingPosition;

	// Token: 0x04000C48 RID: 3144
	private bool SwingSound = true;

	// Token: 0x04000C49 RID: 3145
	[Space]
	public AnimationCurve OutroCurve;

	// Token: 0x04000C4A RID: 3146
	public float OutroSpeed = 1f;

	// Token: 0x04000C4B RID: 3147
	private float LerpAmount;

	// Token: 0x04000C4C RID: 3148
	private float LerpResult;

	// Token: 0x04000C4D RID: 3149
	private float DisableTimer;
}
