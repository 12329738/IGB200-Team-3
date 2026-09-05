using UnityEngine;
using UnityEngine.UIElements;

// ==============================================================================
//  JbTheme: All editor graph colours in one place, with USS custom property
//  overrides.
// ==============================================================================
namespace JuiceBox
{
    internal struct JbTheme
    {
        public bool Initialized;
        // -- Strip -----------------------------------------------------------
        public Color StripBg;
        public Color CapBg;
        public Color CapBorder;
        public Color CapBtn;
        public Color CapBtnDisabled;
        public Color CapLabel;
        public Color CapName;
        public Color HRule;
        public Color SlotIdle;
        public Color SlotHi;
        public Color SlotBgHi;
        public Color SlotDenied;
        public Color SlotBgDenied;
        public Color SlotNum;
        public Color Arrow;
        public Color RunBg;
        public Color RunBorder;
        public Color PocketBg;
        public Color PocketBorder;
        public Color StripSpeckle;
        // -- Node ------------------------------------------------------------
        public Color NodeBg;
        public Color NodeBorder;
        public Color NodeBtn;
        public Color NodeBorderDrag;
        public Color NodeBorderFloat;
        public Color NodeBorderSel;
        public Color NodeBorderFloatSel;
        public Color NodeHeader;
        public Color BadgeTweenBg;
        public Color BadgeTweenText;
        public Color BadgeAdvBg;
        public Color BadgeAdvText;
        public Color BadgeShakeBg;
        public Color BadgeShakeText;
        public Color BadgeTypeBg;
        public Color BadgeTypeText;
        public Color FieldBg;
        public Color FieldBgMiss;
        public Color FieldLbl;
        public Color FieldVal;
        public Color FieldMissVal;
        public Color PortAction;
        public Color PortLabel;
        public Color PortDivider;
        public Color Arc;
        // -- Hook ------------------------------------------------------------
        public Color SubnodeBg;
        public Color SubnodeBorder;
        // -- Picker ----------------------------------------------------------
        public Color EvalOnceBg;
        public Color EvalOnceText;
        public Color EvalFrameBg;
        public Color EvalFrameText;
        public Color DragHighlight;
        // -- Message bar -----------------------------------------------------
        public Color MsgBarBg;
        public Color MsgBarBorder;
        public Color MsgBarText;
        public Color MsgWarnBg;
        public Color MsgWarnBorder;
        public Color MsgWarnText;
        public Color MsgErrBg;
        public Color MsgErrBorder;
        public Color MsgErrText;
        // -- Window chrome ---------------------------------------------------
        public Color ScrollTrack;
        public Color ScrollThumb;
        // -- Edge ------------------------------------------------------------
        public float EdgeWidth;
        public float EdgeCornerRadius;
        public float EdgeLanePitch;

