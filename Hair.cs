using Godot;
using System;
using System.Collections.Generic;

/*
	Represents a hair sim in a 2D space, providing param
	for length, width, gravity, additional properties for 
	behavior/shape
*/

public partial class Hair : Node2D
{
	[Export] public float Width { get; set; } = 50.0f;
	[Export] public float EndScale { get; set; } = 0.75f;
	[Export] public float Length { get; set; } = 25.0f;
	[Export] public float SittingAngle { get; set; } = 15.0f;
	[Export] public int PointCount { get; set; } = 3;
	[Export] public int Vertices { get; set; } = 16;
	[Export] public float Gravity { get; set; } = 190.0f;
	[Export] public bool IsScaleX { get; set; } = true;
	[Export] public bool IsStiff { get; set; } = false;
	[Export] public int DirX { get; set; } = 1;
	[Export] public float OffsetAngle { get; set; } = 0.0f;

	private List<Polygon2D> gons = new List<Polygon2D>(); // List to hold hair polygons.
	private Vector2 lastPos = Vector2.Zero; // Last known position of the hair.
	private Vector2 hairEnd = Vector2.Zero; // Current end position of the hair.

	//For using GDScript functions
	private static GDScript BenGDScript = GD.Load<GDScript>("res://Scripts/BenGDScript.gd");
	GodotObject myGDScriptNode = (GodotObject)BenGDScript.New();

	public override void _Ready()
	{
		UpdateStructure();

		// Determine stiffness based on the sitting angle.
		if (!IsStiff)
			IsStiff = Mathf.Abs(SittingAngle) > 90.0f;
	}

	private void UpdateStructure()
	{
		// Free existing children nodes.
		foreach (var child in GetChildren())
		{
			child.QueueFree();
		}
		gons.Clear();

		// Create hair particles based on PointCount.
		for (int p = 0; p < PointCount; p++)
		{
			float s = Width * 0.5f * Mathf.Lerp(1.0f, EndScale, p / (float)(PointCount - 1));
			var verticesList = new Vector2[Vertices];
			// Calculate vertices for the current polygon.
			for (int i = 0; i < Vertices; i++)
			{
				verticesList[i] = s * Vector2.Right.Rotated(Mathf.Tau * (i / (float)Vertices));
			}

			var polygon = new Polygon2D
			{
				Polygon = verticesList
			};
			AddChild(polygon);
			gons.Add(polygon);
		}
	}

	public void _Process(float delta)
	{
		// Angle
		float angle =  Deg2Rad(SittingAngle * DirX) + OffsetAngle;
		if (IsStiff)
			angle += GlobalRotation - OffsetAngle;

		// Movement + gravity
		hairEnd = ToLocal(lastPos + (Vector2.Down.Rotated(angle) * Gravity * delta));

		// Keep length
		if (hairEnd.Length() > Length)
			hairEnd = hairEnd.Normalized() * Length;

		lastPos = ToGlobal(hairEnd);

		// Set points
		for (int i = 0; i < gons.Count; i++)
		{
			gons[i].Position = hairEnd.Normalized() * hairEnd.Length() * (i / (float)(gons.Count - 1));
		}
	}

	public void SetPoints(int arg)
	{
		PointCount = Mathf.Max(2, arg);
		UpdateStructure();
	}

	public void SetVertices(int arg)
	{
		Vertices = Mathf.Max(3, arg);
		UpdateStructure();
	}

	public void SetWidth(float arg)
	{
		Width = arg;
		UpdateStructure();
	}

	public void SetEnd(float arg)
	{
		EndScale = arg;
		UpdateStructure();
	}

	public void ScaleX(int arg)
	{
		if (IsScaleX) DirX = arg;
	}

	public void TurnAngle(float arg)
	{
		OffsetAngle = arg;
	}

	private float Deg2Rad(float deg)
	{
		return (float)(deg * (Math.PI / 180));
	}
}
