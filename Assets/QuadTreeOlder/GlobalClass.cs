using UnityEngine;

public class GlobalClass
{
    public int cntQuads;
    public int cntResults;
    public int cntSearch;
    public bool ynShowQuads;
    public GameObject parentQuads;
    public GameObject parentBalls;
    public bool ynUseQuadTree;
    public QTQuadClass quadTree;
    public int capacity;
    public Vector2 centerQuadTree;
    public Vector2 scaleQuadTree;
    public float elapsedTimeMS;
    public BallStruct[] balls;
    public Vector3 target;
    public GameObject targetGo;
    public float radTarget;
    public bool ynFollowTarget;
    public int cntFrames;
    public bool ynAvoid;
    public bool ynATC;
    public int deepestLevel;
    public float deepestSize;
    public bool ynStep;
    public bool ynRandom;
    public float closestDist;
    public bool ynTrails;

    public void MakeMaterialTransparent(Material material)
    {
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
}
