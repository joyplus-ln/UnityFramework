using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GlassImage : Image
{
    private Material materialInstance;

    [Header("Setting")]
    [Range(0, 10)] [SerializeField] private float blurSize = 3;
    [Range(0, 2)] [SerializeField] private float vibrancy = 0.2f;
    [Range(0, 1)] [SerializeField] private float hui = 0.2f;

    public float BlurSize
    {
        get
        {
            return blurSize;
        }

        set
        {
            blurSize = value;
        }
    }

    public float Vibrancy
    {
        get
        {
            return vibrancy;
        }

        set
        {
            vibrancy = value;
        }
    }

    public float Hui
    {
        get
        {
            return hui;
        }

        set
        {
            hui = value;
        }
    }

    override protected void OnEnable()
    {
        base.OnEnable();
        this.Init();
    }


    void Init()
    {
        this.sprite = EmptySprite;
        if (materialInstance == null)
        {
            materialInstance = new Material(Shader.Find("UI/Blur/UIBlur"));
        }
        this.material = materialInstance;
    }
    public void Update()
    {
        this.UpdateMaterial();
    }

    public override Material GetModifiedMaterial(Material baseMaterial)
    {

        Material m = baseMaterial;

        m.SetFloat("_BlurSize", BlurSize);
        m.SetFloat("_Vibrancy", Vibrancy);
        m.SetFloat("_Hui", Hui);

        return base.GetModifiedMaterial(m);
    }

    public static Sprite emptySprite;

    public Sprite EmptySprite
    {
        get
        {
            if (emptySprite == null)
            {
                emptySprite = OnePixelWhiteSprite();
            }
            return emptySprite;
        }
    }

    Sprite OnePixelWhiteSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), Vector2.zero);
    }
}