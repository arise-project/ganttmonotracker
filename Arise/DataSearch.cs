// Decompiled with JetBrains decompiler
// Type: Arise.Logic.DataSearch
// Assembly: Arise, Version=1.0.2191.787, Culture=neutral, PublicKeyToken=null
// MVID: EAB4A74C-551C-4077-B030-37121C333AAC
// Assembly location: D:\eugene\ganttmonotracker\Lib\arise.dll

using System.Data;
using System.Text.RegularExpressions;

namespace Arise.Logic
{
  public class DataSearch
  {
    private DataSearch()
    {
    }

    public static DataTable GetFilteredTable(DataTable table, string searchFilter)
    {
      DataTable dataTable = table.Clone();
      Regex regex = new Regex(searchFilter, RegexOptions.IgnoreCase);
      foreach (DataRow row1 in (InternalDataCollectionBase) table.Rows)
      {
        foreach (DataColumn column in (InternalDataCollectionBase) table.Columns)
        {
          string input = row1[column].ToString();
          if (regex.Match(input).Captures.Count > 0)
          {
            DataRow row2 = dataTable.NewRow();
            row2.ItemArray = (object[]) row1.ItemArray.Clone();
            dataTable.Rows.Add(row2);
          }
        }
      }
      return dataTable;
    }
  }
}
