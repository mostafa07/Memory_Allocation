using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Memory block is assumed to start at address 0,
 * and of size 1024 (1KB)
 * 
 * Assumed user will not enter intersecting holes
 * ex: h1 (stAdd= 200, size= 100), h2(stAdd= 250, size= 100)
 */
  
namespace MemoryAllocationProject_Console
{

    class Program
    {
        private const int MEMORY_LIMIT = 1023;
        private const int HOLES_LIMIT = 8;
        private const int PROCESSES_LIMIT = 8;
        private static short algChoice;

        public static int numOfHoles;
        public static int numOfProcesses;
        public static List<Block> memory;

        public static List<Process> initialProcesses;
        public static List<Process> readyList;


        static void Main(string[] args)
        {
            //welcome message
            Console.WriteLine("\n\n\t\t\t\t\t||| Memory Allocation Project |||\t\t\t\n\n");

            //getting holes info and checking for some user input and logical errors
            while (true)
            {
                getHolesInfo();
                if (totalHolesSize() > MEMORY_LIMIT)
                    Console.WriteLine("\nError: Holes total size is larger than memory block limit: ({0})\n" +
                                      "Try again:", MEMORY_LIMIT);
                else
                    break;
            }

            //Getting processes info and checking for some user input and logical errors
            while (true)
            {
                getProcessesInfo();
                if (anyProcessLarger())
                    Console.WriteLine("\nError: Processes' sizes can't be larger than memory block limit: ({0})\n" +
                                      "Try again:", MEMORY_LIMIT);
                else
                    break;
            }

            //get choice of allocation algorithm
            Console.WriteLine("\nChoose method of allocation of the following:\n" +
                              "\n\t1. First Fit\n\t2. Best Fit\n\t3. Worst Fit");
            while (true)
            {
                short.TryParse(Console.ReadLine(), out algChoice);
                if (algChoice < 1 || algChoice > 3)
                    Console.WriteLine("Retry:");
                else
                    break;
            }
            
            //form the readyList from the initial list of processes
            readyList = new List<Process>(initialProcesses);

            //sort memory according to starting address
            sortMemoryByAddress();

            //combine consecutive holes initially
            combineHoles();

            //start allocation of processes in readyList
            startAllocation(algChoice);


            //terminating program
            Console.WriteLine("\nPress any key to terminate program!\n");
            Console.ReadKey();
        }


        /*----------------------------------------------------------------------*/
        
            //Helping methods

        private static void getHolesInfo()
        {
            int n;
            Console.WriteLine("\nEnter number of holes:");
            while (true)
            {
                int.TryParse(Console.ReadLine(), out n);
                if (n > HOLES_LIMIT || n < 1)
                    Console.WriteLine("\nError: Number of holes must be positive and smaller than or equal {0}\n" +
                                      "Retry:", HOLES_LIMIT);
                else
                    break;
            }
            numOfHoles = n;

            memory = new List<Block>();

            Console.WriteLine("\nEnter info of each hole (Starting Address & Size):");
            for (int i = 0; i < numOfHoles; ++i)
            {
                int stAdd, sz;
                while (true)
                {
                    Console.Write("\tHole {0} starting address:\t", i+1);
                    int.TryParse(Console.ReadLine(), out stAdd);
                    if (stAdd < 0 || stAdd > MEMORY_LIMIT)
                        Console.WriteLine("\nError: Starting Address must be in the range from 0 to {0}\n" +
                                          "Retry:", MEMORY_LIMIT);
                    else
                        break;
                }

                while (true)
                {
                    Console.Write("\tHole {0} size:\t\t\t", i+1);
                    int.TryParse(Console.ReadLine(), out sz);
                    if (sz < 0 || sz > MEMORY_LIMIT)
                        Console.WriteLine("\nError: Size must be in the range from 0 to {0}" +
                                          "\nRetry:", MEMORY_LIMIT);
                    else
                        break;
                }

                memory.Add(new Block(0, stAdd, sz));
            }
        }

        private static void getProcessesInfo()
        {
            int n;
            Console.WriteLine("\nEnter number of processes:");
            while (true)
            {
                int.TryParse(Console.ReadLine(), out n);
                if (n > PROCESSES_LIMIT || n < 1)
                    Console.WriteLine("\nError: Number of processes must be positive and smaller than {0}\n" +
                                      "Retry:", PROCESSES_LIMIT);
                else
                    break;
            }
            numOfProcesses = n;

            initialProcesses = new List<Process>();

            Console.WriteLine("\nEnter info of each process (Size):");
            for (int i = 0; i < numOfProcesses; ++i)
            {
                int sz;
                while (true)
                {
                    Console.Write("\tP{0} size:\t", i+1);
                    int.TryParse(Console.ReadLine(), out sz);
                    if (sz < 0 || sz > MEMORY_LIMIT)
                        Console.WriteLine("\nError: Size must be in the range from 0 to {0}" +
                                          "\nRetry:", MEMORY_LIMIT);
                    else
                        break;
                }

                initialProcesses.Add(new Process(i+1, sz));
            }
        }

