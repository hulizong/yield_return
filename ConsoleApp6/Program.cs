using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp6
{
    #region yield return 介绍使用
    //class Program
    //{
    //    static private List<int> _numArray; //用来保存1-100 这100个整数

    //    Program() //构造函数。我们可以通过这个构造函数往待测试集合中存入1-100这100个测试数据
    //    {
    //        _numArray = new List<int>(); //给集合变量开始在堆内存上开内存，并且把内存首地址交给这个_numArray变量

    //        for (int i = 1; i <= 100; i++)
    //        {
    //            _numArray.Add(i);  //把1到100保存在集合当中方便操作
    //        }
    //    }

    //    static void Main(string[] args)
    //    {
    //        new Program();

    //        TestMethod();


    //    }

    //    //测试求1到100之间的全部偶数
    //    static public void TestMethod()
    //    {
    //        foreach (var item in GetAllEvenNumber())
    //        {
    //            Console.WriteLine(item); //输出偶数测试
    //        }
    //    }

    //    /// <summary>
    //    /// 使用平常返回集合方法
    //    /// </summary>
    //    /// <returns></returns>
    //    static IEnumerable<int> GetAllEvenNumberOld()
    //    {
    //        var listNum = new List<int>();
    //        foreach (int num in _numArray)
    //        {
    //            if (num % 2 == 0) //判断是不是偶数
    //            {
    //                listNum.Add(num); //返回当前偶数

    //            }
    //        }
    //        return listNum;
    //    }

    //    //使用Yield Return情况下的方法
    //    static IEnumerable<int> GetAllEvenNumber()
    //    {

    //        foreach (int num in _numArray)
    //        {
    //            if (num % 2 == 0) //判断是不是偶数
    //            {
    //                yield return num; //返回当前偶数

    //            }
    //        }
    //        yield break;  //当前集合已经遍历完毕，我们就跳出当前函数，其实你不加也可以
    //        //这个作用就是提前结束当前函数，就是说这个函数运行完毕了。
    //    }


    //}
    #endregion


    #region yield return 与return 比较
    //class Program
    //{ 
    //    static void Main(string[] args)
    //    {
    //        foreach (var item in GetNumsYield())
    //        {
    //            Console.WriteLine($"  common return:{item}");
    //        }
    //    }

    //    /// <summary>
    //    /// 通过yield return 返回集合
    //    /// </summary>
    //    /// <returns></returns>
    //    public static IEnumerable<int> GetNumsYield()
    //    {
    //        for (int i = 0; i < 10; i++)
    //        {
    //            Console.WriteLine($"yield return:{i}");
    //            yield return i;
    //        }
    //    }

    //    /// <summary>
    //    /// 平常return 返回集合
    //    /// </summary>
    //    /// <returns></returns>
    //    public static IEnumerable<int> GetNums()
    //    {
    //        var listNum = new List<int>();
    //        for (int i = 0; i < 10; i++)
    //        {
    //            Console.WriteLine($"yield return:{i}");
    //            listNum.Add(i);
    //        }
    //        return listNum;
    //    }


    //}
    #endregion


    #region  解析yield并定义相似类

    /// <summary>
    /// 解析前的原型
    /// </summary>
    class Program
    {
        public static Random r = new Random();
        static IEnumerable<int> GetList(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return r.Next(10);
            }
        }
        static void Main(string[] args)
        {
            foreach (int item in GetList(5))
                Console.WriteLine(item);
            Console.ReadKey();
        }
    }


    /// <summary>
    /// 解析yield并定义相似类
    /// </summary>
    public sealed class GetRandomNumbersClass : IEnumerable<int>, IEnumerable, IEnumerator<int>, IDisposable, IEnumerator
    {
        public static Random r = new Random();

        /// <summary>
        /// 状态
        /// </summary>
        private int _state;

        /// <summary>
        ///储存当前值
        /// </summary>
        private int _current;

        /// <summary>
        /// 线程id
        /// </summary>
        private int _initialThreadId;

        /// <summary>
        /// 集合元素数量
        /// </summary>
        private int count;

        /// <summary>
        /// 集合元素数量
        /// </summary>
        public int _count;

        /// <summary>
        /// 当前指针
        /// </summary>
        private int i;

        int IEnumerator<int>.Current
        {
            [DebuggerHidden]
            get
            {
                return _current;
            }
        }

        object IEnumerator.Current
        {
            [DebuggerHidden]
            get
            {
                return _current;
            }
        }

        [DebuggerHidden]
        public GetRandomNumbersClass(int state)
        {
            this._state = state;
            _initialThreadId = Environment.CurrentManagedThreadId;
        }

        [DebuggerHidden]
        void IDisposable.Dispose()
        {
        }

        private bool MoveNext()
        {
            switch (_state)
            {
                default:
                    return false;
                case 0:
                    _state = -1;
                    i = 0;
                    break;
                case 1:
                    _state = -1;
                    i++;
                    break;
            }
            if (i < count)
            {
                _current = r.Next(10);
                _state = 1;
                return true;
            }
            return false;
        }

        bool IEnumerator.MoveNext()
        {
            //ILSpy generated this explicit interface implementation from .override directive in MoveNext
            return this.MoveNext();
        }

        [DebuggerHidden]
        void IEnumerator.Reset()
        {
            throw new NotSupportedException();
        }

        [DebuggerHidden]
        public IEnumerator<int> GetEnumerator()
        {
            GetRandomNumbersClass _getRandom;
            if (_state == -2 && _initialThreadId == Environment.CurrentManagedThreadId)
            {
                _state = 0;
                _getRandom = this;
            }
            else
            {
                _getRandom = new GetRandomNumbersClass(0);
            }
            _getRandom.count = _count;
            return _getRandom;
        }

        [DebuggerHidden]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        [IteratorStateMachine(typeof(GetRandomNumbersClass))]
        private static IEnumerable<int> GetList(int count)
        {
            GetRandomNumbersClass getRandomNumbersClass = new GetRandomNumbersClass(-2);
            getRandomNumbersClass._count = count;
            return getRandomNumbersClass;
        }
        private static void Main(string[] args)
        {
            IEnumerator<int> enumerator = GetList(5).GetEnumerator();
            try
            {
                foreach (int item in GetList(5))
                    Console.WriteLine(item);
                //while (enumerator.MoveNext())
                //{
                //    int current = enumerator.Current;
                //    Console.WriteLine(current);
                //}
            }
            finally
            {
                if (enumerator != null)
                {
                    enumerator.Dispose();
                }
            }
            Console.ReadKey();
        }
    }

    #endregion
}
