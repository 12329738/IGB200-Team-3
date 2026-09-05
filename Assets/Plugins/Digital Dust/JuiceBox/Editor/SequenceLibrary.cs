using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static JuiceBox.Processor;

// ==============================================================================
//  SequenceLibrary: Propagates sequence edits to all loaded components that share the same sequence name.
// ==============================================================================
namespace JuiceBox
{
    [InitializeOnLoad]
    internal static class SequenceLibrary
    {
        public static event System.Action<string> OnSequenceChanged;

        static SequenceLibrary() { }

        internal struct SiblingSequence
        {
            public JuiceBoxAnimation anim;
            public Sequence seq;
            public int index;
        }

        private static readonly List<SiblingSequence> _siblingBuffer = new List<SiblingSequence>();

        private static List<SiblingSequence> CollectSiblingSequences(string seqName,
            IAnimationEditorComponent context)
        {
            _siblingBuffer.Clear();
            if (string.IsNullOrEmpty(seqName) || context == null) return _siblingBuffer;

            var allAnims = context.GetInstances();
            for (int a = 0; a < allAnims.Count; a++)
            {
                JuiceBoxAnimation anim = allAnims[a];
                if (anim == null || anim.Sequences == null) continue;
                for (int i = 0; i < anim.Sequences.Count; i++)
                {
                    Sequence seq = anim.Sequences[i];
                    if (seq == null || seq.Name != seqName) continue;
                    _siblingBuffer.Add(new SiblingSequence { anim = anim, seq = seq, index = i });
                }
            }
            return _siblingBuffer;
        }

        private static bool AnimationContainsSequence(JuiceBoxAnimation anim, Sequence target)
        {
            if (anim == null || anim.Sequences == null || target == null) return false;
            for (int i = 0; i < anim.Sequences.Count; i++)
                if (anim.Sequences[i] == target) return true;
            return false;
        }

        public static void NotifySequenceChanged(string name, Sequence source,
            IAnimationEditorComponent context)
        {
            if (string.IsNullOrEmpty(name) || source == null) return;

            string json = EditorJsonUtility.ToJson(source);

            var siblings = CollectSiblingSequences(name, context);
            for (int i = 0; i < siblings.Count; i++)
            {
                SiblingSequence sib = siblings[i];
                if (sib.seq == source) continue;

                EditorJsonUtility.FromJsonOverwrite(json, sib.seq);
                Processor.FinalizeSerialization();
                EditorUtility.SetDirty(sib.anim);
            }

            OnSequenceChanged?.Invoke(name);
        }

        public static void NotifySequenceRenamed(string oldName, string newName, Sequence source,
            IAnimationEditorComponent context)
        {
            if (string.IsNullOrEmpty(oldName)) return;
            if (oldName == newName) return;

            var siblings = CollectSiblingSequences(oldName, context);
            for (int i = 0; i < siblings.Count; i++)
            {
                SiblingSequence sib = siblings[i];
                if (AnimationContainsSequence(sib.anim, source)) continue;

                sib.seq.Name = newName;
                EditorUtility.SetDirty(sib.anim);
            }

            OnSequenceChanged?.Invoke(newName);
        }

        public static int CountReferences(string name, IAnimationEditorComponent context)
        {
            if (string.IsNullOrEmpty(name)) return 0;

            var siblings = CollectSiblingSequences(name, context);
            int count = 0;
            JuiceBoxAnimation lastAnim = null;
            for (int i = 0; i < siblings.Count; i++)
            {
                if (siblings[i].anim != lastAnim)
                {
                    count++;
                    lastAnim = siblings[i].anim;
                }
            }
            return count;
        }

        public static string GetSequenceJson(string name, JuiceBoxAnimation exclude)
        {
            if (string.IsNullOrEmpty(name)) return null;

            var siblings = CollectSiblingSequences(name, exclude);
            for (int i = 0; i < siblings.Count; i++)
                if (siblings[i].anim != exclude)
                    return EditorJsonUtility.ToJson(siblings[i].seq);

            return null;
        }

        // -- Targeted delegate-slot propagation --------------------------------

