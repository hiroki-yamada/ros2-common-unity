using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if SIGVERSE_PUN
using Photon.Pun;
#endif

namespace SIGVerse.Competition
{
	public interface IPanelNoticeHandler : IEventSystemHandler
	{
		void OnPanelNoticeChange(PanelNoticeStatus panelNoticeStatus);
	}
	
	public class PanelNoticeStatus
	{
		public static readonly Color Green = new Color(  0/255f, 143/255f,  36/255f, 255/255f);
		public static readonly Color Red   = new Color(255/255f,   0/255f,   0/255f, 255/255f);
		public static readonly Color Blue  = new Color(  0/255f,   0/255f, 255/255f, 255/255f);

		public string Message  { get; set; }
		public int    FontSize { get; set; }
		public Color  Color    { get; set; }
		public float  Duration { get; set; }

		public PanelNoticeStatus(string message, int fontSize, Color color, float duration)
		{
			this.Message  = message;
			this.FontSize = fontSize;
			this.Color    = color;
			this.Duration = duration;
		}

		public PanelNoticeStatus(PanelNoticeStatus panelNoticeStatus)
		{
			this.Message  = panelNoticeStatus.Message;
			this.FontSize = panelNoticeStatus.FontSize;
			this.Color    = panelNoticeStatus.Color;
			this.Duration = panelNoticeStatus.Duration;
		}
	}


#if SIGVERSE_PUN
	public class PanelNoticeController : MonoBehaviourPun, IPanelNoticeHandler
#else
	public class PanelNoticeController : MonoBehaviour, IPanelNoticeHandler
#endif
	{
		public GameObject noticePanel;

		private Text noticeText;
		private float maxDuration = 1000f;

		void Awake()
		{
			this.noticeText = this.noticePanel.GetComponentInChildren<Text>();
		}

		void Start()
		{
			this.noticePanel.SetActive(false);
		}

		public void SetMaxDuration(float maxDuration)
		{
			this.maxDuration = maxDuration;
		}

#if SIGVERSE_PUN
		private void ShowNotice(PanelNoticeStatus pns)
		{
			this.photonView.RPC(nameof(ShowNoticeRPC), RpcTarget.All, pns.Message, pns.FontSize, pns.Color.r, pns.Color.g, pns.Color.b, pns.Color.a, pns.Duration);
		}

		[PunRPC]
		private void ShowNoticeRPC(string message, int fontSize, float color_r, float color_g, float color_b, float color_a, float duration, PhotonMessageInfo info)
		{
			Color color = new Color(color_r, color_g, color_b, color_a);
			PanelNoticeStatus panelNoticeStatus = new PanelNoticeStatus(message, fontSize, color, duration);

			this.ShowNoticeExe(panelNoticeStatus);
		}
#else
		private void ShowNotice(PanelNoticeStatus panelNoticeStatus)
		{
			this.ShowNoticeExe(panelNoticeStatus);
		}
#endif

		private void ShowNoticeExe(PanelNoticeStatus panelNoticeStatus)
		{
			this.noticePanel.SetActive(true);

			noticeText.text     = panelNoticeStatus.Message;
			noticeText.fontSize = panelNoticeStatus.FontSize;
			noticeText.color    = panelNoticeStatus.Color;

			StartCoroutine(this.HideNotice(panelNoticeStatus.Duration)); // Hide
		}

		private IEnumerator HideNotice(float duration)
		{
			if (duration > this.maxDuration)
			{
				duration = this.maxDuration;
			}

			float hideTime = UnityEngine.Time.time + duration;

			while(UnityEngine.Time.time < hideTime)
			{
				yield return null;
			}

			this.noticePanel.SetActive(false);
		}

		public void OnPanelNoticeChange(PanelNoticeStatus panelNoticeStatus)
		{
			this.ShowNotice(panelNoticeStatus);
		}
	}
}

