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
    public class MailMessage : IDisposable
    {
        private readonly MAPI _mapi;
        private string _subject;
        private string _body;
        public List<String> ToAddresses { get; set; }
        public List<String> CCAddresses { get; set; }
        public List<String> BCCAddresses { get; set; }
        public List<String> AttachmentFilePaths { get; set; }
        public string Subject()
        {
            return _subject;
        }
        public string Body()
        {
            return _body;
        }

        public MailMessage(MAPI mapi) : this()
        {
            _mapi = mapi;
        }

        public MailMessage()
        {
            _mapi = new MAPI();
            ToAddresses = new List<string>();
            CCAddresses = new List<string>();
            BCCAddresses = new List<string>();
            AttachmentFilePaths = new List<string>();        
        }

        public virtual MailMessage AddAddress(string address, AddressType addressType)
        {
            if (!string.IsNullOrEmpty(address))
            {
                switch (addressType)
                {
                    case AddressType.CC:
                        CCAddresses.Add(address);
                        break;
                    case AddressType.BCC:
                        BCCAddresses.Add(address);
                        break;
                    default:
                        ToAddresses.Add(address);
                        break;
                }
            }
            return this;
        }

        public MailMessage AddToAddress(string address)
        {
            return AddAddress(address, AddressType.To);
        }
        public MailMessage AddCCAddress(string address)
        {
            return AddAddress(address, AddressType.CC);
        }
        public MailMessage AddBCCAddress(string address)
        {
            return AddAddress(address, AddressType.BCC);
        }

        public MailMessage AddAttachment(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
                AttachmentFilePaths.Add(filePath);

            return this;
        }

        public MailMessage Subject(string subject)
        {
            _subject = subject;
            return this;
        }

        public MailMessage Body(string body)
        {
            _body = body;
            return this;
        }

        public MailMessage Send()
        {
           _mapi.SendMail(this);
            return this;
        }

        public void Dispose()
        {
           _mapi.Dispose();
        }
    }
}
#endif