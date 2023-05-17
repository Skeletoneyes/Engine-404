using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public static class DemoSettings
{
	public static bool _DiscoveredOnionCutsceneDone = true;
}

public class DemoController : MonoBehaviour
{
	[SerializeField] bool _ResetPrefs;
	[Space]
	[SerializeField] Volume _MainVP;
	[SerializeField] DayTimeManager _DayTimeManager;
	[SerializeField] string _MapName;

	[Header("Fade-in sequence")]
	[SerializeField] CanvasGroup _CanvasGroup;
	[SerializeField] Image _BlackImage;
	[SerializeField] TextMeshProUGUI _Text;

	[Header("Intro Sequence")]
	[SerializeField] bool _DoSequence;
	[SerializeField] Animator _CeresAnimator;

	void Awake()
	{
		_Text.text = "";

		if (!_MainVP.profile.TryGet(out Fog fog))
		{
			return;
		}

		bool editFog = Random.Range(0, 5) < 2.5f;

		// We're going to randomise the VP a bit
		if (editFog)
		{
			fog.meanFreePath.value = Random.Range(50.0f, 250.0f);
			fog.baseHeight.value = Random.Range(-1.0f, 13.0f);
			fog.albedo.value = Color.Lerp(new(1, 0.95f, 0.815f), new(1, 0.578f, 0.306f), Random.Range(0.2f, 0.8f));
		}
		else
		{
			fog.enabled.value = false;
		}
	}

	void Start()
	{
		if (_DoSequence)
		{
			Player._Instance.Pause(PauseType.Paused);
			Player._Instance._UIController.FadeOutUI();
			Player._Instance._ModelObject.SetActive(false);

			StartCoroutine(IE_StartScene());
		}
		else
		{
			Player._Instance.Pause(PauseType.Unpaused);
		}
	}

	void Update()
	{
		if (_ResetPrefs)
		{
			PlayerPrefs.DeleteAll();
			_ResetPrefs = false;
		}
	}

	void OnDrawGizmos()
	{
		if (_ResetPrefs)
		{
			PlayerPrefs.DeleteAll();
			_ResetPrefs = false;
		}
	}

	IEnumerator IE_DoAnimation()
	{
		Ship._Instance.SetEngineFlamesVFX(true);
		_CeresAnimator.SetTrigger("DEMO_INTRO");
		Transform main = Camera.main.transform;

		CameraFollow cameraFollow = main.GetComponent<CameraFollow>();
		cameraFollow.enabled = false;
		Transform shipTransform = _CeresAnimator.transform;

		main.position = shipTransform.position + Vector3.up * 15 + Vector3.back * 20;

		Vector3 position = Vector3.Lerp(
			main.position,
			shipTransform.position + Vector3.up * 20 + Vector3.forward * 35,
			5 * Time.deltaTime
		);

		float t = 0;
		float length = 8.0f;

		while (t <= length)
		{
			// Rotate the camera to look at the Player
			position = Vector3.Lerp(
				position,
				shipTransform.position + Vector3.up * 20 + Vector3.forward * 45,
				5 * Time.deltaTime
			);

			Quaternion rotation = Quaternion.Lerp(
				main.rotation,
				Quaternion.LookRotation(MathUtil.DirectionFromTo(main.position, shipTransform.position + Vector3.up * 5, true)),
				10 * Time.deltaTime
			);

			main.SetPositionAndRotation(position, rotation);

			t += Time.deltaTime;
			yield return null;
		}

		cameraFollow.enabled = true;
		Ship._Instance.SetEngineFlamesVFX(false);
	}

	IEnumerator IE_StartScene()
	{
		_BlackImage.enabled = true;

		yield return new WaitForSeconds(1.5f);

		_Text.color = Color.clear;
		_Text.text = _MapName;

		float t = 0;

		while (t <= 2)
		{
			t += Time.deltaTime;
			_Text.color = Color.Lerp(Color.clear, Color.white, MathUtil.EaseIn3(t / 2.0f));
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSeconds(1.25f);

		StartCoroutine(IE_DoAnimation());

		t = 0;

		while (t <= 2)
		{
			t += Time.deltaTime;
			_Text.color = Color.Lerp(Color.white, Color.clear, MathUtil.EaseIn2(t / 2.0f));
			yield return new WaitForEndOfFrame();
		}

		t = 0;

		while (t <= 1.25f)
		{
			t += Time.deltaTime;
			_CanvasGroup.alpha = 1 - MathUtil.EaseIn3(t / 1.25f);
			yield return new WaitForEndOfFrame();
		}

		_Text.enabled = false;
		_BlackImage.enabled = false;
		_CanvasGroup.alpha = 1;

		yield return new WaitForSecondsRealtime(4.5f);

		// Update UI before the fade in occurs, due to animation
		Player._Instance._UIController.UpdateFullUI();

		FadeManager._Instance.FadeInOut(
			2.0f,
			2.0f,
			() =>
			{
				Player._Instance._ModelObject.SetActive(true);
				Player._Instance.Pause(PauseType.Unpaused);
				_DayTimeManager.enabled = true;
			}
		);
	}
}
