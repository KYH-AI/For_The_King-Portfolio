public class WorldMapHexMovementSlot
{
    /* <�� ��ǥ>
     *  1. �÷��̾ �⺻������ �̵��� �� �ִ� �ּ� ������ ������ ���������� Ȯ���̵����� 2������ ����
     *  2. ������ ������ �÷��̾ ���� �ϴ� ������ �ǹ���
     *      ���� ����  (���� : 3 ,�ٴ� : ��Ʈ������ ����)
     *  3. �⺻ ������ �÷��̾ Ȯ�������� �̵����� �ο��� (Ȳ�ݻ� �̵� ��������)
     *      �⺻ ����  (���� : 2, �ٴ� : ��Ʈ������ ����)
     *
     *  4. ���� �������� ���� �÷��̾ �̵��Ҽ� �ִ� �̵������� 5��, �� �� 2���� Ȯ������ �̵� ������ 3���� �÷��̾��� �̵��ɷ�ġ�� ����ؼ� ����
     */     
    
    
    /// <summary>
    /// ������ ���� �̵��� ����
    /// </summary>
    /// <param name="hexType">���� ��ġ�� ����</param>
    /// <returns>���� ���� �̵���</returns>
    public static int GetHexTerrainSlots(HexType hexType)
    {
        int slots = 0;
        
        switch (hexType)
        {
            //  case HexType.Water : slots = // TODO: ��Ʈ�� ���� ������ �޾ƿ� ���� �ο�
            default: slots = 3; break;
        }

        return slots;
    }

    /// <summary>
    /// �⺻ �̵��� ����
    /// </summary>
    /// <param name="hexType">���� ��ġ�� ����</param>
    /// <returns>�⺻ ���� �̵���</returns>
    public static int GetHexBaseSlots(HexType hexType)
    {
        int slots = 0;
        
        switch (hexType)
        {
            //  case HexType.Water : slots = // TODO: ��Ʈ�� ���� ������ �޾ƿ� ���� �ο�
            default: slots = 2; break;
        }

        return slots;
    }
    
}