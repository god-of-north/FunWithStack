using StackTest;

class Program
{
    public static void Main(string[] args)
    {
        
        FirstCall();
        Console.WriteLine("That's all Folks!");
    }
    static void FirstCall()
    {
        uint flag = 0xABCDEFFF;
        A a = new A()
        {
            Value1 = 0xEEEEEEEE,
            Value2 = true,
            Value3 = 0xFF,
        };
        uint flag2 = 0xCC0CC0CC;
        
        //(flag, flag2) = (flag2, flag);
        //(flag2, flag) = (flag, flag2);

        SomeMethod(0xFA1111AF, 0xFA2222AF, 0xFA3333AF, 0xFA4444AF, true, 0xBB, 0xFA7777AF);
    }


    static void SomeMethod(uint p1, uint p2, uint p3, uint p4, bool p5, byte p6, uint p7)
    {
        uint flag = 0xCDCDCDCD;

        StackViewer.ShowStack();
        //StackWalker.Walk();
    }

    struct A
    {
        public uint Value1;
        public bool Value2;
        public byte Value3;
    }
 
}