using JT.Abp.Application.Editions;
using System.ComponentModel.DataAnnotations.Schema;

namespace JT.Abp.Application.Features
{
    public class JTEditionFeature : JTFeature
    {
        [ForeignKey("EditionId")]
        public virtual JTEdition Edition { get; set; }

        public virtual int EditionId { get; set; }

        public JTEditionFeature()
        {
        }

        public JTEditionFeature(int editionId, string name, string value)
            : base(name, value)
        {
            EditionId = editionId;
        }
    }
}