        private static void CopySlot(IDelegateConnecter from, IDelegateConnecter to, string slotName)
        {
            var (mode, obj, cls, method, relDesc) = from.ReadSlot(slotName);
            to.WriteSlot(slotName, mode, obj, cls, method, relDesc);
            to.WriteValueSlot(slotName, from.ReadValueSlot(slotName));
            to.WriteEvalOnce(slotName, from.ReadEvalOnce(slotName));
            to.Reconstruct();
        }

        public static void PropagateEffectSlot(string seqName, Sequence sourceSeq,
            int effectIndex, string slotName, IAnimationEditorComponent context)
        {
            if (string.IsNullOrEmpty(seqName) || sourceSeq == null || sourceSeq.Property == null) return;
            if (effectIndex < 0 || effectIndex >= sourceSeq.Property.EffectCount) return;

            IDelegateConnecter from = sourceSeq.Property.GetEffect(effectIndex);
            if (from == null) return;

            var siblings = CollectSiblingSequences(seqName, context);
            for (int i = 0; i < siblings.Count; i++)
            {
                SiblingSequence sib = siblings[i];
                if (sib.seq == sourceSeq || sib.seq.Property == null) continue;
                if (effectIndex >= sib.seq.Property.EffectCount) continue;

                IDelegateConnecter to = sib.seq.Property.GetEffect(effectIndex);
                if (to == null) continue;

                CopySlot(from, to, slotName);
                EditorUtility.SetDirty(sib.anim);
            }
        }

        public static void PropagatePropertySlot(string seqName, Sequence sourceSeq, string slotName,
            IAnimationEditorComponent context)
        {
            if (string.IsNullOrEmpty(seqName) || sourceSeq == null || sourceSeq.Property == null) return;

            IDelegateConnecter from = (IDelegateConnecter)sourceSeq.Property;

            var siblings = CollectSiblingSequences(seqName, context);
            for (int i = 0; i < siblings.Count; i++)
            {
                SiblingSequence sib = siblings[i];
                if (sib.seq == sourceSeq || sib.seq.Property == null) continue;

                CopySlot(from, (IDelegateConnecter)sib.seq.Property, slotName);
                EditorUtility.SetDirty(sib.anim);

                if (JuiceBoxCentralController.IsSequenceRunning(sib.seq))
                    sib.anim.StartSequence(sib.index);
            }
        }

        public static void RestartRunningSiblings(string seqName, Sequence sourceSeq,
            IAnimationEditorComponent context)
        {
            if (string.IsNullOrEmpty(seqName)) return;

            var siblings = CollectSiblingSequences(seqName, context);
            for (int i = 0; i < siblings.Count; i++)
            {
                SiblingSequence sib = siblings[i];
                if (sib.seq == sourceSeq) continue;
                if (JuiceBoxCentralController.IsSequenceRunning(sib.seq))
                    sib.anim.StartSequence(sib.index);
            }
        }

        // -- Sequence-field propagation ----------------------------------------

        public static void PropagateTriggers(string seqName, Sequence sourceSeq, TriggerMode triggers,
            IAnimationEditorComponent context)
        {
            if (string.IsNullOrEmpty(seqName)) return;

            var siblings = CollectSiblingSequences(seqName, context);
            for (int i = 0; i < siblings.Count; i++)
            {
                SiblingSequence sib = siblings[i];
                if (sib.seq == sourceSeq) continue;

                sib.seq.Triggers = triggers;
                EditorUtility.SetDirty(sib.anim);
            }
        }

        public static void PropagateSegment(string seqName, Sequence sourceSeq, MEC.Segment segment,
            IAnimationEditorComponent context)
        {
            if (string.IsNullOrEmpty(seqName)) return;

            var siblings = CollectSiblingSequences(seqName, context);
            for (int i = 0; i < siblings.Count; i++)
            {
                SiblingSequence sib = siblings[i];
                if (sib.seq == sourceSeq) continue;

                sib.seq.Segment = segment;
                EditorUtility.SetDirty(sib.anim);

                if (JuiceBoxCentralController.IsSequenceRunning(sib.seq))
                    JuiceBoxCentralController.Instance.SetSegment(sib.seq, segment);
            }
        }
    }
}