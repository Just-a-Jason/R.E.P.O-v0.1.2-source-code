using System;
using UnityEngine;

// Token: 0x0200019E RID: 414
public class VideoGreenScreen : MonoBehaviour
{
	// Token: 0x06000DEE RID: 3566 RVA: 0x0007E103 File Offset: 0x0007C303
	private void Start()
	{
		VideoGreenScreen.instance = this;
	}

	// Token: 0x06000DEF RID: 3567 RVA: 0x0007E10C File Offset: 0x0007C30C
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.KeypadMultiply))
		{
			if (this.greenScreenFloor.GetComponent<Renderer>().material.color == Color.green)
			{
				this.greenScreenFloor.GetComponent<Renderer>().material.color = Color.blue;
				base.GetComponentInChildren<Renderer>().material.color = Color.blue;
			}
			else
			{
				this.greenScreenFloor.GetComponent<Renderer>().material.color = Color.green;
				base.GetComponentInChildren<Renderer>().material.color = Color.green;
			}
		}
		PostProcessing.Instance.VignetteOverride(Color.black, 0f, 1f, 10f, 10f, 0.2f, base.gameObject);
		PostProcessing.Instance.BloomDisable(0.2f);
		PostProcessing.Instance.GrainDisable(0.2f);
		GameplayManager.instance.OverrideCameraAnimation(0f, 0.2f);
		GameplayManager.instance.OverrideCameraNoise(0f, 0.2f);
		GameplayManager.instance.OverrideCameraShake(0f, 0.2f);
		foreach (RaycastHit raycastHit in Physics.RaycastAll(base.transform.position, Vector3.down, 100f, LayerMask.GetMask(new string[]
		{
			"Default"
		})))
		{
			if (raycastHit.collider.CompareTag("Wall"))
			{
				this.greenScreenFloor.position = raycastHit.point + Vector3.up * 0.01f;
			}
		}
		Transform headLookAtTransform = PlayerAvatar.instance.playerAvatarVisuals.headLookAtTransform;
		base.transform.LookAt(headLookAtTransform);
		base.transform.position = headLookAtTransform.position + headLookAtTransform.forward * this.distFromPlayer;
		if (Input.GetAxis("Mouse ScrollWheel") < 0f)
		{
			this.distFromPlayer -= 0.1f;
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0f)
		{
			this.distFromPlayer += 0.1f;
		}
		this.distFromPlayer = Mathf.Clamp(this.distFromPlayer, 1f, 10f);
	}

	// Token: 0x040016D1 RID: 5841
	private float distFromPlayer = 2f;

	// Token: 0x040016D2 RID: 5842
	public Transform greenScreenFloor;

	// Token: 0x040016D3 RID: 5843
	public static VideoGreenScreen instance;
}
