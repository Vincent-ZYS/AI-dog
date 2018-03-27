using UnityEngine;
using System.Collections;
#pragma warning disable 0618
public class bl_MMExampleManager : MonoBehaviour {

    public GameObject ExampleGo = null;
    public int MapID = 2;
    public const string MMName = "MMManagerExample";

    public GameObject[] Maps;

    void Awake()
    {
        MapID = PlayerPrefs.GetInt("MMExampleMapID", 0);
        ApplyMap();
    }

    void ApplyMap()
    {
        Maps[MapID].SetActive(true);
    }


    public void ChangeMap(int i)
    {
        PlayerPrefs.SetInt("MMExampleMapID",i);
        Application.LoadLevel(Application.loadedLevel);
    }
}