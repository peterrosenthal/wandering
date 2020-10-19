using Godot;
using System;
using System.Drawing.Imaging;

public class ProceduralTerrain : StaticBody
{
    [Export] public   int Seed = 0;
    [Export] public float Amplitude = 20.0f;
    [Export] public   int Octaves = 2;
    [Export] public float Period = 10.0f;
    [Export] public float Lacunarity = 2.0f;
    [Export] public float Persistence = 0.5f;
    
    private int _resX;
    private int _resZ;
    private float[] _data;

    private OpenSimplexNoise _noise;
    private SurfaceTool _surfaceTool;
    private MeshDataTool _meshDataTool;
    
    private MeshInstance _meshInstance;

    private bool _initialized = false;

    public override void _Ready()
    {
        Initialize();
        Generate();
    }

    private void Initialize()
    {
        _meshInstance = GetNode<MeshInstance>("Mesh");
        PlaneMesh initialMesh = (PlaneMesh) _meshInstance.Mesh;
        _resX = initialMesh.SubdivideWidth + 1;
        _resZ = initialMesh.SubdivideDepth + 1;
        
        _noise = new OpenSimplexNoise();
        UpdateNoiseSettings();

        _surfaceTool = new SurfaceTool();
        _meshDataTool = new MeshDataTool();

        Control HUD = GetNode<Control>("../HUD");
        HUD.Connect("UpdateSeed", this, nameof(_On_Seed_Updated));
        HUD.Connect("UpdateOctaves", this, nameof(_On_Octaves_Updated));
        HUD.Connect("UpdateAmplitude", this, nameof(_On_Amplitude_Updated));
        HUD.Connect("UpdatePeriod", this, nameof(_On_Period_Updated));
        HUD.Connect("UpdateLacunarity", this, nameof(_On_Lacunarity_Updated));
        HUD.Connect("UpdatePersistence", this, nameof(_On_Persistence_Updated));

        _initialized = true;
    }

    private void UpdateNoiseSettings()
    {
        _noise.Seed = Seed;
        _noise.Octaves = Octaves;
        _noise.Period = Period;
        _noise.Lacunarity = Lacunarity;
        _noise.Persistence = Persistence;
    }

    private void Generate()
    {
        if (!_initialized) return;

        _surfaceTool.Clear();
        _meshDataTool.Clear();
        _surfaceTool.CreateFrom(_meshInstance.Mesh, 0);
        _meshDataTool.CreateFromSurface(_surfaceTool.Commit(), 0);

        int numVertices = _meshDataTool.GetVertexCount();
        
        _data = new float[numVertices];
        
        for (int i = 0; i < numVertices; i++)
        {
            Vector3 vertex = _meshDataTool.GetVertex(i);
            _data[i] = Amplitude * _noise.GetNoise2d(vertex.x, vertex.z);
            _meshDataTool.SetVertex(i, new Vector3(vertex.x, _data[i], vertex.z));
            _meshDataTool.SetVertexNormal(i, Vector3.Up);
        }

        for (int i = 0; i < numVertices - _resX; i++)
        {
            Vector3 a = _meshDataTool.GetVertex(i);
            Vector3 b = _meshDataTool.GetVertex(i + 1);
            Vector3 c = _meshDataTool.GetVertex(i + _resX);

            Vector3 normal = (c - a).Cross(b - a);

            for (int j = i; j < i + 3; j++)
            {
                _meshDataTool.SetVertexNormal(j, _meshDataTool.GetVertexNormal(j) + normal);
            }
        }

        for (int i = 0; i < numVertices; i++)
        {
            _meshDataTool.SetVertexNormal(i, _meshDataTool.GetVertexNormal(i).Normalized());
        }

        ArrayMesh newMesh = new ArrayMesh();
        _meshDataTool.CommitToSurface(newMesh);
        _meshInstance.Mesh = newMesh;
        _meshInstance.RemoveChild(_meshInstance.GetChild(0));
        _meshInstance.CreateTrimeshCollision();
    }

    private void _On_Seed_Updated(int seed)
    {
        Seed = seed;
        UpdateNoiseSettings();
        Generate();
    }

    private void _On_Octaves_Updated(int octaves)
    {
        Octaves = octaves;
        UpdateNoiseSettings();
        Generate();
    }

    private void _On_Amplitude_Updated(float amplitude)
    {
        Amplitude = amplitude;
        UpdateNoiseSettings();
        Generate();
    }

    private void _On_Period_Updated(float period)
    {
        Period = period;
        UpdateNoiseSettings();
        Generate();
    }

    private void _On_Lacunarity_Updated(float lacunarity)
    {
        Lacunarity = lacunarity;
        UpdateNoiseSettings();
        Generate();
    }

    private void _On_Persistence_Updated(float persistence)
    {
        Persistence = persistence;
        UpdateNoiseSettings();
        Generate();
    }
    
}
