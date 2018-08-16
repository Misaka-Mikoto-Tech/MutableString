using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SDGame
{
    /// <summary>
    /// 一种不会频繁产生GC的字符串类，要求打开 unsafe 开关
    /// </summary>
    public unsafe class MutableString
    {
        public string str;
        private int _capacity;
        private int _strLen;

        public MutableString(int capacity)
        {
            str = new string('\0', capacity);
            _capacity = capacity;
        }

        public MutableString Format<T0>(string format, T0 arg0)
        {
            return this;
        }

        public MutableString Format<T0, T1>(string format, T0 arg0, T1 arg1)
        {
            return this;
        }

        public MutableString Append(string str)
        {

            return this;
        }

        public MutableString AppendFormat<T>(string format, T arg)
        {

            return this;
        }


        private void UpdateLen(int len)
        {
            if (len >= _capacity)
                len = _capacity;

            fixed(void * p = str)
            {
                int* pLen = (int*)p;
                pLen -= 1;
                *pLen = len;
            }
            _strLen = len;
        }
    }


    public unsafe static class MutableStringUtil
    {
        public static unsafe void CopyMemory(IntPtr dest_, IntPtr src_, int len)
        {
            byte* dest = (byte*)dest_.ToPointer();
            byte* src = (byte*)src_.ToPointer();

            if (len >= 0x10)
            {
                do
                {
                    *((int*)dest) = *((int*)src);
                    *((int*)(dest + 4)) = *((int*)(src + 4));
                    *((int*)(dest + 8)) = *((int*)(src + 8));
                    *((int*)(dest + 12)) = *((int*)(src + 12));
                    dest += 0x10;
                    src += 0x10;
                }
                while ((len -= 0x10) >= 0x10);
            }
            if (len > 0)
            {
                if ((len & 8) != 0)
                {
                    *((int*)dest) = *((int*)src);
                    *((int*)(dest + 4)) = *((int*)(src + 4));
                    dest += 8;
                    src += 8;
                }
                if ((len & 4) != 0)
                {
                    *((int*)dest) = *((int*)src);
                    dest += 4;
                    src += 4;
                }
                if ((len & 2) != 0)
                {
                    *((short*)dest) = *((short*)src);
                    dest += 2;
                    src += 2;
                }
                if ((len & 1) != 0)
                {
                    *dest = *src;
                }
            }
        }
    } // MutableStringUtil



    public unsafe static class Formatter
    {
        struct Arg
        {
            public Type type;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] buff;
            public object obj;
        }

        private static Arg[] _args;
        private static int _argCount;

        static Formatter()
        {
            _args = new Arg[10];
            for (int i = 0, imax = _args.Length; i < imax; i++)
            {
                Arg arg = new Arg();
                _args[i] = arg;
            }
        }
        public static string Format<T0>(string format, T0 arg0)
        {
            _argCount = 0;
            PushArg(arg0);
            return DoFormat();
        }

        public static string Format<T0, T1>(string format, T0 arg0, T1 arg1)
        {
            _argCount = 0;
            PushArg(arg0);
            PushArg(arg1);
            return DoFormat();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct __ForCopy<T>
        {
            public byte     placeholder;
            public T        val;
        }

        private static void PushArg<T>(T arg)
        {
            if (typeof(T).IsPrimitive)
            {
                int size = Marshal.SizeOf<T>();

                __ForCopy<T> forcopy;
                forcopy.val = arg;
                byte* ptr = &forcopy.placeholder;
                ptr += sizeof(byte);

                fixed (byte * pDst = &_args[_argCount].buff[0])
                {
                    MutableStringUtil.CopyMemory(new IntPtr(pDst), new IntPtr(ptr), size);
                    //Marshal.StructureToPtr<T>(arg, new IntPtr(pDst), false); // 这种方式需要 .net 4.6, 废弃
                }
            }
            else
                _args[_argCount].obj = arg;

            _argCount++;
        }

        private static string DoFormat()
        {
            return string.Empty;
        }
    }

}
