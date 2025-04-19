using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000BC RID: 188
public class RoachOrbit : MonoBehaviour
{
	// Token: 0x060006F2 RID: 1778 RVA: 0x00041928 File Offset: 0x0003FB28
	private void Start()
	{
		this.startPosition = base.transform.position;
		this.noiseOffsetX = base.transform.position.x;
		this.noiseOffsetZ = base.transform.position.z;
		this.noiseOffsetX2 = base.transform.position.x * 1.5f;
		this.noiseOffsetZ2 = base.transform.position.z * 1.5f;
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x000419B8 File Offset: 0x0003FBB8
	[PunRPC]
	private void SquashRPC()
	{
		this.squashSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
		Object.Instantiate<GameObject>(this.roachSmashPrefab, base.transform.position, Quaternion.identity);
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x00041A18 File Offset: 0x0003FC18
	public void Squash()
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.squashSound.Play(base.transform.position, 1f, 1f, 1f, 1f);
			Object.Instantiate<GameObject>(this.roachSmashPrefab, base.transform.position, Quaternion.identity);
			Object.Destroy(base.gameObject);
			return;
		}
		this.photonView.RPC("SquashRPC", RpcTarget.AllBuffered, Array.Empty<object>());
	}

	// Token: 0x060006F5 RID: 1781 RVA: 0x00041A9C File Offset: 0x0003FC9C
	private void Update()
	{
		this.roachLoopSound.PlayLoop(true, 1f, 2f, 1f);
		float num;
		if (GameManager.instance.gameMode == 0)
		{
			num = Time.time;
		}
		else
		{
			num = NetworkManager.instance.gameTime;
		}
		float num2 = num * this.noiseSpeed;
		float num3 = Mathf.PerlinNoise(this.noiseOffsetX + num2 * this.noiseScale, 0f) * 2f - 1f;
		float num4 = Mathf.PerlinNoise(0f, this.noiseOffsetZ + num2 * this.noiseScale) * 2f - 1f;
		num2 = num * this.noiseSpeed2;
		float num5 = Mathf.PerlinNoise(this.noiseOffsetX2 + num2 * this.noiseScale2, 0f) * 2f - 1f;
		float num6 = Mathf.PerlinNoise(0f, this.noiseOffsetZ2 + num2 * this.noiseScale2) * 2f - 1f;
		float x = (num3 + num5) / 2f;
		float z = (num4 + num6) / 2f;
		Vector3 vector = this.startPosition + new Vector3(x, 0f, z) * this.radius;
		Vector3 vector2 = vector - base.transform.position;
		base.transform.position = vector;
		if (vector2 != Vector3.zero)
		{
			Quaternion quaternion = Quaternion.LookRotation(vector2);
			Quaternion rhs = Quaternion.Euler(0f, -90f, 0f);
			quaternion *= rhs;
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, this.rotationSpeed * Time.deltaTime);
		}
	}

	// Token: 0x04000BBE RID: 3006
	[Header("Roach Smash")]
	public GameObject roachSmashPrefab;

	// Token: 0x04000BBF RID: 3007
	public float radius = 5f;

	// Token: 0x04000BC0 RID: 3008
	public float rotationSpeed = 1f;

	// Token: 0x04000BC1 RID: 3009
	public float noiseScale = 1f;

	// Token: 0x04000BC2 RID: 3010
	public float noiseSpeed = 0.5f;

	// Token: 0x04000BC3 RID: 3011
	public float noiseScale2 = 0.5f;

	// Token: 0x04000BC4 RID: 3012
	public float noiseSpeed2 = 1f;

	// Token: 0x04000BC5 RID: 3013
	private Vector3 startPosition;

	// Token: 0x04000BC6 RID: 3014
	private float noiseOffsetX;

	// Token: 0x04000BC7 RID: 3015
	private float noiseOffsetZ;

	// Token: 0x04000BC8 RID: 3016
	private float noiseOffsetX2;

	// Token: 0x04000BC9 RID: 3017
	private float noiseOffsetZ2;

	// Token: 0x04000BCA RID: 3018
	private PhotonView photonView;

	// Token: 0x04000BCB RID: 3019
	public Sound squashSound;

	// Token: 0x04000BCC RID: 3020
	public Sound roachLoopSound;
}