        private static int totalHolesSize()
        {
            int totalSize = 0;
            foreach (Block block in memory)
                if (block.isFree())
                    totalSize += block.Size;
            return totalSize;
        }

        private static int totalProcessesSize()
        {
            int totalSize = 0;
            foreach (Block block in memory)
                if (block.PID > 0)
                    totalSize += block.Size;
            return totalSize;
        }

        private static bool anyProcessLarger()
        {
            foreach (Process process in initialProcesses)
                if (process.Size > MEMORY_LIMIT)
                    return true;
            return false;
        }

        private static void sortMemoryByAddress()
        {
            memory = memory.OrderBy(Block => Block.StartAddress).ToList();
        }

        private static void sortMemoryBySize()
        {
            memory = memory.OrderBy(Block => Block.Size).ToList();
        }

        private static void sortMemoryBySizeDescending()
        {
            memory = memory.OrderByDescending(Block => Block.Size).ToList();
        }

        private static void combineHoles()
        {
            for (int i = 0; i < memory.Count - 1; ++i)
            {
                if (memory[i].EndAddress() + 1 == memory[i+1].StartAddress 
                    && memory[i].isFree() && memory[i+1].isFree())
                {
                    memory[i].Size += memory[i + 1].Size;
                    memory.RemoveAt(i+1);
                }
            }
        }

        private static void startAllocation(short alg)
        {
            switch (alg)
            {
                case (1):
                {
                    sortMemoryByAddress();
                    break;
                }
                case (2):
                {
                    sortMemoryBySize();
                    break;
                }
                case (3):
                {
                    sortMemoryBySizeDescending();
                    break;
                }
            }

            foreach (Process process in readyList)
            {
                bool pAlloc = allocate(process);
                if (pAlloc)
                {
                    Console.WriteLine("\nMemory after allocation of P{0}\n", process.PID);
                    displayMemory();
                }
            }

            //handling non allocated processes
            foreach (Process process in readyList)
            {
                if (!process.IsAlloc)
                {
                    Console.WriteLine("\nP{0} is not allocated!\n" +
                                      "Choose a process to be replaced in memory:\n", process.PID);
                    int pidToReplace;
                    while (true)
                    {
                        Int32.TryParse(Console.ReadLine(), out pidToReplace);
                        if (pidToReplace < 1 || pidToReplace > numOfProcesses)
                            Console.WriteLine("\nError: Process ID to be swapped out must match an allocated" +
                                              "process ID in memory\nRetry:\n");
                        else
                            break;
                    }

                    foreach (Block block in memory)
                    {
                        if (block.PID == pidToReplace)
                        {
                            block.swapOut();      //deallocate process and free a hole in its place
                            Console.WriteLine("\nMemory after swapping out P{0}:\n", pidToReplace);
                            displayMemory();

                            combineHoles();
                            bool pAlloc = allocate(process);

                            //if it is allocated after swapping out
                            if (pAlloc)
                            {
                                Console.WriteLine("\nMemory after swapping out P{0} and allocating P{1}:\n",
                                    pidToReplace, process.PID);
                                displayMemory();
                            }
                            //if still not allocated, try compaction
                            else
                            {
                                Console.WriteLine("\nP{0} could not be allocated in P{1} place\n" +
                                                  "Trying compaction...\n", process.PID, pidToReplace);

                                compactMemory();
                                pAlloc = allocate(process);

                                //if allocated after compaction
                                if (pAlloc)
                                {
                                    Console.WriteLine("\nMemory after compaction and allocation of P{0}\n",
                                        process.PID);
                                    displayMemory();
                                }
                                //if not allocated after compaction as well (can't be allocated)
                                else
                                {
                                    Console.WriteLine("\nP{0} could not be allocated in memory after\n" +
                                                      "swapping out P{1} and memory compaction\n",
                                        process.PID, pidToReplace);
                                    displayMemory();
                                }
                            }

                            break;  //reached pidToReplace and made some operations
                        }
                    }
                }

            }

        }

