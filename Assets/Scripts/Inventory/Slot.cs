using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{

    public Item item;
    public int itemCount;
    public Image itemImage;

    [SerializeField] private TextMeshProUGUI text_Count;
    [SerializeField] private GameObject go_CountImage;

    // 이미지 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 아이템 획득
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        if (item.itemType != Item.ItemType.Equipment)
        {
            go_CountImage.SetActive(true);
            text_Count.text = itemCount.ToString();
        }
        else
        {
            text_Count.text = " ";
            go_CountImage.SetActive(false);
        }


        SetColor(1);
    }

    // 아이템 갯수 조정
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
        {
            ClearSlot();
        }

    }

    // 아이템이 0개 되면 슬롯 초기화
    private void ClearSlot()
    {
        SetColor(0);
        itemCount = 0;
        item = null;
        itemImage.sprite = null;

        text_Count.text = " ";
        go_CountImage.SetActive(false);

    }

    private void MoveSlot()
    {
        if(itemCount<=0)
        {
            SetColor(0);
            item = null;
            itemImage.sprite = null;
            go_CountImage.SetActive(false);
            return;
        }

        itemCount -= 1;
        text_Count.text = itemCount.ToString();
        

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 우클릭
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {
                if (item.itemType == Item.ItemType.Equipment)
                {
                    // 장착

                }
                else
                {
                    //사용
                    SetSlotCount(-1);
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);

            DragSlot.instance.transform.position = eventData.position;

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
        {

            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {

            if(DragSlot.instance.dragSlot.item != item)
                ChangeSlot();

            // DragSlot.instance 랑 비교해서 같으면 갯수올리기
            
        }
    }

    private void ChangeSlot()
    {

        Item _tempItem = item;

        int _tempItemCount = itemCount;


        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);


        if (_tempItem != null)
        {
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);

        }
        else
        {
            if(itemCount < 0)
                DragSlot.instance.dragSlot.ClearSlot();
            
        }

    }

}
