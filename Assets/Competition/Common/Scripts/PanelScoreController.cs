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
	public interface IPanelScoreHandler : IEventSystemHandler
	{
		void OnScoreChange(float score);
		void OnScoreChange(float score, float total);
	}
	
#if SIGVERSE_PUN
	public class PanelScoreController : MonoBehaviourPun, IPanelScoreHandler
#else
	public class PanelScoreController : MonoBehaviour, IPanelScoreHandler
#endif
	{
		public GameObject scorePanel;

		private Text scoreValText;
		private Text totalValText;

#if SIGVERSE_PUN
		private bool isUsingPun = true;
#endif

		void Awake()
		{
			this.scoreValText = this.scorePanel.transform.Find("ScoreValText").GetComponent<Text>();
			this.totalValText = this.scorePanel.transform.Find("TotalValText").GetComponent<Text>();
		}

		public void DontUsePun()
		{
#if SIGVERSE_PUN
			this.isUsingPun = false;
#endif
		}

#if SIGVERSE_PUN
		public void OnScoreChange(float score)
		{
			// For Server side
			this.ScoreChangeRPC(score);
			
			// For Client side
			if(this.isUsingPun && PhotonNetwork.InRoom)
			{
				this.photonView.RPC(nameof(ScoreChangeRPC), RpcTarget.Others, score);
			}
		}

		[PunRPC]
		private void ScoreChangeRPC(float score)
		{
			this.ScoreChangeExe(score);
		}

		public void OnScoreChange(float score, float total)
		{
			// For Server side
			this.ScoreChangeAllRPC(score, total);

			// For Client side (Wait for the login to the PUN)
			if (this.isUsingPun)
			{
				StartCoroutine(this.ChangeScoreAll(score, total));
			}
		}

		private IEnumerator ChangeScoreAll(float score, float total)
		{
			while(!PhotonNetwork.InRoom)
			{
				yield return null;
			}

			this.photonView.RPC(nameof(ScoreChangeAllRPC), RpcTarget.OthersBuffered, score, total);
		}

		[PunRPC]
		private void ScoreChangeAllRPC(float score, float total)
		{
			this.ScoreChangeAllExe(score, total);
		}
#else
		public void OnScoreChange(float score)
		{
			this.ScoreChangeExe(score);
		}

		public void OnScoreChange(float score, float total)
		{
			this.ScoreChangeAllExe(score, total);
		}
#endif

		private void ScoreChangeExe(float score)
		{
			this.scoreValText.text = score.ToString();
		}

		private void ScoreChangeAllExe(float score, float total)
		{
			this.scoreValText.text = score.ToString();
			this.totalValText.text = total.ToString();
		}
	}
}

