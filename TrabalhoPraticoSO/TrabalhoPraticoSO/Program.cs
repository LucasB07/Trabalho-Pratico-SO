using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DemandPagingSimulator
{
    class Program
    {

        static void Main(string[] args)
        {

            //parametros para teste
            int physicalMemorySize = 4096; // Tamanho da memória física
            int virtualMemorySize = 16384; // Tamanho da memória virtual
            string architecture = "x86"; // Arquitetura do sistema
            int pageNumber = 16; // Número de páginas

            //caucular o tamanho da página
            int pageSize = virtualMemorySize / pageNumber;

            Console.WriteLine("Tamanho da pagina inferido: " + pageSize + " bytes");

            //caucular o número de frames na memória física
            int maxFrames = physicalMemorySize / pageSize;
            Console.WriteLine("Número máximo de frames na memória: " + maxFrames);

            //caucular o tamanho do Swap necessário
            int swapSize = (pageNumber - maxFrames) * pageSize;
            Console.WriteLine("Tamanho do Swap necessário: " + swapSize + " bytes \n");


            int[] pageReferences = { 0, 1, 2, 3, 0, 1, 4, 0 };

            //FIFO
            MemoryManager memoryManagerFIFO = new MemoryManager();
            memoryManagerFIFO.maxFrames = maxFrames;

            int currentTimeFIFO = 0;

            Console.WriteLine("FIFO:");

            Stopwatch stopwatchFIFO = new Stopwatch();
            stopwatchFIFO.Start();

            foreach (int pageId in pageReferences)
            {
                memoryManagerFIFO.accessPageFIFO(pageId, currentTimeFIFO);
                currentTimeFIFO++;
            }

            stopwatchFIFO.Stop();
            double TimeInSecondsFIFO = stopwatchFIFO.Elapsed.TotalSeconds;

            Console.WriteLine("Tempo de Execução: " + Math.Round(TimeInSecondsFIFO));
            Console.WriteLine("Total Page Faults: " + memoryManagerFIFO.pageFaultCount);
            string swapStateFIFO = memoryManagerFIFO.CalculateFinalSwapState();
            Console.WriteLine("Estado final do Swap: " + swapStateFIFO);



            //LRU
            MemoryManager memoryManagerLRU = new MemoryManager();
            memoryManagerLRU.maxFrames = maxFrames;

            int currentTimeLRU = 0;

            Console.WriteLine("LRU:");

            Stopwatch stopwatchLRU = new Stopwatch();
            stopwatchLRU.Start();

            foreach (int pageId in pageReferences)
            {
                memoryManagerLRU.accessPageLRU(pageId, currentTimeLRU);
                currentTimeLRU++;
            }
            stopwatchLRU.Stop();
            double TimeInSecondsLRU = stopwatchLRU.Elapsed.TotalSeconds;

            Console.WriteLine("Tempo de Execução: " + Math.Round(TimeInSecondsLRU));
            Console.WriteLine("Total Page Faults: " + memoryManagerLRU.pageFaultCount);
            string swapStateLRU = memoryManagerLRU.CalculateFinalSwapState();
            Console.WriteLine("Estado final do Swap: " + swapStateLRU);

            //RAND
            MemoryManager memoryManagerRAND = new MemoryManager();
            memoryManagerRAND.maxFrames = maxFrames;

            int currentTimeRAND = 0;

            Console.WriteLine("RAND");

            Stopwatch stopwatchRAND = new Stopwatch();
            stopwatchRAND.Start();

            foreach (int pageId in pageReferences)
            {
                memoryManagerRAND.accessPageRAND(pageId, currentTimeRAND);
                currentTimeRAND++;
            }

            stopwatchRAND.Stop();
            double TimeInSecondsRAND = stopwatchRAND.Elapsed.TotalSeconds;

            Console.WriteLine("Tempo de Execução: " + Math.Round(TimeInSecondsRAND));
            Console.WriteLine("Total Page Faults: " + memoryManagerRAND.pageFaultCount);
            string swapStateRAND = memoryManagerRAND.CalculateFinalSwapState();
            Console.WriteLine("Estado final do Swap: " + swapStateRAND);

            //OPT
            MemoryManager memoryManagerOPT = new MemoryManager();
            memoryManagerOPT.maxFrames = maxFrames;

            int currentTimeOPT = 0;

            Console.WriteLine("OPT:");

            Stopwatch stopwatchOPT = new Stopwatch();
            stopwatchOPT.Start();

            foreach (int pageId in pageReferences)
            {
                memoryManagerOPT.accessPageOPT(pageId, currentTimeOPT, pageReferences);
                currentTimeOPT++;
            }

            stopwatchOPT.Stop();
            double TimeInSecondsOPT = stopwatchOPT.Elapsed.TotalSeconds;

            Console.WriteLine("Tempo de Execução: " + Math.Round(TimeInSecondsOPT));
            Console.WriteLine("Total Page Faults: " + memoryManagerOPT.pageFaultCount);
            string swapStateOPT = memoryManagerOPT.CalculateFinalSwapState();
            Console.WriteLine("Estado final do Swap: " + swapStateOPT);

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

            //conjunto de todas as páginas acessadas
            public HashSet<int> allAccessedPages = new HashSet<int>();

            //metodo para caucular estado final do Swap
            public string CalculateFinalSwapState() 
            {
                // Coleção para armazenar as páginas que estão no Swap
                HashSet<int> swapPages = new HashSet<int>();

                //Obtém as páginas que estão atualmente na memória física
                HashSet<int> pagesInMemory = new HashSet<int>();
                foreach (var page in frames)
                {
                    pagesInMemory.Add(page.id);
                }

                //Itera por todas as páginas referenciadas
                foreach (int pageId in allAccessedPages)
                {
                    if (!pagesInMemory.Contains(pageId))
                    {
                        swapPages.Add(pageId);
                    }
                }
                return string.Join(" ", swapPages);
            }

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
                allAccessedPages.Add(pageId);
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
                allAccessedPages.Add(pageId);
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
                allAccessedPages.Add(pageId);
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
                allAccessedPages.Add(pageId);
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
                int farthestAccessTime = -1;

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