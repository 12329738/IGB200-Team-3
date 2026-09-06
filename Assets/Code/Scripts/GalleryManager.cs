using System;
using System.Collections.Generic;
using UnityEngine;

public class GalleryManager : MonoBehaviour
{
    public static GalleryManager Instance { get; private set; }

    public GalleryIslandData LastCapturedIsland =>
        lastCapturedIsland;

    public int GalleryEntryCount =>
        galleryEntries.Count;


    // =========================================================
    // SESSION DATA
    // =========================================================

    private static readonly List<GalleryIslandData> galleryEntries =
        new List<GalleryIslandData>();

    private static GalleryIslandData lastCapturedIsland;

    private static int testPreviewIndex = -1;


    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetGallerySession()
    {
        galleryEntries.Clear();

        lastCapturedIsland = null;

        testPreviewIndex = -1;

        Instance = null;
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    


    // =========================================================
    // CAPTURE
    // =========================================================

    public GalleryIslandData CaptureCurrentIsland()
    {
        GalleryIslandData islandData =
            new GalleryIslandData
            {
                islandId =
                    Guid.NewGuid().ToString("N"),

                createdAtUtc =
                    DateTime.UtcNow.ToString("O")
            };


        if (ZoneManager.instance == null)
        {
            Debug.LogError(
                "[GALLERY] ZoneManager does not exist."
            );

            return islandData;
        }


        if (ZoneManager.instance.zones == null)
        {
            Debug.LogError(
                "[GALLERY] ZoneManager has no zones assigned."
            );

            return islandData;
        }


        foreach (Zone zone in ZoneManager.instance.zones)
        {
            if (zone == null)
            {
                continue;
            }


            // Empty zones don't need to be stored.
            if (zone.currentObject == null)
            {
                continue;
            }


            GalleryObjectData objectData =
                new GalleryObjectData
                {
                    objectId =
                        zone.currentObject.Name,

                    zone =
                        zone.zone
                };


            if (zone.currentMapObjectSprite != null)
            {
                Transform objectTransform =
                    zone.currentMapObjectSprite.transform;

                objectData.localPosition =
                    objectTransform.localPosition;

                objectData.localRotation =
                    objectTransform.localRotation;

                objectData.localScale =
                    objectTransform.localScale;
            }
            else
            {
                objectData.localPosition =
                    Vector3.zero;

                objectData.localRotation =
                    Quaternion.identity;

                objectData.localScale =
                    Vector3.one;
            }


            islandData.objects.Add(objectData);
        }


        lastCapturedIsland = islandData;

        PrintSnapshot(islandData);

        return islandData;
    }


    // Temporary Unity Button wrapper.
    public void CaptureCurrentIslandFromButton()
    {
        CaptureCurrentIsland();
    }


    // =========================================================
    // SAVE TO GALLERY
    // =========================================================

    public void SaveCurrentIslandToGallery()
    {
        GalleryIslandData islandData =
            CaptureCurrentIsland();


        if (islandData.objects.Count == 0)
        {
            Debug.LogWarning(
                "[GALLERY] The island is empty. " +
                "Nothing was added to the Gallery."
            );

            return;
        }


        galleryEntries.Add(islandData);

        testPreviewIndex =
            galleryEntries.Count - 1;


        Debug.Log(
            $"[GALLERY] Island added to Gallery. " +
            $"Entry #{galleryEntries.Count} | " +
            $"ID: {islandData.islandId}"
        );
    }


    // =========================================================
    // ACCESS SAVED ENTRIES
    // =========================================================

    public GalleryIslandData GetGalleryEntry(int index)
    {
        if (index < 0 ||
            index >= galleryEntries.Count)
        {
            Debug.LogWarning(
                $"[GALLERY] Invalid Gallery index: {index}"
            );

            return null;
        }


        return galleryEntries[index];
    }


    // =========================================================
    // TEMPORARY RESTORE TESTING
    // =========================================================

    public void RestoreLastCapturedIslandForTest()
    {
        if (LastCapturedIsland == null)
        {
            Debug.LogWarning(
                "[GALLERY] No island has been captured yet."
            );

            return;
        }


        RestoreIslandForTest(
            LastCapturedIsland
        );
    }


    public void RestorePreviousGalleryEntryForTest()
    {
        if (galleryEntries.Count == 0)
        {
            Debug.LogWarning(
                "[GALLERY] There are no Gallery entries."
            );

            return;
        }


        testPreviewIndex--;

        if (testPreviewIndex < 0)
        {
            testPreviewIndex =
                galleryEntries.Count - 1;
        }


        RestoreIslandForTest(
            galleryEntries[testPreviewIndex]
        );


        Debug.Log(
            $"[GALLERY] Viewing entry " +
            $"{testPreviewIndex + 1} / " +
            $"{galleryEntries.Count}"
        );
    }


    public void RestoreNextGalleryEntryForTest()
    {
        if (galleryEntries.Count == 0)
        {
            Debug.LogWarning(
                "[GALLERY] There are no Gallery entries."
            );

            return;
        }


        testPreviewIndex++;

        if (testPreviewIndex >= galleryEntries.Count)
        {
            testPreviewIndex = 0;
        }


        RestoreIslandForTest(
            galleryEntries[testPreviewIndex]
        );


        Debug.Log(
            $"[GALLERY] Viewing entry " +
            $"{testPreviewIndex + 1} / " +
            $"{galleryEntries.Count}"
        );
    }


    private void RestoreIslandForTest(
        GalleryIslandData islandData)
    {
        if (islandData == null)
        {
            return;
        }


        if (ZoneManager.instance == null)
        {
            Debug.LogError(
                "[GALLERY] ZoneManager does not exist."
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


        // -----------------------------------------------------
        // Clear current island
        // -----------------------------------------------------

        foreach (Zone zone in ZoneManager.instance.zones)
        {
            if (zone == null)
            {
                continue;
            }


            if (zone.currentMapObjectSprite != null)
            {
                Destroy(
                    zone.currentMapObjectSprite.gameObject
                );

                zone.currentMapObjectSprite = null;
            }


            zone.currentObject = null;
        }


        // -----------------------------------------------------
        // Reconstruct requested island
        // -----------------------------------------------------

        foreach (
            GalleryObjectData objectData
            in islandData.objects)
        {
            Zone targetZone = null;


            foreach (
                Zone zone
                in ZoneManager.instance.zones)
            {
                if (zone != null &&
                    zone.zone == objectData.zone)
                {
                    targetZone = zone;
                    break;
                }
            }


            if (targetZone == null)
            {
                Debug.LogWarning(
                    $"[GALLERY] Could not find zone: " +
                    $"{objectData.zone}"
                );

                continue;
            }


            if (!MapObjectDatabase.instance
                .MapObjectDictionary
                .TryGetValue(
                    objectData.objectId,
                    out MapObject mapObject))
            {
                Debug.LogWarning(
                    $"[GALLERY] Could not find MapObject: " +
                    $"{objectData.objectId}"
                );

                continue;
            }


            SpriteScript visual =
                Instantiate(
                    targetZone.mapObjectPrefab,
                    targetZone.transform
                );


            visual.image.sprite =
                mapObject.image;


            visual.transform.localPosition =
                objectData.localPosition;

            visual.transform.localRotation =
                objectData.localRotation;

            visual.transform.localScale =
                objectData.localScale;


            targetZone.currentObject =
                mapObject;

            targetZone.currentMapObjectSprite =
                visual;


            if (visual.popup != null)
            {
                visual.popup.Disable();
            }


            Debug.Log(
                $"[GALLERY] Restored " +
                $"{objectData.objectId} " +
                $"to {objectData.zone}"
            );
        }


        Debug.Log(
            $"[GALLERY] Island restored: " +
            $"{islandData.islandId}"
        );
    }


    // =========================================================
    // DEBUG
    // =========================================================

    private void PrintSnapshot(
        GalleryIslandData islandData)
    {
        Debug.Log(
            $"[GALLERY] Captured Island\n" +
            $"ID: {islandData.islandId}\n" +
            $"Created: {islandData.createdAtUtc}\n" +
            $"Objects: {islandData.objects.Count}"
        );


        foreach (
            GalleryObjectData objectData
            in islandData.objects)
        {
            Debug.Log(
                $"[GALLERY] " +
                $"{objectData.zone} -> " +
                $"{objectData.objectId} | " +
                $"Position: " +
                $"{objectData.localPosition}"
            );
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}