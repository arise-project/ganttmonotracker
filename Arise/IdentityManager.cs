// Decompiled with JetBrains decompiler
// Type: Arise.Logic.IdentityManager
// Assembly: Arise, Version=1.0.2191.787, Culture=neutral, PublicKeyToken=null
// MVID: EAB4A74C-551C-4077-B030-37121C333AAC
// Assembly location: D:\eugene\ganttmonotracker\Lib\arise.dll

using System;
using System.Collections;
using System.Data;

namespace Arise.Logic
{
  public class IdentityManager
  {
    private Hashtable fObjectIdentityHash = new Hashtable();
    private Hashtable fTypeIdentityHash = new Hashtable();
    private Random r;
    private static IdentityManager fInstance;
    private int fCount;

    public static IdentityManager Instance
    {
      get
      {
        if (IdentityManager.fInstance == null)
          IdentityManager.fInstance = new IdentityManager();
        return IdentityManager.fInstance;
      }
    }

    private IdentityManager()
    {
      this.r = new Random();
    }

    public int CreateObjectIdentity(Type type)
    {
      if (!this.fObjectIdentityHash.Contains((object) type))
      {
        this.fObjectIdentityHash.Add((object) type, (object) 0);
        return 0;
      }
      this.fObjectIdentityHash[(object) type] = (object) ((int) this.fObjectIdentityHash[(object) type] + 1);
      return (int) this.fObjectIdentityHash[(object) type];
    }

    public int CreateTableIdentity(DataTable table)
    {
      if (table == null)
        throw new ArgumentException("Can not create identity for null");
      if (!table.Columns.Contains("ID"))
        throw new ArgumentException("Can not create identity for table without ID column");
      int num1 = -1;
      if (table.Select("ID = max(ID)").Length > 0)
        num1 = this.r.Next();
      int num2;
      return num2 = num1 + 1;
    }

    public int CreateTypeIdentity(Type type)
    {
      if (!this.fTypeIdentityHash.Contains((object) type))
        this.fTypeIdentityHash.Add((object) type, (object) this.fCount++);
      return (int) this.fTypeIdentityHash[(object) type];
    }
  }
}
