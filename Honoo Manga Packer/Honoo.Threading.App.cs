/*
 * https://github.com/LokiHonoo/development-resources
 * Copyright (C) Loki Honoo 2015. All rights reserved.
 *
 * This code page is published by the MIT license.
 */

using System.Threading;

namespace Honoo.Threading
{
    /// <summary>
    /// 程序功能。
    /// </summary>
    internal static class App
    {
        private static Mutex? _mutex;

        /// <summary>
        /// 释放 Mutex 关闭启动检查。关闭后程序可以重复启动。
        /// </summary>
        internal static void DisableCheck()
        {
            _mutex?.Dispose();
        }

        /// <summary>
        /// 检测重复启动。如果具有相同自定义约束的项目存在，返回 true。
        /// </summary>
        /// <param name="unique">
        /// 自定义约束字符串。
        /// 使用常量字符串全局检测，或使用程序全路径检测同一个程序文件的重复启动。
        /// 约束字符串不能包括特殊字符。
        /// </param>
        /// <returns></returns>
        internal static bool PrevInstance(string unique)
        {
            _mutex = new Mutex(true, unique, out bool createdNew);
            if (createdNew)
            {
                _mutex.ReleaseMutex();
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}