using System;
using UnityEngine;

namespace VerdantLog.Data
{
    [Serializable]
    public class LogEntry
    {
        public string plantID;
        public bool isDiscovered;
        public DateTime discoveryDate;
        public int timesHarvested;
        
        public LogEntry(string plantID)
        {
            this.plantID = plantID;
            this.isDiscovered = false;
            this.timesHarvested = 0;
        }
        
        public void Discover()
        {
            if (!isDiscovered)
            {
                isDiscovered = true;
                discoveryDate = DateTime.Now;
            }
        }
    }
}