using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GON.Extensions;

namespace StackTest;

internal class StackWalker
{
    private static LinkedList<byte> _stack;
    private static long _address;


    public static void Walk()
    {
        _stack = new LinkedList<byte>();
        int flag = 0x00FACE00;

        unsafe
        { 
            byte* ptrTop = (byte*)&flag;
            byte* ptrBottom = (byte*)&flag;
            _address = (long)ptrTop;
            _stack.AddLast((*ptrTop));

            ShowStack();
            while (true)
            {

                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q)
                    break;
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    ptrBottom++;
                    _stack.AddLast((*ptrBottom));
                    ShowStack(false);
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    ptrTop--;
                    _address = (long)ptrTop;
                    _stack.AddFirst((*ptrTop));
                    ShowStack(true);
                }
            }
        }
    }

    private static void ShowStack(bool top = true)
    {
        int n = 4;

        if (top)
        {
            for (int i = 0; i < Math.Min(Console.WindowHeight*n, _stack.Count); i+=n)
            {
                Console.SetCursorPosition(0, i/n);
                Console.Write($"{BitConverter.GetBytes(_address + i).Reverse().hex()}\t{_stack.Skip(i).Take(n).Reverse().hex()}");
            }
        }
        else 
        {
            int offset = 0;
            if (_stack.Count <= Console.WindowHeight*n)
            {
                offset = 0;
            }
            else 
            {
                offset = _stack.Count-(Console.WindowHeight*n);
            }

            for (int i = 0; i < Math.Min(Console.WindowHeight*n, _stack.Count); i+=n)
            {
                Console.SetCursorPosition(0, i/n);
                Console.Write($"{BitConverter.GetBytes(_address + (long)i+ (long)offset).Reverse().hex()}\t{_stack.Skip(i+ offset).Take(n).Reverse().hex()}");
            }
        }

    }

}

