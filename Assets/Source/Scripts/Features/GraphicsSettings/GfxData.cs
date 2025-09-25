using System;

[Serializable]
public class GfxData
{
    public float renderScale = 1.0f;
    public int msaa = 0;
    public bool hdr = true;
    public bool depthTex = true;
    public bool opaqueTex = false;

    public int shadowRes = 1024;
    public float shadowDist = 60f;
    public int shadowCasc = 2;

    public int texQ = 0;
    public int aniso = 1;
    public float lodbias = 1.0f;
    public bool softParticles = true;

    public bool postFx = true;
    public bool ssao = false;

    public int display = 0;
    public int screenMode = 0;
    public int width = 1920;
    public int height = 1080;
    public int refreshHz = 60;
}