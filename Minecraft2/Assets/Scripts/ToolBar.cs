using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBar : MonoBehaviour
{
	public UIItemSlot[] slots;
	public RectTransform highlight;
	public int slotIndex = 0;
	public Player player;
	private void Start()
	{
		byte index = 1;
		foreach (UIItemSlot s in slots)
		{
			ItemStack stack = new ItemStack(index, Random.Range(2, 65));
			ItemSlot slot = new ItemSlot(s, stack);
			index++;
		}
	}

	private void Update()
	{
		float scroll = Input.GetAxis("Mouse ScrollWheel");

		if (scroll != 0)
		{
			if (scroll > 0)
				slotIndex--;
			else
				slotIndex++;

			if (slotIndex > slots.Length - 1)
				slotIndex = 0;
			if (slotIndex < 0)
				slotIndex = slots.Length - 1;

			highlight.position = slots[slotIndex].slotIcon.transform.position;
		}

	}
}