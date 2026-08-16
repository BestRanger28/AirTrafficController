using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public MapCreator creator;
    public Pathfinding path;
    public void NewWind()
    {
        WindField.strengthSeed = UnityEngine.Random.Range(0, 10000);
        StartCoroutine( path.Process());
        creator.ChangeMapType();
    }
    public void NewWindAngle()
    {
        WindField.angleSeed = UnityEngine.Random.Range(0, 10000);
        StartCoroutine(path.Process());
        creator.ChangeMapType();
    }
    public void NewWeather()
    {
        WeatherField.weatherSeed = UnityEngine.Random.Range(0, 10000);
        StartCoroutine(path.Process());
        creator.ChangeMapType();
    }
    public void NewMap()
    {
        creator.seed = UnityEngine.Random.Range(0, 10000);
        creator.MakeMap();
        StartCoroutine(path.Process());
        creator.ChangeMapType();
    }
    public void NewAirports()
    {
        creator.GetAirports();
        StartCoroutine(path.Process());
        creator.ChangeMapType();
    }

}
