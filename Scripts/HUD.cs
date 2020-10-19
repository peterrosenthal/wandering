using Godot;
using System;

public class HUD : Control
{
    private Button _startButton;
    private LineEdit _seedInput;
    private LineEdit _octavesInput;
    private LineEdit _amplitudeInput;
    private LineEdit _periodInput;
    private LineEdit _lacunarityInput;
    private LineEdit _persistenceInput;

    [Signal] public delegate void StartGame();

    [Signal] public delegate void UpdateSeed(int seed);

    [Signal] public delegate void UpdateOctaves(int octaves);

    [Signal] public delegate void UpdateAmplitude(float amplitude);

    [Signal] public delegate void UpdatePeriod(float period);

    [Signal] public delegate void UpdateLacunarity(float lacunarity);

    [Signal] public delegate void UpdatePersistence(float persistence);
    
    public override void _Ready()
    {
        _startButton = GetNode<Button>("StartButton");
        _startButton.Connect("pressed", this, nameof(_On_Start_Button_Pressed));

        _seedInput = GetNode<LineEdit>("SeedInput/LineEdit");
        _seedInput.Connect("text_entered", this, nameof(_On_Seed_Changed));

        _octavesInput = GetNode<LineEdit>("OctavesInput/LineEdit");
        _octavesInput.Connect("text_entered", this, nameof(_On_Octaves_Changed));

        _amplitudeInput = GetNode<LineEdit>("AmplitudeInput/LineEdit");
        _amplitudeInput.Connect("text_entered", this, nameof(_On_Amplitude_Changed));

        _periodInput = GetNode<LineEdit>("PeriodInput/LineEdit");
        _periodInput.Connect("text_entered", this, nameof(_On_Period_Changed));

        _lacunarityInput = GetNode<LineEdit>("LacunarityInput/LineEdit");
        _lacunarityInput.Connect("text_entered", this, nameof(_On_Lacunarity_Changed));

        _persistenceInput = GetNode<LineEdit>("PersistenceInput/LineEdit");
        _persistenceInput.Connect("text_entered", this, nameof(_On_Persistence_Changed));
    }

    private void _On_Start_Button_Pressed()
    {
        _startButton.Disabled = true;
        _startButton.Visible = false;

        _seedInput.GetParent<Panel>().Visible = false;
        _octavesInput.GetParent<Panel>().Visible = false;
        _amplitudeInput.GetParent<Panel>().Visible = false;
        _periodInput.GetParent<Panel>().Visible = false;
        _lacunarityInput.GetParent<Panel>().Visible = false;
        _persistenceInput.GetParent<Panel>().Visible = false;
        
        EmitSignal(nameof(StartGame));
    }

    private void _On_Seed_Changed(string text)
    {
        int seed = text.ToInt();
        EmitSignal(nameof(UpdateSeed), seed);
    }

    private void _On_Octaves_Changed(string text)
    {
        int octaves = text.ToInt();
        EmitSignal(nameof(UpdateOctaves), octaves);
    }

    private void _On_Amplitude_Changed(string text)
    {
        float amplitude = text.ToFloat();
        EmitSignal(nameof(UpdateAmplitude), amplitude);
    }

    private void _On_Period_Changed(string text)
    {
        float period = text.ToFloat();
        EmitSignal(nameof(UpdatePeriod), period);
    }

    private void _On_Lacunarity_Changed(string text)
    {
        float lacunarity = text.ToFloat();
        EmitSignal(nameof(UpdateLacunarity), lacunarity);
    }

    private void _On_Persistence_Changed(string text)
    {
        float persistence = text.ToFloat();
        EmitSignal(nameof(UpdatePersistence), persistence);
    }
}
