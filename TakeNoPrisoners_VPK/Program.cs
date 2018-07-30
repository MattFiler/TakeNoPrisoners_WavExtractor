using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TakeNoPrisoners_VPK
{
    class Program
    {
        static void Main(string[] args)
        {
            var vpkArray = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.VPK", SearchOption.AllDirectories);
            foreach (var VPK in vpkArray)
            {
                byte[] vpkBytes = File.ReadAllBytes(VPK);

                Directory.CreateDirectory("SOUNDS");

                int offset = 0;
                while (offset != -1)
                {
                    //Filenames are in the VPK, but will just use offset to name for now.
                    offset = GetFile(vpkBytes, offset.ToString(), offset);
                }
            }
        }

        static int GetFile(byte[] vpkBytes, string fileName, int offset)
        {
            int firstPos = GetFilePosition(vpkBytes, offset);
            int secondPos = GetFilePosition(vpkBytes, firstPos + 10);
            if (firstPos == -1 || secondPos == -1)
            {
                return -1;
            }

            byte[] file = new byte[secondPos - firstPos];
            for (int i=0;i<(secondPos-firstPos);i++)
            {
                file[i] = vpkBytes[firstPos+i];
            }

            File.WriteAllBytes(Directory.GetCurrentDirectory() + "/SOUNDS/"+fileName+".WAV", file);

            return secondPos+10;
        }

        static int GetFilePosition(byte[] vpkBytes, int offset)
        {
            byte[] wavHeader = Encoding.ASCII.GetBytes("WAVEfmt");

            int success = 0;
            for (int i = offset; i < vpkBytes.Length; i++)
            {
                if (vpkBytes[i] == wavHeader[success])
                {
                    success++;
                }
                else
                {
                    success = 0;
                }

                if (wavHeader.Length == success)
                {
                    return i - wavHeader.Length -7; //7 is RIFF offset
                }
            }
            return -1;
        }
    }
}
