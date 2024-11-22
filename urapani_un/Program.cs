using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace urapani_un
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("unpack " + args[0]);
            unpack(args[0]);
        }


        static void unpack( string in_path, string out_path = "out" )
        {
            using ( FileStream infile = File.OpenRead(in_path) )
            {
                using( BinaryReader reader = new BinaryReader( infile ))
                {
                    uint magic_check = reader.ReadUInt32();
                    if( magic_check != 1262698832)
                    {
                        Console.WriteLine("magic check fail");
                        return;
                    }

                    Console.WriteLine("magic check pass");

                    uint file_count = reader.ReadUInt32();

                    Console.WriteLine("file count: "+file_count);

                    long last_pos = reader.BaseStream.Position;
                    for (int i = 0; i < file_count; i++)
                    {
                        string file_path = Encoding.UTF8.GetString(reader.ReadBytes(0x40)).TrimEnd('\0');

                        Console.WriteLine("path "+file_path+" pathlen "+file_path.Length);

                        uint path_crc32 = reader.ReadUInt32();
                        uint file_crc32 = reader.ReadUInt32();

                        uint f_pos = reader.ReadUInt32();
                        uint f_size = reader.ReadUInt32();

                        last_pos = reader.BaseStream.Position;
                        reader.BaseStream.Position = f_pos;
                        byte[] out_dat = reader.ReadBytes((int)f_size);

                        string use_out_path = Path.Combine(out_path, file_path);
                        string out_dir = Path.GetDirectoryName(use_out_path);

                        Console.WriteLine("out dir " + out_dir);

                        DirectoryInfo out_f = new DirectoryInfo(out_dir);
                        out_f.Create();

                        Console.WriteLine("write to "+use_out_path);
                        File.WriteAllBytes(use_out_path, out_dat);

                        reader.BaseStream.Position = last_pos;
                    }
                }
            }
        }
    }
}