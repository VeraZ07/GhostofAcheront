using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.HighDefinition;

public class __LightFlicker : MonoBehaviour {
	
	[SerializeField]
	bool flicker = true;
	
	[SerializeField]
	float flickerIntensity = 0.5f;
	
	private float baseIntensity;
	private HDAdditionalLightData lightData;
	
	
	void Awake()
	{
		lightData = gameObject.GetComponent<HDAdditionalLightData>();
		baseIntensity = lightData.intensity;
	}
	
	
	void Update ()
	{
		if (flicker)
		{
			float noise = Mathf.PerlinNoise(Random.Range(0.0f, 1000.0f), Time.time);
			lightData.intensity = Mathf.Lerp( baseIntensity - flickerIntensity , baseIntensity, noise );
		}
	}
}
