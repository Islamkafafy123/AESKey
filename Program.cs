using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.Tracing;
using System.Text;

namespace AESKey
{
    internal class Program
    {
        private static readonly Dictionary<char, string> hexCharacterToBinary = new Dictionary<char, string>
        {
            { '0', "0000" },
            { '1', "0001" },
            { '2', "0010" },
            { '3', "0011" },
            { '4', "0100" },
            { '5', "0101" },
            { '6', "0110" },
            { '7', "0111" },
            { '8', "1000" },
            { '9', "1001" },
            { 'a', "1010" },
            { 'b', "1011" },
            { 'c', "1100" },
            { 'd', "1101" },
            { 'e', "1110" },
            { 'f', "1111" }
        };
        private static readonly Dictionary<string, string> binarytohex = new Dictionary<string, string>
        {
            { "0000","0" },
            { "0001","1" },
            { "0010","2" },
            { "0011","3" },
            { "0100","4" },
            { "0101","5" },
            { "0110","6" },
            { "0111","7" },
            { "1000","8" },
            { "1001","9" },
            { "1010","a" },
            { "1011","b" },
            { "1100","c" },
            { "1101","d" },
            { "1110","e" },
            { "1111","f" }
        };
        static string[,] subBytes = { { "63", "7c", "77", "7b", "f2", "6b", "6f", "c5", "30", "01", "67", "2b", "fe", "d7", "ab", "76" },//00
                                { "ca", "82", "c9", "7d", "fa", "59", "47", "f0", "ad", "d4", "a2", "af", "9c", "a4", "72", "c0" },//10
                                { "b7", "fd", "93", "26", "36", "3f", "f7", "cc", "34", "a5", "e5", "f1", "71", "d8", "31", "15" },//20
                                { "04", "c7", "23", "c3", "18", "96", "05", "9a", "07", "12", "80", "e2", "eb", "27", "b2", "75" },//30
                                { "09", "83", "2c", "1a", "1b", "6e", "5a", "a0", "52", "3b", "d6", "b3", "29", "e3", "2f", "84" },//40
                                { "53", "d1", "00", "ed", "20", "fc", "b1", "5b", "6a", "cb", "be", "39", "4a", "4c", "58", "cf" },//50
                                { "d0", "ef", "aa", "fb", "43", "4d", "33", "85", "45", "f9", "02", "7f", "50", "3c", "9f", "a8" },//60
                                { "51", "a3", "40", "8f", "92", "9d", "38", "f5", "bc", "b6", "da", "21", "10", "ff", "f3", "d2" },//70
                                { "cd", "0c", "13", "ec", "5f", "97", "44", "17", "c4", "a7", "7e", "3d", "64", "5d", "19", "73" },//80
                                { "60", "81", "4f", "dc", "22", "2a", "90", "88", "46", "ee", "b8", "14", "de", "5e", "0b", "db" },//90
                                { "e0", "32", "3a", "0a", "49", "06", "24", "5c", "c2", "d3", "ac", "62", "91", "95", "e4", "79" },//a0
                                { "e7", "c8", "37", "6d", "8d", "d5", "4e", "a9", "6c", "56", "f4", "ea", "65", "7a", "ae", "08" },//b0
                                { "ba", "78", "25", "2e", "1c", "a6", "b4", "c6", "e8", "dd", "74", "1f", "4b", "bd", "8b", "8a" },//c0
                                { "70", "3e", "b5", "66", "48", "03", "f6", "0e", "61", "35", "57", "b9", "86", "c1", "1d", "9e" },//d0
                                { "e1", "f8", "98", "11", "69", "d9", "8e", "94", "9b", "1e", "87", "e9", "ce", "55", "28", "df" },//e0
                                { "8c", "a1", "89", "0d", "bf", "e6", "42", "68", "41", "99", "2d", "0f", "b0", "54", "bb", "16" } };//f0
        private static readonly Dictionary<char, int> hexCharacterTodecimal = new Dictionary<char, int>
        {
            { '0', 0 },
            { '1', 1 },
            { '2', 2 },
            { '3', 3 },
            { '4', 4 },
            { '5', 5 },
            { '6', 6 },
            { '7', 7 },
            { '8', 8 },
            { '9', 9 },
            { 'a', 10 },
            { 'b', 11 },
            { 'c', 12 },
            { 'd', 13 },
            { 'e', 14 },
            { 'f', 15 }
        };

