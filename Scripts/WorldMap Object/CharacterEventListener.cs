using UnityEngine;

public class CharacterEventListener
{
    /*  <�� ��ǥ : ĳ���Ϳ� ���õ� ������ �����Ѵ�.>
     *  1. �ִϸ��̼�
     *  2. Mesh Renderer
     */

    public enum AnimationTrigger
    {
        Idle = 0,
        Defense,
        Avoid,
        TakeSmallDamage,
        TakeHeavyDamage,
        Skill1,
        Skill2,
        Skill3,
        Skill4,
        Skill5,
        Skill6,
        Taunt,
        Dead,
        Revive,
        LevelUp,
        Escape,
        UseItem,
        Run,
        WorldMapDead,
        Victory,
    }

    // �ش� ������Ʈ�� �ִϸ��̼� ������Ʈ
    private Animator _animator;

    // �ش� ������Ʈ�� ��Ƽ����
    private SkinnedMeshRenderer _characterRenderers;

    private bool _isVisible = true;
    
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible == value) return;
            _isVisible = value;
            _characterRenderers.enabled = _isVisible;
        }
    } 

    public CharacterEventListener(Animator ownerAnimator)
    {
        _animator = ownerAnimator;
    }

    public void UpdateCharacterRenderer(SkinnedMeshRenderer ownerRenderer)
    {
        if (_characterRenderers != null) return;
       _characterRenderers = ownerRenderer;
    }
    
    public void SetRunTimeAnimator(RuntimeAnimatorController runtimeAnimatorController)
    {
        _animator.runtimeAnimatorController = runtimeAnimatorController;
    }

    public void PlayAnimationTrigger(AnimationTrigger animationTrigger)
    {
        _animator.SetTrigger(animationTrigger.ToString());
    }
    
    public Animator GetAnimator()
    {
        return _animator;
    }
    
}