        public static readonly JbTheme Default = new JbTheme
        {
            Initialized = true,
            StripBg            = new Color(0.1373f, 0.1373f, 0.1373f),
            CapBg              = new Color(0.1098f, 0.1569f, 0.2000f),
            CapBorder          = new Color(0.2000f, 0.2980f, 0.4000f),
            CapBtn             = new Color(0.4980f, 0.6275f, 0.7373f),
            CapBtnDisabled     = new Color(0.2902f, 0.3451f, 0.4000f),
            CapLabel           = new Color(0.4314f, 0.6157f, 0.7529f),
            CapName            = new Color(0.7765f, 0.8745f, 0.9608f),
            HRule              = new Color(0.2000f, 0.3098f, 0.4118f),
            SlotIdle           = new Color(0.2392f, 0.2392f, 0.2392f),
            SlotHi             = new Color(0.3490f, 0.6118f, 0.9216f),
            SlotBgHi           = new Color(0.0784f, 0.1765f, 0.2784f),
            SlotDenied         = new Color(0.8000f, 0.3333f, 0.3333f),
            SlotBgDenied       = new Color(0.2000f, 0.0588f, 0.0588f),
            SlotNum            = new Color(0.2784f, 0.2784f, 0.2784f),
            Arrow              = new Color(0.5333f, 0.5333f, 0.5333f),
            RunBg              = new Color(0.1020f, 0.2431f, 0.6980f, 0.1412f),
            RunBorder          = new Color(0.2392f, 0.5412f, 0.9020f, 0.8000f),
            PocketBg           = new Color(0.0745f, 0.1137f, 0.1569f),
            PocketBorder       = new Color(0.1647f, 0.3059f, 0.4431f),
            StripSpeckle       = new Color(1.0000f, 1.0000f, 1.0000f, 0.1020f),
            NodeBg             = new Color(0.1098f, 0.1647f, 0.2235f),
            NodeBorder         = new Color(0.2314f, 0.3569f, 0.4824f),
            NodeBtn            = new Color(0.4980f, 0.6275f, 0.7373f),
            NodeBorderDrag     = new Color(0.3412f, 0.5216f, 0.6980f),
            NodeBorderFloat    = new Color(0.8157f, 0.2902f, 0.2353f),
            NodeBorderSel      = new Color(0.6588f, 0.5882f, 0.1294f),
            NodeBorderFloatSel = new Color(0.8784f, 0.3569f, 0.2941f),
            NodeHeader         = new Color(0.0824f, 0.1294f, 0.1804f),
            BadgeTweenBg       = new Color(0.0824f, 0.1412f, 0.0824f),
            BadgeTweenText     = new Color(0.3608f, 0.6196f, 0.3608f),
            BadgeAdvBg         = new Color(0.1412f, 0.0824f, 0.0824f),
            BadgeAdvText       = new Color(0.8078f, 0.4039f, 0.4039f),
            BadgeShakeBg       = new Color(0.1412f, 0.1098f, 0.0392f),
            BadgeShakeText     = new Color(0.8196f, 0.6392f, 0.2314f),
            BadgeTypeBg        = new Color(0.1020f, 0.1020f, 0.1686f),
            BadgeTypeText      = new Color(0.5412f, 0.5412f, 0.8510f),
            FieldBg            = new Color(0.0667f, 0.1098f, 0.1529f),
            FieldBgMiss        = new Color(0.1294f, 0.0510f, 0.0510f),
            FieldLbl           = new Color(0.4863f, 0.6078f, 0.6980f),
            FieldVal           = new Color(0.3843f, 0.6000f, 0.7412f),
            FieldMissVal       = new Color(0.8784f, 0.4157f, 0.4157f),
            PortAction         = new Color(0.2980f, 0.6000f, 0.9020f),
            PortLabel          = new Color(0.6000f, 0.6510f, 0.6902f),
            PortDivider        = new Color(0.1216f, 0.2000f, 0.2784f),
            Arc                = new Color(0.3686f, 0.5255f, 0.6275f),
            SubnodeBg          = new Color(0.0784f, 0.1216f, 0.1804f),
            SubnodeBorder      = new Color(0.2549f, 0.4510f, 0.6784f),
            EvalOnceBg         = new Color(0.1804f, 0.2510f, 0.1804f),
            EvalOnceText       = new Color(0.6000f, 0.9020f, 0.6000f),
            EvalFrameBg        = new Color(0.2196f, 0.2196f, 0.2784f),
            EvalFrameText      = new Color(0.7490f, 0.7804f, 0.9020f),
            DragHighlight      = new Color(0.5020f, 0.5020f, 0.5020f, 0.4000f),
            MsgBarBg           = new Color(0.1294f, 0.1294f, 0.1294f),
            MsgBarBorder       = new Color(0.2000f, 0.2000f, 0.2000f),
            MsgBarText         = new Color(0.6275f, 0.6275f, 0.6275f),
            MsgWarnBg          = new Color(0.1686f, 0.1412f, 0.0588f),
            MsgWarnBorder      = new Color(0.4000f, 0.3216f, 0.0588f),
            MsgWarnText        = new Color(0.8784f, 0.7216f, 0.2196f),
            MsgErrBg           = new Color(0.2000f, 0.0784f, 0.0784f),
            MsgErrBorder       = new Color(0.5490f, 0.1843f, 0.1843f),
            MsgErrText         = new Color(0.9412f, 0.4784f, 0.4784f),
            ScrollTrack        = new Color(0.0000f, 0.0000f, 0.0000f, 0.3020f),
            ScrollThumb        = new Color(1.0000f, 1.0000f, 1.0000f, 0.4000f),
            EdgeWidth = 3f,
            EdgeCornerRadius = 6f,
            EdgeLanePitch = 10f,
        };

