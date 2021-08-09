using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace server
{
    /// <summary>
    /// Summary description for ClientCollection.
    /// </summary>
    public class ClientCollection : System.Collections.CollectionBase
    {
        public ClientCollection()
        {
        }

        public void AddClient(Object client)
        {
            List.Add(client);
        }

        public int GetCount()
        {
            return List.Count;
        }

        public Object GetAt(int npos)
        {
            Object obj = List[npos];

            if (npos > GetCount())
                return null;

            return obj;
        }

        public ArrayList SearchFiles(string pat)
        {
            ArrayList objList = new ArrayList();
            for (int i = 0; i < List.Count; i++)
            {
                ClientInfo obj = (ClientInfo)List[i];
                int pos = obj.sharedfileName.IndexOf(pat);
                if (pos >= 0)
                    objList.Add(obj);
            }
            return objList;
        }
    }
}
