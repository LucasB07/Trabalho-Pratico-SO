using System;
using System.Collections.Generic;

namespace DemandPagingSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            //MemoryManager memoryManager = new MemoryManager();
            int maxFrames = 3; // Definindo o número máximo de frames na memória

            int[] pageReferences = { 1, 2, 3, 1, 4, 2, 3, 2, 1, 4 };

            //FIFO
            MemoryManager memoryManagerFIFO = new MemoryManager();
            memoryManagerFIFO.maxFrames = maxFrames;

            int currentTimeFIFO = 0;

            Console.WriteLine("FIFO:");
            foreach (int pageId in pageReferences)
            {
                memoryManagerFIFO.accessPageFIFO(pageId, currentTimeFIFO);
                memoryManagerFIFO.PrintFrames();
                currentTimeFIFO++;
            }


            //LRU
            MemoryManager memoryManagerLRU = new MemoryManager();
            memoryManagerLRU.maxFrames = maxFrames;

            int currentTimeLRU = 0;

            Console.WriteLine("LRU:");
            foreach (int pageId in pageReferences)
            {
                memoryManagerLRU.accessPageLRU(pageId, currentTimeLRU);
                memoryManagerLRU.PrintFrames();
                currentTimeLRU++;
            }

        }

        class PageEntry
        {
            public int id;
            public int loadTime;//FIFO
            public int lastAccessedTime;//LRU
            public int nextAccessedTime;//OPT 
        }

        class MemoryManager
        {
            public void PrintFrames()
            {
                Console.Write(" ");
                foreach (var page in frames)
                {
                    Console.Write(page.id + " ");
                }
                Console.WriteLine();
            }

            public int maxFrames;
            public List<PageEntry> frames = new List<PageEntry>();

            //Metodo auxiliar para FIFO
            public void accessPageFIFO(int pageId, int currentTime)
            {
                PageEntry found = null;

                // Verifica se a página já está na memória
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
                    return;
                }

                //PAGE FAULT

                //ainda há espaço na memória
                if (frames.Count < maxFrames)
                {
                    PageEntry newPage = new PageEntry();
                    newPage.id = pageId;
                    newPage.loadTime = currentTime;
                    newPage.lastAccessedTime = currentTime;

                    frames.Add(newPage);
                    return;
                }

                // memória cheia, precisa substituir uma página
                PageEntry victim = frames[0];

                foreach (var page in frames)
                {
                    if (page.loadTime < victim.loadTime)
                    {
                        victim = page;
                    }
                }
                //remove a página vítima
                frames.Remove(victim);

                //adiciona a nova página
                PageEntry replacementPage = new PageEntry();
                replacementPage.id = pageId;
                replacementPage.loadTime = currentTime;
                replacementPage.lastAccessedTime = currentTime;

                frames.Add(replacementPage);
                return;

            }

            //Metodo auxiliar para LRU
            public void accessPageLRU(int pageId, int currentTime)
            {
                PageEntry found = null;

                // Verifica se a página já está na memória
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

                //PAGE FAULT

                //ainda há espaço na memória
                if (frames.Count < maxFrames)
                {
                    PageEntry newPage = new PageEntry();
                    newPage.id = pageId;
                    newPage.loadTime = currentTime;
                    newPage.lastAccessedTime = currentTime;

                    frames.Add(newPage);
                    return;
                }

                // memória cheia, precisa substituir uma página
                PageEntry victim = frames[0];

                foreach (var page in frames)
                {
                    if (page.lastAccessedTime < victim.lastAccessedTime)
                    {
                        victim = page;
                    }
                }
                //remove a página vítima
                frames.Remove(victim);

                //adiciona a nova página
                PageEntry replacementPage = new PageEntry();
                replacementPage.id = pageId;
                replacementPage.loadTime = currentTime;
                replacementPage.lastAccessedTime = currentTime;

                frames.Add(replacementPage);

            }
        }
    }
}