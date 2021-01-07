
namespace SelfSignedCertificateGenerator
{


    public class StringStream
        : System.IO.Stream, System.IDisposable
    {

        protected System.IO.MemoryStream m_stream;
        protected System.IO.StreamWriter m_writer;
        private bool disposedValue;

        public StringStream(string s)
        {
            this.m_stream = new System.IO.MemoryStream();
            this.m_writer = new System.IO.StreamWriter(this.m_stream);
            this.m_writer.Write(s);
            this.m_writer.Flush();
            this.m_stream.Position = 0;
        } // End Constructor 


        public override bool CanRead => this.m_stream.CanRead;

        public override bool CanSeek => this.m_stream.CanSeek;

        public override bool CanWrite => this.m_stream.CanWrite;

        public override long Length => this.m_stream.Length;

        public override long Position { get => this.m_stream.Position; set => this.m_stream.Position = value; }

        public override void Flush()
        {
            this.m_stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.m_stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, System.IO.SeekOrigin origin)
        {
            return this.m_stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.m_stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.m_stream.Write(buffer, offset, count);
        }

        protected void OnDispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: Verwalteten Zustand (verwaltete Objekte) bereinigen
                    if (this.m_writer != null)
                        this.m_writer.Dispose();

                    if (this.m_stream != null)
                        this.m_stream.Dispose();
                }

                // TODO: Nicht verwaltete Ressourcen (nicht verwaltete Objekte) freigeben und Finalizer überschreiben
                // TODO: Große Felder auf NULL setzen
                disposedValue = true;
            }
        } // End Sub OnDispose 

        // // TODO: Finalizer nur überschreiben, wenn "Dispose(bool disposing)" Code für die Freigabe nicht verwalteter Ressourcen enthält
        // ~StringStream()
        // {
        //     // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
        //     Dispose(disposing: false);
        // }

        void System.IDisposable.Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in der Methode "Dispose(bool disposing)" ein.
            OnDispose(true);
            System.GC.SuppressFinalize(this);
        } // End Sub Dispose 


    } // End Class StringStream 


}
