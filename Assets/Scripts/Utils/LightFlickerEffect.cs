using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.HighDefinition;

// Written by Steve Streeting 2017
// License: CC0 Public Domain http://creativecommons.org/publicdomain/zero/1.0/

/// <summary>
/// Component which will flicker a linked light while active by changing its
/// intensity between the min and max values given. The flickering can be
/// sharp or smoothed depending on the value of the smoothing parameter.
///
/// Just activate / deactivate this component as usual to pause / resume flicker
/// </summary>
public class LightFlickerEffect : MonoBehaviour
{
	[SerializeField]
	bool flicker = true;

	[SerializeField]
	float flickerIntensity = 50f;

	private float baseIntensity;
	private HDAdditionalLightData lightData;


	void Awake()
	{
		lightData = gameObject.GetComponent<HDAdditionalLightData>();
		baseIntensity = lightData.intensity;
	}


	void Update()
	{
		if (flicker)
		{
			float noise = Mathf.PerlinNoise(Random.Range(0.0f, 1000.0f), Time.time);
			lightData.intensity = Mathf.Lerp(baseIntensity - flickerIntensity, baseIntensity, noise);
		}
	}
}

