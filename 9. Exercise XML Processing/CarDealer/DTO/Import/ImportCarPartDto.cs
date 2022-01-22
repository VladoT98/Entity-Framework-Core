using System.Xml.Serialization;

namespace CarDealer.DTO.Import
{
    [XmlType("partId")]
    public class ImportCarPartDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
