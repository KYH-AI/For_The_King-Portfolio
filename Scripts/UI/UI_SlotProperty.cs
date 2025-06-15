using UnityEngine;
using UnityEngine.UI;

public class UI_SlotProperty : UI_Base
{
    private enum Images
    {
        RequestSlot,
        ResultSlot
    }
    
    // 테스트 용도
    private Sprite _slotFailIcon;
    private Sprite _slotBlankIcon;
    private Sprite _slotRequestIcon;
    private Sprite _slotBaseResultIcon;
    
    
    public override void Init()
    {
        Bind<Image>(typeof(Images));
        _slotFailIcon = Resources.Load<Sprite>("Sprite/SlotIcon/uiHitMiss");
        _slotBlankIcon = Resources.Load<Sprite>("Sprite/SlotIcon/uiSlotBlank");
    }

    private void InitResultSlots()
    {
        Get<Image>((int)Images.ResultSlot).sprite = _slotBlankIcon;
    }

    public void SetRequestSlot(Sprite requestIcon)
    {
        InitResultSlots();
        _slotRequestIcon = requestIcon;
        Get<Image>((int)Images.RequestSlot).sprite = _slotRequestIcon;
    }
    
    public void SetRequestSlot(string requestIcon)
    {
        InitResultSlots();
        _slotRequestIcon = Resources.Load<Sprite>("Sprite/SlotIcon/" + requestIcon);
        Get<Image>((int)Images.RequestSlot).sprite = _slotRequestIcon;
    }

    public void SetResultSlot(Sprite resultIcon)
    {
        Get<Image>((int)Images.ResultSlot).sprite = resultIcon;
    }
}
