using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace Sojaner.MemoryScanner
{
    public class RegularMemoryScan
    {
        #region Constant fields
        //Maximum memory block size to read in every read process.

        //Experience tells me that,
        //if ReadStackSize be bigger than 20480, there will be some problems
        //retrieving correct blocks of memory values.
        const Int64 ReadStackSize = 20480;

        #endregion
        
        #region Global fields
        //Instance of ProcessMemoryReader class to be used to read the memory.
        ProcessMemoryReader reader;

        //Start and End addresses to be scaned.
        IntPtr baseAddress;
        IntPtr lastAddress;

        //New thread object to run the scan in
        Thread thread; 
        #endregion

        #region Delegate and Event objects
        //Delegate and Event objects for raising the ScanProgressChanged event.
        public delegate void ScanProgressedEventHandler(object sender, ScanProgressChangedEventArgs e);
        public event ScanProgressedEventHandler ScanProgressChanged;

        //Delegate and Event objects for raising the ScanCompleted event.
        public delegate void ScanCompletedEventHandler(object sender, ScanCompletedEventArgs e);
        public event ScanCompletedEventHandler ScanCompleted;

        //Delegate and Event objects for raising the ScanCanceled event.
        public delegate void ScanCanceledEventHandler(object sender, ScanCanceledEventArgs e);
        public event ScanCanceledEventHandler ScanCanceled; 
        #endregion

        #region Methods
        //Class entry point.
        //The process, StartAddress and EndAdrress will be defined in the class definition.
        public RegularMemoryScan(Process process, Int64 StartAddress, Int64 EndAddress)
        {
            //Set the reader object an instant of the ProcessMemoryReader class.
            reader = new ProcessMemoryReader();

            //Set the ReadProcess of the reader object to process passed to this method
            //to define the process we are going to scan its memory.
            reader.ReadProcess = process;

            //Set the Start and End addresses of the scan to what is wanted.
            baseAddress = (IntPtr)StartAddress;
            lastAddress = (IntPtr)EndAddress;//The scan starts from baseAddress,
            //and progresses up to EndAddress.
        }

        #region Public methods

        //Get ready to scan the memory for the string value.
        public void StartScanForString(string stringValue)
        {
            //Check if the thread is already defined or not.
            if (thread != null)
            {
                //If the thread is already defined and is Alive,
                if (thread.IsAlive)
                {
                    //raise the event that shows that the last scan task is canceled
                    //(because a new task is going to be started as wanted),
                    ScanCanceledEventArgs cancelEventArgs = new ScanCanceledEventArgs();
                    ScanCanceled(this, cancelEventArgs);

                    //and then abort the alive thread and so cancel last scan task.
                    thread.Abort();
                }
            }
            //Set the thread object as a new instant of the Thread class and pass
            //a new ParameterizedThreadStart class object with the needed method passed to it
            //to run in the new thread.
            thread = new Thread(new ParameterizedThreadStart(StringScanner));

            //Start the new thread and set the 32 bit value to look for.
            thread.Start(stringValue);
        }

        //Cancel the scan started.
        public void CancelScan()
        {
            //Raise the event that shows that the last scan task is canceled as user asked,
            ScanCanceledEventArgs cancelEventArgs = new ScanCanceledEventArgs();
            ScanCanceled(this, cancelEventArgs);

            //and then abort the thread that scanes the memory.
            thread.Abort();
        } 
        #endregion

        #region Private methods

        private void StringScanner(object stringObject)
        {
            //Get the value out of the object to look for it.
            string stringValue = (string)stringObject;
            
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(stringValue);
            int bytesCount = bytes.Length;

            //The difference of scan start point in all loops except first loop,
            //that doesn't have any difference, is type's Bytes count minus 1.
            int arraysDifference = bytesCount - 1;

            //prealloc buffer
            byte[] buffer = new byte[ReadStackSize + arraysDifference];

            //Define a List object to hold the found memory addresses.
            List<Int64> finalList = new List<Int64>();

            //Open the pocess to read the memory.
            reader.OpenProcess();

            //Create a new instant of the ScanProgressEventArgs class to be used to raise the
            //ScanProgressed event and pass the percentage of scan, during the scan progress.
            ScanProgressChangedEventArgs scanProgressEventArgs;

            //Calculate the size of memory to scan.
            Int64 memorySize = (Int64)((Int64)lastAddress - (Int64)baseAddress);
            bool found = false;

            //If more that one block of memory is requered to be read,
            if (memorySize >= ReadStackSize)
            {
                //Count of loops to read the memory blocks.
                Int64 loopsCount = memorySize / ReadStackSize;

                //Look to see if there is any other bytes let after the loops.
                Int64 outOfBounds = memorySize % ReadStackSize;

                //Set the currentAddress to first address.
                Int64 currentAddress = (Int64)baseAddress;

                //This will be used to check if any bytes have been read from the memory.
                Int64 bytesReadSize;

                //Set the size of the bytes blocks.
                Int64 bytesToRead = ReadStackSize;

                //An array to hold the bytes read from the memory.
                byte[] array;

                //Progress percentage.
                int progress;

                for (Int64 i = 0; i < loopsCount; i++)
                {
                    if (found)
                        break;

                    //Calculte and set the progress percentage.
                    progress = (int)(((double)(currentAddress - (Int64)baseAddress) / (double)memorySize) * 100d);

                    //Prepare and set the ScanProgressed event and raise the event.
                    scanProgressEventArgs = new ScanProgressChangedEventArgs(progress);
                    ScanProgressChanged(this, scanProgressEventArgs);

                    //Read the bytes from the memory.
                    reader.ReadProcessMemory((IntPtr)currentAddress,(UInt64)bytesToRead, out bytesReadSize, buffer);
                    array = buffer;


                    //If any byte is read from the memory (there has been any bytes in the memory block),
                    if (bytesReadSize > 0)
                    {
                        //Loop through the bytes one by one to look for the values.
                        for (Int64 j = 0; j < array.Length - arraysDifference; j++)
                        {
                            if (found)
                                break;
                            int matches = 0;
                            for (Int64 b = 0; b < bytes.Length && (j+b) < array.Length - arraysDifference; b++)
                            {
                                if (bytes[b] != array[j + b])
                                {
                                    found = false;
                                    break;
                                }
                                else
                                {
                                    matches++;
                                }
                            }
                            if (matches == bytes.Length)
                            {


                                Debug.WriteLine("Found string: " + System.Text.Encoding.ASCII.GetString(array, (int)j, bytes.Length).Replace(Convert.ToChar(0x0).ToString(), " "));
                                finalList.Add(j + (Int64)currentAddress);
                                Debug.Flush();
                                found = true;
                                break;
                            }

                        }
                    }
                    //Move currentAddress after the block already scaned, but
                    //move it back some steps backward (as much as arraysDifference)
                    //to avoid loosing any values at the end of the array.
                    currentAddress += array.Length - arraysDifference;

                    //Set the size of the read block, bigger, to  the steps backward.
                    //Set the size of the read block, to fit the back steps.
                    bytesToRead = ReadStackSize + arraysDifference;
                }
                //If there is any more bytes than the loops read,
                if (!found && outOfBounds > 0)
                {
                    //Read the additional bytes.
                    reader.ReadProcessMemory((IntPtr)currentAddress, (UInt64)((Int64)lastAddress - currentAddress), out bytesReadSize, buffer);
                    byte[] outOfBoundsBytes = buffer;

                    //If any byte is read from the memory (there has been any bytes in the memory block),
                    if (bytesReadSize > 0)
                    {
                        //Loop through the bytes one by one to look for the values.
                        for (Int64 j = 0; j < outOfBoundsBytes.Length - arraysDifference; j++)
                        {
                            if (found)
                                break;
                            int matches = 0;
                            for (Int64 b = 0; b < bytes.Length && (j + b) < outOfBoundsBytes.Length - arraysDifference; b++)
                            {
                                if (bytes[b] != outOfBoundsBytes[j + b])
                                {
                                    found = false;
                                    break;
                                }
                                else
                                {
                                    matches++;
                                }
                            }
                            if (matches == bytes.Length)
                            {
                                finalList.Add(j + currentAddress);
                                found = true;
                                break;
                            }

                        }
                    }
                }
            }
            //If the block could be read in just one read,
            else
            {
                //Calculate the memory block's size.
                Int64 blockSize = memorySize % ReadStackSize;

                //Set the currentAddress to first address.
                Int64 currentAddress = (Int64)baseAddress;

                //Holds the count of bytes read from the memory.
                Int64 bytesReadSize;

                //If the memory block can contain at least one 64 bit variable.
                if (blockSize > bytesCount)
                {
                    //Read the bytes to the array.
                    reader.ReadProcessMemory((IntPtr)currentAddress, (UInt64)blockSize, out bytesReadSize, buffer);
                    byte[] array = buffer;
                    
                    //If any byte is read,
                    if (bytesReadSize > 0)
                    {
                        //Loop through the array to find the values.
                        for (int j = 0; j < array.Length - arraysDifference; j++)
                        {

                            if (found)
                                break;
                            int matches = 0;
                            for (int b = 0; b < bytes.Length && (j + b) < array.Length - arraysDifference; b++)
                            {
                                if (bytes[b] != array[j + b])
                                {
                                    found = false;
                                    break;
                                }
                                else
                                {
                                    matches++;
                                }
                            }
                            if (matches == bytes.Length)
                            {
                                Debug.WriteLine("Found string: " + System.Text.Encoding.ASCII.GetString(array, j, bytes.Length));
                                finalList.Add(j + currentAddress);
                                Debug.Flush();
                                found = true;
                                break;
                            }
                        }
                    }
                }
            }
            //Close the handle to the process to avoid process errors.
            reader.CloseHandle();

            //Prepare the ScanProgressed and set the progress percentage to 100% and raise the event.
            scanProgressEventArgs = new ScanProgressChangedEventArgs(100);
            ScanProgressChanged(this, scanProgressEventArgs);

            //Prepare and raise the ScanCompleted event.
            ScanCompletedEventArgs scanCompleteEventArgs = new ScanCompletedEventArgs(finalList.ToArray());
            ScanCompleted(this, scanCompleteEventArgs);
        }
        #endregion
        #endregion
    }

    
    
    #region EventArgs classes
    public class ScanProgressChangedEventArgs : EventArgs
    {
        public ScanProgressChangedEventArgs(int Progress)
        {
            progress = Progress;
        }
        private int progress;
        public int Progress
        {
            set
            {
                progress = value;
            }
            get
            {
                return progress;
            }
        }
    }

    public class ScanCompletedEventArgs : EventArgs
    {
        public ScanCompletedEventArgs(Int64[] MemoryAddresses)
        {
            memoryAddresses = MemoryAddresses;
        }
        private Int64[] memoryAddresses;
        public Int64[] MemoryAddresses
        {
            set
            {
                memoryAddresses = value;
            }
            get
            {
                return memoryAddresses;
            }
        }
    }

    public class ScanCanceledEventArgs : EventArgs
    {
        public ScanCanceledEventArgs()
        {
        }
    } 
    #endregion

    #region ProcessMemoryReader class
    //Thanks goes to Arik Poznanski for P/Invokes and methods needed to read and write the Memory
    //For more information refer to "Minesweeper, Behind the scenes" article by Arik Poznanski at Codeproject.com
    class ProcessMemoryReader
    {

        public ProcessMemoryReader()
        {
        }

        /// <summary>	
        /// Process from which to read		
        /// </summary>
        public Process ReadProcess
        {
            get
            {
                return m_ReadProcess;
            }
            set
            {
                m_ReadProcess = value;
            }
        }

        private Process m_ReadProcess = null;

        private IntPtr m_hProcess = IntPtr.Zero;

        public void OpenProcess()
        {
            ProcessMemoryReaderApi.ProcessAccessType access;
            access = ProcessMemoryReaderApi.ProcessAccessType.PROCESS_VM_READ;
//                | ProcessMemoryReaderApi.ProcessAccessType.PROCESS_VM_WRITE
  //              | ProcessMemoryReaderApi.ProcessAccessType.PROCESS_VM_OPERATION;
            m_hProcess = ProcessMemoryReaderApi.OpenProcess((uint)access, 1, (uint)m_ReadProcess.Id);

        }

        public void CloseHandle()
        {
            try
            {
                int iRetValue;
                iRetValue = ProcessMemoryReaderApi.CloseHandle(m_hProcess);
                if (iRetValue == 0)
                {
                    throw new Exception("CloseHandle failed");
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            }
        }

        public byte[] ReadProcessMemory(IntPtr MemoryAddress, UInt64 bytesToRead, out Int64 bytesRead, byte[] buffer)
        {

            IntPtr ptrBytesRead = new IntPtr();
            int rval = ProcessMemoryReaderApi.ReadProcessMemory(m_hProcess, MemoryAddress, buffer, bytesToRead, out ptrBytesRead);

            bytesRead = ptrBytesRead.ToInt64();

            return buffer;
        }

        public void WriteProcessMemory(IntPtr MemoryAddress, byte[] bytesToWrite, out int bytesWritten)
        {
            IntPtr ptrBytesWritten;
            ProcessMemoryReaderApi.WriteProcessMemory(m_hProcess, MemoryAddress, bytesToWrite, (uint)bytesToWrite.Length, out ptrBytesWritten);

            bytesWritten = ptrBytesWritten.ToInt32();
        }


        /// <summary>
        /// ProcessMemoryReader is a class that enables direct reading a process memory
        /// </summary>
        class ProcessMemoryReaderApi
        {
            // constants information can be found in <winnt.h>
            [Flags]
            public enum ProcessAccessType
            {
                PROCESS_TERMINATE = (0x0001),
                PROCESS_CREATE_THREAD = (0x0002),
                PROCESS_SET_SESSIONID = (0x0004),
                PROCESS_VM_OPERATION = (0x0008),
                PROCESS_VM_READ = (0x0010),
                PROCESS_VM_WRITE = (0x0020),
                PROCESS_DUP_HANDLE = (0x0040),
                PROCESS_CREATE_PROCESS = (0x0080),
                PROCESS_SET_QUOTA = (0x0100),
                PROCESS_SET_INFORMATION = (0x0200),
                PROCESS_QUERY_INFORMATION = (0x0400)
            }

            // function declarations are found in the MSDN and in <winbase.h> 

            //		HANDLE OpenProcess(
            //			DWORD dwDesiredAccess,  // access flag
            //			BOOL bInheritHandle,    // handle inheritance option
            //			DWORD dwProcessId       // process identifier
            //			);
            [DllImport("kernel32.dll")]
            public static extern IntPtr OpenProcess(UInt32 dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);

            //		BOOL CloseHandle(
            //			HANDLE hObject   // handle to object
            //			);
            [DllImport("kernel32.dll")]
            public static extern Int32 CloseHandle(IntPtr hObject);

            //		BOOL ReadProcessMemory(
            //			HANDLE hProcess,              // handle to the process
            //			LPCVOID lpBaseAddress,        // base of memory area
            //			LPVOID lpBuffer,              // data buffer
            //			SIZE_T nSize,                 // number of bytes to read
            //			SIZE_T * lpNumberOfBytesRead  // number of bytes read
            //			);
            [DllImport("kernel32.dll")]
            public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt64 size, out IntPtr lpNumberOfBytesRead);


            //		BOOL WriteProcessMemory(
            //			HANDLE hProcess,                // handle to process
            //			LPVOID lpBaseAddress,           // base of memory area
            //			LPCVOID lpBuffer,               // data buffer
            //			SIZE_T nSize,                   // count of bytes to write
            //			SIZE_T * lpNumberOfBytesWritten // count of bytes written
            //			);
            [DllImport("kernel32.dll")]
            public static extern Int32 WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer, UInt32 size, out IntPtr lpNumberOfBytesWritten);


        }
    } 
    #endregion
}
