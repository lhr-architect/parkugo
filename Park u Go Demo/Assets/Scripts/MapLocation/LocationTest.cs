using UnityEngine;
using System.Collections;
using LitJson;
using UnityEngine.Networking;
using UnityEngine.Android;
using TMPro;
using System;

namespace Location
{
    public class LocationTest : MonoBehaviour
    {

        public GameObject playerImg;
        public int Time = 1;
        public TextMeshProUGUI tmpUI;
        public Transform NorthWestTransform, SouthEastTransform;

        public double windowLongtitude, windowLatitude;

        private MyLocation _location;

        private double WestBound = 116.305611f,
            EastBound = 116.314631f, 
            NorthBound = 39.996563f, 
            SouthBound = 39.992374f;

        private void OnLocationChanged(MyLocation.LocationChangedEventArgs e)
        {
            var pos = GetPosition(_location.Longitude, _location.Latitude);
            playerImg.transform.position = new Vector3(pos.x, pos.y, playerImg.transform.position.z);
          
        }

        //经度，纬度
        private Vector2 GetPosition(double longitude,double latitudes)
        {
            
            //longtitude -> Y, latitudes -> X
            double fracLongtitude = (longitude - WestBound) / (EastBound - WestBound);
            double fracLatitudes = (latitudes - SouthBound) / (NorthBound - SouthBound);

            float Y = Mathf.LerpUnclamped(NorthWestTransform.position.y, SouthEastTransform.position.y, (float)fracLongtitude);
            float X = Mathf.LerpUnclamped(SouthEastTransform.position.x, NorthWestTransform.position.x, (float)fracLatitudes);

            //Debug.Log($"X: {X} Y: {Y}");

            return new Vector2(X, Y);
        }

        private void changeText(string text)
        {
            tmpUI.text = text;
        }

        public void Start()
        {
            _location = new MyLocation();
            _location.LocationChanged += OnLocationChanged;

            if (Application.platform == RuntimePlatform.Android)
            {
                StartCoroutine(StartGPS());
            }

        }
           

        public void Update()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                var (x, y) = (Input.location.lastData.longitude, Input.location.lastData.latitude);
                double[] tmp = CoordinateConverter.Wgs84ToGcj02(x, y);
                _location.Longitude = (float)tmp[0];
                _location.Latitude = (float)tmp[1];

                tmpUI.text = $" 经纬度：({tmp[0]},{tmp[1]})";
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                _location.Longitude = windowLongtitude;
                _location.Latitude = windowLatitude;
            }

        }

        IEnumerator StartGPS()
        {
            //Unity给我们提供的一个相关权限类 Permission，可以判断当前相关权限是否开启
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                //如果没有开启就提示开启权限
                Permission.RequestUserPermission(Permission.FineLocation);
            }

            // 检查位置服务是否可用  
            if (!Input.location.isEnabledByUser)
            {
                changeText("位置服务不可用");
                yield break;
            }
            // 查询位置之前先开启位置服务
            Input.location.Start(3f, 1f);
            // 等待服务初始化  
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                changeText(Input.location.status.ToString() + ">>>" + maxWait.ToString());
                yield return new WaitForSeconds(1);
                maxWait--;
            }
            // 服务初始化超时  
            if (maxWait < 1)
            {
                changeText("服务初始化超时");
                yield break;
            }
            // 连接失败  
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                changeText("无法确定设备位置");
                yield break;
            }
            else
            {
                changeText("连接失败");

            }
            // 停止服务，如果没必要继续更新位置，（为了省电

        }
              //去高德地图开发者申请
        IEnumerator GetRequest(string uri)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();
                string[] pages = uri.Split('/');
                int page = pages.Length - 1;
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    tmpUI.text = webRequest.error;
                }
                else
                {
                    try
                    {
                        JsonData jd = JsonMapper.ToObject(webRequest.downloadHandler.text);
                        changeText(_location.ToString() + jd["regeocode"]["formatted_address"].ToString());
                       
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                    
                }
            }
        }
    }
}
