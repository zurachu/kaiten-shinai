using UnityEngine;
using DG.Tweening;

public class UnityChanController : MonoBehaviour
{
    [SerializeField] Animator bodyAnimator;
    [SerializeField] DOTweenAnimation downAnimation;

    public bool IsStarted { get; set; }
    public bool IsDown { get; private set; }
    public bool IsJumping { get; private set; }

    private void Start()
    {
        IsDown = false;
        IsJumping = false;
    }

    private void Update()
    {
        if (!IsStarted || IsDown)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            bodyAnimator.Play("JumpToTop", 0, 0.35f);
        }

        if (Input.GetMouseButtonUp(0))
        {
            bodyAnimator.Play("TopToGround");
        }

        var stateInfo = bodyAnimator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("JumpToTop") && stateInfo.normalizedTime >= 0.5)
        {
            IsJumping = true;
        }

        if (stateInfo.IsName("TopToGround"))
        {
            IsJumping = stateInfo.normalizedTime < 0.2;
        }

        bodyAnimator.transform.eulerAngles = Vector3.zero;
    }

    public void Down()
    {
        if (IsDown)
        {
            bodyAnimator.Play("GoDown", 0, 0.25f);
        }
        else
        {
            bodyAnimator.Play("GoDown", 0, 0.125f);
            downAnimation.DORestart();
            IsDown = true;
        }
    }
}
