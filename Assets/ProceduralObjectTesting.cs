using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;

[Tool]
public partial class ProceduralObjectTesting : Node2D
{
[Export] public int Width { get; set; } = 100;
[Export] public int Height { get; set; } = 100;

private Color _color = new Color(1, 0, 0); //default to red

private float[,] _coordsHead =
{
	{ 22.952f, 83.271f },  { 28.385f, 98.623f },
	{ 53.168f, 107.647f }, { 72.998f, 107.647f },
	{ 99.546f, 98.623f },  { 105.048f, 83.271f },
	{ 105.029f, 55.237f }, { 110.740f, 47.082f },
	{ 102.364f, 36.104f }, { 94.050f, 40.940f },
	{ 85.189f, 34.445f },  { 85.963f, 24.194f },
	{ 73.507f, 19.930f },  { 68.883f, 28.936f },
	{ 59.118f, 28.936f },  { 54.494f, 19.930f },
	{ 42.039f, 24.194f },  { 42.814f, 34.445f },
	{ 33.951f, 40.940f },  { 25.637f, 36.104f },
	{ 17.262f, 47.082f },  { 22.973f, 55.237f }
};


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//initial drawing
		_Draw();
		QueueRedraw();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint()) {
			//Update when dimensions change
			QueueRedraw();
		}
	}

	public override void _Draw()
	{
		//Draw square with width/height
		DrawRect(new Rect2(Vector2.Zero, new Vector2(Width, Height)), _color);
	}
}
