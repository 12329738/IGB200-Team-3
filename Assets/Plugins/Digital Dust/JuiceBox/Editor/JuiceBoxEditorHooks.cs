using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

// ==============================================================================
//  JuiceBoxEditorHooks: Re-subscribes to static editor events after domain reload.
// ==============================================================================
namespace JuiceBox
{
    [InitializeOnLoad]
    internal static class JuiceBoxEditorHooks
    {
        [System.ThreadStatic]
        private static bool _writingObjectId;
#if UNITY_6000_4_OR_NEWER
        private static readonly System.Collections.Generic.Dictionary<EntityId, string> _writeIdCache = new System.Collections.Generic.Dictionary<EntityId, string>();
#else
        private static readonly System.Collections.Generic.Dictionary<int, string> _writeIdCache = new System.Collections.Generic.Dictionary<int, string>();
#endif
        private static readonly System.Collections.Generic.Dictionary<string, Object> _resolveIdCache = new System.Collections.Generic.Dictionary<string, Object>();

        static JuiceBoxEditorHooks()
        {
            Processor.WriteObjectIdFunc = WriteObjectId;
            Processor.ResolveObjectIdFunc = ResolveObjectId;
            Processor.ResolveTypeFunc = ResolveTypeTolerant;

            SequenceBackupManager.OnSnapshotWritten -= OnSnapshotWritten;
            SequenceBackupManager.OnSnapshotWritten += OnSnapshotWritten;

            SequenceLibrary.OnSequenceChanged -= OnSequenceChanged;
            SequenceLibrary.OnSequenceChanged += OnSequenceChanged;
        }

#if UNITY_6000_8_OR_NEWER
        [Unity.Scripting.LifecycleManagement.BeforeCodeUnloading]
        private static void UninstallObjectIdFuncs()
        {
            Processor.WriteObjectIdFunc = null;
            Processor.ResolveObjectIdFunc = null;
            Processor.ResolveTypeFunc = null;
        }
#endif

        // -- Tolerant type resolution -----------------------------------------

        private static readonly System.Collections.Generic.HashSet<string> _typeResolveLogged =
            new System.Collections.Generic.HashSet<string>();

        private static System.Type ResolveTypeTolerant(string aqn)
        {
            var assemblies = JuiceBoxSettings.GetLoadedAssemblies();

            for (int i = 0; i < assemblies.Count; i++)
            {
                System.Type t = assemblies[i].GetType(aqn);
                if (t == null) continue;
                if (_typeResolveLogged.Add(aqn))
                    Debug.LogWarning("JuiceBox: recovered type " + aqn + " by assembly scan (found in "
                        + t.Assembly.GetName().Name + "). The stored name did not resolve directly.");
                return t;
            }

            int comma = aqn.IndexOf(',');
            if (comma <= 0) return null;
            string shortName = aqn.Substring(0, comma).Trim();

            System.Type found = null;
            string candidates = "";
            int matches = 0;
            for (int i = 0; i < assemblies.Count; i++)
            {
                System.Type t = assemblies[i].GetType(shortName);
                if (t == null) continue;
                matches++;
                if (found == null) found = t;
                if (candidates.Length > 0) candidates += ", ";
                candidates += t.Assembly.GetName().Name;
            }

            if (matches == 0) return null;
            if (matches > 1)
            {
                if (_typeResolveLogged.Add(aqn))
                    Debug.LogError("JuiceBox: type name " + shortName + " (stored as " + aqn
                        + ") is ambiguous - found in " + candidates
                        + ". Refusing to guess; re-pick this delegate to store an exact type.");
                return null;
            }

            if (_typeResolveLogged.Add(aqn))
                Debug.LogWarning("JuiceBox: recovered type " + shortName + " from assembly "
                    + found.Assembly.GetName().Name + ", but it was saved as " + aqn
                    + ". Re-save this animation to update the stored type name.");
            return found;
        }

        private static void OnSnapshotWritten(string sequenceName)
        {
            if (!EditorWindow.HasOpenInstances<SequenceEditorWindow>()) return;

            var w = EditorWindow.GetWindow<SequenceEditorWindow>(false, null, false);
            w?.SetMessage("Snapshot saved.", SequenceEditorWindow.MessageSeverity.Info, 30f);
            w?.RefreshRestoreButtons();
        }

        private static void OnSequenceChanged(string sequenceName)
        {
            InternalEditorUtility.RepaintAllViews();

            if (!EditorWindow.HasOpenInstances<SequenceEditorWindow>()) return;

            var w = EditorWindow.GetWindow<SequenceEditorWindow>(false, null, false);
            w?.OnSequenceLibraryChanged(sequenceName);
        }

        private static string WriteObjectId(Object obj)
        {
            if (obj == null) return "";
#if UNITY_6000_4_OR_NEWER
            EntityId key = obj.GetEntityId();
#else
            int key = obj.GetInstanceID();
#endif
            if (_writeIdCache.TryGetValue(key, out string cached)) return cached;
            if (_writingObjectId) return "";
            _writingObjectId = true;
            try
            {
                var gid = GlobalObjectId.GetGlobalObjectIdSlow(obj);
                string s = gid.identifierType != 0 ? gid.ToString() : "";
                if (!string.IsNullOrEmpty(s)) _writeIdCache[key] = s;
                return s;
            }
            finally
            {
                _writingObjectId = false;
            }
        }

        internal static void WarmObjectIds(System.Collections.Generic.IReadOnlyList<string> ids)
        {
            if (ids == null || ids.Count == 0) return;

            var gids = new System.Collections.Generic.List<GlobalObjectId>(ids.Count);
            var keys = new System.Collections.Generic.List<string>(ids.Count);
            for (int i = 0; i < ids.Count; i++)
            {
                string id = ids[i];
                if (string.IsNullOrEmpty(id)) continue;
                if (_resolveIdCache.TryGetValue(id, out Object hit) && hit != null) continue;
                if (!GlobalObjectId.TryParse(id, out var gid)) continue;
                gids.Add(gid);
                keys.Add(id);
            }

            if (gids.Count == 0) return;

            var arr = gids.ToArray();
            var objs = new Object[arr.Length];
            GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow(arr, objs);
            for (int i = 0; i < objs.Length; i++)
                if (objs[i] != null) _resolveIdCache[keys[i]] = objs[i];
        }

        private static Object ResolveObjectId(Object obj, string id)
        {
            if (obj != null) return obj;
            if (string.IsNullOrEmpty(id)) return null;
            if (_resolveIdCache.TryGetValue(id, out Object hit) && hit != null) return hit;
            if (!GlobalObjectId.TryParse(id, out var gid)) return null;
            var resolved = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(gid);
            if (resolved != null) _resolveIdCache[id] = resolved;
            return resolved;
        }
    }
}
