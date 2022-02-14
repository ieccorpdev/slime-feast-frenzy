/* The MIT License (MIT)

Copyright (c) 2014 Northwoods Consulting Partners

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

using System;
using System.Collections.Generic;

namespace MAPIWrapper
{
    public class MAPIException : Exception
    {
        public MAPIException(uint errorCode) : base(GetDescription(errorCode)) {}
        public MAPIException(string message) : base(message) {}

        public static string GetDescription(uint id)
        {
            var errors = new Dictionary<int, string>
                {
                    {0, "OK [0]"},
                    {1, "User abort [1]"},
                    {2, "General email failure [2]"},
                    {3, "Email login failure [3]"},
                    {4, "Disk full [4]"},
                    {5, "Insufficient memory [5]"},
                    {6, "Access denied [6]"},
                    {7, "-unknown- [7]"},
                    {8, "Too many sessions [8]"},
                    {9, "Too many files were specified [9]"},
                    {10, "Too many recipients were specified [10]"},
                    {11, "A specified attachment was not found [11]"},
                    {12, "Attachment open failure [12]"},
                    {13, "Attachment write failure [13]"},
                    {14, "Unknown recipient [14]"},
                    {15, "Bad recipient type [15]"},
                    {16, "No messages [16]"},
                    {17, "Invalid message [17]"},
                    {18, "Text too large [18]"},
                    {19, "Invalid session [19]"},
                    {20, "Type not supported [20]"},
                    {21, "A recipient was specified ambiguously [21]"},
                    {22, "Message in use [22]"},
                    {23, "Network failure [23]"},
                    {24, "Invalid edit fields [24]"},
                    {25, "Invalid recipients [25]"},
                    {26, "Not supported [26]"}
                };

            return (id <= 26 ? errors[(int)id] : string.Format("MAPI error [{0}]", id));
        }
    }
}

#endif