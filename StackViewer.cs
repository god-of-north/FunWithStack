using Binarysharp.Assemblers.Fasm;
using GON.Extensions;
using Process.NET.Memory;
using Process.NET.Native.Types;
using Process.NET;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace StackTest;

internal class StackViewer
{
    [SuppressUnmanagedCodeSecurity] // disable security checks for better performance
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] // cdecl - let caller (.NET CLR) clean the stack
    private delegate int GeneratedGetEBPFunction();

    public static int GetEBP()
    {
        var currentProcess = new ProcessSharp(System.Diagnostics.Process.GetCurrentProcess(), MemoryType.Local);
        FasmNet fasmNet = new FasmNet();

        fasmNet.AddLine("use32");           // Tell FASM.Net to use x86 (32bit) mode
        fasmNet.AddLine("push ebp");        // init stack frame
        fasmNet.AddLine("mov ebp, esp");    // move base pointer
        fasmNet.AddLine("mov edx, [ebp]");  // EDX = EBP of GetEBP
        fasmNet.AddLine("mov eax, [edx]");  // EAX = EBP of Calling Function (Return Value)
        fasmNet.AddLine("mov esp, ebp");    // reset base pointer
        fasmNet.AddLine("pop ebp");         // leave stack frame
        fasmNet.AddLine("ret");             // in cdecl calling convention, return value is stored in eax; so this will return both params added up

        byte[] assembledCode = fasmNet.Assemble();
        var allocatedCodeMemory = currentProcess.MemoryFactory.Allocate(
            name: "GetEBP", // only used for debugging; not really needed
            size: assembledCode.Length,
            protection: MemoryProtectionFlags.ExecuteReadWrite /* It is important to mark the memory as executeable or we will get exceptions from DEP */
        );

        allocatedCodeMemory.Write(0, assembledCode);
        var generatedGetEBPFunction = Marshal.GetDelegateForFunctionPointer<GeneratedGetEBPFunction>(allocatedCodeMemory.BaseAddress);
        var ebp = generatedGetEBPFunction();
        allocatedCodeMemory.Dispose();

        //Console.WriteLine($"EBP: {BitConverter.GetBytes(ebp).Reverse().hex()}");
        //Console.ReadKey(true);

        return ebp;
    }

    public static unsafe byte[] ReadFrame(uint ebp)
    {
        byte[] ret;
        unsafe
        {
            uint* ptrStart = (uint*)ebp;
            uint start = ebp;
            uint end = (*ptrStart);
            uint length = end - start + 4;

            ret = new byte[length];
            fixed (byte* dest = ret)
            {
                Buffer.MemoryCopy(ptrStart, dest, length, length);
            }
        }

        return ret;
    }


    public static void ShowStack()
    {
        uint ebp = (uint)GetEBP();
        var frame = ReadFrame(ebp); //StackWalker method Stack Frame

        StackTrace trace = new StackTrace();
        var frames = trace.GetFrames();
        for (int i = 1; i < frames.Length; i++) //Skip StackWalker method
        {
            ebp = BitConverter.ToUInt32(frame.Take(4).ToArray());
            frame = ReadFrame(ebp);

            Console.WriteLine();
            Console.WriteLine(frames[i].GetMethod()!.Name);
            ShowFrame(ebp, frame);
        }
    }

    public static void ShowFrame(uint start, byte[] frame)
    {
        for (int i = 0; i < frame.Length; i += 4)
        {
            Console.WriteLine($"\t{BitConverter.GetBytes(start + i).Reverse().hex()}\t{frame.Skip(i).Take(4).Reverse().hex()}");
        }
    }

}
