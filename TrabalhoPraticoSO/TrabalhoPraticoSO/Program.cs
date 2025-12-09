using System;
using System.Collections.Generic;

namespace DemandPagingSimulator
{

    class PageEntry
    {
        public int id;
        public int loadTime;//FIFO
        public int lastAccessedTime;//LRU
        public int nextAccessedTime;//OPT
        
    }

    class MemoryManager
    {
        public int maxFrames;
        public List<PageEntry> frames = new List<PageEntry>();

        void accessPage(int pageId, int currentTime) 
        {
            PageEntry found = null;

            foreach (var page in frames)
            {
                if (page.id == pageId)
                {
                    found = page;
                    break;
                }
            }

            if (found != null)
            {
                found.lastAccessedTime = currentTime;
                return;
            }

            if (frames.Count < maxFrames) 
            {
                PageEntry newPage = new PageEntry();
                newPage.id = pageId;
                newPage.loadTime = currentTime;
                newPage.lastAccessedTime = currentTime;

                frames.Add(newPage);
            }
        }

    }
}