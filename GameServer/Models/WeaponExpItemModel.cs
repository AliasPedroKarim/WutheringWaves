using Google.FlatBuffers;

namespace GameServer.Models;

public class WeaponExpItemModel
{
  public static WeaponExpItemModel? getRootAsWeaponExp(byte[] payload)
  {
    if (payload == null)
    {
      return null;
    }
    ByteBuffer _bb = new ByteBuffer(payload);
    return new WeaponExpItemModel().init(_bb.GetInt(_bb.Position) + _bb.Position, _bb);
  }

  public int offset(int bb_pos, int vtableOffset)
  {
    int vtable = bb_pos - bb.GetInt(bb_pos);
    return vtableOffset < bb.GetShort(vtable) ? bb.GetShort(vtable + vtableOffset) : 0;
  }

  public WeaponExpItemModel init(int i, ByteBuffer bb)
  {
    position = i;
    this.bb = bb;
    return this;
  }

  private ByteBuffer? bb;
  private int position;

  public int Id
  {
    get
    {
      int o = offset(position, 4);
      return o != 0 ? bb.GetInt(o + this.position) : 0;
    }
  }

  public int Cost
  {
    get
    {
      int o = offset(position, 6);
      return o != 0 ? bb.GetInt(o + this.position) : 0;
    }
  }

  public int BasicExp
  {
    get
    {
      int o = offset(position, 8);
      return o != 0 ? bb.GetInt(o + this.position) : 0;
    }
  }
}