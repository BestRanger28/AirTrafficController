using UnityEngine;

public static class WeatherField
{
    public static int weatherSeed = UnityEngine.Random.Range(0, 10000);

    public static float GetWeatherAtPoint(float x, float y, int width, int height, float timeFactor)
    {
        return Mathf.Pow((Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float3(x/width,y/height,weatherSeed))+1)/2,2);
    }
}

