using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NetGenericSerialization: MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MyTestInfo testInfo = new MyTestInfo(1, new MyPlayer(10), 100, "小明", true);
        byte[] bytes = testInfo.Writeing();
        int index = testInfo.Reabing(bytes);
        Debug.Log("等级:" + testInfo.lev + " 玩家:" + testInfo.p + " ask:" + testInfo.p.atk + " hp:" + testInfo.hp + " name:" + testInfo.name + " sex:" + testInfo.sex);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class MyTestInfo : MyBaseData
{
    public short lev;
    public MyPlayer p;
    public int hp;
    public string name;
    public bool sex;

    public MyTestInfo(short lev, MyPlayer p, int hp, string name, bool sex)
    {
        this.lev = lev;
        this.p = p;
        this.hp = hp;
        this.name = name;
        this.sex = sex;
    }

    public override int GetBytesNum()
    {
        int num = 2 + p.GetBytesNum() + 4 + 4 + Encoding.UTF8.GetBytes(name).Length + 1;
        return num;
    }
    public override byte[] Writeing()
    {
        int index = 0;
        byte[] bytes = new byte[GetBytesNum()];
        WriteType(bytes, lev, ref index);
        WriteType(bytes, p, ref index);
        WriteType(bytes, hp, ref index);
        WriteType(bytes, name, ref index);
        WriteType(bytes, sex, ref index);
        return bytes;
    }

    public override int Reabing(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        lev = ReadType<short>(bytes, ref index);
        p = ReadType<MyPlayer>(bytes, ref index);
        hp = ReadType<int>(bytes, ref index);
        name = ReadType<string>(bytes, ref index);
        sex = ReadType<bool>(bytes, ref index);
        return index - beginIndex;
    }

}

public abstract class MyBaseData
{
    /// <summary>
    /// 获取byte数组长度
    /// </summary>
    /// <returns></returns>
    public abstract int GetBytesNum();

    /// <summary>
    /// 开始序列化
    /// </summary>
    /// <returns></returns>
    public abstract byte[] Writeing();

    /// <summary>
    /// 开始反序列化
    /// </summary>
    /// <param name="bytes">反序列化的byte数组</param>
    /// <param name="beginIndex">开始反序列化的索引</param>
    /// <returns></returns>
    public abstract int Reabing(byte[] bytes, int beginIndex = 0);

    /// <summary>
    /// 序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bytes">byte储存数组</param>
    /// <param name="num">序列化内容</param>
    /// <param name="index">开始序列化的索引</param>
    protected void WriteType<T>(byte[] bytes, T num, ref int index)
    {
        switch (Type.GetTypeCode(typeof(T)))
        {
            case TypeCode.String:
                {
                    byte[] byteStr = Encoding.UTF8.GetBytes(num as string);
                    int strSum = byteStr.Length;
                    BitConverter.GetBytes(strSum).CopyTo(bytes, index);
                    index += 4; // int 类型 4 字节
                    byteStr.CopyTo(bytes, index);
                    index += strSum;
                    return;
                }
            case TypeCode.Int32:
                BitConverter.GetBytes(Convert.ToInt32(num)).CopyTo(bytes, index);
                index += 4;
                return;
            case TypeCode.Int16:
                BitConverter.GetBytes(Convert.ToInt16(num)).CopyTo(bytes, index);
                index += 2;  //short 类型 2字节
                return;
            case TypeCode.Boolean:
                BitConverter.GetBytes(Convert.ToBoolean(num)).CopyTo(bytes, index);
                index += 1;
                return;
            default:
                var p = num as MyPlayer;
                p.Writeing().CopyTo(bytes, index);
                index += p.GetBytesNum();
                return;
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bytes">读取的byte数组</param>
    /// <param name="index">开始读取位置索引</param>
    /// <returns></returns>
    protected T ReadType<T>(byte[] bytes, ref int index)
    {
        switch (Type.GetTypeCode(typeof(T)))
        {
            case TypeCode.String:
                int strSum = BitConverter.ToInt32(bytes, index);
                index += 4;
                string content = Encoding.UTF8.GetString(bytes, index, strSum);
                index += strSum;
                return (T)(object)content;
            case TypeCode.Int32:
                int IntNum = BitConverter.ToInt32(bytes, index);
                index += 4;
                return (T)(object)IntNum;
            case TypeCode.Int16:
                short ShortNum = BitConverter.ToInt16(bytes, index);
                index += 2;
                return (T)(object)ShortNum;
            case TypeCode.Boolean:
                bool BoolNum = BitConverter.ToBoolean(bytes, index);
                index += 1;
                return (T)(object)BoolNum;
            default:
                MyPlayer myPlayer = new MyPlayer(0);
                index += myPlayer.Reabing(bytes, index);
                return (T)(object)myPlayer;
        }
    }
}
public class MyPlayer : MyBaseData
{
    public int atk;
    public MyPlayer(int atk)
    {
        this.atk = atk;
    }
    public override int GetBytesNum()
    {
        return 4;
    }
    public override byte[] Writeing()
    {
        byte[] bytes = new byte[GetBytesNum()];
        int index = 0;
        WriteType(bytes, atk, ref index);
        return bytes;
    }

    public override int Reabing(byte[] bytes, int beginIndex = 0)
    {
        int index = beginIndex;
        atk = ReadType<int>(bytes, ref index);
        return index - beginIndex;
    }
}

