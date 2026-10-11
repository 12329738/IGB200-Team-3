using LitMotion;
using LitMotion.Animation;
using LitMotion.Extensions;
using UnityEngine;

public class MenuOpenAnimation : MonoBehaviour
{
    public MotionHandle animation;
    
    public void PlayOpenAnimation()
    {
        if (animation.IsActive())
            animation.Cancel();
        animation = LMotion.Create(new Vector3(0, 1), Vector3.one, 0.5f)
            .WithEase(Ease.OutBack)
            .WithLoops(1, LoopType.Restart)
            .BindToLocalScale(transform);
    }


}
