
namespace MemoryAllocationProject_Console
{
    /*
     * Notes:
     * Blocks with pid = 0 are holes / free blocks,
     * while ones with -ve pid are pre allocated processes (not holes),
     * and ones with +ve pid are ones allocated by this program
     */

    class Block
    {
        private int pid;
        private int startAddress;
        private int size;

        public int PID
        {
            get { return this.pid; }
            set { this.pid = value; }
        }

        public int StartAddress
        {
            get { return this.startAddress; }
            set { this.startAddress = value; }
        }

        public int Size
        {
            get { return this.size; }
            set { this.size = value; }
        }


        //Constructors

        public Block()
        {
            this.pid = 0;
            this.startAddress = 0;
            this.size = 0;
        }

        public Block(int pid, int startAddress, int size)
        {
            this.pid = pid;
            this.startAddress = startAddress;
            this.size = size;
        }


        //Other methods

        public bool isFree()
        {
            return this.pid == 0;
        }

        public void swapOut()
        {
            if (this.PID > 0)
                this.PID = 0;
        }

        public int EndAddress()
        {
            return this.startAddress + this.size - 1;
        }

        public override string ToString()
        {
            return $"{nameof(pid)}: {pid}, {nameof(startAddress)}: {startAddress}, {nameof(size)}: {size}";
        }
    }
}
