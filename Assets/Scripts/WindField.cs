using UnityEngine;

public static class WindField
{
    public static int strengthSeed = UnityEngine.Random.Range(0, 10000);
    public static int angleSeed = UnityEngine.Random.Range(0, 10000);
    public static float GetWindAtPoint(float x, float y, int width, int height, float timeFactor)
    {
        return (Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float3(x / width, y / height, strengthSeed))+1)/2 ;
    }
    public static float GetWindAngleAtPoint(float x, float y, int width, int height, float timeFactor) // in RADIANS
    {
        return (Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float3(x / width, y / height, angleSeed))) * Mathf.PI;
    }
}
