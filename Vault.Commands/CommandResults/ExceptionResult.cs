using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Archivarius;

namespace Vault.Commands
{
    [Guid("FD0FCF27-D349-4025-B4ED-FF20D03553DC")]
    public class ExceptionResult : Result
    {
        private string? _message;
        private string? _source;
        private string? _helpLink;
        private int _hResult;
        private string? _stackTrace;
        private List<KeyValuePair<string?, string?>>? _data;
        private ExceptionResult? _innerException;

        public string Message => _message ?? "";
        public string? Source => _source;
        public string? HelpLink => _helpLink;
        public int HResult => _hResult;
        public string? StackTrace => _stackTrace;
        public IEnumerable<KeyValuePair<string?, string?>>? Data => _data;
        public ExceptionResult? InnerException => _innerException;

        public ExceptionResult()
        {
        }

        public ExceptionResult(Exception ex)
        {
            _message = ex.Message;
            _source = ex.Source;
            _helpLink = ex.HelpLink;
            _hResult = ex.HResult;
            _stackTrace = ex.StackTrace;
            if (ex.Data.Count > 0)
            {
                _data = new List<KeyValuePair<string?, string?>>();
                foreach (DictionaryEntry kv in ex.Data)
                {
                    _data.Add(new KeyValuePair<string?, string?>(kv.Key.ToString(), kv.Value?.ToString()));
                }
            }

            _innerException =
                ex.InnerException != null ? new ExceptionResult(ex.InnerException) : null;
        }

        public override void WriteTo(IOutputTextStream dst)
        {
            dst.WriteLine("Exception: " + Message);
            if (_innerException != null)
            {
                dst.WriteLine("Inner:");
                var offsetDst = new OutputTextStreamWithOffset(dst, () => "    ", true);
                _innerException.WriteTo(offsetDst);
                offsetDst.FinishBlock();
            }
            if (_stackTrace != null)
            {
                dst.WriteLine(_stackTrace);
            }
        }

        public override void Serialize(ISerializer serializer)
        {
            serializer.Add(ref _message);
            serializer.Add(ref _source);
            serializer.Add(ref _helpLink);
            serializer.Add(ref _hResult);
            serializer.Add(ref _stackTrace);
            serializer.AddClass(ref _innerException);
            serializer.AddList(ref _data, (ISerializer s, ref KeyValuePair<string?, string?> kv) =>
            {
                if (s.IsWriter)
                {
                    var k = kv.Key;
                    var v = kv.Value;
                    serializer.Add(ref k);
                    serializer.Add(ref v);
                }
                else
                {
                    string? k = null;
                    string? v = null;
                    serializer.Add(ref k);
                    serializer.Add(ref v);
                    kv = new KeyValuePair<string?, string?>(k, v);
                }
            });
        }
    }
}