using System.Xml.Serialization;

namespace Plukliste;
[XmlRoot("Pluklist")]
public class Pluklist
{
    public string? Name;
    public string? Shipment;
    [XmlElement("Adresse")]
    public string? Address;
    public List<Item> Lines = new List<Item>();
    public void AddItem(Item item) { Lines.Add(item); }
}

public class Item
{
    public string ProductID;
    public string Title;
    public ItemType Type;
    public int Amount;
}

public enum ItemType
{
    Fysisk, Print //why no change "fysisk" :(
}


