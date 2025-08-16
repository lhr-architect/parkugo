using UnityEngine;
using System;

public class MyLocation 
{
    private double _longitude;
    private double _latitude;



    public override string ToString()
    {

        return $"��γ�� ({_longitude},{_latitude})\n";
    }

    // ���ԣ����ڻ�ȡ�����þ���
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

    // ���ԣ����ڻ�ȡ������γ��
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

    // �¼�����λ�ñ仯ʱ����
    public event Action<LocationChangedEventArgs> LocationChanged;

    // �����¼�
    protected virtual void OnLocationChanged()
    {
        LocationChanged?.Invoke(new LocationChangedEventArgs(_longitude, _latitude));
    }

    // ����changeText����

    // �¼�������
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