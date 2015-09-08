using System;
using System.IO;

namespace crypter
{
    class MainClass
    {
        private static byte gen_key()
        {
            Random random = new Random();
            byte key = (byte)random.Next(256);
            return key;
        }

        private static bool parse_args(string[] args, ref string method, ref string in_filename, ref string out_filename, ref char? chr, ref int? chr_pos)
        {
            try
            {
                method = args[0];
                for (int i = 1; i < args.Length; i = i + 2)
                {
                    switch (args[i])
                    {
                        case "-i":
                            {
                                in_filename = args[i + 1];
                                break;
                            }
                        case "-o":
                            {
                                out_filename = args[i + 1];
                                break;
                            }
                        case "-c":
                            {
                                chr = args[i + 1][0];
                                break;
                            }
                        case "-p":
                            {
                                string chrp = args[i + 1];
                                int chr_p;
                                if (!Int32.TryParse(chrp, out chr_p))
                                {
                                    Console.WriteLine("chr_pos must be integer value");
                                    return false;
                                }
                                chr_pos = chr_p;
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("Invalid key " + args[0]); 
                                return false;
                            }
                    }
                }
            }
            catch
            {
                Console.WriteLine("invalid args");
                return false;
            }
            return true;
        }

        private static void crypt(string in_filename, string out_filename)
        {
            if (!File.Exists(in_filename))
            {
                Console.WriteLine("In file not exists");
                return;
            }

            byte key = gen_key();

            FileStream in_stream = new FileStream(in_filename, FileMode.Open);
            FileStream out_stream = new FileStream(out_filename, FileMode.Create);

            while (in_stream.Position != in_stream.Length)
            {
                    byte in_b = (byte)in_stream.ReadByte();
                byte out_b = (byte)(in_b ^ key);
                out_stream.WriteByte(out_b);
            }

            in_stream.Close();
            out_stream.Close();
        }

        private static void decrypt(string in_filename, string out_filename, char chr, int chr_pos)
        {
            if (!File.Exists(in_filename))
            {
                Console.WriteLine("In file not exists");
                return;
            }

            FileStream in_stream = new FileStream(in_filename, FileMode.Open);

            in_stream.Position = chr_pos;
            byte c = (byte)in_stream.ReadByte();
            byte key = (byte)(c ^ (byte)chr);

            in_stream.Position = 0;

            FileStream out_stream = new FileStream(out_filename, FileMode.Create);

            while (in_stream.Position != in_stream.Length)
            {
                    byte in_b = (byte)in_stream.ReadByte();
                byte out_b = (byte)(in_b ^ key);
                out_stream.WriteByte(out_b);
            }
            in_stream.Close();
            out_stream.Close();
        }

        public static void Main(string[] args)
        {
            string method = null;
            string in_filename = null;
            string out_filename = null;
            char? chr = null;
            int? chr_pos = null;
            if (!parse_args(args, ref method, ref in_filename, ref out_filename, ref chr, ref chr_pos))
            {
                Console.WriteLine("Error");
                return;
            }

            if (String.IsNullOrEmpty(in_filename))
            {
                Console.WriteLine("in filename missed");
                return;
            }
            else if (String.IsNullOrEmpty(out_filename))
            {
                Console.WriteLine("out filename missed");
                return;
            }

            switch (method)
            {
                case "crypt":
                    {
                        crypt(in_filename, out_filename);
                        Console.WriteLine("crypt finished");
                        break;
                    }
                case "decrypt":
                    {
                        if (!chr.HasValue)
                        {
                            Console.WriteLine("chr missed");
                            return;
                        }
                        else if (!chr_pos.HasValue)
                        {
                            Console.WriteLine("chr_pos");
                            return;
                        }

                        decrypt(in_filename, out_filename, chr.Value, chr_pos.Value);
                        Console.WriteLine("decrypt finished");
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Unknown method");
                        return;
                    }
            }
        }
    }
}
