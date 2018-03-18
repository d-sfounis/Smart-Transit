using System;
using System.Text;

public class Example
{
    public static string ConvertHex(String hexString)
    {
        try
        {
            string ascii = string.Empty;

            for (int i = 0; i < hexString.Length; i += 2)
            {
                String hs = string.Empty;

                hs = hexString.Substring(i, 2);
                uint decval = System.Convert.ToUInt32(hs, 16);
                char character = System.Convert.ToChar(decval);
                ascii += character;

            }

            return ascii;
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
        return string.Empty;
    }

    public static byte[] StringToByteArray(String hex)
    {
        int NumberChars = hex.Length;
        byte[] bytes = new byte[NumberChars / 2];
        for (int i = 0; i < NumberChars; i += 2)
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }

    public static string ByteArrayToString(byte[] ba)
    {
        string hex = BitConverter.ToString(ba);
        return hex.Replace("-", "");
    }

    public static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    public static string hexReverserInPairs(string str)
    {
        return string.Empty;
    }

    public static void Main()
    {
        // Define a byte array.
        //NEO MAINNET Asset Hash: c56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b
        //GAS Asset Hash: 602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7
        byte[] neo_bytes = { 155, 124, 255, 218, 166, 116, 190, 174, 15, 147, 14, 190, 96, 133, 175, 144, 147, 229, 254, 86, 179, 74, 92, 34, 12, 205, 207, 110, 252, 51, 111, 197 };
        string hexStr = "48656c6c6f20576f726c6421";
        string hex_Reply = "602c7971";
        string neo_hex = "c56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b";
        string gas_hex = "602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7";
        string possible_solution= System.Text.Encoding.UTF8.GetString(neo_bytes, 0, neo_bytes.Length);
        string bytes_to_string = ByteArrayToString(neo_bytes);
        byte[] string_to_bytes = StringToByteArray(bytes_to_string);
        byte[] gas_to_bytes = StringToByteArray(gas_hex);
        byte[] reverse_gas_array = new byte[gas_to_bytes.Length];

        Console.WriteLine("The byte array, through Bitconverter.toString(): ");
        Console.WriteLine("   {0}\n", bytes_to_string);
        Console.WriteLine("Back to Bytes: ");
        for (int j = 0; j < string_to_bytes.Length; j++)
        {
            Console.Write(string_to_bytes[j] + " ");
        }
        Console.WriteLine(""); Console.WriteLine("");
        Console.WriteLine("Gas Hex (as found on the site, to bytes[]:");
        for(int i=0; i<gas_to_bytes.Length; i++)
        {
            Console.Write(gas_to_bytes[i] + " ");
        }
        //Create the reverse
        int it = 0;
        for (int i = gas_to_bytes.Length - 1; i >= 0; i--)
        {
            reverse_gas_array[it] = gas_to_bytes[i];
            it++;
        }
        Console.WriteLine(""); Console.WriteLine("");
        //Let's see if it's correct
        Console.WriteLine("Gas Hex Reversed, to bytes[]:");
        for (int i = 0; i < reverse_gas_array.Length; i++)
        {
            Console.Write(reverse_gas_array[i] + " ");
        }
        Console.WriteLine(""); Console.WriteLine("");
        Console.WriteLine("Gas Hex Reversed back to String: {0}", ByteArrayToString(reverse_gas_array));
        Console.ReadLine();
    }
}
// The example displays the following output:
//     The byte array:
//        02-04-06-08-0A-0C-0E-10-12-14
//     
//     The base 64 string:
//        AgQGCAoMDhASFA==
//     
//     The restored byte array:
//        02-04-06-08-0A-0C-0E-10-12-14