﻿// ==++== 
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--== 
/*============================================================
** 
** Class:  StringReader 
**
** <owner>[....]</owner> 
**
** Purpose: For reading text from strings
**
** 
===========================================================*/

using System;

namespace Samarium.IO {

    using System.IO;
    using System.Runtime.InteropServices;
    using System.Diagnostics.Contracts;

    // This class implements a text reader that reads from a string.
    // 
    [Serializable]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class StringReader : TextReader {
        private String _s;
        private int _pos;
        private int _length;

        [System.Security.SecuritySafeCritical]  // auto-generated 
        public StringReader(String s) {
            if (s == null)
                throw new ArgumentNullException("s");
            Contract.EndContractBlock();
            _s = s;
            _length = s == null ? 0 : s.Length;
        }

        // Closes this StringReader. Following a call to this method, the String 
        // Reader will throw an ObjectDisposedException.
        public override void Close() {
            Dispose(true);
        }

        protected override void Dispose(bool disposing) {
            _s = null;
            _pos = 0;
            _length = 0;
            base.Dispose(disposing);
        }

        // Returns the next available character without actually reading it from
        // the underlying string. The current position of the StringReader is not 
        // changed by this operation. The returned value is -1 if no further
        // characters are available.
        //
        [Pure]
        [System.Security.SecuritySafeCritical]  // auto-generated
        public override int Peek() {
            if (_s == null)
                throw new IOException("Reader is closed");
            if (_pos == _length) return -1;
            return _s[_pos];
        }

        // Reads the next character from the underlying string. The returned value 
        // is -1 if no further characters are available.
        // 
        [System.Security.SecuritySafeCritical]  // auto-generated 
        public override int Read() {
            if (_s == null)
                throw new IOException("Reader is closed");
            if (_pos == _length) return -1;
            return _s[_pos++];
        }

        // Reads a block of characters. This method will read up to count 
        // characters from this StringReader into the buffer character 
        // array starting at position index. Returns the actual number of
        // characters read, or zero if the end of the string is reached. 
        //
        public override int Read([In, Out] char[] buffer, int index, int count) {
            if (buffer == null)
                throw new ArgumentNullException("buffer", "Buffer must not be null!");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", "Non-negative number required!");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Non-negative number required!");
            if (buffer.Length - index < count)
                throw new ArgumentException("Invalid offset length!");
            Contract.EndContractBlock();
            if (_s == null)
                throw new IOException("Reader is closed");

            int n = _length - _pos;
            if (n > 0) {
                if (n > count) n = count;
                _s.CopyTo(_pos, buffer, index, n);
                _pos += n;
            }
            return n;
        }

        public override String ReadToEnd() {
            if (_s == null)
                throw new IOException("Reader is closed");
            String s;
            if (_pos == 0)
                s = _s;
            else
                s = _s.Substring(_pos, _length - _pos);
            _pos = _length;
            return s;
        }

        // Reads a line. A line is defined as a sequence of characters followed by
        // a carriage return ('\r'), a line feed ('\n'), or a carriage return
        // immediately followed by a line feed. The resulting string does not
        // contain the terminating carriage return and/or line feed. The returned 
        // value is null if the end of the underlying string has been reached.
        // 
        [System.Security.SecuritySafeCritical]  // auto-generated 
        public override String ReadLine() {
            if (_s == null)
                throw new IOException("Reader is closed");
            int i = _pos;
            while (i < _length) {
                char ch = _s[i];
                if (ch == '\r' || ch == '\n') {
                    String result = _s.Substring(_pos, i - _pos);
                    _pos = i + 1;
                    if (ch == '\r' && _pos < _length && _s[_pos] == '\n') _pos++;
                    return result;
                }
                i++;
            }
            if (i > _pos) {
                String result = _s.Substring(_pos, i - _pos);
                _pos = i;
                return result;
            }
            return null;
        }

        // Custom code        
        /// <summary>
        /// Gets the current position of the reader.
        /// </summary>
        /// <value>
        /// The position of the reader.
        /// </value>
        public int Position => _pos;

        /// <summary>
        /// Gets the length of the string being read.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public int Length => _length;

    }
}