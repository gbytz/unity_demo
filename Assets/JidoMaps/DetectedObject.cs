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
	public float CenterX;
	public float CenterY;
	public float CenterZ;
	public float Confidence;
	public float Height;

	public DetectedObject (string name, float centerX, float centerY, float centerZ, float confidence, float height)
	{
		this.Name = name;
		this.CenterX = centerX;
		this.CenterY = centerY;
		this.CenterZ = centerZ;
		this.Confidence = confidence;
		this.Height = height;
	}
}
