using System;

namespace KTM.BuildingAssistant.Common.Data
{
  public class BAFamilyInstance : IEquatable<BAFamilyInstance>
  {
    public string Name { get; set; }
    public int ElementId { get; set; }

    public int HashCode { get; set; }

    public bool Equals(BAFamilyInstance other) {
      return other.ElementId == this.ElementId;
    }
  }
}
