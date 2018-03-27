using UnityEngine;
using UnityEngine.UI;

public class bl_MaskHelper : MonoBehaviour {

    public Texture2D MiniMapMask = null;
    public Texture2D WorldMapMask = null;
    [Space(5)]
    public Image Background = null;
    public Sprite MiniMapBackGround = null;
    public Sprite WorldMapBackGround = null;

    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        m_image.texture = MiniMapMask;
    }


    private RawImage _image = null;
    private RawImage m_image
    {
        get
        {
            if (_image == null)
            {
                _image = this.GetComponent<RawImage>();
            }
            return _image;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="full"></param>
    public void OnChange(bool full = false)
    {
        if (full)
        {
            m_image.texture = WorldMapMask;
            if (Background != null) { Background.sprite = WorldMapBackGround; }
        }
        else
        {
            m_image.texture = MiniMapMask;
            if (Background != null) { Background.sprite = MiniMapBackGround; }
        }
    }
}