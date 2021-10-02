using System;
using System.Security.Cryptography;

namespace Furball.Engine.Engine.Helpers {
    public static class MathHelper {
        //I wish i knew how lerp worked to explain this
        //i know tho >:3 -beyley
        /// <summary>
        /// Takes a linear interpolation of 2 values and an amount, the algorithm works as follows<br></br>
        ///
        /// The goal of this algorithm is to have
        /// <code>amount=0.0 == start</code>
        /// and
        /// <code>amount=1.0 == end</code>
        /// To be able to achive this we need to be able to change start by at least the difference of end and start,
        /// as start plus the difference of end and start is equal to end<br></br><br></br>
        /// Due to <code>start + (difference) == end</code>
        /// Adding half of difference to start should be half of the way to end from start, 1/4th of difference is 1/4th the way to end from start,
        /// which is the lerped value we want 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="amount"></param>
        /// <returns>The interpolated value</returns>
        public static double Lerp(double start, double end, double amount) => start + (end - start) * amount;
        
        /// <summary>
        /// Convert a rotation amount in degrees to radians 
        /// </summary>
        /// <param name="deg">Amount in degrees</param>
        /// <returns>Amount in radians</returns>
        public static double DegreesToRadians(double deg) => deg * Math.PI / 180.0;

        private static MD5 _md5 = new MD5CryptoServiceProvider();

        public static string GetMD5(byte[] bytes) {
            byte[] hash = _md5.ComputeHash(bytes);

            string hashString = string.Empty;
            for (int index = 0; index < hash.Length; index++) {
                byte x = hash[index];
                hashString += $"{x:x2}";
            }

            return hashString;
        }
        
        public static byte CRC8(byte[] data, int dataLimit)
        {
            byte sum = 0;
            unchecked// Let overflow occur without exceptions
            {
                for (int index = 0; index < Math.Min(dataLimit, data.Length); index++) {
                    byte b = data[index];
                    sum += b;
                }
            }
            return sum;
        }
        
        public static short CRC16(byte[] data, int dataLimit)
        {
            short sum = 0;
            unchecked// Let overflow occur without exceptions
            {
                for (int index = 0; index < Math.Min(dataLimit, data.Length); index++) {
                    byte b = data[index];
                    sum += b;
                }
            }
            return sum;
        }
        
        public static int CRC32(byte[] data, int dataLimit)
        {
            int sum = 0;
            unchecked// Let overflow occur without exceptions
            {
                for (int index = 0; index < Math.Min(dataLimit, data.Length); index++) {
                    byte b = data[index];
                    sum += b;
                }
            }
            return sum;
        }
        
        public static long CRC64(byte[] data, int dataLimit)
        {
            long sum = 0;
            unchecked// Let overflow occur without exceptions
            {
                for (int index = 0; index < Math.Min(dataLimit, data.Length); index++) {
                    byte b = data[index];
                    sum += b;
                }
            }
            return sum;
        }
    }
}
