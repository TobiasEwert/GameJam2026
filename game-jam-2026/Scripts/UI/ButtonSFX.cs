using Godot;
using System;

public partial class ButtonSFX : Button
{
	[Export] public AudioStream hoverSound;
	[Export] public AudioStream pressedSound;
	[Export] public AudioStreamPlayer2D audioPlayer;
	public override void _Ready()
	{
		MouseEntered += OnButtonHover;
		Pressed += OnButtonPressed;
	}
	
	private void OnButtonHover()
	{
		if (hoverSound != null)
		{
			audioPlayer.Stream = hoverSound;
			audioPlayer.Play();
		}
	}
	private void OnButtonPressed()
	{
		if (pressedSound != null)
		{
			audioPlayer.Stream = pressedSound;
			audioPlayer.Play();
		}
	}

}
