using UnityEngine;

public interface I_Dummy
{
    public enum CharacterPartsTransform
    {
        LeftHandWeaponRoot = 0,
        RightHandWeaponRoot = 1,
        HeadArmorRoot = 2,
        BodyArmorRoot = 3,
        LeftFootRoot = 4,
        RightFootRoot = 5
    }

    /// <summary>
    /// ĳ���� ������ ��ġ �� ���� �ʱ�ȭ
    /// </summary>
    /// <param name="transform">ĳ����</param>
    public void SetDummyCharacterItemPartsTransform(UnityEngine.Transform transform);
    
    /// <summary>
    /// ĳ���� ������ ���� �� �𵨸� ������Ʈ
    /// </summary>
    /// <param name="item">����� ������</param>
    public void UpdateCharacterItemPart(Equipment item, bool isEquip);
    
}