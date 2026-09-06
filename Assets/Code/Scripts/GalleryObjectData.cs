using System;
using UnityEngine;

[Serializable]
public class GalleryObjectData
{
    // Stable gameplay identifier.
    // For example:
    // "Mature Tree"
    // "Cardboard"
    // "Composite Panel"
    public string objectId;

    // Which gameplay zone contains the object.
    public ZoneEnum zone;

    // Saved as local transforms so the entry is still useful if objects/decorations later gain positional variation.
    public Vector3 localPosition;
    public Quaternion localRotation;
    public Vector3 localScale;
}