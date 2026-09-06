using System;
using System.Collections.Generic;

[Serializable]
public class GalleryIslandData
{
    // Unique identifier for this Gallery submission.
    public string islandId;

    // Useful later when displaying/sorting Gallery entries.
    public string createdAtUtc;

    // Everything visually placed on the island.
    public List<GalleryObjectData> objects = new();
}