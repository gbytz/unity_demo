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
	public float Height;

	public DetectedObject (string name, float centerX, float centerY, float centerZ, float confidence, float height)
	{
		this.Name = name;
		this.X = centerX;
		this.Y = centerY;
		this.Z = centerZ;
		this.Confidence = confidence;
		this.Height = height;
	}

	public override string ToString ()
	{
		return string.Format ("{0}, {1}, {2}, {3}", this.Name, this.X, this.Y, this.Z);
	}
}
