using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] possibleDrop;
    private List<ItemData> dropList = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;
    //������Ʒ
    protected void DropItem(ItemData itemData) 
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position,Quaternion.identity);

        Vector2 randomVelocity = new Vector2(Random.Range(-5,5),Random.Range(12,15));

        newDrop.GetComponent<ItemObject>().SetupItem(itemData, randomVelocity);
    }

    public virtual void  GenerateDrop()
    {
        for(int i=0; i < possibleDrop.Length;i++)
        {
            if(Random.Range(0,100) <= possibleDrop[i].dropChance)
            {
                dropList.Add(possibleDrop[i]);
            }
        }

        for(int i=0;i<(possibleItemDrop >= dropList.Count? dropList.Count: possibleItemDrop);i++)
        {
            ItemData randomItem = dropList[Random.Range(0,dropList.Count-1)];
            dropList.Remove(randomItem);
            DropItem(randomItem);
        }
    }
}
