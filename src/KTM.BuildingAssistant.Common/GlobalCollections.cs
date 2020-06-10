using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using KTM.BuildingAssistant.Common.Data;

namespace KTM.BuildingAssistant.Common
{
  public class GlobalCollections {
    public static ObservableCollection<BAFamilyInstance> FamilyInstanceCollection { get; set; } = new ObservableCollection<BAFamilyInstance>();

    public static HashSet<int> InstanceHashSet { get; set; } = new HashSet<int>();

    //memo table of views and collections
    //key = viewId as int
    //val = list of faminstances
    public static Dictionary<int, List<BAFamilyInstance>> FamilyInstancesCachedByView {get;set;}
  }
}