        static void Main(string[] args)
        {

            //key with no spaces if start with 0x it will be removed
            List<string> key =key_generation("0x00000000000000000000000000000000");
            for(int i = 0; i < key.Count; i++)
            {
                Console.WriteLine(key[i]);
            }
        }
        public static List<string> key_generation(string hex)
        {
            string rcon1 = "01000000";
            string rcon2 = "02000000";
            string rcon3 = "04000000";
            string rcon4 = "08000000";
            string rcon5 = "10000000";
            string rcon6 = "20000000";
            string rcon7 = "40000000";
            string rcon8 = "80000000";
            string rcon9 = "1b000000";
            string rcon10 = "36000000";
            if (hex[0]=='0' && hex[1]=='x')
            {
                hex = hex.Remove(0,2);
            }
            List<string> result = new List<string>();
            //Splitting key into 4 different Arrays
            result.Add(hex);
            for (int k = 1; k <=10;k++)
            {
                int counter = 0;
                string w0 = "";
                string w1 = "";
                string w2 = "";
                string w3 = "";
                foreach (char c in hex)
                {

                    w0 += hex[counter];
                    if (counter >= 7)
                        break;

                    counter++;
                }

                foreach (char c in hex)
                {

                    w1 += hex[counter + 1];
                    if (counter == 14)
                        break;

                    counter++;
                }
                foreach (char c in hex)
                {

                    w2 += hex[counter + 2];
                    if (counter == 21)
                        break;

                    counter++;
                }
                foreach (char c in hex)
                {

                    w3 += hex[counter + 3];
                    if (counter == 28)
                        break;

                    counter++;
                }
                //The Magic Begins here
                //left Circular Shift of w3
                string result1 = leftcircularShift(w3, 2);
                int rowfirst;
                int columnfirst;
                int row2nd;
                int column2nd;
                int row3rd;
                int column3rd;
                int row4th;
                int column4th;
                string first2bytes = "";
                string seconed2bytes = "";
                string third2bytes = "";
                string fourth2bytes = "";
                string w3new = "";
                for (int i = 0; i < 2; i++)
                {
                    first2bytes += result1[i];
                    seconed2bytes += result1[i + 2];
                    third2bytes += result1[i + 4];
                    fourth2bytes += result1[i + 6];
                }

                rowfirst = hexCharacterTodecimal[char.ToLower(first2bytes[0])];
                columnfirst = hexCharacterTodecimal[char.ToLower(first2bytes[1])];
                w3new += subBytes[rowfirst, columnfirst];
                row2nd = hexCharacterTodecimal[char.ToLower(seconed2bytes[0])];
                column2nd = hexCharacterTodecimal[char.ToLower(seconed2bytes[0])];
                w3new += subBytes[row2nd, column2nd];
                row3rd = hexCharacterTodecimal[char.ToLower(third2bytes[0])];
                column3rd = hexCharacterTodecimal[char.ToLower(third2bytes[0])];
                w3new += subBytes[row3rd, column3rd];
                row4th = hexCharacterTodecimal[char.ToLower(fourth2bytes[0])];
                column4th = hexCharacterTodecimal[char.ToLower(fourth2bytes[0])];
                w3new += subBytes[row4th, column4th];
                string binaryw3 = tobinary(w3new);
                string binaryrcon = "";
                switch (k)
                {
                    case 1:
                        binaryrcon = tobinary(rcon1);
                        break;
                    case 2:
                         binaryrcon = tobinary(rcon2);
                        break;
                    case 3:
                         binaryrcon = tobinary(rcon3);
                        break;
                    case 4:
                         binaryrcon = tobinary(rcon4);
                        break;
                    case 5:
                         binaryrcon = tobinary(rcon5);
                        break;
                    case 6:
                         binaryrcon = tobinary(rcon6);
                        break;
                    case 7:
                        binaryrcon = tobinary(rcon7);
                        break;
                    case 8:
                        binaryrcon = tobinary(rcon8);
                        break;
                    case 9:
                        binaryrcon = tobinary(rcon9);
                        break;
                    case 10:
                        binaryrcon = tobinary(rcon10);
                        break;
                }

                string after_xor_with_rcon = XOR(binaryw3, binaryrcon, 32);
                string w3hex = tohex(after_xor_with_rcon);
                string w4 = XOR(tobinary(w0), tobinary(w3hex), 32);
                string w4hex = tohex(w4);
                string w5 = XOR(tobinary(w4hex), tobinary(w1), 32);
                string w5hex = tohex(w5);
                string w6 = XOR(tobinary(w5hex), tobinary(w2), 32);
                string w6hex = tohex(w6);
                string w7 = XOR(tobinary(w6hex), tobinary(w3), 32);
                string w7hex = tohex(w7);
                string round = w4hex + w5hex + w6hex + w7hex;
                result.Add(round);
                hex = round;
            }
            return result;
        }
        private static string tobinary(string hex)
        {
            string binary = "";
            foreach (char c in hex)
            {
                binary += hexCharacterToBinary[char.ToLower(c)];
            }
            return binary;
        }
        private static string tohex(string binary)
        {
            
            string firstbyte = "";
            string seconedbyte = "";
            string thirdbyte = "";
            string fourthbyte = "";
            string fifthbyte = "";
            string sixthbyte = "";
            string seventhbyte = "";
            string eighthsbyte = "";
            string frombinarytohex = "";
            for (int i = 0; i < 4; i++)
            {
                firstbyte += binary[i];
                seconedbyte += binary[i + 4];
                thirdbyte += binary[i + 8];
                fourthbyte += binary[i + 12];
                fifthbyte += binary[i + 16];
                sixthbyte += binary[i + 20];
                seventhbyte += binary[i + 24];
                eighthsbyte += binary[i + 28];
            }
            frombinarytohex += binarytohex[firstbyte];
            frombinarytohex += binarytohex[seconedbyte];
            frombinarytohex += binarytohex[thirdbyte];
            frombinarytohex += binarytohex[fourthbyte];
            frombinarytohex += binarytohex[fifthbyte];
            frombinarytohex += binarytohex[sixthbyte];
            frombinarytohex += binarytohex[seventhbyte];
            frombinarytohex += binarytohex[eighthsbyte];
            return frombinarytohex;
        }

        private static string leftcircularShift(string key, int shift)
        {
            shift %= key.Length;
            return key.Substring(shift) + key.Substring(0, shift);
        }
        public static string XOR(string FirstBinary, string SecondBinary, int NumberOfItrastion)
        {

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < NumberOfItrastion; i++)
            {
                if (FirstBinary[i] == SecondBinary[i])
                    sb.Append("0");
                else
                    sb.Append("1");
            }
            return sb.ToString();
        }   //checked

    }

}