        /*
        private static void firstFit()
        {
            sortMemoryByAddress();
            foreach (Process process in readyList)
            {
                bool pAlloc = allocate(process, algChoice);
                if (pAlloc)
                {
                    Console.WriteLine("\nMemory after allocation of P{0}\n", process.PID);
                    displayMemory();
                }
            }

            //handling non allocated processes
            foreach (Process process in readyList)
            {
                if (!process.IsAlloc)
                {
                    Console.WriteLine("\nP{0} is not allocated!\n" +
                                        "Choose a process to be replaced in memory:\n", process.PID);
                    int pidToReplace;
                    while (true)
                    {
                        Int32.TryParse(Console.ReadLine(), out pidToReplace);
                        if (pidToReplace < 1 || pidToReplace > numOfProcesses)
                            Console.WriteLine("\nError: Process ID to be swapped out must match an allocated" +
                                              "process ID in memory\nRetry:\n");
                        else
                            break;
                    }
                    
                    foreach (Block block in memory)
                    {
                        if (block.PID == pidToReplace)
                        {
                            block.swapOut();      //deallocate process and free a hole in its place
                            Console.WriteLine("\nMemory after swapping out P{0}:\n", pidToReplace);
                            displayMemory();

                            combineHoles();
                            bool pAlloc = allocate(process, algChoice);

                            //if it is allocated after swapping out
                            if (pAlloc)
                            {
                                Console.WriteLine("\nMemory after swapping out P{0} and allocating P{1}:\n",
                                                   pidToReplace, process.PID);
                                displayMemory();
                            }
                            //if still not allocated, try compaction
                            else
                            {
                                Console.WriteLine("\nP{0} could not be allocated in P{1} place\n" +
                                                  "Trying compaction...\n", process.PID, pidToReplace);

                                compactMemory();
                                pAlloc = allocate(process, algChoice);

                                //if allocated after compaction
                                if (pAlloc)
                                {
                                    Console.WriteLine("\nMemory after compaction and allocation of P{0}\n",
                                                      process.PID);
                                    displayMemory();
                                }
                                //if not allocated after compaction as well (can't be allocated)
                                else
                                {
                                    Console.WriteLine("\nP{0} could not be allocated in memory after\n" +
                                                      "swapping out P{1} and memory compaction\n",
                                                       process.PID, pidToReplace);
                                    displayMemory();
                                }
                            }
                            
                            break;  //reached pidToReplace and made some operations
                        }
                    }
                }

            }

        }   //firstFit()

        private static void BestFit()
        {
            
        }   //bestFit()

        private static void WorstFit()
        {

        }   //worstFit()
        */

        private static bool allocate(Process process)
        {
            foreach (Block block in memory)
            {
                if (process.Size <= block.Size && block.isFree())   //if process fits
                {
                    if (process.Size == block.Size)    //if process fits perfectly
                    {
                        block.PID = process.PID;
                    }
                    else
                    {
                        memory.Add(new Block(process.PID, block.StartAddress, process.Size));
                        block.StartAddress += process.Size;
                        block.Size -= process.Size;
                    }
                    process.IsAlloc = true;

                    switch (algChoice)
                    {
                        case (1):
                        {
                            sortMemoryByAddress();
                                break;
                        }
                        case (2):
                        {
                            sortMemoryBySize();
                            break;
                        }
                        case (3):
                        {
                            sortMemoryBySizeDescending();
                            break;
                        }
                    }
                    
                    return true;
                }
            }

            return false;
        }

        private static void compactMemory()
        {
            int holesSize = totalHolesSize();
            int userProcessesSize = totalProcessesSize();
            int preAllocProcessesSize = MEMORY_LIMIT - holesSize - userProcessesSize;

            //put pre allocated processes in the start (do nothing really)
            //put user processes after them
            //combine holes at the end as one big hole

            List<Block> tmpMemory = new List<Block>();

            int address = preAllocProcessesSize + 1;
            foreach (Block block in memory)
            {
                if (block.PID > 0)
                {
                    tmpMemory.Add(new Block(block.PID, address, block.Size));
                    address += block.Size;
                }
            }

            tmpMemory.Add(new Block(0, address, holesSize));
            memory = tmpMemory;
        }

        private static void displayMemory()
        {
            sortMemoryByAddress();
            foreach (Block block in memory)
                Console.WriteLine(block.ToString());
            Console.WriteLine("\n________________________________\n");

            switch (algChoice)
            {
                case (1):
                {
                    sortMemoryByAddress();
                    break;
                }
                case (2):
                {
                    sortMemoryBySize();
                    break;
                }
                case (3):
                {
                    sortMemoryBySizeDescending();
                    break;
                }
            }
        }

        
    }   //class Program
}
