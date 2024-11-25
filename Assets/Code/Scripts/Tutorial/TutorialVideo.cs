// *******************************************************************************
// *******************************************************************************
// <info>
//   File: TutorialVideo.cs
//   Author: FotosFrangoudes
//   Project: CYENSverse/Assembly-CSharp
//   Creation Date: 2024/4/3 13:47
//   Last Modification Date: 2024/4/3 16:15
// </info>
// <copyright file="TutorialVideo.cs"/>
// Copyright © 2024-2024, All rights reserved.
// No part of this work may be reproduced, stored, or distributed in any
// form or by any means, without the prior permission of the author.
// *******************************************************************************
// *******************************************************************************

using System;
using System.Collections;

using TMPro;

using TriInspector;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class TutorialVideo : MonoBehaviour
{
	private Vector3 initialImageScale;

	[SerializeField]
	private XRSimpleInteractable videoPokeInteractable;

	[SerializeField]
	private SpinnerController loadingEffect;

	[SerializeField]
	private TutorialInstructions instructionManager;

	[SerializeField]
	private GameObject hoverVideoAnim;

	[SerializeField]
	private RectTransform instructionCloseVideo;

	[SerializeField]
	private GameObject handCloseVideo;

	#region Frame

	[Title("Frame")]
	[SerializeField]
	private Image frame;

	[SerializeField]
	private Sprite disabledFrame;

	[SerializeField]
	private Sprite enabledFrame;

	[SerializeField]
	private Sprite hoveredFrame;

	[SerializeField]
	private Sprite selectedFrame;

	#endregion Frame

	public float scaleFactor;

	[SerializeField]
	private XRSimpleInteractable closeButton;

	public Action<TutorialVideo> OnVideoSelected;

	void OnEnable()
	{
		initialImageScale = transform.localScale;
		hoverVideoAnim.SetActive(true);
	}

	public void HoverEntered()
	{
		ScaleUp();
		StartCoroutine(Hovering());
	}

	IEnumerator Hovering()
	{
		yield return new WaitForSeconds(0.5f);
		StartCoroutine(Loading());
	}

	public void HoverExited()
	{
		StopAllCoroutines();
		ScaleDown();
		loadingEffect.Hide();
	}

	private void ScaleUp()
	{
		gameObject.transform.localScale *= scaleFactor;
		frame.sprite = hoveredFrame;
	}

	private void ScaleDown()
	{
		gameObject.transform.localScale = initialImageScale;
		frame.sprite = enabledFrame;
	}

	IEnumerator Loading()
	{
		loadingEffect.Show();
		loadingEffect.Load();
		yield return new WaitUntil(() => !loadingEffect.IsLoading());
		loadingEffect.Hide();
		hoverVideoAnim.SetActive(false);
		instructionManager.FadeOutInstruction();
		FocusPlayVideo();
	}

	// Called when the video is selected through the interactor
	public void FocusPlayVideo()
	{
		Deactivate();

		Vector3 newScale = new Vector3(1.5f, 1.5f, initialImageScale.z);
		LeanTween.scale(gameObject, newScale, 1.5f).setOnComplete(EnableVideo);

		frame.sprite = selectedFrame;
	}

	/// <summary>
	///     Enable the video
	/// </summary>
	private void EnableVideo()
	{
		EnableCloseButton();
		StartCoroutine(instructionManager.NextInstruction(0.1f, 1));

		ShowCloseVideoInstruction(0.75f);
		ShowCloseVideoButton(1.25f);
	}

	/// <summary>
	///     Show the close video instruction
	/// </summary>
	private void ShowCloseVideoInstruction(float interpolationTime)
	{
		if (!instructionCloseVideo.TryGetComponent<TextMeshProUGUI>(out var closeVideoInstructionText)) return;
		SetFontSize(0);
		LeanTween.value(gameObject, SetFontSize, 0, 25, interpolationTime);
		return;

		void SetFontSize(float value)
		{
			closeVideoInstructionText.fontSize = value;
		}
	}

	/// <summary>
	///     Show the video button
	/// </summary>
	private void ShowCloseVideoButton(float interpolationTime)
	{
		// Move close video instruction towards user
		var xrCameraTransform = XrReferences.XrCameraTransform;
		var cameraPosition = xrCameraTransform.position;

		// Position instruction near the user
		Vector3 newPosition = cameraPosition + (transform.position - cameraPosition).normalized * 0.55f;
		newPosition.y = cameraPosition.y - 0.3f;
		newPosition.x = cameraPosition.x + 0.3f;
		LeanTween.move(instructionCloseVideo.gameObject, newPosition, interpolationTime);

		instructionCloseVideo.LeanRotateX(40, interpolationTime * 0.5f);
	}

	private void EnableCloseButton()
	{
		// Enable video close button
		var xrCameraTransform = XrReferences.XrCameraTransform;
		var cameraPosition = xrCameraTransform.position;
		// Position video close button near the user
		Vector3 newPosition = cameraPosition + (transform.position - cameraPosition).normalized * 0.6f;
		newPosition.y = cameraPosition.y - 0.3f;
		var interactableTransform = closeButton.transform.parent;
		interactableTransform.position = newPosition;

		closeButton.gameObject.SetActive(true);
		closeButton.selectEntered.AddListener(delegate { StartCoroutine(LoadCloseVideo()); });
		closeButton.enabled = true;
		handCloseVideo.SetActive(true);
	}

	private IEnumerator LoadCloseVideo()
	{
		yield return new WaitForSeconds(0.6f);
		CloseVideo();
		instructionManager.FadeOutVideo();
		instructionManager.FadeOutInstruction();
	}

	public void CloseVideo()
	{
		frame.sprite = enabledFrame;
		handCloseVideo.SetActive(false);
		DisableCloseButton();
	}

	public void DisableCloseButton()
	{
		// Disable video close button
		closeButton.selectEntered.RemoveAllListeners();
		closeButton.enabled = false;
		closeButton.gameObject.SetActive(false);
		StartCoroutine(instructionManager.NextInstruction(2, 1));
	}

	public void Deactivate()
	{
		videoPokeInteractable.enabled = false;
	}

	public void ResetScale()
	{
		LeanTween.scale(gameObject, initialImageScale, 2f);
	}
}