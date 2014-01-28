namespace Illallangi
{
    using System.IO;

    public static class StreamExtensions
    {
        public static char PeekChar(this Stream stream)
        {
            var result = stream.ReadChar();
            stream.Seek(-1, SeekOrigin.Current);
            return result;
        }

        public static char ReadChar(this Stream stream)
        {
            return (char)stream.ReadByte();
        }
    }
}