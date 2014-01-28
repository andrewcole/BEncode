namespace Illallangi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public static class BEncode
    {
        public static dynamic ReadText(string text)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return BEncode.Read(stream);
            }
        }

        public static dynamic ReadFile(string filename)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                return BEncode.Read(stream);
            }
        }

        private static dynamic Read(Stream stream)
        {
            switch (stream.ReadChar())
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return BEncode.ReadString(stream);
                case 'i':
                    return BEncode.ReadInt(stream);
                case 'l':
                    return BEncode.ReadList(stream);
                case 'd':
                    return BEncode.ReadDictionary(stream);
                default:
                    throw new FormatException();
            }
        }

        private static string ReadString(Stream stream)
        {
            var sb = new StringBuilder();
            stream.Seek(-1, SeekOrigin.Current);
            var length = BEncode.ReadInt(stream, ':');
            
            for (var i = 0; i < length; i++)
            {
                sb.Append(stream.ReadChar());
            }

            return sb.ToString();
        }

        private static int ReadInt(Stream stream, char terminator = 'e')
        {
            var sb = new StringBuilder();
            char? c = null;

            do
            {
                if (c.HasValue)
                {
                    sb.Append(c.Value);
                }

                c = stream.ReadChar();
            }
            while (terminator != c);

            return int.Parse(sb.ToString());
        }

        private static IList<dynamic> ReadList(Stream stream, char terminator = 'e')
        {
            var result = new List<dynamic>();

            if (terminator == stream.PeekChar())
            {
                return result;
            }

            do
            {
                result.Add(BEncode.Read(stream));
            }
            while (terminator != stream.PeekChar());
            stream.ReadChar();

            return result;
        }

        private static IDictionary<string, dynamic> ReadDictionary(Stream stream, char terminator = 'e')
        {
            var result = new Dictionary<string, dynamic>();

            if (terminator == stream.PeekChar())
            {
                return result;
            }

            do
            {
                result.Add(BEncode.Read(stream), BEncode.Read(stream));
            }
            while (terminator != stream.PeekChar());
            stream.ReadChar();

            return result;
        }
    }
}
