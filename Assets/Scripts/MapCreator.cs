using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapCreator : MonoBehaviour
{
    public float windAngleSizeMultiplier;
    public int width;
    public int height;
    public int seed;
    public Material material;
    public float noiseFactor;
    public int numAirports;
    public int textureDetail;
    public List<Vector2Int> airports;
    void Start()
    {
        seed = UnityEngine.Random.Range(0, 10000);
        transform.localScale = new Vector3(width,height,0);
        transform.localPosition = new Vector3(width/2f,height/2f,0);
        MakeMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public float landThreshold = .7f;
    public void MakeMap()
    {
        Texture2D texture = new Texture2D(width, height);
        for(int x= 0 ; x < width; x++ )
        {
            for(int y =0;y< height; y++)
            {
                Color color = GetNoise(x,y)>landThreshold?Color.green:Color.blue;
                texture.SetPixel(x,y,color);
            }
        }
        texture.Apply();
        material.mainTexture = texture;
        GetAirports();
    }
    float GetNoise(int x,int y)
    {
        float val = ((Unity.Mathematics.noise.cnoise(new Unity.Mathematics.float3( x/(float)width /noiseFactor,y/(float)height /noiseFactor,seed)))+1)/2;
        return val;
    }
    public void GetAirports()
    {
        airports.Clear();
        int cur = 0;
        while (cur < numAirports)
        {
            int x = UnityEngine.Random.Range(0, width);
            int y = UnityEngine.Random.Range(0, height);
            if (GetNoise(x, y) > landThreshold && !airports.Contains(new Vector2Int(x, y))&&x>5&&x<95&&y>5&&y<95)
            {
                airports.Add(new Vector2Int(x, y));
                cur++;
            }
        }
        airport1.transform.position = new Vector3( airports[0].x, airports[0].y,0);
        airport2.transform.position = new Vector3(airports[1].x, airports[1].y, 0);
        airport3.transform.position = new Vector3(airports[2].x, airports[2].y, 0);
    }
    public GameObject airport1;
    public GameObject airport2;
    public GameObject airport3;


    /*    
     *    MAP TEXTURES
     */
    public void SetMapTexture()
    {
        DeleteArrows();
        Texture2D texture = new Texture2D(width , height );
        for (int x = 0; x < width ; x++)
        {
            for (int y = 0; y < height ; y++)
            {
                Color color = GetNoise(x, y) > landThreshold ? Color.green : Color.blue;
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        material.mainTexture = texture;
    }
    public void SetWeatherTexture()
    {
        DeleteArrows();
        Texture2D tex = new Texture2D(width , height );
        for (int i = 0; i < width ; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float val = WeatherField.GetWeatherAtPoint(i  / noiseFactor, j  / noiseFactor, width, height, 0);
                tex.SetPixel(i, j, new Color(val, 0, -val + 1));
            }
        }
        tex.Apply();
        material.mainTexture = tex;
    }
    public void SetWindTexture()
    {
        DeleteArrows();
        Texture2D tex = new Texture2D(width , height );
        for (int i = 0; i < width ; i++)
        {
            for (int j = 0; j < height ; j++)
            {
                float val = WindField.GetWindAtPoint(i  / noiseFactor, j  / noiseFactor, width, height, 0);
                tex.SetPixel(i, j, new Color(val, 0, -val + 1));
            }
        }
        tex.Apply();
        material.mainTexture = tex;
        ShowArrows();
    }
    public int spacing;
    public GameObject arrowPrefab;
    List<GameObject> arrows = new List<GameObject>();
    public Transform arrowParent;
    public void ShowArrows()
    {
        DeleteArrows();
        for (int x = 0; x < width; x += spacing)
        {
            for (int y = 0; y < height; y += spacing)
            {
                Vector2 position = new Vector2(x+(spacing/2f), y+(spacing/2f));

                float angle = WindField.GetWindAngleAtPoint(position.x/noiseFactor*windAngleSizeMultiplier, position.y/noiseFactor*windAngleSizeMultiplier, width, height, 0);

                GameObject arrow = Instantiate(
                    arrowPrefab,
                    position,
                    Quaternion.identity,
                    arrowParent
                );

                arrow.transform.rotation =
                    Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
                arrow.transform.localScale = new Vector3(.3f,.3f,1.0f);
                arrows.Add(arrow);
            }
        }
    }
    void DeleteArrows()
    {
        for (int i = 0; i < arrows.Count; i++)
        {
            GameObject.Destroy(arrows[i]);
        }
        arrows.Clear();
    }
    public TMP_Text dropdown;
    public void ChangeMapType()
    {
        switch (dropdown.text)
        {
            case "Show World Map":
                SetMapTexture(); break;
            case "Show Weather Map": SetWeatherTexture(); break;
            case "Show Wind Map": SetWindTexture(); break;
        }
    }
}
