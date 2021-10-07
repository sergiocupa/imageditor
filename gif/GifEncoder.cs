

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;


namespace halba.imageditor.gif
{
    internal class GifEncoder : IDisposable
    {

        private const string FileType = "GIF";
        private const string FileVersion = "89a";
        private const byte FileTrailer = 0x3b;

        private const int ApplicationExtensionBlockIdentifier = 0xff21;
        private const byte ApplicationBlockSize = 0x0b;
        private const string ApplicationIdentification = "NETSCAPE2.0";

        private const int GraphicControlExtensionBlockIdentifier = 0xf921;
        private const byte GraphicControlExtensionBlockSize = 0x04;

        private const long SourceGlobalColorInfoPosition = 10;
        private const long SourceGraphicControlExtensionPosition = 781;
        private const long SourceGraphicControlExtensionLength = 8;
        private const long SourceImageBlockPosition = 789;
        private const long SourceImageBlockHeaderLength = 11;
        private const long SourceColorBlockPosition = 13;
        private const long SourceColorBlockLength = 768;


        private bool _isFirstImage = true;
        private int? _width;
        private int? _height;
        private int? _repeatCount;
        private readonly Stream _stream;

        public TimeSpan FrameDelay { get; set; }

        public GifEncoder(Stream stream, int? width = null, int? height = null, int? repeatCount = null)
        {
            _stream = stream;
            _width = width;
            _height = height;
            _repeatCount = repeatCount;
        }

        public void AddFrame(Image img, int x = 0, int y = 0, TimeSpan? frameDelay = null)
        {
            using (var gifStream = new MemoryStream())
            {
                img.Save(gifStream, ImageFormat.Gif);
                if (_isFirstImage) 
                {
                    InitHeader(gifStream, img.Width, img.Height);
                }
                WriteGraphicControlBlock(gifStream, frameDelay.GetValueOrDefault(FrameDelay));
                WriteImageBlock(gifStream, !_isFirstImage, x, y, img.Width, img.Height);
            }
            _isFirstImage = false;
        }

        private void InitHeader(Stream sourceGif, int w, int h)
        {
            // File Header
            WriteString(FileType);
            WriteString(FileVersion);
            WriteShort(_width.GetValueOrDefault(w)); 
            WriteShort(_height.GetValueOrDefault(h)); 
            sourceGif.Position = SourceGlobalColorInfoPosition;
            WriteByte(sourceGif.ReadByte());
            WriteByte(0); 
            WriteByte(0); 
            WriteColorTable(sourceGif);

            WriteShort(ApplicationExtensionBlockIdentifier);
            WriteByte(ApplicationBlockSize);
            WriteString(ApplicationIdentification);
            WriteByte(3); 
            WriteByte(1);
            WriteShort(_repeatCount.GetValueOrDefault(0)); 
            WriteByte(0); 
        }

        private void WriteColorTable(Stream sourceGif)
        {
            sourceGif.Position = SourceColorBlockPosition; 
            var colorTable = new byte[SourceColorBlockLength];
            sourceGif.Read(colorTable, 0, colorTable.Length);
            _stream.Write(colorTable, 0, colorTable.Length);
        }

        private void WriteGraphicControlBlock(Stream sourceGif, TimeSpan frameDelay)
        {
            sourceGif.Position = SourceGraphicControlExtensionPosition; 
            var blockhead = new byte[SourceGraphicControlExtensionLength];
            sourceGif.Read(blockhead, 0, blockhead.Length); 

            WriteShort(GraphicControlExtensionBlockIdentifier); 
            WriteByte(GraphicControlExtensionBlockSize);
            WriteByte(blockhead[3] & 0xf7 | 0x08); 
            WriteShort(Convert.ToInt32(frameDelay.TotalMilliseconds / 10));
            WriteByte(blockhead[6]); 
            WriteByte(0); 
        }

        private void WriteImageBlock(Stream sourceGif, bool includeColorTable, int x, int y, int h, int w)
        {
            sourceGif.Position = SourceImageBlockPosition; 
            var header = new byte[SourceImageBlockHeaderLength];
            sourceGif.Read(header, 0, header.Length);
            WriteByte(header[0]); 
            WriteShort(x); 
            WriteShort(y); 
            WriteShort(h); 
            WriteShort(w); 

            if (includeColorTable) 
            {
                sourceGif.Position = SourceGlobalColorInfoPosition;
                WriteByte(sourceGif.ReadByte() & 0x3f | 0x80);
                WriteColorTable(sourceGif);
            }
            else
            {
                WriteByte(header[9] & 0x07 | 0x07); 
            }

            WriteByte(header[10]); 

            sourceGif.Position = SourceImageBlockPosition + SourceImageBlockHeaderLength;

            var dataLength = sourceGif.ReadByte();
            while (dataLength > 0)
            {
                var imgData = new byte[dataLength];
                sourceGif.Read(imgData, 0, dataLength);

                _stream.WriteByte(Convert.ToByte(dataLength));
                _stream.Write(imgData, 0, dataLength);
                dataLength = sourceGif.ReadByte();
            }

            _stream.WriteByte(0); 

        }

        private void WriteByte(int value)
        {
            _stream.WriteByte(Convert.ToByte(value));
        }

        private void WriteShort(int value)
        {
            _stream.WriteByte(Convert.ToByte(value & 0xff));
            _stream.WriteByte(Convert.ToByte((value >> 8) & 0xff));
        }

        private void WriteString(string value)
        {
            _stream.Write(value.ToArray().Select(c => (byte)c).ToArray(), 0, value.Length);
        }

        public void Dispose()
        {
            WriteByte(FileTrailer);
            _stream.Flush();
        }
    }
}
