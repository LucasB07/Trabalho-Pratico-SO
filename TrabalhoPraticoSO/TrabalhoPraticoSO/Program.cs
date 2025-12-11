using System;
using System.Collections.Generic;

namespace DemandPagingSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            MemoryManager memoryManager = new MemoryManager();
            memoryManager.maxFrames = 3; // Definindo o número máximo de frames na memória

            int[] pageReferences = { 1, 2, 3, 1, 4, 2, 3, 2, 1, 4 };
            int currentTime = 0;

            foreach (int pageId in pageReferences)
            {
                memoryManager.accessPage(pageId, currentTime);
                currentTime++;

                Console.Write("Acesso à página " + pageId + ": ");
                memoryManager.PrintFrames();
            }
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
            Console.Write("Frames na memória: ");
            foreach (var page in frames)
            {
                Console.Write(page.id + " ");
            }
            Console.WriteLine();
        } 

        public int maxFrames;
        public List<PageEntry> frames = new List<PageEntry>();


        public void accessPage(int pageId, int currentTime) 
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

            // Página encontrada na memória, atualiza o tempo de acesso
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

            //memória cheia, precisa substituir uma página
            //FIFO

            //encontrar a página com menor loadTime

            PageEntry victim = frames[0];

            foreach(var page in frames)
            {
                if (page.loadTime < victim.loadTime)
                {
                    victim = page;
                }
            }

            //Remover essa página
            frames.Remove(victim);

            //inserir a nova página na memória
            PageEntry replacementPage = new PageEntry();
            replacementPage.id = pageId;
            replacementPage.loadTime = currentTime;
            replacementPage.lastAccessedTime = currentTime;

            frames.Add(replacementPage);
        }

    }
}