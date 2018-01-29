
namespace MemoryAllocationProject_Console
{
    /*
     * Notes:
     * Processes with pid +ve pid are ones to be allocated,
     * while ones with -ve pid are pre allocated ones (blocks that aren't holes) 
     */

    class Process
    {
        private int pid;
        private int size;
        private bool isAlloc;

        public int PID
        {
            get { return pid; }
            set { this.pid = value; }
        }

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public bool IsAlloc
        {
            get { return this.isAlloc; }
            set { this.isAlloc = value; }
        }


        //Constructors

        public Process()
        {
            this.pid = 0;
            this.size = 0;
            this.isAlloc = false;
        }

        public Process(int pid, int size)
        {
            this.pid = pid;
            this.size = size;
        }

        public Process(int pid, int size, bool isAlloc)
        {
            this.pid = pid;
            this.size = size;
            this.isAlloc = isAlloc;
        }

        public override string ToString()
        {
            return $"{nameof(pid)}: {pid}, {nameof(size)}: {size}";
        }

    }
}
