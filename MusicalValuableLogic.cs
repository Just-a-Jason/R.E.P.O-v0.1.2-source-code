using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200022E RID: 558
public class MusicalValuableLogic : MonoBehaviour
{
	// Token: 0x060011DA RID: 4570 RVA: 0x0009E73B File Offset: 0x0009C93B
	private void Start()
	{
		this.photonView = base.GetComponent<PhotonView>();
		this.grabArea = base.GetComponent<PhysGrabObjectGrabArea>();
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
		this.numberOfKeys = this.musicKeys.Count * this.numberOfOctaves;
	}

	// Token: 0x060011DB RID: 4571 RVA: 0x0009E77C File Offset: 0x0009C97C
	private void RemovePhysGrabberFromDictionary(PhysGrabber _physGrabber)
	{
		foreach (KeyValuePair<AudioSource, PhysGrabber> keyValuePair in this.currentlyPlayedKeys.ToList<KeyValuePair<AudioSource, PhysGrabber>>())
		{
			AudioSource key = keyValuePair.Key;
			if (keyValuePair.Value == _physGrabber)
			{
				key.priority = 50;
				this.currentlyPlayedKeys.Remove(key);
				break;
			}
		}
	}

	// Token: 0x060011DC RID: 4572 RVA: 0x0009E7FC File Offset: 0x0009C9FC
	private void UpdateGrabbedByLocalPlayerGrabRelease()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			this.RemovePhysGrabberFromDictionary(PhysGrabber.instance);
			return;
		}
		this.photonView.RPC("UpdateGrabbedByThisPhysGrabberGrabReleaseRPC", RpcTarget.All, new object[]
		{
			PhysGrabber.instance.photonView.ViewID
		});
	}

	// Token: 0x060011DD RID: 4573 RVA: 0x0009E84C File Offset: 0x0009CA4C
	[PunRPC]
	public void UpdateGrabbedByThisPhysGrabberGrabReleaseRPC(int physGrabberPhotonViewID)
	{
		PhysGrabber component = PhotonView.Find(physGrabberPhotonViewID).GetComponent<PhysGrabber>();
		this.RemovePhysGrabberFromDictionary(component);
	}

	// Token: 0x060011DE RID: 4574 RVA: 0x0009E86C File Offset: 0x0009CA6C
	private void PitchShiftLogic()
	{
		if (PhysGrabber.instance.grabbed && PhysGrabber.instance.grabbedPhysGrabObject == this.physGrabObject)
		{
			this.grabbedByLocalPlayer = true;
		}
		else
		{
			if (this.grabbedByLocalPlayer)
			{
				this.UpdateGrabbedByLocalPlayerGrabRelease();
			}
			this.grabbedByLocalPlayer = false;
		}
		foreach (KeyValuePair<AudioSource, PhysGrabber> keyValuePair in this.currentlyPlayedKeys.ToList<KeyValuePair<AudioSource, PhysGrabber>>())
		{
			AudioSource key = keyValuePair.Key;
			PhysGrabber value = keyValuePair.Value;
			if (!key || value == null)
			{
				this.currentlyPlayedKeys.Remove(key);
			}
			else
			{
				AudioSource audioSource = key;
				Vector3 physGrabPointPullerPosition = value.physGrabPointPullerPosition;
				Vector3 position = value.physGrabPoint.position;
				float forceMax = value.forceMax;
				float b = Mathf.Clamp(1f + (Vector3.ClampMagnitude(physGrabPointPullerPosition - position, forceMax) * 10f).magnitude / forceMax, 1f, 1f + this.pitchShiftAmount);
				audioSource.pitch = Mathf.Lerp(audioSource.pitch, b, Time.deltaTime * 10f);
				audioSource.priority = 20;
			}
		}
	}

	// Token: 0x060011DF RID: 4575 RVA: 0x0009E9C0 File Offset: 0x0009CBC0
	private void Update()
	{
		if (this.hasPitchShift)
		{
			this.PitchShiftLogic();
		}
	}

	// Token: 0x060011E0 RID: 4576 RVA: 0x0009E9D0 File Offset: 0x0009CBD0
	public void MusicKeyPressed()
	{
		int num = this.numberOfKeys;
		PlayerAvatar latestGrabber = this.grabArea.GetLatestGrabber();
		Vector3 position = latestGrabber.physGrabber.physGrabPoint.position;
		Vector3 position2 = this.musicKeysStart.position;
		Vector3 position3 = this.musicKeysEnd.position;
		Vector3 normalized = (position3 - position2).normalized;
		float num2 = Vector3.Dot(position - position2, normalized);
		float num3 = Vector3.Distance(position2, position3);
		int num4;
		if (num2 <= 0f)
		{
			num4 = 0;
		}
		else if (num2 >= num3)
		{
			num4 = num - 1;
		}
		else
		{
			float num5 = num3 / (float)num;
			num4 = (int)(num2 / num5);
		}
		if (SemiFunc.IsMultiplayer())
		{
			this.photonView.RPC("MusicKeyPressedRPC", RpcTarget.All, new object[]
			{
				num4,
				latestGrabber.physGrabber.photonView.ViewID
			});
			return;
		}
		this.MusicKeyPressedRPC(num4, -1);
	}

	// Token: 0x060011E1 RID: 4577 RVA: 0x0009EAB6 File Offset: 0x0009CCB6
	[PunRPC]
	public void MusicKeyPressedRPC(int keyIndex, int grabberID = -1)
	{
		this.PlayKey(keyIndex, grabberID);
		SemiFunc.EnemyInvestigate(this.physGrabObject.midPoint, 25f);
	}

	// Token: 0x060011E2 RID: 4578 RVA: 0x0009EAD8 File Offset: 0x0009CCD8
	private void PlayKey(int key, int grabberID = -1)
	{
		float num = 0.05f;
		int num2 = 0;
		int num3 = this.numberOfKeys / this.musicKeys.Count;
		for (int i = 0; i < this.numberOfKeys; i++)
		{
			int index = i % this.musicKeys.Count;
			if (key >= num2 && key < num2 + num3)
			{
				PhysGrabber physGrabber;
				if (grabberID != -1)
				{
					physGrabber = PhotonView.Find(grabberID).GetComponent<PhysGrabber>();
				}
				else
				{
					physGrabber = PhysGrabber.instance;
				}
				int num4 = 0;
				int num5 = this.numberOfKeys - 1;
				float num6 = Mathf.Clamp(1f - (float)(key - num4) / (float)(num5 - num4), 0f, 1f) * this.lowKeyAmpAmount;
				this.musicKeys[index].Volume = this.volume * (1f + num6);
				this.musicKeys[index].Pitch = 1f + (float)(key - num2) * num;
				AudioSource audioSource = this.musicKeys[index].Play(this.physGrabObject.midPoint, 1f, 1f, 1f, 1f);
				audioSource.priority = 225;
				if (this.hasPitchShift && physGrabber)
				{
					this.currentlyPlayedKeys.Add(audioSource, physGrabber);
				}
			}
			num2 += num3;
		}
	}

	// Token: 0x04001E12 RID: 7698
	private PhotonView photonView;

	// Token: 0x04001E13 RID: 7699
	[FormerlySerializedAs("pianoKeysStart")]
	public Transform musicKeysStart;

	// Token: 0x04001E14 RID: 7700
	[FormerlySerializedAs("pianoKeysEnd")]
	public Transform musicKeysEnd;

	// Token: 0x04001E15 RID: 7701
	[Range(0f, 1f)]
	public float volume = 0.25f;

	// Token: 0x04001E16 RID: 7702
	[Range(0f, 3f)]
	public float lowKeyAmpAmount;

	// Token: 0x04001E17 RID: 7703
	[FormerlySerializedAs("pitchShift")]
	public bool hasPitchShift;

	// Token: 0x04001E18 RID: 7704
	public float pitchShiftAmount = 1f;

	// Token: 0x04001E19 RID: 7705
	public int numberOfOctaves = 6;

	// Token: 0x04001E1A RID: 7706
	public List<Sound> musicKeys;

	// Token: 0x04001E1B RID: 7707
	private PhysGrabObject physGrabObject;

	// Token: 0x04001E1C RID: 7708
	private PhysGrabObjectGrabArea grabArea;

	// Token: 0x04001E1D RID: 7709
	private int numberOfKeys = 108;

	// Token: 0x04001E1E RID: 7710
	private Dictionary<AudioSource, PhysGrabber> currentlyPlayedKeys = new Dictionary<AudioSource, PhysGrabber>();

	// Token: 0x04001E1F RID: 7711
	private bool grabbedByLocalPlayer;
}
