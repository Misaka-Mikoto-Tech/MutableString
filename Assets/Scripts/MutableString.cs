using System;
using System.Collections;
using System.Collections.Generic;
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

        /// <summary>
        /// 解析格式
        /// </summary>
        /// <param name="format"></param>
        /// <param name="callback">遇到变量定义时回调，参数为第几个参数, 目前不支持格式</param>
        private void ParseFormat(string format, Action<int> callback)
        {
            //int * p = stackalloc int[10];
           
            
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


    }
}
