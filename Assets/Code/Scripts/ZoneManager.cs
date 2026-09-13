using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public Zone[] zones;
    public static ZoneManager instance = null;
    public IslandDecorate decorationArea;
    private AudioHandle creationSoundHandle;
    void Awake()
    {

        if (instance == null)

            instance = this;

        else if (instance != this)

            Destroy(gameObject);


    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HighlightObject(Material? material = null, Action? action = null)
    {
        foreach (Zone zone in zones)
        {
            zone.HighlightObject(material, action);
        }
    }

    public void UnHighlightObject()
    {
        foreach (Zone zone in zones)
        {
            zone.UnHighlightObject();
        }
    }

    internal void PlaceItemOnIsland(SpriteScript currentMapObjectSprite)
    {
        decorationArea.PlaceItemOnIsland(currentMapObjectSprite);
    }

    public void PlayCreationSound(MapObject mapObject)
    {
        StopSound();
        creationSoundHandle = AudioManager.instance.PlaySound(mapObject.creationSound, transform.position);
    }
    
    public void StopSound()
    {
        creationSoundHandle?.Stop();
        creationSoundHandle = null;

    }
}
