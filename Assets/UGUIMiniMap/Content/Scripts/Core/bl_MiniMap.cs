using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class bl_MiniMap : MonoBehaviour
{
    [Separator("General Settings")]
    // Target for the minimap.
    public GameObject m_Target;
    [Tooltip("Keycode to toggle map size mode (world and mini map)")]
    public KeyCode ToogleKey = KeyCode.E;
    public Camera MMCamera = null;
    public RenderType m_Type = RenderType.Picture;
    public RenderMode m_Mode = RenderMode.Mode2D;
    [Separator("UI")]
    public Canvas m_Canvas = null;
    public RectTransform MMUIRoot = null;
    public Image PlayerIcon = null;
    [Separator("Height")]
    [Tooltip("How much should we move for each small movement on the mouse wheel?")]
    public int scrollSensitivity = 3;
    [Tooltip("Maximum heights that the camera can reach.")]
    public float maxHeight = 80;
    [Tooltip("Minimum heights that the camera can reach.")]
    public float minHeight = 5;
    //If you can that the player cant Increase or decrease, just put keys as "None".
    public KeyCode IncreaseHeightKey = KeyCode.KeypadPlus;
    //If you can that the player cant Increase or decrease, just put keys as "None".
    public KeyCode DecreaseHeightKey = KeyCode.KeypadMinus;
    //Default height to view from, if you need have a static height, just edit this.
    private float height = 30;
    [Range(1, 15)]
    [Tooltip("Smooth speed to height change.")]
    public float LerpHeight = 8;
    [Separator("Rotation")]
    [Tooltip("Compass rotation for circle maps, rotate icons around pivot.")]
    public bool useCompassRotation = false;
    [Range(25, 500)]
    [Tooltip("Size of Compass rotation diametre.")]
    public float CompassSize = 175f;
    [Tooltip("Should the minimap rotate with the player?")]
    public bool DynamicRotation = true;
    [Tooltip("this work only is dynamic rotation.")]
    public bool SmoothRotation = true;
    [Range(1, 15)]
    public float LerpRotation = 8;
    [Separator("Map Rect")]
    [Tooltip("Position for MiniMap.")]
    public Vector3 MiniMapPosition = Vector2.zero;
    [Tooltip("Rotation for MiniMap. (3D Mode)")]
    public Vector3 MiniMapRotation = Vector3.zero;
    [Tooltip("Size of MiniMap.")]
    public Vector2 MiniMapSize = Vector2.zero;
    [Space(5)]
    [Tooltip("Position for World Map.")]
    public Vector3 FullMapPosition = Vector2.zero;
    [Tooltip("Rotation for World Map.")]
    public Vector3 FullMapRotation = Vector3.zero;
    [Tooltip("Size of World Map.")]
    public Vector2 FullMapSize = Vector2.zero;
    [Space(5)]
    [Tooltip("Smooth Speed for MiniMap World Map transition.")]
    public float LerpTransition = 7;
    [Space(5)]
    [InspectorButton("GetMiniMapSize")]
    public string GetMiniMapRect = "";
    [InspectorButton("GetFullMapSize")]
    public string GetWorldRect = "";
    [Separator("Picture Mode Settings")]
    [Tooltip("Texture for MiniMap renderer, you can take a snaphot from map.")]
    public Texture MapTexture = null;
    public Color TintColor = new Color(1, 1, 1, 0.9f);
    public Color SpecularColor = new Color(1, 1, 1, 0.9f);
    public Color EmessiveColor = new Color(0, 0, 0, 0.9f);
    [Space(3)]
    public GameObject MapPlane = null;
    public RectTransform WorldSpace = null;

    //Global variables
    public static bool isFullScreen = false;
    public static Camera MiniMapCamera = null;
    public static RectTransform MapUIRoot = null;
    public const string MiniMapLayer = "MiniMap";

    //Privates
    private bool DefaultRotationMode = false;
    private Vector3 DeafultMapRot = Vector3.zero;
    private bool DefaultRotationCircle = false;

    const string MMHeightKey = "MinimapCameraHeight";


    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        MiniMapCamera = MMCamera;
        MapUIRoot = MMUIRoot;
        DefaultRotationMode = DynamicRotation;
        DeafultMapRot = m_Transform.eulerAngles;
        DefaultRotationCircle = useCompassRotation;

        if (m_Type == RenderType.Picture) { CreateMapPlane(); }
        if (m_Mode == RenderMode.Mode3D) { ConfigureCamera3D(); }
        //Get Save Height
        height = PlayerPrefs.GetFloat(MMHeightKey, height);
    }
    void Start()
    { 
        ToggleSize();
       
    }
    /// <summary>
    /// Create a Plane with Map Texture
    /// MiniMap Camera will be renderer only this plane.
    /// This is more optimizing that RealTime type.
    /// </summary>
    void CreateMapPlane()
    {
        //Verify is MiniMap Layer Exist in Layer Mask List.
        string layer = LayerMask.LayerToName(10);
        //If not exist.
        if (layer != MiniMapLayer)
        {
            Debug.LogError(MiniMapLayer + " has not defined in Layer Mask list, please added this in place No. 10");
            MMUIRoot.gameObject.SetActive(false);
            this.enabled = false;
        }
        if (MapTexture == null)
        {
            Debug.LogError("Map Texture not been allocated.");
            return;
        }
        //Get Position reference from world space rect.
        Vector3 pos = WorldSpace.localPosition;
        //Get Size reference from world space rect.
        Vector3 size = WorldSpace.sizeDelta;
        //Set to camera culling only MiniMap Layer.
        MMCamera.cullingMask = 1 << 10;
        //Create plane
        GameObject plane = Instantiate(MapPlane) as GameObject;
        //Set position
        plane.transform.localPosition = pos;
        //Set Correct size.
        plane.transform.localScale = (new Vector3(size.x, 10, size.y) / 10);
        //Apply material with map texture.
        plane.GetComponent<Renderer>().material = CreateMaterial();
    }

    /// <summary>
    /// Avoid to UI world space collision with other objects in scene.
    /// </summary>
    void ConfigureCamera3D()
    {
        Camera cam = (Camera.main != null) ? Camera.main : Camera.current;
        if (cam == null)
        {
            Debug.LogError("Not to have found a camera to configure");
            return;
        }
        //Avoid to 3D UI transferred other objects in the scene.
        cam.nearClipPlane = 0.015f;
        m_Canvas.planeDistance = 0.1f;
    }
    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (m_Target == null)
            return;
        if (MMCamera == null)
            return;

        Inputs();
        PositionControll();
        RotationControll();
        MapSize();
    }
    /// <summary>
    /// Mininimap follow the target.
    /// </summary>
    void PositionControll()
    {
        Vector3 p = m_Transform.position;
        // Update the transformation of the camera as per the target's position.
        p.x = Target.position.x;
        p.z = Target.position.z;
        // For this, we add the predefined (but variable, see below) height var.
        p.y = (maxHeight + minHeight / 2) + (Target.position.y * 2);
        //Camera follow the target
        m_Transform.position = p;

    }
    /// <summary>
    /// 
    /// </summary>
    void RotationControll()
    {
        // If the minimap should rotate as the target does, the rotateWithTarget var should be true.
        // An extra catch because rotation with the fullscreen map is a bit weird.
        RectTransform rt = PlayerIcon.GetComponent<RectTransform>();

        if (DynamicRotation)
        {
            //get local reference.
            Vector3 e = m_Transform.eulerAngles;
            e.y = Target.eulerAngles.y;
            if (SmoothRotation)
            {
                if (m_Mode == RenderMode.Mode2D)
                {
                    //For 2D Mode
                    rt.rotation = Quaternion.identity;
                }
                else
                {
                    //For 3D Mode
                    rt.localRotation = Quaternion.identity;
                }

                if (m_Transform.eulerAngles.y != e.y)
                {
                    //calculate the diference 
                    float d = e.y - m_Transform.eulerAngles.y;
                    //avoid lerp from 360 to 0 or reverse
                    if (d > 180 || d < -180)
                    {
                        m_Transform.eulerAngles = e;
                    }
                }
                //Lerp rotation of map
                m_Transform.eulerAngles = Vector3.Lerp(this.transform.eulerAngles, e, Time.deltaTime * LerpRotation);
            }
            else
            {
                m_Transform.eulerAngles = e;
            }
        }
        else
        {
            m_Transform.eulerAngles = DeafultMapRot;
            if (m_Mode == RenderMode.Mode2D)
            {
                //When map rotation is static, only rotate the player icon
                Vector3 e = Vector3.zero;
                //get and fix the correct angle rotation of target
                e.z = -Target.eulerAngles.y;
                rt.eulerAngles = e;
            }
            else
            {
                //Use local rotation in 3D mode.
                Vector3 tr = Target.localEulerAngles;
                Vector3 r = Vector3.zero;
                r.z = -tr.y;
                rt.localEulerAngles = r;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void Inputs()
    {
        // If the minimap button is pressed then toggle the map state.
        if (Input.GetKeyDown(ToogleKey))
        {
            ToggleSize();
        }
        if (Input.GetKeyDown(DecreaseHeightKey) && height < maxHeight)
        {
            ChangeHeight(true);
        }
        if (Input.GetKeyDown(IncreaseHeightKey) && height > minHeight)
        {
            ChangeHeight(false);
        }      
    }

    /// <summary>
    /// Map FullScreen or MiniMap
    /// Lerp all transition for smooth effect.
    /// </summary>
    void MapSize()
    {
        RectTransform rt = MMUIRoot;
        if (isFullScreen)
        {
            if (DynamicRotation) { DynamicRotation = false; ResetMapRotation(); }
            rt.sizeDelta = Vector2.Lerp(rt.sizeDelta, FullMapSize, Time.deltaTime * LerpTransition);
            rt.anchoredPosition = Vector3.Lerp(rt.anchoredPosition, FullMapPosition, Time.deltaTime * LerpTransition);
            rt.localEulerAngles = Vector3.Lerp(rt.localEulerAngles, FullMapRotation, Time.deltaTime * LerpTransition);
        }
        else
        {
            if (DynamicRotation != DefaultRotationMode) { DynamicRotation = DefaultRotationMode; }
            rt.sizeDelta = Vector2.Lerp(rt.sizeDelta, MiniMapSize, Time.deltaTime * LerpTransition);
            rt.anchoredPosition = Vector3.Lerp(rt.anchoredPosition, MiniMapPosition, Time.deltaTime * LerpTransition);
            rt.localEulerAngles = Vector3.Lerp(rt.localEulerAngles, MiniMapRotation, Time.deltaTime * LerpTransition);
        }
        MMCamera.orthographicSize = Mathf.Lerp(MMCamera.orthographicSize, height, Time.deltaTime * LerpHeight);
    }

    /// <summary>
    /// This called one time when press the toggle key
    /// </summary>
    void ToggleSize()
    {
        isFullScreen = !isFullScreen;
        if (isFullScreen)
        {
            Debug.Log("isFullScreen");
            //when change to fullscreen, the height is the max
            height = maxHeight;
            useCompassRotation = false;
            if (m_maskHelper) { m_maskHelper.OnChange(true); }
        }
        else
        {
            //when return of fullscreen, return to current height
            height = PlayerPrefs.GetFloat(MMHeightKey, height);
            if (useCompassRotation != DefaultRotationCircle) { useCompassRotation = DefaultRotationCircle; }
            if (m_maskHelper) { m_maskHelper.OnChange(); }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    public void ChangeHeight(bool b)
    {
        if (b)
        {
            if (height + scrollSensitivity <= maxHeight)
            {
                height += scrollSensitivity;
            }
            else
            {
                height = maxHeight;
            }
        }
        else
        {
            if (height - scrollSensitivity >= minHeight)
            {
                height -= scrollSensitivity;
            }
            else
            {
                height = minHeight;
            }
        }
        PlayerPrefs.SetFloat("MinimapCameraHeight", height);
    }
    /// <summary>
    /// Create Material for Minimap image in plane.
    /// you can edit and add your own shader.
    /// </summary>
    /// <returns></returns>
    public Material CreateMaterial()
    {
        Shader shader = Shader.Find("Legacy Shaders/VertexLit");
        Material mat = new Material(shader);
        mat.mainTexture = MapTexture;
        mat.color = TintColor;
        mat.SetColor("_SpecColor", SpecularColor);
        mat.SetColor("_Emission", EmessiveColor);

        return mat;
    }
    /// <summary>
    /// Reset this transform rotation helper.
    /// </summary>
    void ResetMapRotation() { m_Transform.eulerAngles = new Vector3(90, 0, 0); }
    /// <summary>
    /// Call this fro change the mode of rotation of map
    /// Static or dynamic
    /// </summary>
    /// <param name="mode">static or dinamic</param>
    /// <returns></returns>
    public void RotationMap(bool mode) { if (isFullScreen) return; DynamicRotation = mode; DefaultRotationMode = DynamicRotation; }
    /// <summary>
    /// Change the size of Map fullscreen or mini
    /// </summary>
    /// <param name="fullscreen">is fullscreen?</param>
    public void ChangeMapSize(bool fullscreen) { isFullScreen = fullscreen; }


    //Context Menu
    [ContextMenu("GetMiniMapRect")]
    void GetMiniMapSize()
    {
        MiniMapSize = MMUIRoot.sizeDelta;
        MiniMapPosition = MMUIRoot.anchoredPosition;
        MiniMapRotation = MMUIRoot.eulerAngles;
    }
    [ContextMenu("GetFullMapRect")]
    void GetFullMapSize()
    {
        FullMapSize = MMUIRoot.sizeDelta;
        FullMapPosition = MMUIRoot.anchoredPosition;
        FullMapRotation = MMUIRoot.eulerAngles;
    }

    public Transform Target
    {
        get
        {
            if (m_Target != null)
            {
                return m_Target.GetComponent<Transform>();
            }
            return this.GetComponent<Transform>();
        }
    }
    public Vector3 TargetPosition
    {
        get
        {
            Vector3 v = Vector3.zero;
            if (m_Target != null)
            {
                v = m_Target.transform.position;
            }
            return v;
        }
    }
    //Get Transform
    private Transform t;
    private Transform m_Transform
    {
        get
        {
            if (t == null)
            {
                t = this.GetComponent<Transform>();
            }
            return t;
        }
    }
    //Get Mask Helper (if exist one)for managament of texture mask
    private bl_MaskHelper _maskHelper = null;
    private bl_MaskHelper m_maskHelper
    {
        get
        {
            if (_maskHelper == null)
            {
                _maskHelper = this.transform.root.GetComponentInChildren<bl_MaskHelper>();
            }
            return _maskHelper;
        }
    }

    [System.Serializable]
    public enum RenderType
    {
        RealTime,
        Picture,
    }

    [System.Serializable]
    public enum RenderMode
    {
        Mode2D,
        Mode3D,
    }
}