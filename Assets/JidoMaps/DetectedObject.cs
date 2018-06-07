using System;
using System.Collections.Generic;

[Serializable]
public class DetectedObjects {
	public List<DetectedObject> Objects;
}

[Serializable]
public class DetectedObject
{
	public string Name;
	public float X;
	public float Y;
	public float Z;
	public float Confidence;
	public float Width;
	public float Height;
	public float Depth;
	public float Orientation;
	public int SeenCount;
	public int Id;

	public DetectedObject (string name, float centerX, float centerY, float centerZ, float confidence, float width, float height, float depth, float orientation, int seenCount, int id)
	{
		this.Name = name;
		this.X = centerX;
		this.Y = centerY;
		this.Z = centerZ;
		this.Confidence = confidence;
		this.Width = width;
		this.Height = height;
		this.Depth = depth;
		this.Orientation = orientation;
		this.SeenCount = seenCount;
		this.Id = id;
	}

	public override string ToString ()
	{
		return string.Format ("{0}, {1}, {2}, {3}", this.Name, this.X, this.Y, this.Z);
	}
}
