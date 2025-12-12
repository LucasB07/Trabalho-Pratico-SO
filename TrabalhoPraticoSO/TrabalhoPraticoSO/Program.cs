using System;
using System.Collections.Generic;

namespace DemandPagingSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            int maxFrames = 3; // Definindo o número máximo de frames na memória

            int[] pageReferences = { 1, 2, 3, 1, 4, 2, 1, 3, 2, 1, 4, 3 };

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
            //print page fult do FIFO
            Console.WriteLine("Total Page Faults (FIFO): " + memoryManagerFIFO.pageFaultCount);


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
            //print page fult do LRU
            Console.WriteLine("Total Page Faults (LRU): " + memoryManagerLRU.pageFaultCount);

            //RAND
            MemoryManager memoryManagerRAND = new MemoryManager();
            memoryManagerRAND.maxFrames = maxFrames;

            int currentTimeRAND = 0;

            Console.WriteLine("RAND");
            foreach (int pageId in pageReferences)
            {
                memoryManagerRAND.accessPageRAND(pageId, currentTimeRAND);
                memoryManagerRAND.PrintFrames();
                currentTimeRAND++;
            }
            //print page fult do RAND
            Console.WriteLine("Total Page Faults (RAND): " + memoryManagerRAND.pageFaultCount);

            //OPT
            MemoryManager memoryManagerOPT = new MemoryManager();
            memoryManagerOPT.maxFrames = maxFrames;

            int currentTimeOPT = 0;

            Console.WriteLine("OPT:");
            foreach (int pageId in pageReferences)
            {
                memoryManagerOPT.accessPageOPT(pageId, currentTimeOPT, pageReferences);
                memoryManagerOPT.PrintFrames();
                currentTimeOPT++;
            }
            //print page fult do OPT
            Console.WriteLine("Total Page Faults (OPT): " + memoryManagerOPT.pageFaultCount);

        }

        class PageEntry
        {
            public int id;
            public int loadTime;//FIFO
            public int lastAccessedTime;//LRU
        }

        class MemoryManager 
        {
            private Random random = new Random();

            //contador de Page Faults
            public int pageFaultCount = 0;


            //print frames in memory
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


            //Metodo para FIFO
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
                    found.lastAccessedTime = currentTime;
                    return;
                }

                //PAGE FAULT
                pageFaultCount++;


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

            //Metodo para LRU
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
                pageFaultCount++;

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

            //Meto para RAND
            public void accessPageRAND(int pageId, int currentTime)
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
                pageFaultCount++;

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
                

                int victimIndex = random.Next(frames.Count);
                PageEntry victim = frames[victimIndex];

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

            //Metodo para OPT
            public void accessPageOPT(int pageId, int currentTime, int[] references)
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
                pageFaultCount++;

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
                int farthestAccessTime = -1;
                PageEntry victim = null;

                foreach (var page in frames)
                {
                    int nextUseTime = -1; // -1 significa que nunca será usada novamente

                    //Busca pelo próximo uso da página
                    for (int i = currentTime + 1; i < references.Length; i++)
                    {
                        if (references[i] == page.id)
                        {
                            nextUseTime = i;
                            break;
                        }
                    }

                    //se a página nunca será usada novamente, escolhe-a como vítima
                    if (nextUseTime == -1)
                    {
                        victim = page;
                        break;
                    }

                    //encontra a página com o uso mais distante no futuro
                    if (nextUseTime > farthestAccessTime)
                    {
                        farthestAccessTime = nextUseTime;
                        victim = page;
                    }
                }
                //remove a página vítima
                frames.Remove(victim);//

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