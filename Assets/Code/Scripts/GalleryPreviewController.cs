using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GalleryPreviewController : MonoBehaviour
{
    [Header("Display Points")]
    [SerializeField]
    private List<GalleryDisplayPoint> displayPoints = new();

    [Header("Visual Prefab")]
    [SerializeField]
    private SpriteScript mapObjectPrefab;

    [Header("UI")]
    [SerializeField]
    private TMP_Text entryCounterText;

    private readonly List<GameObject> spawnedVisuals =
        new List<GameObject>();

    private int currentEntryIndex = 0;


    private void Start()
    {
        ShowCurrentEntry();
    }


    // =========================================================
    // NAVIGATION
    // =========================================================

    public void ShowPrevious()
    {
        int count =
            GalleryManager.Instance.GalleryEntryCount;

        if (count == 0)
        {
            return;
        }

        currentEntryIndex--;

        if (currentEntryIndex < 0)
        {
            currentEntryIndex = count - 1;
        }

        ShowCurrentEntry();
    }


    public void ShowNext()
    {
        int count =
            GalleryManager.Instance.GalleryEntryCount;

        if (count == 0)
        {
            return;
        }

        currentEntryIndex++;

        if (currentEntryIndex >= count)
        {
            currentEntryIndex = 0;
        }

        ShowCurrentEntry();
    }


    // =========================================================
    // DISPLAY
    // =========================================================

    private void ShowCurrentEntry()
    {
        ClearPreview();


        if (GalleryManager.Instance == null)
        {
            Debug.LogError(
                "[GALLERY] GalleryManager does not exist in Gallery scene."
            );

            return;
        }


        int count =
            GalleryManager.Instance.GalleryEntryCount;


        if (count == 0)
        {
            if (entryCounterText != null)
            {
                entryCounterText.text =
                    "No islands saved";
            }

            return;
        }


        GalleryIslandData islandData =
            GalleryManager.Instance.GetGalleryEntry(
                currentEntryIndex
            );


        if (islandData == null)
        {
            return;
        }


        foreach (
            GalleryObjectData objectData
            in islandData.objects)
        {
            CreateObjectVisual(objectData);
        }


        if (entryCounterText != null)
        {
            entryCounterText.text =
                $"Island {currentEntryIndex + 1} / {count}";
        }


        Debug.Log(
            $"[GALLERY] Displaying island " +
            $"{currentEntryIndex + 1} / {count} | " +
            $"ID: {islandData.islandId}"
        );
    }


    private void CreateObjectVisual(
        GalleryObjectData objectData)
    {
        GalleryDisplayPoint displayPoint =
            displayPoints.Find(
                point =>
                    point.zone == objectData.zone
            );


        if (displayPoint == null ||
            displayPoint.displayTransform == null)
        {
            Debug.LogWarning(
                $"[GALLERY] No display point for zone: " +
                $"{objectData.zone}"
            );

            return;
        }


        if (MapObjectDatabase.instance == null)
        {
            Debug.LogError(
                "[GALLERY] MapObjectDatabase does not exist."
            );

            return;
        }


        if (!MapObjectDatabase.instance
            .MapObjectDictionary
            .TryGetValue(
                objectData.objectId,
                out MapObject mapObject))
        {
            Debug.LogWarning(
                $"[GALLERY] Could not find object: " +
                $"{objectData.objectId}"
            );

            return;
        }


        SpriteScript visual =
            Instantiate(
                mapObjectPrefab,
                displayPoint.displayTransform
            );


        visual.image.sprite =
            mapObject.image;


        visual.transform.localPosition =
            objectData.localPosition;

        visual.transform.localRotation =
            objectData.localRotation;

        visual.transform.localScale =
            objectData.localScale;


        // Gallery objects are read-only.
        if (visual.popup != null)
        {
            visual.popup.Disable();
        }


        spawnedVisuals.Add(
            visual.gameObject
        );
    }


    private void ClearPreview()
    {
        foreach (
            GameObject visual
            in spawnedVisuals)
        {
            if (visual != null)
            {
                Destroy(visual);
            }
        }

        spawnedVisuals.Clear();
    }
}