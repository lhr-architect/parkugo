using LitJson;
using System;
using System.Collections.Generic;

public class Command
{
    public int count;
    public List<int> pids;
    public List<string> userNames;
    public List<int> uiPosition;

   public Command() {
        count = 0;
        pids = new List<int>();
        userNames = new List<string>();
        uiPosition = new List<int>();
   }

    // 将Command对象序列化为JSON字符串
    public string ToJsonSelective(Landmark Lm, PlayerController.Party party)
    {
        if (party == PlayerController.Party.RED)
        {
            for (int i = 0; i < Lm.IndexMax; i++)
            {
                if (Lm.red[i] != null)
                {
                    count++;
                    pids.Add(Lm.red[i].pid);
                    userNames.Add(Lm.red[i].userName);
                    uiPosition.Add(i);
                }
            }
        }
        else
        {
            for (int i = 0; i < Lm.IndexMax; i++)
            {
                if (Lm.blue[i] != null)
                {
                    count++;
                    pids.Add(Lm.blue[i].pid);
                    userNames.Add(Lm.blue[i].userName);
                    uiPosition.Add(i);
                }
            }
        }
        return JsonMapper.ToJson(this);
    }
    public string ToJsonBoth(Landmark Lm)
    {
        for (int i = 0; i < Lm.IndexMax; i++)
        {
            if (Lm.red[i] != null)
            {
                count++;
                pids.Add(Lm.red[i].pid);
                userNames.Add(Lm.red[i].userName);
                uiPosition.Add(i);
            }

            if(Lm.blue[i] != null)
            {
                count++;
                pids.Add(Lm.blue[i].pid);
                userNames.Add(Lm.blue[i].userName);
                uiPosition.Add(i);
            }
        }
        return JsonMapper.ToJson(this);
    }

    public int GetIndex(string _userName,int _pid)
    {
        for (int i = 0; i < count; i++)
        {
            if (pids[i] == _pid && _userName == userNames[i])
            {
                return uiPosition[i];
            }
        }
        return -1;
    }

    public static Command FromJson(string json)
    {
        return JsonMapper.ToObject<Command>(json);
    }

    public Tuple<string,int> GetImgInfoAt(int UIposition)
    {
        for (int i = 0; i < count; i++)
        {
            if (uiPosition[i] == UIposition)
            {
                return new Tuple<string, int>(userNames[i], pids[i]);
            }
        }
        return null;
    }
}