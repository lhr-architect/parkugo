using System;


public class CoordinateConverter
{
    private const double X_Pi = 3.14159265358979324 * 3000.0 / 180.0;
    private const double Pi = 3.1415926535897932384626; // π
    private const double A = 6378245.0; // 长半轴
    private const double Ee = 0.00669342162296594323; // 扁率


    public static double[] Wgs84ToGcj02(double lng, double lat)
    {
        if (OutOfChina(lng, lat)) // 判断是否在国内
        {
            return new double[] { lng, lat };
        }
        double dlat = TransformLat(lng - 105.0, lat - 35.0);
        double dlng = TransformLng(lng - 105.0, lat - 35.0);
        double radLat = lat / 180.0 * Pi;
        double magic = Math.Sin(radLat);
        magic = 1 - Ee * magic * magic;
        double sqrtMagic = Math.Sqrt(magic);
        dlat = (dlat * 180.0) / ((A * (1 - Ee)) / (magic * sqrtMagic) * Pi);
        dlng = (dlng * 180.0) / (A / sqrtMagic * Math.Cos(radLat) * Pi);
        double mgLat = lat + dlat;
        double mgLng = lng + dlng;
        return new double[] { mgLng, mgLat };
    }

    
    public static double[] Gcj02ToWgs84(double lng, double lat)
    {
        if (OutOfChina(lng, lat))
        {
            return new double[] { lng, lat };
        }
        double dlat = TransformLat(lng - 105.0, lat - 35.0);
        double dlng = TransformLng(lng - 105.0, lat - 35.0);
        double radLat = lat / 180.0 * Pi;
        double magic = Math.Sin(radLat);
        magic = 1 - Ee * magic * magic;
        double sqrtMagic = Math.Sqrt(magic);
        dlat = (dlat * 180.0) / ((A * (1 - Ee)) / (magic * sqrtMagic) * Pi);
        dlng = (dlng * 180.0) / (A / sqrtMagic * Math.Cos(radLat) * Pi);
        double mgLat = lat + dlat;
        double mgLng = lng + dlng;
        return new double[] { lng * 2 - mgLng, lat * 2 - mgLat };
    }

    private static double TransformLat(double lng, double lat)
    {
        double ret = -100.0 + 2.0 * lng + 3.0 * lat + 0.2 * lat * lat +
            0.1 * lng * lat + 0.2 * Math.Sqrt(Math.Abs(lng));
        ret += (20.0 * Math.Sin(6.0 * lng * Pi) + 20.0 *
                Math.Sin(2.0 * lng * Pi)) * 2.0 / 3.0;
        ret += (20.0 * Math.Sin(lat * Pi) + 40.0 *
                Math.Sin(lat / 3.0 * Pi)) * 2.0 / 3.0;
        ret += (160.0 * Math.Sin(lat / 12.0 * Pi) + 320 *
                Math.Sin(lat * Pi / 30.0)) * 2.0 / 3.0;
        return ret;
    }

    private static double TransformLng(double lng, double lat)
    {
        double ret = 300.0 + lng + 2.0 * lat + 0.1 * lng * lng +
            0.1 * lng * lat + 0.1 * Math.Sqrt(Math.Abs(lng));
        ret += (20.0 * Math.Sin(6.0 * lng * Pi) + 20.0 *
                Math.Sin(2.0 * lng * Pi)) * 2.0 / 3.0;
        ret += (20.0 * Math.Sin(lng * Pi) + 40.0 *
                Math.Sin(lng / 3.0 * Pi)) * 2.0 / 3.0;
        ret += (150.0 * Math.Sin(lng / 12.0 * Pi) + 300.0 *
                Math.Sin(lng / 30.0 * Pi)) * 2.0 / 3.0;
        return ret;
    }

    private static bool OutOfChina(double lng, double lat)
    {
        if (lng < 72.004 || lng > 137.8347)
            return true;
        if (lat < 0.8293 || lat > 55.8271)
            return true;
        return false;
    }


}