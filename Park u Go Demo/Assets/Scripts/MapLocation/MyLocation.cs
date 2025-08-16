using UnityEngine;
using System;

public class MyLocation 
{
    private double _longitude;
    private double _latitude;



    public override string ToString()
    {

        return $"经纬度 ({_longitude},{_latitude})\n";
    }

    // 属性，用于获取和设置经度
    public double Longitude
    {
        get { return _longitude; }
        set
        {
            if (_longitude != value)
            {
                _longitude = value;
                OnLocationChanged();
            }
        }
    }

    // 属性，用于获取和设置纬度
    public double Latitude
    {
        get { return _latitude; }
        set
        {
            if (_latitude != value)
            {
                _latitude = value;
                OnLocationChanged();
            }
        }
    }

    // 事件，当位置变化时触发
    public event Action<LocationChangedEventArgs> LocationChanged;

    // 调用事件
    protected virtual void OnLocationChanged()
    {
        LocationChanged?.Invoke(new LocationChangedEventArgs(_longitude, _latitude));
    }

    // 调用changeText方法

    // 事件参数类
    public class LocationChangedEventArgs : EventArgs
    {
        public double Longitude { get; }
        public double Latitude { get; }

        public LocationChangedEventArgs(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }
    }
}