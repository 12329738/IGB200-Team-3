using LitMotion;
using LitMotion.Animation;
using LitMotion.Extensions;
using UnityEngine;

public class MenuOpenAnimation : MonoBehaviour
{
    public MotionHandle animation;

    public Vector3 startScale =
        new Vector3(0, 1);

    public Vector3 endScale =
        Vector3.one;

    public float duration = 0.5f;

    public Ease ease =
        Ease.OutBack;


    private void OnEnable()
    {
        CancelAnimation();

        animation =
            LMotion.Create(
                startScale,
                endScale,
                duration
            )
            .WithEase(ease)
            .WithLoops(
                1,
                LoopType.Restart
            )
            .BindToLocalScale(
                transform
            );
    }


    private void OnDisable()
    {
        CancelAnimation();
    }


    private void OnDestroy()
    {
        CancelAnimation();
    }


    private void CancelAnimation()
    {
        if (animation.IsActive())
        {
            animation.Cancel();
        }
    }
}