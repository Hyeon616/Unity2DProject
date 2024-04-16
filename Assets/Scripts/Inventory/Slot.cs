using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    
    public Item item;
    public Image itemImage;

    [SerializeField] private TextMeshProUGUI text_Count;
    [SerializeField] private GameObject go_CountImage;

    // �̹��� ���� ����
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // ������ ȹ��
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        item.amount = _count;
        itemImage.sprite = item.itemImage;

        if (item.itemType != Item.ItemType.Equipment)
        {
            go_CountImage.SetActive(true);
            text_Count.text = item.amount.ToString();
        }
        else
        {
            text_Count.text = "0";
            go_CountImage.SetActive(false);
        }


        SetColor(1);
    }

    // ������ ���� ����
    public void SetSlotCount(int _count)
    {
        item.amount += _count;
        text_Count.text = item.amount.ToString();

        if (item.amount <= 0)
        {
            ClearSlot();
        }

    }

    // �������� 0�� �Ǹ� ���� �ʱ�ȭ
    private void ClearSlot()
    {
        SetColor(0);
        item.amount = 0;
        item = null;
        itemImage.sprite = null;

        text_Count.text = "0";
        go_CountImage.SetActive(false);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // ��Ŭ��
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {
                if (item.itemType == Item.ItemType.Equipment)
                {
                    // ����

                }
                else
                {
                    SetSlotCount(-1);
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);

            DragSlot.instance.transform.position = eventData.position;
            Debug.Log(DragSlot.instance.name);
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
        if(DragSlot.instance.dragSlot!=null)
        {
            ChangeSlot();
            // DragSlot.instance �� ���ؼ� ������ �����ø���
        }
    }

    private void ChangeSlot()
    {
        Item _tempItem = item;
        //int _tempItemCount = item.amount;
        int _tempItemCount = 1;

        AddItem(DragSlot.instance.dragSlot.item, 1);

        if(_tempItem != null)
        {
            
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
            item.amount -= 1;
        }
        else
        {
            if(item.amount <= 0)
            {
                DragSlot.instance.dragSlot.ClearSlot();

            }
        }

    }

}
