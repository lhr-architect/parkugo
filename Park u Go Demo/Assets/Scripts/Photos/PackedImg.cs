using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

public class PackedImg
{
    public string userName; // 存储字符串信息
    public byte[] ImageData; // 存储图片的字节数据
    public int width, height;
    public int pid;
    public float x, y;

    public Vector3 POSITION {
        get{
            return new Vector3(x, y, 0);
        } 
    }


    public PackedImg(string name, byte[] imageData, int _width, int _height, int _pid, float _x,float _y)
    {
        userName = name;
        ImageData = imageData;
        width = _width;
        height = _height;
        pid = _pid;
        x = _x; 
        y = _y;
    }

    public override string ToString()
    {
        return $"userName: {userName} pid : {pid} x: {x} y: {y} \n" ;
    }


    public bool Equals(PackedImg other)
    {
        return (userName.Equals(other.userName) && pid == other.pid);
    }

    public bool Equals(string UserName,int Pid)
    {
        return userName.Equals(UserName) && pid == Pid;
    }

    // 打包、解包顺序对应 Int Str Int byte[] Int Int
    public byte[] encode2Bytes()
    {
        // 将字符串转换为字节
        byte[] messageBytes = Encoding.UTF8.GetBytes(userName);

        // 将图片数据直接使用packet.ImageData 构建总的发送数据包

        List<byte> totalData = new List<byte>();

        totalData.AddRange(BitConverter.GetBytes(messageBytes.Length)); // 字符串长度
        totalData.AddRange(messageBytes); // 字符串内容
        totalData.AddRange(BitConverter.GetBytes(ImageData.Length)); // 图片数据长度
        totalData.AddRange(ImageData); // 图片内容

        totalData.AddRange(BitConverter.GetBytes(width));
        totalData.AddRange(BitConverter.GetBytes(height));
        totalData.AddRange(BitConverter.GetBytes(pid));

        totalData.AddRange(BitConverter.GetBytes(x));
        totalData.AddRange(BitConverter.GetBytes(y));

        return totalData.ToArray();
    }


    static public PackedImg decode2Class(byte[] packedData)
    {
        int nameLength = BitConverter.ToInt32(packedData, 0);
        byte[] nameBytes = new byte[nameLength];
        Array.Copy(packedData, 4, nameBytes, 0, nameLength);
        string userName = Encoding.UTF8.GetString(nameBytes);

        int imageLength = BitConverter.ToInt32(packedData, 4 + nameLength);
        byte[] imageData = new byte[imageLength];
        Array.Copy(packedData, 8 + nameLength, imageData, 0, imageLength);

        int _width = BitConverter.ToInt32(packedData, 8 + nameLength + imageLength);
        int _heiht = BitConverter.ToInt32(packedData, 12 + nameLength + imageLength);
        int _pid = BitConverter.ToInt32(packedData, 16 + nameLength + imageLength);

        float X = BitConverter.ToSingle(packedData, 16 + sizeof(float) + nameLength + imageLength);
        float Y = BitConverter.ToSingle(packedData, 16 + sizeof(float) + sizeof(float) + nameLength + imageLength);


        return new PackedImg(userName, imageData, _width, _heiht, _pid, X, Y);
    }

    public bool isImg(string _userName, int _pid)
    {
        if(userName.Equals(_userName) && pid == _pid)
        {
            return true;
        }
        return false;
    }

}