        public static JbTheme ReadFrom(ICustomStyle s)
        {
            var t = Default;
            R(s, "--jb-strip-bg", ref t.StripBg);
            R(s, "--jb-cap-bg", ref t.CapBg);
            R(s, "--jb-cap-border", ref t.CapBorder);
            R(s, "--jb-cap-btn", ref t.CapBtn);
            R(s, "--jb-cap-btn-disabled", ref t.CapBtnDisabled);
            R(s, "--jb-cap-label", ref t.CapLabel);
            R(s, "--jb-cap-name", ref t.CapName);
            R(s, "--jb-h-rule", ref t.HRule);
            R(s, "--jb-slot-idle", ref t.SlotIdle);
            R(s, "--jb-slot-hi", ref t.SlotHi);
            R(s, "--jb-slot-bg-hi", ref t.SlotBgHi);
            R(s, "--jb-slot-denied", ref t.SlotDenied);
            R(s, "--jb-slot-bg-denied", ref t.SlotBgDenied);
            R(s, "--jb-slot-num", ref t.SlotNum);
            R(s, "--jb-arrow", ref t.Arrow);
            R(s, "--jb-run-bg", ref t.RunBg);
            R(s, "--jb-run-border", ref t.RunBorder);
            R(s, "--jb-pocket-bg", ref t.PocketBg);
            R(s, "--jb-pocket-border", ref t.PocketBorder);
            R(s, "--jb-strip-speckle", ref t.StripSpeckle);
            R(s, "--jb-node-bg", ref t.NodeBg);
            R(s, "--jb-node-border", ref t.NodeBorder);
            R(s, "--jb-node-btn", ref t.NodeBtn);
            R(s, "--jb-node-border-drag", ref t.NodeBorderDrag);
            R(s, "--jb-node-border-float", ref t.NodeBorderFloat);
            R(s, "--jb-node-border-sel", ref t.NodeBorderSel);
            R(s, "--jb-node-border-float-sel", ref t.NodeBorderFloatSel);
            R(s, "--jb-node-header", ref t.NodeHeader);
            R(s, "--jb-badge-tween-bg", ref t.BadgeTweenBg);
            R(s, "--jb-badge-tween-text", ref t.BadgeTweenText);
            R(s, "--jb-badge-adv-bg", ref t.BadgeAdvBg);
            R(s, "--jb-badge-adv-text", ref t.BadgeAdvText);
            R(s, "--jb-badge-shake-bg", ref t.BadgeShakeBg);
            R(s, "--jb-badge-shake-text", ref t.BadgeShakeText);
            R(s, "--jb-badge-type-bg", ref t.BadgeTypeBg);
            R(s, "--jb-badge-type-text", ref t.BadgeTypeText);
            R(s, "--jb-field-bg", ref t.FieldBg);
            R(s, "--jb-field-bg-miss", ref t.FieldBgMiss);
            R(s, "--jb-field-lbl", ref t.FieldLbl);
            R(s, "--jb-field-val", ref t.FieldVal);
            R(s, "--jb-field-miss-val", ref t.FieldMissVal);
            R(s, "--jb-port-action", ref t.PortAction);
            R(s, "--jb-port-label", ref t.PortLabel);
            R(s, "--jb-port-divider", ref t.PortDivider);
            R(s, "--jb-arc", ref t.Arc);
            R(s, "--jb-subnode-bg", ref t.SubnodeBg);
            R(s, "--jb-subnode-border", ref t.SubnodeBorder);
            R(s, "--jb-eval-once-bg", ref t.EvalOnceBg);
            R(s, "--jb-eval-once-text", ref t.EvalOnceText);
            R(s, "--jb-eval-frame-bg", ref t.EvalFrameBg);
            R(s, "--jb-eval-frame-text", ref t.EvalFrameText);
            R(s, "--jb-drag-highlight", ref t.DragHighlight);
            R(s, "--jb-msg-bar-bg", ref t.MsgBarBg);
            R(s, "--jb-msg-bar-border", ref t.MsgBarBorder);
            R(s, "--jb-msg-bar-text", ref t.MsgBarText);
            R(s, "--jb-msg-warn-bg", ref t.MsgWarnBg);
            R(s, "--jb-msg-warn-border", ref t.MsgWarnBorder);
            R(s, "--jb-msg-warn-text", ref t.MsgWarnText);
            R(s, "--jb-msg-err-bg", ref t.MsgErrBg);
            R(s, "--jb-msg-err-border", ref t.MsgErrBorder);
            R(s, "--jb-msg-err-text", ref t.MsgErrText);
            R(s, "--jb-scroll-track", ref t.ScrollTrack);
            R(s, "--jb-scroll-thumb", ref t.ScrollThumb);
            RF(s, "--jb-edge-width", ref t.EdgeWidth);
            RF(s, "--jb-edge-radius", ref t.EdgeCornerRadius);
            RF(s, "--jb-edge-lane-pitch", ref t.EdgeLanePitch);
            return t;
        }

        private static void R(ICustomStyle s, string name, ref Color c)
        {
            if (s.TryGetValue(new CustomStyleProperty<Color>(name), out var v)) c = v;
        }

        private static void RF(ICustomStyle s, string name, ref float f)
        {
            if (s.TryGetValue(new CustomStyleProperty<float>(name), out var v)) f = v;
        }
    }
}
